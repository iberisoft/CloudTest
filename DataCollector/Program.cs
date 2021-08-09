using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MqttHelper;
using System;
using System.Threading.Tasks;

namespace DataCollector
{
    static class Program
    {
        static DbContext m_DbContext;

        static async Task Main()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var settings = config.Get<Settings>();

            m_DbContext = new DbContext(Environment.GetEnvironmentVariable("DB_CONNECTION") ?? settings.DbConnection);

            var netClient = new NetClient(settings.BaseTopic);
            await netClient.StartAsync(Environment.GetEnvironmentVariable("BROKER_HOST") ?? settings.BrokerHost);
            await netClient.SubscribeAsync("#");
            netClient.MessageReceived += NetClient_MessageReceived;

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Press Enter to quit...");
                Console.ReadLine();
            }
            else
            {
                await Task.Delay(-1);
            }

            await netClient.StopAsync();
        }

        private static void NetClient_MessageReceived(object sender, NetMessage e)
        {
            Console.WriteLine($"{e.Topic}: {e.Payload}");

            var tokens = e.Topic.Split('/');
            var message = new Message { DeviceId = tokens[0], Topic = tokens[1], Payload = BsonDocument.Parse(e.Payload) };
            m_DbContext.Messages.InsertOne(message);
        }
    }
}
