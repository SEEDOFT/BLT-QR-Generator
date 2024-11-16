using BLT_Generator.Helpers;
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

namespace BLT_Generator.SubPages
{
    /// <summary>
    /// Interaction logic for Theme_Panel.xaml
    /// </summary>
    public partial class Theme_Panel : UserControl
    {
        public Theme_Panel()
        {
            InitializeComponent();
            LoadTheme();
        }

        private void Rd_LightMode_Checked(object? sender, RoutedEventArgs? e)
        {
            if (LightModeButton.IsChecked == true)
            {
                SetTheme("Light");
            }
            else if (DarkModeButton.IsChecked == true)
            {
                SetTheme("Dark");
            }
        }

        private void LoadLightMode()
        {
            var lightMode = new ResourceDictionary { Source = new Uri("/ResourceDictionary/LightMode.xaml", UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(lightMode);
        }

        private void LoadDarkMode()
        {
            var darkMode = new ResourceDictionary { Source = new Uri("/ResourceDictionary/DarkMode.xaml", UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(darkMode);
        }

        private void SetTheme(string theme)
        {
            if (theme == "Light")
            {
                LoadLightMode();
            }
            else if (theme == "Dark")
            {
                LoadDarkMode();
            }

            // Save the theme to settings
            Properties.Settings.Default.Theme = theme;
            Properties.Settings.Default.Save();
        }

        public void LoadTheme()
        {
            string savedTheme = Properties.Settings.Default.Theme;

            if (savedTheme == "Dark")
            {
                DarkModeButton.IsChecked = true;
                LoadDarkMode();
            }
            else
            {
                LightModeButton.IsChecked = true;
                LoadLightMode();
            }
        }


    }
}
