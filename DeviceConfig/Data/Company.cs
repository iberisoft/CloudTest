using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DeviceConfig.Data
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [MaxLength(4)]
        public string Code { get; set; }

        [Required]
        [MaxLength(64)]
        public string ServerHost { get; set; }

        public int ServerPort { get; set; }

        [Required]
        [MaxLength(32)]
        public string TopicPrefix { get; set; }

        public int LastDeviceId { get; set; }

        public string FullDeviceId => $"{Code}{LastDeviceId:d5}";

        public virtual ICollection<Network> Networks { get; set; }

        public override string ToString() => Name;

        public object ToDeviceSettings() => new
        {
            serverHost = ServerHost,
            serverPort = ServerPort,
            topicPrefix = TopicPrefix,
            deviceId = FullDeviceId,
            networks = Networks.Select(network => network.ToDeviceSettings()).ToList()
        };
    };
}
