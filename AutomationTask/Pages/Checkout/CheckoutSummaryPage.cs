using AutomationTask.Configuration;
using AutomationTask.Helpers;
using AutomationTask.Pages.Cart.Controls;
using AutomationTask.Pages.Controls;
using Microsoft.Playwright;

namespace AutomationTask.Pages
{
    public class CheckoutSummaryPage : BasePage
    {
        public CheckoutSummaryPage(IPage page, TestSettings settings) : base(page, settings)
        {
        }

        private ILocator FinishButton => Page.Locator("[data-test='finish']");
        private ILocator PaymentInformation => Page.Locator("[data-test='payment-info-value']");
        private ILocator ShippingInformation => Page.Locator("[data-test='shipping-info-value']");
        private ILocator CartListSelector => Page.Locator("[data-test='cart-list']");
        private ILocator HeaderSelector => Page.Locator("[data-test='primary-header']");
        private ILocator SubtotalPrice => Page.Locator("[data-test='subtotal-label']");
        private ILocator TaxAmount => Page.Locator("[data-test='tax-label']");
        private ILocator TotalPrice => Page.Locator("[data-test='total-label']");

        public CartListControl CartList => new CartListControl(CartListSelector);
        public HeaderControl Header => new HeaderControl(HeaderSelector);

        public async Task FinishCheckoutAsync()
        {
            await FinishButton.ClickAsync();
        }
        public async Task<string> GetPaymentInformationAsync()
        {
            return await PaymentInformation.TextContentAsync() ?? string.Empty;
        }

        public async Task<string> GetShippingInformationAsync()
        {
            return await ShippingInformation.TextContentAsync() ?? string.Empty;
        }

        public async Task<decimal> GetSubtotalPriceAsync()
        {
            var subtotalText = await SubtotalPrice.TextContentAsync();
            return PriceHelper.ParsePrice(subtotalText);
        }

        public async Task<decimal> GetTaxAmountAsync()
        {
            var taxText = await TaxAmount.TextContentAsync();
            return PriceHelper.ParsePrice(taxText);
        }

        public async Task<decimal> GetTotalPriceAsync()
        {
            var totalText = await TotalPrice.TextContentAsync();
            return PriceHelper.ParsePrice(totalText);
        }

        public async Task WaitForPageLoadAsync()
        {
            await Page.WaitForURLAsync("**/checkout-step-two.html");
        }
    }
}
