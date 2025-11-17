using Microsoft.Playwright;

namespace AutomationTask.Pages.Controls
{
    public class HeaderControl : BaseControl
    {
        public HeaderControl(ILocator parent) : base(parent)
        {
        }

        private ILocator MenuButton => Parent.Locator("#react-burger-menu-btn");   
        private ILocator ShoppingCartButton => Parent.Locator("[data-test='shopping-cart-link']");
        private ILocator ShoppingCartBadge => Parent.Locator("[data-test='shopping-cart-badge']");

        public async Task ClickMenuButtonAsync()
        {
            await MenuButton.ClickAsync();
        }
        public async Task ClickShoppingCartButtonAsync()
        {
            await ShoppingCartButton.ClickAsync();
        }

        public async Task<int> GetShoppingCartItemCountAsync()
        {
            if (await ShoppingCartBadge.IsVisibleAsync())
            {
                var badgeText = await ShoppingCartBadge.InnerTextAsync();
                if (int.TryParse(badgeText, out int itemCount))
                {
                    return itemCount;
                }
            }
            return 0;
        }
    }
}
