namespace AutomationTask.Helpers
{
    public static class PriceHelper
    {
        /// <summary>
        /// Parses price text (e.g. "$29.99", "Item total: $45.98") to decimal
        /// Removes common prefixes and currency symbols
        /// Uses InvariantCulture to handle decimal points correctly
        /// </summary>
        public static decimal ParsePrice(string? priceText)
        {
            if (string.IsNullOrWhiteSpace(priceText))
                return 0m;

            // Remove common prefixes
            var cleanText = priceText
                .Replace("Item total:", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Tax:", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Total:", "", StringComparison.OrdinalIgnoreCase)
                .Replace("$", "")
                .Trim();

            return decimal.TryParse(cleanText,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var price) ? price : 0m;
        }
    }
}
