using AutomationTask.Pages;
using FluentAssertions;
using NUnit.Framework;

namespace AutomationTask.Tests.UI;

[TestFixture]
[Category("UI")]
[Category("Checkout")]
public class CheckoutTests : UiTestBase
{
    private LoginPage LoginPage => ResolvePage<LoginPage>();
    private ProductsPage ProductsPage => ResolvePage<ProductsPage>();
    private CartPage CartPage => ResolvePage<CartPage>();
    private CheckoutInfoPage CheckoutInfoPage => ResolvePage<CheckoutInfoPage>();
    private CheckoutSummaryPage CheckoutSummaryPage => ResolvePage<CheckoutSummaryPage>();
    private CheckoutComplete CheckoutCompletePage => ResolvePage<CheckoutComplete>();

    [Test]
    [Category("Smoke")]
    [Category("Checkout")]
    [Category("HighPriority")]
    [Description("Verify successful checkout with products")]
    public async Task SuccessfulCheckoutWithProducts()
    {
        LogInfo("Starting checkout test");

        await LoginPage.NavigateAsync();
        await LoginPage.WaitForPageLoadAsync();

        LogInfo("Login with standard user credentials");
        await LoginPage.LoginAsync(await LoginPage.GetFirstUsernameAsync(), await LoginPage.GetFirstPasswordAsync());

        await ProductsPage.WaitForPageLoadAsync();

        var products = await ProductsPage.ProductsGrid.GetAllProductsAsync();
        var firstProduct = products[0];
        var lastProduct = products[^1];

        var firstProductName = await firstProduct.GetNameAsync();
        var lastProductName = await lastProduct.GetNameAsync();

        (await ProductsPage.Header.GetShoppingCartItemCountAsync()).Should().Be(0, "Cart should be empty initially");

        LogInfo($"Adding first and last products to cart");
        await firstProduct.AddToCartAsync();
        await lastProduct.AddToCartAsync();

        (await ProductsPage.Header.GetShoppingCartItemCountAsync()).Should().Be(2, "Cart badge should display two products");

        LogInfo("Navigating to cart");
        await ProductsPage.Header.ClickShoppingCartButtonAsync();
        await CartPage.WaitForPageLoadAsync();

        var cartProducts = await CartPage.CartList.GetAllCartItemsAsync();

        cartProducts.Should().HaveCount(2, "Cart shoud display two products");

        (await cartProducts[0].GetNameAsync()).Should().Be(firstProductName, "First cart product should match the first added product");
        (await cartProducts[^1].GetNameAsync()).Should().Be(lastProductName, "Last cart product should match the last added product");

        LogInfo("Removing first product from cart");
        await cartProducts[0].RemoveAsync();
        (await ProductsPage.Header.GetShoppingCartItemCountAsync()).Should().Be(1, "Cart badge should display one product");
        (await CartPage.CartList.GetAllCartItemsAsync()).Should().HaveCount(1, "Cart should display one product after removal");

        LogInfo("Return to products page");
        await CartPage.ContinueShoppingAsync();
        await ProductsPage.WaitForPageLoadAsync();

        LogInfo("Verifying first product is no longer in cart");
        firstProduct = (await ProductsPage.ProductsGrid.GetAllProductsAsync())[0];
        var isInCart = await firstProduct.IsInCartAsync();
        isInCart.Should().Be(false, "First product is not in the cart");

        LogInfo("Add second to last item in cart");
        var secondToLastProduct = products[^2];
        var secondToLastProductName = await secondToLastProduct.GetNameAsync();
        await secondToLastProduct.AddToCartAsync();

        (await ProductsPage.Header.GetShoppingCartItemCountAsync()).Should().Be(2, "Cart badge should display two products again");

        LogInfo("Navigating to cart again");
        await ProductsPage.Header.ClickShoppingCartButtonAsync();
        await CartPage.WaitForPageLoadAsync();

        cartProducts = await CartPage.CartList.GetAllCartItemsAsync();
        cartProducts.Should().HaveCount(2, "cart should display two products ");
        var currentFirstItemName = await cartProducts[0].GetNameAsync();
        var currentSecondItemName = await cartProducts[1].GetNameAsync();

        currentFirstItemName.Should().Be(lastProductName,
               "first position should now be the last product (was second, moved to first after removal)");
        currentSecondItemName.Should().Be(secondToLastProductName,
            "second position should be the newly added product");

        var allCurrentProductNames = await CartPage.CartList.GetAllProductNamesAsync();
        allCurrentProductNames.Should().NotContain(firstProductName,
            "cart should NOT contain removed item");

        LogInfo("Go to checkout");
        await CartPage.CheckoutAsync();
        await CheckoutInfoPage.WaitForPageLoadAsync();

        LogInfo("Entering checkout information");
        await CheckoutInfoPage.EnterCheckoutInformationAsync("Horen", "Kirazyan", "1000");

        LogInfo("Proceed to checkout summary details");
        await CheckoutInfoPage.ContinueToNextStepAsync();
        await CheckoutSummaryPage.WaitForPageLoadAsync();

        LogInfo("Verifying checkout summary details");
        var checkoutItems = await CheckoutSummaryPage.CartList.GetAllCartItemsAsync();
        checkoutItems.Should().HaveCount(2, "checkout should show 2 items");

        (await CheckoutSummaryPage.GetPaymentInformationAsync()).Should().Be("SauceCard #31337",
            "payment information should match expected value");
        (await CheckoutSummaryPage.GetShippingInformationAsync()).Should().Be("Free Pony Express Delivery!",
            "shipping information should match expected value");

        var item1Price = await checkoutItems[0].GetPriceAsync();
        var item2Price = await checkoutItems[1].GetPriceAsync();
        var expectedSubtotal = item1Price + item2Price;

        var actualSubtotal = await CheckoutSummaryPage.GetSubtotalPriceAsync();
        var tax = await CheckoutSummaryPage.GetTaxAmountAsync();
        var total = await CheckoutSummaryPage.GetTotalPriceAsync();
        var cartTotal = await CheckoutSummaryPage.CartList.GetCartTotalPriceAsync();
        var expectedTotal = actualSubtotal + tax;

        cartTotal.Should().Be(expectedSubtotal,
            "cart total should equal subtotal before tax");

        actualSubtotal.Should().Be(expectedSubtotal,
                $"subtotal should equal sum of item prices: ${item1Price} + ${item2Price} = ${expectedSubtotal}");

        total.Should().Be(expectedTotal,
            $"total should equal subtotal + tax: ${actualSubtotal} + ${tax} = ${expectedTotal}");

        await CheckoutSummaryPage.CartList.GetCartTotalPriceAsync();

        LogInfo("Finishing checkout");
        await CheckoutSummaryPage.FinishCheckoutAsync();

        await CheckoutCompletePage.WaitForPageLoadAsync();
        var completionMessage = await CheckoutCompletePage.GetCompleteHeaderTextAsync();
        completionMessage.Should().Be("Thank you for your order!",
            "Completion message should confirm successful order placement");

        var completionSubMessage = await CheckoutCompletePage.GetCompleteTextAsync();
        completionSubMessage.Should().Be("Your order has been dispatched, and will arrive just as fast as the pony can get there!",
            "Completion sub-message should provide order dispatch information");

        LogInfo("Perform logout");
        await CheckoutCompletePage.Header.ClickMenuButtonAsync();
        await CheckoutCompletePage.SidebarMenu.LogoutAsync();

        await LoginPage.WaitForPageLoadAsync();
        bool areCredentialsVisible = await LoginPage.AreCredentialsVisibleAsync();
        areCredentialsVisible.Should().BeTrue("User should be logged out and see login credentials");

        LogPass("Checkout test completed successfully");
    }

    [Test]
    [Category("Checkout")]
    [Category("Negative")]
    [Category("HighPriority")]
    [Description("Verify checkout without added products is not possible")]
    public async Task NotPossibleCheckoutWithoutProduct()
    {
        LogInfo("Starting checkout test without product");
        await LoginPage.NavigateAsync();

        LogInfo("Login with standard user credentials");
        await LoginPage.LoginAsync(await LoginPage.GetFirstUsernameAsync(), await LoginPage.GetFirstPasswordAsync());

        await ProductsPage.WaitForPageLoadAsync();

        await ProductsPage.Header.ClickShoppingCartButtonAsync();
        await CartPage.WaitForPageLoadAsync();

        var isCheckoutEnabled = await CartPage.IsCheckoutButtonEnabledAsync();
        isCheckoutEnabled.Should().BeFalse("Checkout button should be disabled when cart is empty");

        LogPass("Checkout without added products verified succesfully");
    }
}
