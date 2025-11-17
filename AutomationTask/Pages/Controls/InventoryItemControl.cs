using AutomationTask.Helpers;
using Microsoft.Playwright;

namespace AutomationTask.Pages.Controls
{
    public abstract class InventoryItemControl : BaseControl
    {
        protected InventoryItemControl(ILocator parent) : base(parent)
        {
        }
        protected ILocator Name => Parent.Locator("[data-test='inventory-item-name']");
        protected ILocator Description => Parent.Locator("[data-test='inventory-item-desc']");
        protected ILocator Price => Parent.Locator("[data-test='inventory-item-price']");

        public async Task<string> GetNameAsync()
        {
            return await Name.TextContentAsync() ?? string.Empty;
        }

        public async Task<string> GetDescriptionAsync()
        {
            return await Description.TextContentAsync() ?? string.Empty;
        }

        public async Task<decimal> GetPriceAsync()
        {
            var priceText = await Price.TextContentAsync();
            return PriceHelper.ParsePrice(priceText);
        }

        public async Task ClickNameAsync()
        {
            await Name.ClickAsync();
        }
    }
}