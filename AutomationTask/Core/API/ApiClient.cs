using AutomationTask.Configuration;
using Microsoft.Playwright;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutomationTask.Core.API;

public interface IApiClient
{
    Task<IAPIResponse> GetAsync(string endpoint, Dictionary<string, string>? headers = null);
    Task<IAPIResponse> PostAsync(string endpoint, object? body = null, Dictionary<string, string>? headers = null);
    Task<IAPIResponse> DeleteAsync(string endpoint, Dictionary<string, string>? headers = null);
}

public class ApiClient : IApiClient
{
    private readonly IAPIRequestContext _apiContext;
    private readonly TestSettings _settings;

    private ApiClient(IAPIRequestContext apiContext, TestSettings settings)
    {
        _apiContext = apiContext;
        _settings = settings;
    }

    public static async Task<ApiClient> CreateAsync(TestSettings settings)
    {
        var playwright = await Playwright.CreateAsync();
        var headers = new Dictionary<string, string>(settings.Api.Headers);
        var apiContext = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = settings.Api.BaseUrl,
            ExtraHTTPHeaders = headers,
            Timeout = settings.Api.Timeout
        });

        return new ApiClient(apiContext, settings);
    }

    public async Task<IAPIResponse> GetAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        return await _apiContext.GetAsync(endpoint, new APIRequestContextOptions
        {
            Headers = headers
        });
    }

    public async Task<IAPIResponse> PostAsync(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var requestHeaders = headers ?? new Dictionary<string, string>();
        
        // Ensure Content-Type is set to application/json
        if (!requestHeaders.ContainsKey("Content-Type"))
        {
            requestHeaders["Content-Type"] = "application/json";
        }

        var options = new APIRequestContextOptions
        {
            Headers = requestHeaders
        };

        if (body != null)
        {
            options.Data = JsonSerializer.Serialize(body, jsonOptions);
        }

        return await _apiContext.PostAsync(endpoint, options);
    }

    public async Task<IAPIResponse> DeleteAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        return await _apiContext.DeleteAsync(endpoint, new APIRequestContextOptions
        {
            Headers = headers
        });
    }

    public async Task DisposeAsync()
    {
        await _apiContext.DisposeAsync();
    }
}
