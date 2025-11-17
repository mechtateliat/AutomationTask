using AutomationTask.Configuration;
using AutomationTask.Pages.Controls;
using AutomationTask.Pages.Products.Controls;
using Microsoft.Playwright;

namespace AutomationTask.Pages
{
    public class ProductsPage : BasePage
    {
        public ProductsPage(IPage page, TestSettings settings) : base(page, settings)
        {
        }

        private ILocator HeaderSelector => Page.Locator("[data-test='primary-header']");
        private ILocator SidebarMenuSelector => Page.Locator(".bm-menu");
        private ILocator SortSelector => Page.Locator("[data-test='product-sort-container']");
        private ILocator ProductsGridSelector => Page.Locator("[data-test='inventory-list']");

        public HeaderControl Header => new HeaderControl(HeaderSelector);
        public SideBarMenuControl SidebarMenu => new SideBarMenuControl(SidebarMenuSelector);
        public ProductSortControl ProductSort => new ProductSortControl(SortSelector);
        public ProductsGridControl ProductsGrid => new ProductsGridControl(ProductsGridSelector);

        public async Task WaitForPageLoadAsync()
        {
            await Page.WaitForURLAsync("**/inventory.html");
        }

        public async Task AddProductToCartByNameAsync(string productName)
        {
            var product = await ProductsGrid.GetProductByNameAsync(productName);
            if (product == null)
                throw new Exception($"Product '{productName}' not found");

            await product.AddToCartAsync();
        }

        public async Task AddProductToCartByIndex(int index)
        {
            var products = await ProductsGrid.GetAllProductsAsync();
            if (index < 0 || index >= products.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range");
            await products[index].AddToCartAsync();
        }
    }
}
