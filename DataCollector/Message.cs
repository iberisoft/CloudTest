using MongoDB.Bson;
using System;

namespace DataCollector
{
    class Message
    {
        public Message()
        {
            Timestamp = DateTime.Now;
        }

        public ObjectId Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string DeviceId { get; set; }

        public string Topic { get; set; }

        public BsonDocument Payload { get; set; }
    }
}
