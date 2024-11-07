using BLT_Generator.Pages;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLT_Generator.SubPages
{
    /// <summary>
    /// Interaction logic for QR_Panel.xaml
    /// </summary>
    public partial class QR_Panel : UserControl
    {
        private AddIcon? icon;
        public QR_Panel()
        {
            InitializeComponent();
        }

        private void Btn_AddIcon_Click(object sender, RoutedEventArgs e)
        {
            icon = new AddIcon();
            icon.Owner = Application.Current.MainWindow;
            icon.WindowStartupLocation = WindowStartupLocation.Manual;
            CenterAddIcon();

            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged!;
            Application.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;

            icon.Closed += (s, args) =>
            {
                // Detach events when message closes
                Application.Current.MainWindow.LocationChanged -= MainWindow_LocationChanged!;
                Application.Current.MainWindow.SizeChanged -= MainWindow_SizeChanged;
            };

            icon.ShowDialog();
        }

        private void CenterAddIcon()
        {
            if (icon != null && icon.Owner != null)
            {
                Window mainWindow = icon.Owner;
                icon.Left = mainWindow.Left + (mainWindow.Width - icon.Width) / 2;
                icon.Top = mainWindow.Top + (mainWindow.Height - icon.Height) / 2;
            }
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            CenterAddIcon();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterAddIcon();
        }
    }
}
