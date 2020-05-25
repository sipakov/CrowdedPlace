using System;

namespace OnlineDemonstrator.Libraries.Domain.Entities
{
    public class Poster
    {
        public Guid DeviceId { get; set; }

        public Device Device { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
        
        public int DemonstrationId { get; set; }

        public Demonstration Demonstration { get; set; }

        public bool IsDeleted { get; set; }
    }
}