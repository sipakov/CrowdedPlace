namespace CrowdedPlace.Libraries.Domain.Models
{
    public class CrowdedPlaceDatabaseSettings : ICrowdedPlaceDatabaseSettings
    {
        public string ContactsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}