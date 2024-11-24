using BLT_Generator.Pages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BLT_Generator.SubPages
{
    public partial class WIFI_History : UserControl
    {
        private List<WIFI_Data> allData = new List<WIFI_Data>();
        private List<WIFI_Data> filteredData = new List<WIFI_Data>();
        private int currentPage = 0;
        private const int ITEMS_PER_PAGE = 7;
        private WIFI_Data currentPinnedItem = null;

        public WIFI_History()
        {
            InitializeComponent();
            LoadData();
            UpdatePageDisplay();

            BtnPrevious.Click += BtnPrevious_Click;
            BtnNext.Click += BtnNext_Click;
            SortDate.SelectedDateChanged += SortDate_SelectedDateChanged;
        }

        private void InitializeWifiData(WIFI_Data data)
        {
            data.PinStateChanged += Data_PinStateChanged;
            data.DeleteRequested += Data_DeleteRequested;
            data.RegenerateRequested += Data_RegenerateRequested;
        }

        private void Data_RegenerateRequested(object? sender, WIFI_Data data)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var generatePage = new GeneratePage();

                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={generatePage.databasePath}"))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(
                        "SELECT password FROM tbl_wifi WHERE ssid = @ssid AND date = @date",
                        connection))
                    {
                        command.Parameters.AddWithValue("@ssid", data.TxbWifiName.Text);
                        command.Parameters.AddWithValue("@date", DateTime.ParseExact(data.TxbDate.Text, "dd/MMM/yyyy", null).ToString("yyyy-MM-dd"));

                        var password = command.ExecuteScalar()?.ToString() ?? "";

                        generatePage.SetWifiCredentials(
                            data.TxbWifiName.Text,
                            password,
                            data.TxbPassword.Text
                        );

                        var wifiPanel = new WIFI_Panel(generatePage);

                        if (wifiPanel.Txb_WIFI != null)
                            wifiPanel.Txb_WIFI.Text = data.TxbWifiName.Text;

                        if (wifiPanel.Txb_Password != null)
                            wifiPanel.Txb_Password.Text = password;

                        switch (data.TxbPassword.Text.ToUpper())
                        {
                            case "WEP":
                                wifiPanel.RbWEP.IsChecked = true;
                                break;
                            case "NOPASS":
                                wifiPanel.RbNone.IsChecked = true;
                                break;
                            default: // WPA
                                wifiPanel.RbWPA.IsChecked = true;
                                break;
                        }

                        mainWindow.PageBig(generatePage);
                        generatePage.PageCenter(wifiPanel);

                        var generateButton = mainWindow.FindName("Btn_Generate") as ToggleButton;
                        if (generateButton != null)
                        {
                            mainWindow.SetSelectedButton(generateButton);
                        }

                        generatePage.SelectWifiButton();

                        wifiPanel.BtnWifiGenerate.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                }
            }
        }

        private void LoadData()
        {
            allData.Clear();
            filteredData.Clear();
            GeneratePage generatePage = new GeneratePage();

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={generatePage.databasePath}"))
            {
                connection.Open();
                EnsureIsPinnedColumn(connection);

                string sql = @"
                SELECT date, ssid, password, encryptionType, COALESCE(is_pinned, 0) as is_pinned 
                FROM tbl_wifi 
                ORDER BY 
                    is_pinned DESC,
                    CASE 
                        WHEN is_pinned = 1 THEN date 
                        ELSE date 
                    END DESC";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WIFI_Data data = new WIFI_Data();

                        if (DateTime.TryParse(reader["date"].ToString(), out DateTime parsedDate))
                        {
                            data.TxbDate.Text = parsedDate.ToString("dd/MMM/yyyy");
                        }

                        data.TxbWifiName.Text = reader["ssid"].ToString();
                        data.TxbPassword.Text = reader["password"].ToString();

                        bool isPinned = Convert.ToBoolean(reader["is_pinned"]);
                        data.SetPinned(isPinned);

                        if (isPinned && currentPinnedItem == null)
                        {
                            currentPinnedItem = data;
                        }

                        InitializeWifiData(data);
                        allData.Add(data);
                    }
                }
            }

            filteredData = new List<WIFI_Data>(allData);
            UpdatePageDisplay();
        }

        private void Data_DeleteRequested(object? sender, WIFI_Data data)
        {
            if (data == null) return;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={(new GeneratePage()).databasePath}"))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SQLiteCommand deleteCommand = new SQLiteCommand(
                                "DELETE FROM tbl_wifi WHERE ssid = @ssid AND date = @date;",
                                connection))
                            {
                                deleteCommand.Parameters.AddWithValue("@ssid", data.TxbWifiName.Text);
                                deleteCommand.Parameters.AddWithValue("@date", DateTime.ParseExact(data.TxbDate.Text, "dd/MMM/yyyy", null).ToString("yyyy-MM-dd"));
                                deleteCommand.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            allData.Remove(data);
                            filteredData.Remove(data);

                            if (currentPinnedItem == data)
                            {
                                currentPinnedItem = null;
                            }

                            if (currentPage > 0 && currentPage * ITEMS_PER_PAGE >= GetCurrentDataSource().Count)
                            {
                                currentPage--;
                            }

                            UpdatePageDisplay();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting WIFI record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnsureIsPinnedColumn(SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(
                "SELECT COUNT(*) FROM pragma_table_info('tbl_wifi') WHERE name='is_pinned';",
                connection))
            {
                bool columnExists = Convert.ToInt32(command.ExecuteScalar()) > 0;

                if (!columnExists)
                {
                    using (SQLiteCommand alterCommand = new SQLiteCommand(
                        "ALTER TABLE tbl_wifi ADD COLUMN is_pinned INTEGER DEFAULT 0;",
                        connection))
                    {
                        alterCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private void Data_PinStateChanged(object sender, bool isPinned)
        {
            if (sender is not WIFI_Data data) return;

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={(new GeneratePage()).databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (isPinned)
                        {
                            using (SQLiteCommand unpinCommand = new SQLiteCommand(
                                "UPDATE tbl_wifi SET is_pinned = 0;",
                                connection))
                            {
                                unpinCommand.ExecuteNonQuery();
                            }

                            if (currentPinnedItem != null && currentPinnedItem != data)
                            {
                                currentPinnedItem.SetPinned(false);
                            }
                            currentPinnedItem = data;
                        }
                        else if (currentPinnedItem == data)
                        {
                            currentPinnedItem = null;
                        }

                        using (SQLiteCommand updateCommand = new SQLiteCommand(
                            "UPDATE tbl_wifi SET is_pinned = @isPinned WHERE ssid = @ssid AND date = @date;",
                            connection))
                        {
                            updateCommand.Parameters.AddWithValue("@isPinned", isPinned ? 1 : 0);
                            updateCommand.Parameters.AddWithValue("@ssid", data.TxbWifiName.Text);
                            updateCommand.Parameters.AddWithValue("@date", DateTime.ParseExact(data.TxbDate.Text, "dd/MMM/yyyy", null).ToString("yyyy-MM-dd"));
                            updateCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error updating pin status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            LoadData();
            UpdatePageDisplay();
        }

        private void SortDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortDate.SelectedDate.HasValue)
            {
                FilterDataByDate(SortDate.SelectedDate.Value);
            }
            else
            {
                filteredData = new List<WIFI_Data>(allData);
            }

            currentPage = 0;
            UpdatePageDisplay();
        }

        private void FilterDataByDate(DateTime selectedDate)
        {
            filteredData = allData.Where(data =>
                DateTime.TryParse(data.TxbDate.Text, out DateTime itemDate) &&
                itemDate.Date == selectedDate.Date
            ).ToList();
        }

        private void UpdatePageDisplay()
        {
            Data1.Children.Clear();
            Data2.Children.Clear();
            Data3.Children.Clear();
            Data4.Children.Clear();
            Data5.Children.Clear();
            Data6.Children.Clear();
            Data7.Children.Clear();

            var currentData = GetCurrentDataSource();
            int startIndex = currentPage * ITEMS_PER_PAGE;
            int endIndex = Math.Min(startIndex + ITEMS_PER_PAGE, currentData.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var data = currentData[i];
                switch (i - startIndex)
                {
                    case 0: Data1.Children.Add(data); break;
                    case 1: Data2.Children.Add(data); break;
                    case 2: Data3.Children.Add(data); break;
                    case 3: Data4.Children.Add(data); break;
                    case 4: Data5.Children.Add(data); break;
                    case 5: Data6.Children.Add(data); break;
                    case 6: Data7.Children.Add(data); break;
                }
            }

            BtnPrevious.IsEnabled = currentPage > 0;
            BtnNext.IsEnabled = (currentPage + 1) * ITEMS_PER_PAGE < currentData.Count;
        }

        private List<WIFI_Data> GetCurrentDataSource()
        {
            return SortDate.SelectedDate.HasValue ? filteredData : allData;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if ((currentPage + 1) * ITEMS_PER_PAGE < GetCurrentDataSource().Count)
            {
                currentPage++;
                UpdatePageDisplay();
            }
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                UpdatePageDisplay();
            }
        }
    }
}