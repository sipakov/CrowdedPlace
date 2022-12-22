using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.MobileApi.Implementations;
using OnlineDemonstrator.MobileApi.Interfaces;

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

           var actualValueRu = Extensions.LocalizationExtension.GetString(stringLocalizer, targetLocaleRu, "NewPosterPush").Value;
           var actualValueEn = Extensions.LocalizationExtension.GetString(stringLocalizer, targetLocaleEn, "NewPosterPush").Value;

            Assert.AreEqual(expectedValueEn, actualValueEn);
            Assert.AreEqual(expectedValueRu, actualValueRu);
        }

        [Test]
        public void GenerateLocalizedPushes_Test()
        {
            var options = Options.Create(new LocalizationOptions {ResourcesPath = "Localization"});
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            var postService = new PosterService(new Mock<IContextFactory<ApplicationContext>>().Object, new Mock<IDemonstrationService>().Object,
                new Mock<IDistanceCalculator>().Object, new StringLocalizer<AppResources>(factory), new Mock<IReverseGeoCodingPlaceGetter>().Object,
                new Mock<IPushNotifier>().Object, new NullLogger<PosterService>());

            var fcmTokensToLocale = new Dictionary<string, string>();
            fcmTokensToLocale.Add("test_token1", "ru");
            fcmTokensToLocale.Add("test_token2", "en");

            const string expectedValueRu = "Новый плакат в вашем митинге (India)";
            const string expectedValueEn = "New poster (India)";

            var pushes = postService.GenerateLocalizedPushes(fcmTokensToLocale, "NewPosterPush", "test_body", "India").ToList();
            
            var actualValueRu = pushes[0].notification.title;
            var actualValueEn = pushes[1].notification.title;
            
            Assert.AreEqual(expectedValueRu, actualValueRu);
            Assert.AreEqual(expectedValueEn, actualValueEn);   
        }
        
    }
}