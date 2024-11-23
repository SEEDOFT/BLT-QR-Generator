using BLT_Generator.Pages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BLT_Generator.SubPages
{
    public partial class WIFI_Panel : UserControl
    {
        private GeneratePage parentPage;

        public WIFI_Panel(GeneratePage parent)
        {
            InitializeComponent();
            parentPage = parent;
            BtnWifiGenerate.Click += BtnWifiGenerate_Click;
            RbWPA.IsChecked = true;
        }

        private void BtnWifiGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txb_WIFI.Text))
            {
                MessageBox.Show("Please enter WIFI SSID", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string encryptionType = "WPA";
            if (RbWEP.IsChecked == true)
                encryptionType = "WEP";
            else if (RbNone.IsChecked == true)
                encryptionType = "nopass";

            string password = "";
            if (encryptionType != "nopass")
            {
                if (string.IsNullOrWhiteSpace(Txb_Password.Text))
                {
                    MessageBox.Show("Please enter WIFI password", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                password = Txb_Password.Text;
            }

            // Set the credentials in the parent page
            parentPage.SetWifiCredentials(Txb_WIFI.Text, password, encryptionType);

            // Generate the WIFI configuration string
            string wifiConfig = $"WIFI:S:{Txb_WIFI.Text};T:{encryptionType};";
            if (encryptionType != "nopass")
            {
                wifiConfig += $"P:{password};;";
            }
            else
            {
                wifiConfig += ";";
            }

            parentPage.path = wifiConfig;
            parentPage.GenerateQR();
        }
    }
}