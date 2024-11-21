using BLT_Generator.Pages;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLT_Generator.SubPages
{
    /// <summary>
    /// Interaction logic for URL_History.xaml
    /// </summary>
    public partial class URL_History : UserControl
    {
        private List<URL_Data> allData = new List<URL_Data>();
        private List<URL_Data> filteredData = new List<URL_Data>();
        private int currentPage = 0;
        private const int ITEMS_PER_PAGE = 7;
        private URL_Data currentPinnedItem = null;

        public URL_History()
        {
            InitializeComponent();

            EnsureIsPinnedColumn();
            LoadData();
            UpdatePageDisplay();

            BtnPrevious.Click += BtnPrevious_Click;
            BtnNext.Click += BtnNext_Click;
            SortDate.SelectedDateChanged += SortDate_SelectedDateChanged;
        }

        private void SortDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortDate.SelectedDate.HasValue)
            {
                FilterDataByDate(SortDate.SelectedDate.Value);
            }
            else
            {
                filteredData = new List<URL_Data>(allData);
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

        private List<URL_Data> GetCurrentDataSource()
        {
            return SortDate.SelectedDate.HasValue ? filteredData : allData;
        }

        private void EnsureIsPinnedColumn()
        {
            GeneratePage generatePage = new GeneratePage();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={generatePage.databasePath}"))
            {
                connection.Open();

                bool columnExists = false;
                using (SQLiteCommand command = new SQLiteCommand(
                    "SELECT COUNT(*) FROM pragma_table_info('tbl_url') WHERE name='is_pinned';",
                    connection))
                {
                    columnExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                }

                if (!columnExists)
                {
                    using (SQLiteCommand command = new SQLiteCommand(
                        "ALTER TABLE tbl_url ADD COLUMN is_pinned INTEGER DEFAULT 0;",
                        connection))
                    {
                        command.ExecuteNonQuery();
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
                string sql = @"
                    SELECT url, date, is_pinned 
                    FROM tbl_url 
                    ORDER BY 
                        is_pinned DESC,
                        CASE WHEN is_pinned = 1 THEN date END ASC,
                        CASE WHEN is_pinned = 0 THEN date END DESC";

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    bool foundPinnedItem = false;

                    while (reader.Read())
                    {
                        URL_Data data = new URL_Data();
                        if (DateTime.TryParse(reader["date"].ToString(), out DateTime parsedDate))
                        {
                            data.TxbDate.Text = parsedDate.ToString("dd/MMM/yyyy");
                        }
                        else
                        {
                            data.TxbDate.Text = "Invalid Date";
                        }

                        data.TxbURL.Text = reader["url"].ToString();
                        bool isPinned = Convert.ToBoolean(reader["is_pinned"]);

                        if (isPinned)
                        {
                            if (!foundPinnedItem)
                            {
                                data.SetPinned(true);
                                currentPinnedItem = data;
                                foundPinnedItem = true;
                            }
                            else
                            {
                                UpdatePinStatus(data.TxbURL.Text, false);
                                data.SetPinned(false);
                            }
                        }

                        data.PinStateChanged += Data_PinStateChanged;
                        allData.Add(data);
                    }
                }
            }

            filteredData = new List<URL_Data>(allData);
        }

        private void Data_PinStateChanged(object sender, bool isPinned)
        {
            if (sender is not URL_Data data) return;

            if (isPinned)
            {
                if (currentPinnedItem != null && currentPinnedItem != data)
                {
                    currentPinnedItem.SetPinned(false);
                    UpdatePinStatus(currentPinnedItem.TxbURL.Text, false);
                }

                currentPinnedItem = data;
                UpdatePinStatus(data.TxbURL.Text, true);
            }
            else
            {
                if (currentPinnedItem == data)
                {
                    currentPinnedItem = null;
                    UpdatePinStatus(data.TxbURL.Text, false);
                }
            }

            LoadData();
            currentPage = 0;
            UpdatePageDisplay();
        }

        private void UpdatePinStatus(string url, bool isPinned)
        {
            GeneratePage generatePage = new GeneratePage();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={generatePage.databasePath}"))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (isPinned)
                        {
                            using (SQLiteCommand unpinCommand = new SQLiteCommand(
                                "UPDATE tbl_url SET is_pinned = 0 WHERE url != @url;",
                                connection))
                            {
                                unpinCommand.Parameters.AddWithValue("@url", url);
                                unpinCommand.ExecuteNonQuery();
                            }
                        }

                        using (SQLiteCommand updateCommand = new SQLiteCommand(
                            "UPDATE tbl_url SET is_pinned = @isPinned WHERE url = @url;",
                            connection))
                        {
                            updateCommand.Parameters.AddWithValue("@isPinned", isPinned ? 1 : 0);
                            updateCommand.Parameters.AddWithValue("@url", url);
                            updateCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
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
                int gridIndex = (i - startIndex) + 1;
                switch (gridIndex)
                {
                    case 1:
                        Data1.Children.Add(currentData[i]);
                        break;
                    case 2:
                        Data2.Children.Add(currentData[i]);
                        break;
                    case 3:
                        Data3.Children.Add(currentData[i]);
                        break;
                    case 4:
                        Data4.Children.Add(currentData[i]);
                        break;
                    case 5:
                        Data5.Children.Add(currentData[i]);
                        break;
                    case 6:
                        Data6.Children.Add(currentData[i]);
                        break;
                    case 7:
                        Data7.Children.Add(currentData[i]);
                        break;
                }
            }

            BtnPrevious.IsEnabled = currentPage > 0;
            BtnNext.IsEnabled = (currentPage + 1) * ITEMS_PER_PAGE < currentData.Count;
        }
    }
}
