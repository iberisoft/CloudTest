using System.ComponentModel.DataAnnotations;

namespace DeviceConfig.Data
{
    public class Network
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Ssid { get; set; }

        [MaxLength(64)]
        public string Password { get; set; }

        [Required]
        public virtual Company Company { get; set; }

        public override string ToString() => Ssid;

        public object ToDeviceSettings() => new
        {
            ssid = Ssid,
            password = Password
        };
    }
}
