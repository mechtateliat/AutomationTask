using Microsoft.Playwright;
using AutomationTask.Configuration;

namespace AutomationTask.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly TestSettings Settings;

    protected BasePage(IPage page, TestSettings settings)
    {
        Page = page;
        Settings = settings;
    }

    protected async Task NavigateToAsync(string path = "")
    {
        var baseUrl = Settings.Ui.BaseUrl;
        var url = string.IsNullOrEmpty(path) ? baseUrl : $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
        await Page.GotoAsync(url);
    }
}
