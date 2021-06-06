using System.IO.Ports;

namespace DeviceConfig
{
    static class SerialPortExtention
    {
        public static SerialPort OpenPort()
        {
            var port = new SerialPort(Properties.Settings.Default.PortName)
            {
                ReadTimeout = 3000
            };
            port.Open();
            return port;
        }

        public static void EnableServerDebug(this SerialPort port, bool enable)
        {
            port.WriteLine($"SD{(enable ? 1 : 0)}");
        }

        public static string ReadParameter(this SerialPort port, string name)
        {
            port.WriteLine($"{name}?");
            return ParseResponse(port, name);
        }

        public static void WriteParameter(this SerialPort port, string name, string value)
        {
            port.WriteLine($"{name}={value}");
            ParseResponse(port, name);
        }

        private static string ParseResponse(SerialPort port, string name)
        {
            var prefix = $"{name}=";
            while (true)
            {
                var line = port.ReadLine();
                if (line.StartsWith(prefix))
                {
                    return line.Substring(prefix.Length);
                }
            }
        }
    }
}
