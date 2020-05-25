using System;

namespace OnlineDemonstrator.Libraries.Domain.Entities
{
    public class ObjectionableContent
    {
        public int Id { get; set; }

        public Guid DeviceId { get; set; }
        
        public Guid ObjectionableDeviceId { get; set; }
        
        public DateTime ObjectionablePosterCreatedDate { get; set; }
        
        public int ObjectionableReasonId { get; set; }
        
        public string Comment { get; set; }
    }
}