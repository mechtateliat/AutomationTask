using AutomationTask.Pages.Controls;
using Microsoft.Playwright;

namespace AutomationTask.Pages.Cart.Controls
{
    public class CartItemControl : InventoryItemControl
    {
        public CartItemControl(ILocator parent) : base(parent)
        {
        }
        private ILocator Quantity => Parent.Locator("[data-test='item-quantity']");
        private ILocator RemoveButton => Parent.Locator("button[data-test^='remove']");

        public async Task<int> GetQuantityAsync()
        {
            var qtyText = await Quantity.TextContentAsync();
            return int.TryParse(qtyText, out var qty) ? qty : 0;
        }

        public async Task RemoveAsync()
        {
            await RemoveButton.ClickAsync();
        }
    }
}
