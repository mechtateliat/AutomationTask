using AutomationTask.Core.API;
using AutomationTask.Models;

namespace AutomationTask.Helpers;

/// <summary>
/// Helper class for user-related API operations
/// </summary>
public static class UserApiHelper
{
    /// <summary>
    /// Fetches all users from all pages
    /// </summary>
    /// <param name="apiUserClient">The API user client to use for requests</param>
    /// <returns>List of all users</returns>
    public static async Task<List<User>> GetAllUsersAsync(ApiUserClient apiUserClient)
    {
        var allUsers = new List<User>();
        int page = 1;
        UserResponse usersResponse;
        
        do
        {
            usersResponse = await apiUserClient.GetUsersAsync(page);
            if (usersResponse?.Data != null)
            {
                allUsers.AddRange(usersResponse.Data);
            }
            page++;
        } while (usersResponse != null && page <= usersResponse.TotalPages);

        return allUsers;
    }
}
