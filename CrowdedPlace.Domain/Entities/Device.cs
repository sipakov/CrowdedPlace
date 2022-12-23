using System;

namespace CrowdedPlace.Libraries.Domain.Entities
{
    public class Device
    {
        public string Id { get; set; }

        public int OsId { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public DateTime LastVisitDate { get; set; }

        public string FcmToken { get; set; }

        public bool IsNotSendNotifications { get; set; }

        public bool IsLicenseActivated { get; set; }

        public string Locale { get; set; }

        public int SharedCount { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsBanned { get; set; }
        
    }
}