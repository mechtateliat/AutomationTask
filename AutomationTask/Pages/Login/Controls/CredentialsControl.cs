using AutomationTask.Extensions;
using Microsoft.Playwright;

namespace AutomationTask.Pages.Login.Controls
{
    public class CredentialsControl : BaseControl
    {

        public CredentialsControl(ILocator parent) : base(parent)
        {
        }

        public async Task<string> GetFirstUsernameAsync()
        {
            return await Parent.GetFirstLineAfterHeaderAsync();
        }
    }
}
