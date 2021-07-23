using MongoDB.Bson;

namespace DataCollector
{
    class Message
    {
        public ObjectId Id { get; set; }

        public string DeviceId { get; set; }

        public string Topic { get; set; }

        public BsonDocument Payload { get; set; }
    }
}
