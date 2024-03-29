using System;

namespace CrowdedPlace.Libraries.Domain.Dto
{
    public class PosterIn
    {
        public string Name { get; set; }
        
        public string Title { get; set; }

        public string Message { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string AreaName { get; set; }
        public string DeviceId { get; set; }

        public int DemonstrationId { get; set; }

        public string BaseOs { get; set; }

        public string Locale { get; set; }
    }
}