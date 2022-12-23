using System;

namespace CrowdedPlace.Libraries.Domain.Dto
{
    public class DeviceIn
    {
        public string DeviceId { get; set; }

        public bool IsLicenseActivated { get; set; }

        public string BaseOs { get; set; }

        public string FcmToken { get; set; }

        public string Locale { get; set; }
    }
}