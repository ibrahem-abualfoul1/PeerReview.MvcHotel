using System;
using System.Collections.Generic;
using System.Globalization;

namespace PeerReview.MvcHotel.Services
{
    public static class LocalizationExtensions
    {
        public static bool IsArabic()
        {
            return string.Equals(
                CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,
                "ar",
                StringComparison.OrdinalIgnoreCase);
        }

        public static string Localize(string? arabic, string? english, string fallback = "")
        {
            var primary = IsArabic() ? arabic : english;
            if (!string.IsNullOrWhiteSpace(primary))
            {
                return primary!;
            }

            var secondary = IsArabic() ? english : arabic;
            if (!string.IsNullOrWhiteSpace(secondary))
            {
                return secondary!;
            }

            return fallback ?? string.Empty;
        }

        public static IEnumerable<string> LocalizeOptions(string? optionsArabic, string? optionsEnglish)
        {
            var merged = Localize(optionsArabic, optionsEnglish, string.Empty);
            if (string.IsNullOrWhiteSpace(merged)) yield break;

            foreach (var option in merged.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = option.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    yield return trimmed;
                }
            }
        }
    }
}
