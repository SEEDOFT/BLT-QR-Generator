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
using System.Windows.Media.Animation;
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
        private bool isDrawerOpen = false;
        public void OpenPage(UserControl Page)
        {
            Grid_Display?.Children.Clear();
            Grid_Display?.Children.Add(Page);
        }

        public HistoryPage()
        {
            InitializeComponent();
            OpenPage(new SubPages.URL_History());

            var faqPanel = new BLT_Generator.SubPages.FAQ_Panel();
            faqPanel.RequestCloseDrawer += HandleDrawerToggle;

            Grid_FAQ.Children.Clear();
            Grid_FAQ.Children.Add(faqPanel);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox? comboBox = sender as ComboBox;

            if (comboBox?.SelectedIndex == -1)
            {
                comboBox.SelectedIndex = 0;
            }
            else if (comboBox?.SelectedIndex == 0)
            {
                OpenPage(new SubPages.URL_History());
            }
            else if (comboBox?.SelectedIndex == 1)
            {
                OpenPage(new SubPages.WIFI_History());
            }
        }

        private void Btn_FAQ_Click(object sender, RoutedEventArgs e)
        {
            HandleDrawerToggle();
        }

        private void HandleDrawerToggle()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.3),
                To = isDrawerOpen ? 0 : 210,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            Grid_FAQ.BeginAnimation(WidthProperty, animation);

            isDrawerOpen = !isDrawerOpen;
        }
    }
}
