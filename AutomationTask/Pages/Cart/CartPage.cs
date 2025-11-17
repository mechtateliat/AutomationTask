using AutomationTask.Configuration;
using AutomationTask.Pages.Cart.Controls;
using AutomationTask.Pages.Controls;
using Microsoft.Playwright;

namespace AutomationTask.Pages
{
    public class CartPage : BasePage
    {
        public CartPage(IPage page, TestSettings settings) : base(page, settings)
        {
        }

        private ILocator CartListSelector => Page.Locator("[data-test='cart-list']");
        private ILocator CheckoutButton => Page.Locator("[data-test='checkout']");
        private ILocator ContinueShoppingButton => Page.Locator("[data-test='continue-shopping']");
        private ILocator HeaderSelector => Page.Locator("[data-test='primary-header']");

        public CartListControl CartList => new CartListControl(CartListSelector);
        public HeaderControl Header => new HeaderControl(HeaderSelector);

        public async Task WaitForPageLoadAsync()
        {
            await Page.WaitForURLAsync("**/cart.html");
        }
        public async Task CheckoutAsync()
        {
            await CheckoutButton.ClickAsync();
        }

        public async Task ContinueShoppingAsync()
        {
            await ContinueShoppingButton.ClickAsync();
        }

        public async Task<bool> IsCheckoutButtonEnabledAsync()
        {
            try
            {
                // Check if button is visible and enabled
                var isVisible = await CheckoutButton.IsVisibleAsync();
                if (!isVisible)
                {
                    return false;
                }
                
                var isEnabled = await CheckoutButton.IsEnabledAsync();
                var isDisabled = await CheckoutButton.IsDisabledAsync();
                
                // Return true only if enabled and not disabled
                return isEnabled && !isDisabled;
            }
            catch
            {
                return false;
            }
        }

    }
}
