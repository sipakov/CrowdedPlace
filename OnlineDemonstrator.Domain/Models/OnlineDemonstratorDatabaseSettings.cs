namespace OnlineDemonstrator.Libraries.Domain.Models
{
    public class OnlineDemonstratorDatabaseSettings : IOnlineDemonstratorDatabaseSettings
    {
        public string ContactsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}