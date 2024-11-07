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
    /// Interaction logic for SettingPage.xaml
    /// </summary>
    public partial class SettingPage : UserControl
    {
        private void OpenPage(UserControl page)
        {
            Grid_Display.Children.Clear();
            Grid_Display.Children.Add(page);
        }
        public SettingPage()
        {
            InitializeComponent();

            OpenPage(new SubPages.Language_Panel());
        }

        private void Btn_Msg_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Language_Click(object sender, RoutedEventArgs e)
        {
            OpenPage(new SubPages.Language_Panel());
        }

        private void Btn_Theme_Click(object sender, RoutedEventArgs e)
        {
            OpenPage(new SubPages.Theme_Panel());
        }

        private void Btn_Passcode_Click(object sender, RoutedEventArgs e)
        {
            OpenPage(new SubPages.PIN_Panel());
        }
    }
}
