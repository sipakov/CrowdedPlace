using System;
using System.Reflection;

namespace CrowdedPlace.Libraries.Domain.Dto
{
    public class PosterOut
    {
                
        public string DeviceId { get; set; }
        
        public int DemonstrationId { get; set; }
        
        public string Name { get; set; }
        
        public string Title { get; set; }

        public string Message { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public bool IsExpired { get; set; }
    }
}