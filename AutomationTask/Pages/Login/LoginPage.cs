using AutomationTask.Configuration;
using AutomationTask.Pages.Login.Controls;
using Microsoft.Playwright;

namespace AutomationTask.Pages;

public class LoginPage : BasePage
{
    private ILocator UsernameInput => Page.Locator("form input[type='text']");
    private ILocator PasswordInput => Page.Locator("form input[type='password']");
    private ILocator LoginButton => Page.Locator("[data-test='login-button']");
    private ILocator CredentialsControlSelector => Page.Locator("[data-test='login-credentials']");
    private ILocator PasswordControlSelector => Page.Locator("[data-test='login-password']");
    private CredentialsControl CredentialsControl => new CredentialsControl(CredentialsControlSelector);
    private PasswordControl PasswordControl => new PasswordControl(PasswordControlSelector);

    public LoginPage(IPage page, TestSettings settings) : base(page, settings)
    {
    }

    public async Task NavigateAsync()
    {
        await NavigateToAsync("");
    }

    public async Task WaitForPageLoadAsync()
    {
        var expectedUrl = Settings.Ui.BaseUrl?.TrimEnd('/');
        await Page.WaitForURLAsync($"{expectedUrl}/");
    }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<bool> AreCredentialsVisibleAsync()
    {
        return await CredentialsControl.IsVisible();
    }

    public async Task<string> GetFirstUsernameAsync()
    {
        return await CredentialsControl.GetFirstUsernameAsync();
    }
    public async Task<string> GetFirstPasswordAsync()
    {
        return await PasswordControl.GetFirstPasswordAsync();
    }
}
