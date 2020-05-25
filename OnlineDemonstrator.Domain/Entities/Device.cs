using System;

namespace OnlineDemonstrator.Libraries.Domain.Entities
{
    public class Device
    {
        public Guid Id { get; set; }

        public int OsId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsLicenseActivated { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsBanned { get; set; }
        
    }
}