using AutomationTask.Pages;
using AutomationTask.Pages.Products.Controls;
using FluentAssertions;
using NUnit.Framework;

namespace AutomationTask.Tests.UI;

[TestFixture]
[Category("UI")]
[Category("Sorting")]
public class SortingTests : UiTestBase
{
    private LoginPage LoginPage => ResolvePage<LoginPage>();
    private ProductsPage ProductsPage => ResolvePage<ProductsPage>();

    [Test]
    [Category("Sorting")]
    [Category("MediumPriority")]
    [Description("Verify product sorting by price high to low")]
    public async Task ProductSorting_ByPrice_HighToLow()
    {
        LogInfo("Starting product sorting test by price high to low");
        await LoginPage.NavigateAsync();

        LogInfo("Login with standard user credentials");
        await LoginPage.LoginAsync(await LoginPage.GetFirstUsernameAsync(), await LoginPage.GetFirstPasswordAsync());
        await ProductsPage.WaitForPageLoadAsync();

        LogInfo("Sorting products by Price: High to Low");
        await ProductsPage.ProductSort.SortByAsync(SortOption.PriceHighToLow);

        LogInfo("Verifying products are sorted correctly");
        var products = await ProductsPage.ProductsGrid.GetAllProductsAsync();
        decimal previousPrice = decimal.MaxValue;
        foreach (var product in products)
        {
            var currentPrice = await product.GetPriceAsync();
            currentPrice.Should().BeLessThanOrEqualTo(previousPrice, "Products should be sorted from high to low price");
            previousPrice = currentPrice;
        }

        LogPass("Product sorting by price high to low verified successfully");
    }
}
