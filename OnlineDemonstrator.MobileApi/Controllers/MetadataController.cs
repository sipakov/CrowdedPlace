using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineDemonstrator.MobileApi.Models;

namespace OnlineDemonstrator.MobileApi.Controllers
{
    [Route("[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly IStringLocalizer<AppResources> _stringLocalizer;
        private readonly IStringLocalizer _stringLocalizer1;
        public MetadataController(IStringLocalizer<AppResources> stringLocalizer, IStringLocalizer stringLocalizer1)
        {
            _stringLocalizer = stringLocalizer;
            _stringLocalizer1 = stringLocalizer1;
        }

        [HttpGet("getLicense")]
        public ActionResult<Metadata> GetLicense()
        {
            var z = _stringLocalizer.WithCulture(new CultureInfo("es"))["NewDemonstrationPush"];
            var metadata = new Metadata
            {
                Value = _stringLocalizer["License"]
            };
            return metadata;
        }
        
        [HttpGet("getPrivacyPolicy")]
        public ActionResult<Metadata> GetPrivacyPolicy()
        {
            var metadata = new Metadata
            {
                Value = _stringLocalizer["PrivacyPolicy"]
            };
            return metadata;
        }
    }
}