using DeviceConfig.Data;
using ManagedNativeWifi;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeviceConfig
{
    /// <summary>
    /// Interaction logic for NetworkWindow.xaml
    /// </summary>
    public partial class NetworkWindow : Window
    {
        public NetworkWindow()
        {
            InitializeComponent();
        }

        public void GetNetwork(Network network)
        {
            network.Ssid = SsidBox.Text;
            network.Password = PasswordBox.Password;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SsidBox.ItemsSource = NativeWifi.EnumerateBssNetworks().Where(network => network.Frequency < 2500000).Select(network => network.Ssid.ToString());
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DialogResult == true && SsidBox.Text.Length == 0)
            {
                e.Cancel = true;
            }
        }

        private void Ok(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
