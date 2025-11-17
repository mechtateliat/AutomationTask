using AutomationTask.Helpers;
using AventStack.ExtentReports.Model;
using FluentAssertions;
using NUnit.Framework;
using System.Net;

namespace AutomationTask.Tests.API;

[TestFixture]
[Category("API")]
[Category("Users")]
public class UserApiTests : ApiTestBase
{
    [Test]
    [Category("Smoke")]
    [Category("HighPriority")]
    [Description("Verify that users can be retrieved from the API with JSON response")]
    public async Task GetUsers_FromPage()
    {
        // Arrange
        LogInfo("Sending GET request to /users endpoint with page=1");

        // Act
        var usersResponse = await ApiUserClient.GetUsersAsync(page: 1);

        LogInfo($"Response received - Page: {usersResponse.Page}, Total Users: {usersResponse.Total}");

        // Assert
        // Extensive assertions on the response structure and data
        usersResponse.Should().NotBeNull("Response should not be null");
        usersResponse.Page.Should().Be(1, "Page number should be 1");
        usersResponse.PerPage.Should().Be(6, "Per page should be 6");
        usersResponse.Total.Should().Be(12, "Total count should be 12");
        usersResponse.TotalPages.Should().BeGreaterThan(0, "Total pages should be greater than 0");

        usersResponse.Data.Should().NotBeNull("User data should not be null");
        usersResponse.Data.Should().NotBeEmpty("User data should contain at least one user");
        usersResponse.Data.Count.Should().BeLessThanOrEqualTo(usersResponse.PerPage, "Data count should not exceed per page limit");

        // Extract the first user from the response and verify its data
        var firstUser = usersResponse.Data.First();

        firstUser.Id.Should().Be(1, "User ID should be 1");
        firstUser.Email.Should().Be("george.bluth@reqres.in", "User email should be valid");
        firstUser.FirstName.Should().Be("George", "User first name should be George");
        firstUser.LastName.Should().Be("Bluth", "User last name should be correct");
        firstUser.Avatar.Should().NotBeNullOrEmpty("User avatar URL should not be empty");
        firstUser.Avatar.Should().StartWith("http", "Avatar should be a valid URL");

        LogInfo($"First user extracted and validated - ID: {firstUser.Id}, Name: {firstUser.FirstName} {firstUser.LastName}, Email: {firstUser.Email}");

        // Get all users, sort by first name, and print them
        var allUsers = await UserApiHelper.GetAllUsersAsync(ApiUserClient);
        var sortedUsers = allUsers.OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToList();

        Console.WriteLine("\nAll users sorted by name:");
        Console.WriteLine("==========================");
        foreach (var user in sortedUsers)
        {
            Console.WriteLine($"{user.FirstName} {user.LastName} - {user.Email}");
        }
        Console.WriteLine("==========================\n");

        LogPass("Successfully retrieved and validated users with JSON response");
    }

    [Test]
    [Category("Smoke")]
    [Category("HighPriority")]
    [Description("Verify that the first user can be retrieved via /users/[userId] and validated from the API")]
    public async Task FirstUserVerification()
    {
        // Arrange
        LogInfo("Sending GET request to /users/1");

        // Act
        var userResponse = await ApiUserClient.GetUserByIdAsync(1);

        // Assert
        var firstUser = userResponse;
        firstUser.Id.Should().Be(1, "User ID should be 1");
        firstUser.Email.Should().Be("george.bluth@reqres.in", "User email should be valid");
        firstUser.FirstName.Should().Be("George", "User first name should be George");
        firstUser.LastName.Should().Be("Bluth", "User last name should be correct");
        firstUser.Avatar.Should().NotBeNullOrEmpty("User avatar URL should not be empty");
        firstUser.Avatar.Should().StartWith("http", "Avatar should be a valid URL");

        LogInfo($"User validated - ID: {firstUser.Id}, Name: {firstUser.FirstName} {firstUser.LastName}, Email: {firstUser.Email}");
        LogPass("Successfully retrieved and validated the first user");
    }

    [Test]
    [Category("Negative")]
    [Category("MediumPriority")]
    [Description("Verify that requesting a non-existent user returns Not Found error")]
    public async Task GetUserById_NonExistentUser()
    {
        // Arrange
        LogInfo("Fetching all existing user IDs to ensure non-existent ID selection");

        var allUsers = await UserApiHelper.GetAllUsersAsync(ApiUserClient);
        var allUserIds = allUsers.Select(u => u.Id).ToHashSet();

        // Get max id and add 1 to ensure non-existence
        int nonExistentUserId = allUserIds.Count > 0 ? allUserIds.Max() + 1 : 1;

        LogInfo($"Sending GET request to /users/{nonExistentUserId} endpoint (ID not present in system)");

        // Act
        var response = await ApiUserClient.TryGetUserByIdAsync(nonExistentUserId);

        // Assert
        response.Status.Should().Be((int)HttpStatusCode.NotFound, "Status code is not 404");
        response.StatusText.Should().Be("Not Found", "Status text should indicate Not Found");

        LogPass("Non-existent user request correctly returned Not Found error");
    }

    [Test]
    [Category("User manipulation")]
    [Category("HighPriority")]
    [Description("Verify that a new user can be created with a unique name and then deleted")]
    public async Task CreateAndDeleteUniqueUser()
    {
        // Arrange
        LogInfo("Fetching all existing user names to ensure uniqueness");

        var allUsers = await UserApiHelper.GetAllUsersAsync(ApiUserClient);
        var allUserNames = allUsers.Select(u => $"{u.FirstName} {u.LastName}").ToHashSet();

        // Generate a unique name
        string baseName = "Horen";
        string uniqueName = baseName;
        int suffix = 1;
        while (allUserNames.Contains(uniqueName))
        {
            uniqueName = $"{baseName}{suffix}";
            suffix++;
        }

        var newUser = new
        {
            name = uniqueName,
            job = "QA",
            location = "Sofia",
            hobby = "Travel"
        };

        LogInfo($"Creating a new user via POST /users with unique name: {newUser.name}");

        // Act
        var createdUserResponse = await ApiUserClient.CreateUserAsync(newUser.name, newUser.job, newUser.location, newUser.hobby);

        // Assert
        createdUserResponse.Should().NotBeNull("Created user response should not be null");
        createdUserResponse.Id.Should().NotBeNull("Created user ID should not be null");
        createdUserResponse.CreatedAt.Should().NotBeNullOrEmpty("CreatedAt timestamp should not be null or empty");
        createdUserResponse.Name.Should().Be("Horen", "User name is not the same as create request");
        createdUserResponse.Job.Should().Be("QA", "User job is not same as create request");
        createdUserResponse.Location.Should().Be("Sofia", "User location is not the same as create request");
        createdUserResponse.Hobby.Should().Be("Travel", "User hobby is not the same as create request");

        LogInfo($"New user created successfully with Name: {newUser.name}, Job: {newUser.job}");

        // Continue with delete newly created user
        LogInfo("Deleting user via Delete /users");

        var deletedUserResponse = await ApiUserClient.DeleteUserAsync(int.Parse(createdUserResponse.Id));

        deletedUserResponse.Status.Should().Be((int)HttpStatusCode.NoContent, "Status code is not 204");

        LogInfo($"User deleted successfully with ID: {createdUserResponse.Id}");
        LogPass("User created and deleted successfully");
    }
}
