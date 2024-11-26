using BLT_Generator.Pages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BLT_Generator.SubPages
{
    public partial class URL_Panel : UserControl
    {
        private readonly GeneratePage parentPage;
        private string lastSavedUrl = "";

        public URL_Panel(GeneratePage parent)
        {
            InitializeComponent();
            parentPage = parent;

            // Add LostFocus event handler to the TextBox
            TxtUrl.LostFocus += TxtUrl_LostFocus;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (parentPage == null) return;

            string url = TxtUrl.Text;

            if (string.IsNullOrWhiteSpace(url))
            {
                parentPage.ClearQRCode();
            }
            else
            {
                parentPage.path = url;
                parentPage.GenerateQR();
            }
        }

        private void TxtUrl_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveUrlToDatabase();
        }

        private void SaveUrlToDatabase()
        {
            if (TxtUrl == null || parentPage == null) return;
            if (TxtUrl.Text == "Enter URL") return;

            string currentUrl = TxtUrl.Text;

            if (!string.IsNullOrWhiteSpace(currentUrl) &&
                currentUrl != "បញ្ចូល URL" &&
                currentUrl != lastSavedUrl)
            {
                try
                {
                    using (var connection = new System.Data.SQLite.SQLiteConnection($"Data Source={parentPage.databasePath}"))
                    {
                        connection.Open();
                        string sql = "INSERT INTO tbl_url (url, date) VALUES (@url, @date);";
                        using (var command = new System.Data.SQLite.SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@url", currentUrl);
                            command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                            command.ExecuteNonQuery();
                        }
                    }
                    lastSavedUrl = currentUrl;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving URL to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}