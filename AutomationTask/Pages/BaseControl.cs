using Microsoft.Playwright;

namespace AutomationTask.Pages
{
    // Base class for UI controls in automation
    public abstract class BaseControl
    {
        protected BaseControl(ILocator parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        protected ILocator Parent { get; }

        public Task<bool> IsVisible() => Parent.IsVisibleAsync();
    }
}