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
    /// Interaction logic for Language_Panel.xaml
    /// </summary>
    public partial class Language_Panel : UserControl
    {
        public Language_Panel()
        {
            InitializeComponent();
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBoxLanguage.SelectedItem;
            string language = selectedItem.Content.ToString();

            switch (language)
            {
                case "Khmer":
                    LanguageManager.ChangeLanguage("km-KH");
                    break;
                default:
                    LanguageManager.ChangeLanguage("en-US");
                    break;
            }
        }
    }
}
