using AutomationTask.Extensions;
using Microsoft.Playwright;

namespace AutomationTask.Pages.Login.Controls
{
    public class PasswordControl : BaseControl
    {
        public PasswordControl(ILocator parent) : base(parent)
        {
        }

        public async Task<string> GetFirstPasswordAsync()
        {
            return await Parent.GetFirstLineAfterHeaderAsync();
        }
    }
}
