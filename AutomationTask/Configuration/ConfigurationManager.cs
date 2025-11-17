using Microsoft.Extensions.Configuration;

namespace AutomationTask.Configuration;

public class ConfigurationManager
{
    private static TestSettings? _testSettings;
    private static readonly object _lock = new();

    public static TestSettings GetTestSettings()
    {
        if (_testSettings == null)
        {
            lock (_lock)
            {
                if (_testSettings == null)
                {
                    _testSettings = LoadConfiguration();
                }
            }
        }
        return _testSettings;
    }

    private static TestSettings LoadConfiguration()
    {
        var environmentVariable = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
        
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentVariable ?? "dev"}.json", optional: true, reloadOnChange: false);
        
        // Add User Secrets support (only works in development, safe to call in production)
        // User Secrets are stored outside the project directory and never committed to source control
        try
        {
            configBuilder.AddUserSecrets<ConfigurationManager>(optional: true);
        }
        catch
        {
            // User Secrets not available in this environment (e.g., CI/CD), continue without them
        }
        
        // Environment variables take precedence over everything else (for CI/CD pipelines)
        configBuilder.AddEnvironmentVariables();
        
        var configuration = configBuilder.Build();

        var settings = new TestSettings();
        configuration.Bind(settings);

        // Override viewport settings from environment variables if provided
        OverrideViewportFromEnvironment(settings);

        return settings;
    }

    private static void OverrideViewportFromEnvironment(TestSettings settings)
    {
        // Check for viewport profile override
        var viewportProfile = Environment.GetEnvironmentVariable("UI__Profile");
        if (!string.IsNullOrEmpty(viewportProfile))
        {
            settings.Ui.Profile = viewportProfile;
        }

        // Check for custom viewport width/height overrides
        var viewportWidth = Environment.GetEnvironmentVariable("UI__ViewportWidth");
        if (!string.IsNullOrEmpty(viewportWidth) && int.TryParse(viewportWidth, out var width))
        {
            settings.Ui.ViewportWidth = width;
        }

        var viewportHeight = Environment.GetEnvironmentVariable("UI__ViewportHeight");
        if (!string.IsNullOrEmpty(viewportHeight) && int.TryParse(viewportHeight, out var height))
        {
            settings.Ui.ViewportHeight = height;
        }
    }

    public static void ResetConfiguration()
    {
        lock (_lock)
        {
            _testSettings = null;
        }
    }
}
