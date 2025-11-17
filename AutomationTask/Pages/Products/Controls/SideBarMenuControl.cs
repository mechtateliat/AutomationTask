using Microsoft.Playwright;

namespace AutomationTask.Pages.Products.Controls
{
    public class SideBarMenuControl : BaseControl
    {
        public SideBarMenuControl(ILocator parent) : base(parent)
        {
        }

        private ILocator AllItemsLink => Parent.Locator("[data-test='inventory-sidebar-link']");
        private ILocator LogoutLink => Parent.Locator("[data-test='logout-sidebar-link']");
        private ILocator CloseButton => Parent.Locator(".bm-cross-button");

        public async Task OpenProductsPageAsync()
        {
            await AllItemsLink.ClickAsync();
        }
        public async Task LogoutAsync()
        {
            await LogoutLink.ClickAsync();
        }
        public async Task CloseMenuAsync()
        {
            await CloseButton.ClickAsync();
        }
    }
}
