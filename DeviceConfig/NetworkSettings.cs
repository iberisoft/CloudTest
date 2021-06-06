using System;
using System.Collections.Generic;

namespace DeviceConfig
{
    class NetworkSettings
    {
        public NetworkSettings(string ssid)
        {
            Ssid = ssid;
        }

        public string Ssid { get; }

        public static IEnumerable<NetworkSettings> Load()
        {
            using (var port = SerialPortExtention.OpenPort())
            {
                port.EnableServerDebug(false);
                foreach (var ssid in port.ReadParameter("NW").Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return new NetworkSettings(ssid);
                }
                port.EnableServerDebug(true);
            }
        }

        public void Save(string password)
        {
            using (var port = SerialPortExtention.OpenPort())
            {
                port.EnableServerDebug(false);
                port.WriteParameter("NW", Ssid + "+" + password);
                port.WriteLine("SS");
                port.EnableServerDebug(true);
            }
        }

        public void Forget()
        {
            using (var port = SerialPortExtention.OpenPort())
            {
                port.EnableServerDebug(false);
                port.WriteParameter("NW", Ssid);
                port.WriteLine("SS");
                port.EnableServerDebug(true);
            }
        }
    }
}
