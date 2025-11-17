using Microsoft.Playwright;

namespace AutomationTask.Extensions
{
    public static class LocatorExtensions
    {
        public static async Task<string> GetFirstLineAfterHeaderAsync(this ILocator locator)
        {
            var innerText = await locator.InnerTextAsync();

            var lines = innerText
                .Split('\n')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .ToList();

            return lines.Count > 1 ? lines[1] : lines.LastOrDefault() ?? string.Empty;
        }
    }
}