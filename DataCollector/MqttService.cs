using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MqttHelper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataCollector
{
    class MqttService : IHostedService
    {
        readonly Settings m_Settings;
        readonly DbContext m_DbContext;
        readonly NetClient m_NetClient;


        public MqttService(Settings settings)
        {
            m_Settings = settings;
            m_DbContext = new DbContext(Environment.GetEnvironmentVariable("DB_CONNECTION") ?? m_Settings.DbConnection);
            m_NetClient = new NetClient(m_Settings.BaseTopic);
            m_NetClient.MessageReceived += NetClient_MessageReceived;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await m_NetClient.StartAsync(Environment.GetEnvironmentVariable("BROKER_HOST") ?? m_Settings.BrokerHost);
            await m_NetClient.SubscribeAsync("#");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await m_NetClient.StopAsync();
        }

        private void NetClient_MessageReceived(object sender, NetMessage e)
        {
            Console.WriteLine($"{e.Topic}: {e.Payload}");

            var tokens = e.Topic.Split('/');
            var message = new Message { DeviceId = tokens[0], Topic = tokens[1], Payload = BsonDocument.Parse(e.Payload) };
            m_DbContext.Messages.InsertOne(message);
        }
    }
}
