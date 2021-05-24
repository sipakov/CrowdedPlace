using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineDemonstrator.MobileApi.Models;

namespace OnlineDemonstrator.MobileApi.Controllers
{
    [Route("[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly IStringLocalizer<AppResources> _stringLocalizer;

        public MetadataController(IStringLocalizer<AppResources> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet("getLicense")]
        public ActionResult<Metadata> GetLicense()
        {
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