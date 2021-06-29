using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
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
        readonly SerialPort m_Port = new SerialPort()
        {
            ReadTimeout = 3000
        };

        public MainWindow()
        {
            InitializeComponent();

            m_Port.DataReceived += Port_DataReceived;
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
            m_Port.Close();

            Properties.Settings.Default.Save();
        }

        public string[] PortNames => SerialPort.GetPortNames();

        private void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_Port.Close();
            m_Port.PortName = Properties.Settings.Default.PortName;
            m_Port.Open();
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (m_Port)
            {
                Dispatcher.Invoke(() =>
                {
                    LogBox.AppendText(m_Port.ReadExisting());
                    LogBox.ScrollToEnd();
                });
            }
        }

        private async void LoadSettings(object sender, RoutedEventArgs e)
        {
            await DoWork(() =>
            {
                lock (m_Port)
                {
                    m_Port.EnableServerDebug(false);
                    Properties.Settings.Default.SettingsText = m_Port.ReadParameter("SS");
                    m_Port.EnableServerDebug(true);
                }
            });
        }

        private async void SaveSettings(object sender, RoutedEventArgs e)
        {
            await DoWork(() =>
            {
                lock (m_Port)
                {
                    m_Port.EnableServerDebug(false);
                    m_Port.WriteParameter("SS", Properties.Settings.Default.SettingsText);
                    Properties.Settings.Default.SettingsText = m_Port.ReadParameter("SS");
                    m_Port.EnableServerDebug(true);
                }
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
                await AddNetwork(window.Ssid, window.Password);
            }
        }

        private async Task AddNetwork(string ssid, string password)
        {
            await DoWork(() =>
            {
                lock (m_Port)
                {
                    m_Port.EnableServerDebug(false);
                    m_Port.WriteParameter("NW", $"{ssid}+{password}");
                    Properties.Settings.Default.SettingsText = m_Port.ReadParameter("SS");
                    m_Port.EnableServerDebug(true);
                }
            });
        }

        private async void RestartDevice(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Restart device?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await DoWork(() =>
                {
                    m_Port.WriteLine("RST");
                    Thread.Sleep(5000);
                });
            }
        }

        private void BrowseSketch(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Sketch Files (*.ino)|*.ino",
                FileName = Properties.Settings.Default.SketchPath
            };
            if (dialog.ShowDialog() == true)
            {
                Properties.Settings.Default.SketchPath = dialog.FileName;
            }
        }

        private async void UploadSketch(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Properties.Settings.Default.SketchPath))
            {
                MessageBox.Show("Sketch not found.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var arduinoIdePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Arduino", "arduino_debug.exe");
            if (!File.Exists(arduinoIdePath))
            {
                MessageBox.Show("Arduino IDE not found.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Upload sketch?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process process = null;
                await DoWork(() =>
                {
                    lock (m_Port)
                    {
                        m_Port.Close();
                        process = Process.Start(arduinoIdePath, $"--upload {Properties.Settings.Default.SketchPath} --port {Properties.Settings.Default.PortName}");
                        process.WaitForExit();
                        m_Port.Open();
                    }
                });
                if (process?.ExitCode == 0)
                {
                    MessageBox.Show("Sketch uploaded.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Arduino IDE failed.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
