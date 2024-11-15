using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for GeneratePage.xaml
    /// </summary>
    public partial class GeneratePage : UserControl
    {
        private AddIcon? icon;

        public void PageCenter(UserControl Page)
        {
            Grid_Center.Children.Clear();
            Grid_Center.Children.Add(Page);
        }

        public GeneratePage()
        {
            InitializeComponent();

            Btn_URL.IsChecked = true;
            SetSelectedButton(Btn_URL);

            PageCenter(new SubPages.URL_Panel());
        }

        private void SetSelectedButton(ToggleButton selectedButton)
        {
            foreach (var child in ((StackPanel)Btn_URL.Parent).Children)
            {
                if (child is ToggleButton button && button != selectedButton)
                {
                    button.IsChecked = false;
                }
            }

            selectedButton.IsChecked = true;
        }

        private void Btn_WIFI_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(Btn_WIFI);
            PageCenter(new SubPages.WIFI_Panel());
        }

        private void Btn_URL_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(Btn_URL);
            PageCenter(new SubPages.URL_Panel());
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
