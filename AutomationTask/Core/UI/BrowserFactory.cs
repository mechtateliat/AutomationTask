using Microsoft.Playwright;
using AutomationTask.Configuration;
using AutomationTask.Core.TestArtifacts;

namespace AutomationTask.Core.UI;

public interface IBrowserFactory
{
    Task<IBrowser> CreateBrowserAsync();
    Task<IPage> CreatePageAsync(IBrowser browser);
}

public class BrowserFactory : IBrowserFactory
{
    private readonly TestSettings _settings;
    private IPlaywright? _playwright;

    public BrowserFactory(TestSettings settings)
    {
        _settings = settings;
    }

    public async Task<IBrowser> CreateBrowserAsync()
    {
        _playwright = await Playwright.CreateAsync();
        
        var browserType = _settings.Ui.Browser.ToLower() switch
        {
            "firefox" => _playwright.Firefox,
            "chromium" => _playwright.Chromium,
            "chrome" => _playwright.Chromium,
            _ => _playwright.Chromium
        };

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = _settings.Ui.Headless,
            SlowMo = _settings.Ui.SlowMo
        };

        // Use Chrome channel for "chrome" browser setting
        if (_settings.Ui.Browser.ToLower() == "chrome")
        {
            launchOptions.Channel = "chrome";
        }

        return await browserType.LaunchAsync(launchOptions);
    }

    public async Task<IPage> CreatePageAsync(IBrowser browser)
    {
        // Get the effective viewport size (from profile or custom dimensions)
        var (width, height) = _settings.Ui.GetViewportSize();

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = width,
                Height = height
            },
            RecordVideoDir = _settings.Ui.Video != "off" ? TestArtifactsManager.VideosDirectory : null,
            RecordVideoSize = new RecordVideoSize { Width = width, Height = height }
        });

        // Enable tracing if configured
        if (_settings.Ui.Trace != "off")
        {
            await context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(_settings.Ui.Timeout);
        
        return page;
    }
}
