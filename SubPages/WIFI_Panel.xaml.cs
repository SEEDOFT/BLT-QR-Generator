using BLT_Generator.Pages;
using System;
using System.Data.SQLite;
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
            generateWIfiQR();
        }

        private void generateWIfiQR()
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

            parentPage.SetWifiCredentials(Txb_WIFI.Text, password, encryptionType);

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
            SaveWIFI(encryptionType);
            parentPage.path = "";
        }

        private void SaveWIFI(string encryptionType)
        {
            GeneratePage generatePage = new GeneratePage();
            SQLiteConnection connection = new SQLiteConnection($"Data Source={generatePage.databasePath}");
            connection.Open();
            string sql = "INSERT INTO tbl_wifi (date, ssid, password, encryptionType) VALUES (@date, @ssid, @password, @encryptionType);";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@ssid", Txb_WIFI.Text);
                command.Parameters.AddWithValue("@password", Txb_Password.Text);
                command.Parameters.AddWithValue("@encryptionType", encryptionType);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}