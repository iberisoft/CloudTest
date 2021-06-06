using System.Collections.Generic;
using System.ComponentModel;

namespace DeviceConfig
{
    class DeviceSettings : INotifyPropertyChanged
    {
        string m_ServerHost;

        public string ServerHost
        {
            get => m_ServerHost;
            set
            {
                if (m_ServerHost != value)
                {
                    m_ServerHost = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerHost)));
                }
            }
        }

        int m_ServerPort;

        public int ServerPort
        {
            get => m_ServerPort;
            set
            {
                if (m_ServerPort != value)
                {
                    m_ServerPort = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerPort)));
                }
            }
        }

        string m_TopicPrefix;

        public string TopicPrefix
        {
            get => m_TopicPrefix;
            set
            {
                if (m_TopicPrefix != value)
                {
                    m_TopicPrefix = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TopicPrefix)));
                }
            }
        }

        string m_DeviceId;

        public string DeviceId
        {
            get => m_DeviceId;
            set
            {
                if (m_DeviceId != value)
                {
                    m_DeviceId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceId)));
                }
            }
        }

        List<NetworkSettings> m_Networks;

        public List<NetworkSettings> Networks
        {
            get => m_Networks;
            set
            {
                m_Networks = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Networks)));
            }
        }

        public void Load()
        {
            using (var port = SerialPortExtention.OpenPort())
            {
                port.EnableServerDebug(false);
                ServerHost = port.ReadParameter("SH");
                ServerPort = int.Parse(port.ReadParameter("SP"));
                TopicPrefix = port.ReadParameter("TP");
                DeviceId = port.ReadParameter("ID");
                port.EnableServerDebug(true);
            }
        }

        public void Save()
        {
            using (var port = SerialPortExtention.OpenPort())
            {
                port.EnableServerDebug(false);
                port.WriteParameter("SH", ServerHost);
                port.WriteParameter("SP", ServerPort.ToString());
                port.WriteParameter("TP", TopicPrefix);
                port.WriteParameter("ID", DeviceId);
                port.WriteLine("SS");
                port.EnableServerDebug(true);
            }
        }

        public static void Restart()
        {
            using (var port = SerialPortExtention.OpenPort())
            {
                port.WriteLine("RST");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
