using Microsoft.Playwright;

namespace AutomationTask.Core.UI;

public interface IPageContext
{
    IPage Page { get; }
    IBrowser Browser { get; }
    Task InitializeAsync();
    Task CleanupAsync();
}

public class PageContext : IPageContext
{
    private readonly IBrowserFactory _browserFactory;
    private IBrowser? _browser;
    private IPage? _page;

    public PageContext(IBrowserFactory browserFactory)
    {
        _browserFactory = browserFactory;
    }

    public IPage Page => _page ?? throw new InvalidOperationException("Page not initialized. Call InitializeAsync first.");
    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Browser not initialized. Call InitializeAsync first.");

    public async Task InitializeAsync()
    {
        _browser = await _browserFactory.CreateBrowserAsync();
        _page = await _browserFactory.CreatePageAsync(_browser);
    }

    public async Task CleanupAsync()
    {
        if (_page != null)
        {
            await _page.Context.CloseAsync();
        }
        
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }
    }
}
