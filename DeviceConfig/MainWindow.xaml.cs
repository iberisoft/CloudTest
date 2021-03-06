using DeviceConfig.Data;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        readonly AppDbContext m_DbContext = new();
        readonly SerialPort m_Port = new()
        {
            ReadTimeout = 3000
        };

        public MainWindow()
        {
            m_DbContext.Database.EnsureCreated();

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

            Settings.Default.Save();
        }

        public string[] PortNames => SerialPort.GetPortNames();

        private void PortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Settings.Default.PortName == null)
            {
                return;
            }

            m_Port.Close();
            m_Port.PortName = Settings.Default.PortName;
            try
            {
                m_Port.Open();
            }
            catch
            {
                Settings.Default.PortName = null;
            }
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

        public List<Company> Companies => m_DbContext.Companies.OrderBy(company => company.Name).ToList();

        private Company SelectedCompany
        {
            get => (Company)CompanyBox.SelectedItem;
            set => CompanyBox.SelectedItem = value;
        }

        private void UpdateCompanyBox() => CompanyBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

        private void CompanyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateNetworkBox();
        }

        private async void AddCompany(object sender, RoutedEventArgs e)
        {
            var window = new CompanyWindow
            {
                Owner = this
            };
            if (window.ShowDialog() == true)
            {
                var company = new Company();
                window.GetCompany(company);
                m_DbContext.Companies.Add(company);
                await m_DbContext.SaveChangesAsync();
                UpdateCompanyBox();
                SelectedCompany = company;
            }
        }

        private async void EditCompany(object sender, RoutedEventArgs e)
        {
            var window = new CompanyWindow
            {
                Owner = this
            };
            var company = SelectedCompany;
            window.SetCompany(company);
            if (window.ShowDialog() == true)
            {
                window.GetCompany(company);
                await m_DbContext.SaveChangesAsync();
                UpdateCompanyBox();
                SelectedCompany = null;
                SelectedCompany = company;
            }
        }

        private async void DeleteCompany(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Delete company?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                m_DbContext.Companies.Remove(SelectedCompany);
                await m_DbContext.SaveChangesAsync();
                UpdateCompanyBox();
            }
        }

        public List<Network> Networks => SelectedCompany?.Networks.OrderBy(network => network.Ssid).ToList();

        private Network SelectedNetwork
        {
            get => (Network)NetworkBox.SelectedItem;
            set => NetworkBox.SelectedItem = value;
        }

        private void UpdateNetworkBox() => NetworkBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

        private async void ApplySettings(object sender, RoutedEventArgs e)
        {
            await ApplySettings();
        }

        private async Task ApplySettings()
        {
            var settings = JsonConvert.SerializeObject(SelectedCompany.ToDeviceSettings(), Formatting.None);
            var settings2 = "";
            await DoWork(() =>
            {
                lock (m_Port)
                {
                    m_Port.EnableServerDebug(false);
                    m_Port.WriteParameter("SS", settings);
                    settings2 = m_Port.ReadParameter("SS");
                    m_Port.EnableServerDebug(true);
                }
            });
            MessageBox.Show(settings == settings2 ? $"Device {SelectedCompany.FullDeviceId} has done." : "Device has failed.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
            //MessageBox.Show(settings2, Title, MessageBoxButton.OK, MessageBoxImage.Information);

            ++SelectedCompany.LastDeviceId;
            await m_DbContext.SaveChangesAsync();
        }

        private async void AddNetwork(object sender, RoutedEventArgs e)
        {
            var window = new NetworkWindow
            {
                Owner = this
            };
            if (window.ShowDialog() == true)
            {
                var network = new Network { Company = SelectedCompany };
                window.GetNetwork(network);
                m_DbContext.Networks.Add(network);
                await m_DbContext.SaveChangesAsync();
                UpdateNetworkBox();
                SelectedNetwork = network;
            }
        }

        private async void RemoveNetwork(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Remove network?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                m_DbContext.Networks.Remove(SelectedNetwork);
                await m_DbContext.SaveChangesAsync();
                UpdateNetworkBox();
            }
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
                FileName = Settings.Default.SketchPath
            };
            if (dialog.ShowDialog() == true)
            {
                Settings.Default.SketchPath = dialog.FileName;
            }
        }

        private async void UploadSketch(object sender, RoutedEventArgs e)
        {
            if (await UploadSketch())
            {
                await ApplySettings();
            }
        }

        private async Task<bool> UploadSketch()
        {
            if (!File.Exists(Settings.Default.SketchPath))
            {
                MessageBox.Show("Sketch not found.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            var arduinoIdePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Arduino", "arduino_debug.exe");
            if (!File.Exists(arduinoIdePath))
            {
                MessageBox.Show("Arduino IDE not found.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (MessageBox.Show("Upload sketch?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process process = null;
                await DoWork(() =>
                {
                    lock (m_Port)
                    {
                        m_Port.Close();
                        process = Process.Start(arduinoIdePath, $"--upload {Settings.Default.SketchPath} --port {Settings.Default.PortName}");
                        process.WaitForExit();
                        m_Port.Open();
                    }
                });
                if (process?.ExitCode == 0)
                {
                    MessageBox.Show("Sketch uploaded.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Arduino IDE failed.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return false;
        }

        private void ClearLog(object sender, RoutedEventArgs e)
        {
            LogBox.Clear();
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
