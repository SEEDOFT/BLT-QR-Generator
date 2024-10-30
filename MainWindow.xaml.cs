using System.Text;
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

namespace BLT_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void PageMain(UserControl page)
        {
            Grid_Main.Children.Clear();
            Grid_Main.Children.Add(page);
        }

        public MainWindow()
        {
            InitializeComponent();

            Btn_Generate.IsChecked = true;
            SetSelectedButton(Btn_Generate);
            PageMain(new Pages.GeneratePage());
        }

        private void SetSelectedButton(ToggleButton selectedButton)
        {
            foreach (var child in ((StackPanel)Btn_Generate.Parent).Children)
            {
                if (child is ToggleButton button && button != selectedButton)
                {
                    button.IsChecked = false;
                }
            }

            selectedButton.IsChecked = true;
        }

        private void Btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(Btn_Generate);
            PageMain(new Pages.GeneratePage());
        }

        private void Btn_Profile_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(Btn_Profile);
            PageMain(new Pages.ProfilePage());
        }

        private void Btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(Btn_Setting);
            PageMain(new Pages.SettingPage());
        }

        private void Btn_Leave_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(Btn_Leave);
        }
    }
}