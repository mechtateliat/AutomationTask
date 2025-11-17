using Microsoft.Playwright;

namespace AutomationTask.Pages.Cart.Controls
{
    public class CartListControl : BaseControl
    {
        public CartListControl(ILocator parent) : base(parent)
        {
        }

        private ILocator CartItems => Parent.Locator("[data-test='inventory-item']");

        public async Task<List<CartItemControl>> GetAllCartItemsAsync()
        {
            var count = await CartItems.CountAsync();
            var items = new List<CartItemControl>();

            for (int i = 0; i < count; i++)
            {
                items.Add(new CartItemControl(CartItems.Nth(i)));
            }

            return items;
        }

        public async Task<int> GetCartItemsCountAsync()
        {
            return await CartItems.CountAsync();
        }

        public async Task<CartItemControl?> GetCartItemByNameAsync(string productName)
        {
            var items = await GetAllCartItemsAsync();

            foreach (var item in items)
            {
                var name = await item.GetNameAsync();
                if (name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return null;
        }

        public async Task RemoveItemByNameAsync(string productName)
        {
            var item = await GetCartItemByNameAsync(productName);
            if (item == null)
                throw new Exception($"Product '{productName}' not found in cart");

            await item.RemoveAsync();
        }

        public async Task<List<string>> GetAllProductNamesAsync()
        {
            var items = await GetAllCartItemsAsync();
            var names = new List<string>();

            foreach (var item in items)
            {
                names.Add(await item.GetNameAsync());
            }

            return names;
        }
        public async Task<decimal> GetCartTotalPriceAsync()
        {
            var items = await GetAllCartItemsAsync();
            decimal total = 0;

            foreach (var item in items)
            {
                var price = await item.GetPriceAsync();
                var quantity = await item.GetQuantityAsync();
                total += price * quantity;
            }

            return total;
        }
    }
}
