using System;

namespace CrowdedPlace.Libraries.Domain.Entities
{
    public class Demonstration
    {
        public int Id { get; set; }

        public DateTime DemonstrationDate { get; set; }
        
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string AreaName { get; set; }

        public bool IsDeleted { get; set; }
    }
}