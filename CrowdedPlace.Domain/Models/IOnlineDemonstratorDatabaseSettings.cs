namespace CrowdedPlace.Libraries.Domain.Models
{
    public interface ICrowdedPlaceDatabaseSettings
    {
        string ContactsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}