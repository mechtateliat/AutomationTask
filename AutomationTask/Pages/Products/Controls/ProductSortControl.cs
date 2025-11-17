using Microsoft.Playwright;

namespace AutomationTask.Pages.Products.Controls
{
    public class ProductSortControl : BaseControl
    {
        public ProductSortControl(ILocator parent) : base(parent)
        {
        }

        public async Task SortByAsync(SortOption sortOption)
        {
            string optionValue = sortOption switch
            {
                SortOption.NameAtoZ => "az",
                SortOption.NameZtoA => "za",
                SortOption.PriceLowToHigh => "lohi",
                SortOption.PriceHighToLow => "hilo",
                _ => "az"
            };

            await Parent.SelectOptionAsync(optionValue);
        }
    }

    public enum SortOption
    {
        NameAtoZ,
        NameZtoA,
        PriceLowToHigh,
        PriceHighToLow
    }
}
