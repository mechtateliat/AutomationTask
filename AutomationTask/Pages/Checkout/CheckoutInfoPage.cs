using AutomationTask.Configuration;
using Microsoft.Playwright;

namespace AutomationTask.Pages
{
    public class CheckoutInfoPage : BasePage
    {
        public CheckoutInfoPage(IPage page, TestSettings settings) : base(page, settings)
        {
        }

        private ILocator FirstNameInput => Page.Locator("[data-test='firstName']");
        private ILocator LastNameInput => Page.Locator("[data-test='lastName']");
        private ILocator PostalCodeInput => Page.Locator("[data-test='postalCode']");
        private ILocator ContinueButton => Page.Locator("[data-test='continue']");

        public async Task EnterCheckoutInformationAsync(string firstName, string lastName, string postalCode)
        {
            await FirstNameInput.FillAsync(firstName);
            await LastNameInput.FillAsync(lastName);
            await PostalCodeInput.FillAsync(postalCode);
        }

        public async Task ContinueToNextStepAsync()
        {
            await ContinueButton.ClickAsync();
        }

        public async Task WaitForPageLoadAsync()
        {
            await Page.WaitForURLAsync("**/checkout-step-one.html");
        }
    }
}
