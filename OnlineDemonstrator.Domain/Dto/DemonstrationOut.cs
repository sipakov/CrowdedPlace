using System;

namespace OnlineDemonstrator.Libraries.Domain.Dto
{
    public class DemonstrationOut
    {
        public int Id { get; set; }

        public DateTime DemonstrationDate { get; set; }
        
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string CountryName { get; set; }
        public string DetailName { get; set; }

        public int PostersCount { get; set; }

        public string ExpDays { get; set; }

        public bool IsExpired { get; set; }

        public string DemonstrationTitle { get; set; }
    }
}