using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DataCollector
{
    class DbContext
    {
        public DbContext(string connectionString)
        {
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register(nameof(CamelCaseElementNameConvention), conventionPack, _ => true);

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("collector");
            Messages = database.GetCollection<Message>("messages");
        }

        public IMongoCollection<Message> Messages { get; }
    }
}
