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
        private LetterMessage? msg;
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

        private void Btn_Msg_Click(object sender, RoutedEventArgs e)
        {
            msg = new LetterMessage();
            msg.Owner = Application.Current.MainWindow;
            msg.WindowStartupLocation = WindowStartupLocation.Manual; // Allow manual positioning
            CenterThanksMessage();

            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged!;
            Application.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;

            msg.Closed += (s, args) =>
            {
                // Detach events when message closes
                Application.Current.MainWindow.LocationChanged -= MainWindow_LocationChanged!;
                Application.Current.MainWindow.SizeChanged -= MainWindow_SizeChanged;
            };

            msg.ShowDialog();
        }

        private void CenterThanksMessage()
        {
            if (msg != null && msg.Owner != null)
            {
                Window mainWindow = msg.Owner;
                msg.Left = mainWindow.Left + (mainWindow.Width - msg.Width) / 2;
                msg.Top = mainWindow.Top + (mainWindow.Height - msg.Height) / 2;
            }
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            CenterThanksMessage();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterThanksMessage();
        }
    }
}
