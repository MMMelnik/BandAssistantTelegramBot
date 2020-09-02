

namespace BandAssistantBot.Models
{
    public class BandAssistantDatabaseSettings : IBandAssistantDatabaseSettings
    {
        public string RehearsalsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBandAssistantDatabaseSettings
    {
        string RehearsalsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
