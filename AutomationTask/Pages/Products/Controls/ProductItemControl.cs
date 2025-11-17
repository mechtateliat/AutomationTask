using AutomationTask.Pages.Controls;
using Microsoft.Playwright;

namespace AutomationTask.Pages.Products.Controls
{
    public class ProductItemControl : InventoryItemControl
    {
        public ProductItemControl(ILocator parent) : base(parent)
        {
        }

        private ILocator Image => Parent.Locator(".inventory_item_img > a > img");
        private ILocator AddToCartButton => Parent.Locator("button[data-test^='add-to-cart']");
        private ILocator RemoveFromCartButton => Parent.Locator("button[data-test^='remove']");

        public async Task AddToCartAsync()
        {
            await AddToCartButton.ClickAsync();
        }

        public async Task RemoveFromCartAsync()
        {
            await RemoveFromCartButton.ClickAsync();
        }

        public async Task<string> GetImageSrcAsync()
        {
            return await Image.GetAttributeAsync("src") ?? string.Empty;
        }

        public async Task<bool> IsInCartAsync()
        {
            return await RemoveFromCartButton.IsVisibleAsync();
        }

        public async Task OpenProductDetailsAsync()
        {
            await Name.ClickAsync();
        }
    }
}
