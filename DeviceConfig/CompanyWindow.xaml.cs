using DeviceConfig.Data;
using System.ComponentModel;
using System.Windows;

namespace DeviceConfig
{
    /// <summary>
    /// Interaction logic for CompanyWindow.xaml
    /// </summary>
    public partial class CompanyWindow : Window
    {
        public CompanyWindow()
        {
            InitializeComponent();
        }

        public void GetCompany(Company company)
        {
            company.Name = NameBox.Text;
            company.Code = CodeBox.Text;
            company.ServerHost = ServerHostBox.Text;
            company.ServerPort = ServerPortBox.Value.Value;
            company.TopicPrefix = TopicPrefixBox.Text;
            company.LastDeviceId = LastDeviceIdBox.Value.Value;
        }

        public void SetCompany(Company company)
        {
            NameBox.Text = company.Name;
            CodeBox.Text = company.Code;
            ServerHostBox.Text = company.ServerHost;
            ServerPortBox.Value = company.ServerPort;
            TopicPrefixBox.Text = company.TopicPrefix;
            LastDeviceIdBox.Value = company.LastDeviceId;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DialogResult == true && (NameBox.Text.Length == 0 || CodeBox.Text.Length == 0 || ServerHostBox.Text.Length == 0 || TopicPrefixBox.Text.Length == 0))
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
