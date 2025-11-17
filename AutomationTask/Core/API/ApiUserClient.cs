using AutomationTask.Models;
using Microsoft.Playwright;
using System.Text.Json;

namespace AutomationTask.Core.API;

public class ApiUserClient
{
    private readonly ApiClient _apiClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiUserClient(ApiClient apiClient)
    {
        _apiClient = apiClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Gets a paginated list of users
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="perPage">Number of users per page (optional)</param>
    /// <returns cref="UserResponse">UserResponse containing user data and pagination info</returns>
    public async Task<UserResponse> GetUsersAsync(int page = 1, int? perPage = null)
    {
        var endpoint = $"users?page={page}";

        if (perPage.HasValue)
        {
            endpoint += $"&per_page={perPage.Value}";
        }

        var response = await _apiClient.GetAsync(endpoint);

        if (!response.Ok)
        {
            throw new HttpRequestException(
                $"Failed to get users. Status: {response.Status}, " +
                $"StatusText: {response.StatusText}");
        }

        var responseBody = await response.TextAsync();

        var userResponse = JsonSerializer.Deserialize<UserResponse>(responseBody, _jsonOptions);

        if (userResponse == null)
        {
            throw new InvalidOperationException("Failed to deserialize user response");
        }

        return userResponse;
    }

    /// <summary>
    /// Gets a single user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns cref="User">User response</returns>
    public async Task<User> GetUserByIdAsync(int userId)
    {
        var endpoint = $"users/{userId}";
        var response = await _apiClient.GetAsync(endpoint);

        if (!response.Ok)
        {
            throw new HttpRequestException(
                $"Failed to get user {userId}. Status: {response.Status}, " +
                $"StatusText: {response.StatusText}");
        }

        var responseBody = await response.TextAsync();
        var jsonDocument = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonOptions);

        if (jsonDocument.TryGetProperty("data", out JsonElement userData))
        {
            var user = JsonSerializer.Deserialize<User>(userData.GetRawText(), _jsonOptions);

            if (user == null)
            {
                throw new InvalidOperationException($"Failed to deserialize user {userId}");
            }

            return user;
        }

        throw new InvalidOperationException($"User {userId} data not found in response");
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="name">Name of user</param>
    /// <param name="job">Job of user</param>
    /// <returns cref="CreateUserResponse">User response</returns>
    public async Task<CreateUserResponse> CreateUserAsync(string name, string job, string location, string hobby)
    {
        var endpoint = "users";
        var body = new { name, job, location, hobby };

        var response = await _apiClient.PostAsync(endpoint, body);

        if (!response.Ok)
        {
            throw new HttpRequestException(
                $"Failed to create user. Status: {response.Status}, " +
                $"StatusText: {response.StatusText}");
        }

        var responseBody = await response.TextAsync();

        var createUserResponse = JsonSerializer.Deserialize<CreateUserResponse>(responseBody, _jsonOptions);

        if (createUserResponse == null)
        {
            throw new InvalidOperationException("Failed to deserialize create user response");
        }

        return createUserResponse;
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns cref="IAPIResponse">API response</returns>
    public async Task<IAPIResponse> DeleteUserAsync(int userId)
    {
        var endpoint = $"users/{userId}";
        return await _apiClient.DeleteAsync(endpoint);
    }

    /// <summary>
    /// Try to get a user by ID, returns the raw API response
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns cref="IAPIResponse">API response</returns>
    public async Task<IAPIResponse> TryGetUserByIdAsync(int userId)
    {
        var endpoint = $"/users/{userId}";
        var response = await _apiClient.GetAsync(endpoint);
        return response;
    }
}
