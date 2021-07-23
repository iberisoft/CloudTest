using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DataCollector
{
    class DbContext
    {
        public DbContext(string host)
        {
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register(nameof(CamelCaseElementNameConvention), conventionPack, _ => true);

            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host)
            };
            var client = new MongoClient(settings);

            var database = client.GetDatabase("collector");
            Messages = database.GetCollection<Message>("messages");
        }

        public IMongoCollection<Message> Messages { get; }
    }
}
