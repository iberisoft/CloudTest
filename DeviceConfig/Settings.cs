using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;

namespace DeviceConfig
{
    public class Settings : INotifyPropertyChanged
    {
        string m_PortName = "";

        public string PortName
        {
            get => m_PortName;
            set
            {
                if (m_PortName != value)
                {
                    m_PortName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PortName)));
                }
            }
        }

        string m_SketchPath = "";

        public string SketchPath
        {
            get => m_SketchPath;
            set
            {
                if (m_SketchPath != value)
                {
                    m_SketchPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SketchPath)));
                }
            }
        }

        static Settings m_Default;

        public static Settings Default
        {
            get
            {
                if (m_Default == null)
                {
                    try
                    {
                        m_Default = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FilePath));
                    }
                    catch
                    {
                        m_Default = new Settings();
                    }
                }
                return m_Default;
            }
        }

        public void Save() => File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));

        private static string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(Settings) + ".json");

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
