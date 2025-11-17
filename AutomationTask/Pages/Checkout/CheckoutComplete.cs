using AutomationTask.Configuration;
using AutomationTask.Pages.Controls;
using AutomationTask.Pages.Products.Controls;
using Microsoft.Playwright;

namespace AutomationTask.Pages
{
    public class CheckoutComplete : BasePage
    {
        public CheckoutComplete(IPage page, TestSettings settings) : base(page, settings)
        {
        }

        private ILocator CompleteHeader => Page.Locator("[data-test='complete-header']");
        private ILocator CompleteText => Page.Locator("[data-test='complete-text']");
        private ILocator BackHomeButton => Page.Locator("[data-test='back-to-products']");
        private ILocator HeaderSelector => Page.Locator("[data-test='primary-header']");
        private ILocator SidebarSelector => Page.Locator(".bm-menu");

        public HeaderControl Header => new HeaderControl(HeaderSelector);
        public SideBarMenuControl SidebarMenu => new SideBarMenuControl(SidebarSelector);

        public async Task<string> GetCompleteHeaderTextAsync()
        {
            return await CompleteHeader.TextContentAsync() ?? string.Empty;
        }

        public async Task<string> GetCompleteTextAsync()
        {
            return await CompleteText.TextContentAsync() ?? string.Empty;
        }

        public async Task BackToHomeAsync()
        {
            await BackHomeButton.ClickAsync();
        }

        public async Task WaitForPageLoadAsync()
        {
            await Page.WaitForURLAsync("**/checkout-complete.html");
        }

    }
}
