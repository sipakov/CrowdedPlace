namespace OnlineDemonstrator.Libraries.Domain.Models
{
    public interface IOnlineDemonstratorDatabaseSettings
    {
        string ContactsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}