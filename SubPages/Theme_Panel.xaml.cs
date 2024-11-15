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
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                string theme = radioButton.Tag.ToString()!;
                //ApplyTheme(theme);
            }
        }

        public void ApplyTheme(string theme)
        {
            Application.Current.Resources.MergedDictionaries.Clear();

            if (theme == "Light")
            {
                ResourceDictionary lightTheme = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/ResourceDictionary/LightMode.xaml")
                };
                Application.Current.Resources.MergedDictionaries.Add(lightTheme);
            }
            else if (theme == "Dark")
            {
                ResourceDictionary darkTheme = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/ResourceDictionary/DarkMode.xaml")
                };
                Application.Current.Resources.MergedDictionaries.Add(darkTheme);
            }
            Application.Current.Resources.MergedDictionaries.Count();
        }
    }
}
