using System;
using System.Collections.Generic;
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

namespace BLT_Generator.Pages
{
    /// <summary>
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : UserControl
    {
        public void OpenPage(UserControl Page)
        {
            Grid_Display?.Children.Clear();
            Grid_Display?.Children.Add(Page);
        }
        public HistoryPage()
        {
            InitializeComponent();
            OpenPage(new SubPages.URL_History());
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;

            if (comboBox?.SelectedIndex == -1)
            {
                comboBox.SelectedIndex = 0;
            }
            else if(comboBox?.SelectedIndex == 0)
            {
                OpenPage(new SubPages.URL_History());
            }
            else if (comboBox?.SelectedIndex == 1)
            {
                OpenPage(new SubPages.WIFI_History());
            }
        }
    }
}
