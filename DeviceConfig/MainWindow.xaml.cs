using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeviceConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!IsEnabled)
            {
                e.Cancel = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        public string[] PortNames => SerialPort.GetPortNames();

        private void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = null;
        }

        readonly DeviceSettings m_DeviceSettings = new DeviceSettings();

        private async void LoadContent(object sender, RoutedEventArgs e)
        {
            await DoWork(() =>
            {
                m_DeviceSettings.Load();
                m_DeviceSettings.Networks = NetworkSettings.Load().ToList();
            });

            DataContext = m_DeviceSettings;
        }

        private async void SaveSettings(object sender, RoutedEventArgs e)
        {
            await DoWork(() =>
            {
                m_DeviceSettings.Save();
                m_DeviceSettings.Load();
            });
        }

        private async void AddNetwork(object sender, RoutedEventArgs e)
        {
            var window = new NetworkWindow
            {
                Owner = this
            };
            if (window.ShowDialog() == true)
            {
                var network = new NetworkSettings(window.Ssid);
                await DoWork(() =>
                {
                    network.Save(window.Password);
                    m_DeviceSettings.Networks = NetworkSettings.Load().ToList();
                });
            }
        }

        private async void SetNetworkPassword(object sender, RoutedEventArgs e)
        {
            var window = new NetworkWindow
            {
                Owner = this
            };
            var network = (NetworkSettings)((FrameworkElement)sender).DataContext;
            window.Ssid = network.Ssid;
            if (window.ShowDialog() == true)
            {
                await DoWork(() =>
                {
                    network.Save(window.Password);
                    m_DeviceSettings.Networks = NetworkSettings.Load().ToList();
                });
            }
        }

        private async void ForgetNetwork(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Forget?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var network = (NetworkSettings)((FrameworkElement)sender).DataContext;
                await DoWork(() =>
                {
                    network.Forget();
                    m_DeviceSettings.Networks = NetworkSettings.Load().ToList();
                });
            }
        }

        private void BrowseFirmware(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = Properties.Settings.Default.FirmwarePath;
            if (dialog.ShowDialog() == true)
            {
                Properties.Settings.Default.FirmwarePath = dialog.FileName;
            }
        }

        private async void UploadFirmware(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Properties.Settings.Default.FirmwarePath))
            {
                MessageBox.Show("Firmware not found.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var arduinoIdePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Arduino", "arduino_debug.exe");
            if (!File.Exists(arduinoIdePath))
            {
                MessageBox.Show("Arduino IDE not found.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Upload firmware?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process process = null;
                await DoWork(() =>
                {
                    process = Process.Start(arduinoIdePath, $"--upload {Properties.Settings.Default.FirmwarePath} --port {Properties.Settings.Default.PortName}");
                    process.WaitForExit();
                });
                if (process?.ExitCode == 0)
                {
                    MessageBox.Show("Firmware uploaded.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Arduino IDE failed.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                DataContext = null;
            }
        }

        private async void Restart(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Restart device?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await DoWork(() =>
                {
                    DeviceSettings.Restart();
                    Thread.Sleep(5000);
                });

                DataContext = null;
            }
        }

        private async Task DoWork(Action action)
        {
            try
            {
                IsEnabled = false;
                await Task.Run(action);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsEnabled = true;
            }
        }
    }
}
