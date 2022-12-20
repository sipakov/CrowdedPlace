using System.Globalization;
using Microsoft.Extensions.Localization;

namespace OnlineDemonstrator.MobileApi.Extensions
{
    public static class LocalizationExtension
    {
        public static LocalizedString GetString(this IStringLocalizer stringLocalizer, string targetCulture, string keyName, params object[] arguments) {
            var cultureInfo = string.IsNullOrEmpty(targetCulture) ? CultureInfo.CurrentUICulture : CultureInfo.GetCultureInfo(targetCulture);
            var cultureInfoOriginal = CultureInfo.CurrentUICulture;
            try {
                CultureInfo.CurrentUICulture = cultureInfo;
                CultureInfo.CurrentCulture = cultureInfo;
                return stringLocalizer.GetString(keyName, arguments);
            }
            finally {
                CultureInfo.CurrentUICulture = cultureInfoOriginal;
                CultureInfo.CurrentCulture = cultureInfoOriginal;
            }
        }
    }
}