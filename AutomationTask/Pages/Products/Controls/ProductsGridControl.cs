using Microsoft.Playwright;

namespace AutomationTask.Pages.Products.Controls
{
    public class ProductsGridControl : BaseControl
    {
        public ProductsGridControl(ILocator parent) : base(parent)
        {
        }

        private ILocator AllProducts => Parent.Locator("[data-test='inventory-item']");

        public async Task<List<ProductItemControl>> GetAllProductsAsync()
        {
            var count = await AllProducts.CountAsync();
            var products = new List<ProductItemControl>();

            for (int i = 0; i < count; i++)
            {
                products.Add(new ProductItemControl(AllProducts.Nth(i)));
            }

            return products;
        }

        public async Task<ProductItemControl?> GetProductByNameAsync(string name)
        {
            var products = await GetAllProductsAsync();

            foreach (var product in products)
            {
                var productName = await product.GetNameAsync();
                if (productName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return product;
                }
            }

            return null;
        }
    }
}