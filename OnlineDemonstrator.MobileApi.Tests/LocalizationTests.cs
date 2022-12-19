using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace OnlineDemonstrator.MobileApi.Tests
{
    public class LocalizationTests
    {
        [Test]
        public void String_Localization_Test()
        {
            var options = Options.Create(new LocalizationOptions {ResourcesPath = "Localization"});
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            var stringLocalizer = new StringLocalizer<AppResources>(factory);
            
            const string expectedValueRu = "Новый плакат в вашем митинге";
            const string targetLocaleRu = "ru";
            const string expectedValueEn = "New poster";
            const string targetLocaleEn = "en";

            var actualValueRu = stringLocalizer.WithCulture(new CultureInfo(targetLocaleRu))["NewPosterPush"].Value;
            var actualValueEn = stringLocalizer.WithCulture(new CultureInfo(targetLocaleEn))["NewPosterPush"].Value;

            Assert.AreEqual(expectedValueRu, actualValueRu);
            Assert.AreEqual(expectedValueEn, actualValueEn);
        }
    }
}