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
        private Theme_Panel? Theme_Panel;
        public Language_Panel()
        {
            InitializeComponent();
            LoadLanguage();
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxLanguage.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLanguage = selectedItem.Content.ToString()!;
                SetLanguage(selectedLanguage);
            }
        }
        private void LoadEnglishLanguage()
        {
            RemoveLanguageDictionaries();
            var englishLanguage = new ResourceDictionary { Source = new Uri("/ResourceDictionary/EnglishLanguage.xaml", UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Add(englishLanguage);
        }
        private void LoadKhmerLanguage()
        {
            RemoveLanguageDictionaries();
            var khmerLanguage = new ResourceDictionary { Source = new Uri("/ResourceDictionary/KhmerLanguage.xaml", UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Add(khmerLanguage);
        }

        private void RemoveLanguageDictionaries()
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries.ToList();
            foreach (var dictionary in dictionaries)
            {
                if (dictionary.Source != null &&
                   (dictionary.Source.OriginalString.Contains("EnglishLanguage.xaml") ||
                    dictionary.Source.OriginalString.Contains("KhmerLanguage.xaml")))
                {
                    Application.Current.Resources.MergedDictionaries.Remove(dictionary);
                }
            }
        }

        private void SetLanguage(string language)
        {

            if (language == "English")
            {
                LoadEnglishLanguage();
            }
            else if (language == "Khmer")
            {
                LoadKhmerLanguage();
            }

            // Save the theme to settings
            Properties.Settings.Default.Language = language;
            Properties.Settings.Default.Save();
        }

        public void LoadLanguage()
        {
            string savedLanguage = Properties.Settings.Default.Language;

            if (savedLanguage == "English")
            {
                ComboBoxLanguage.SelectedIndex = 0;
                LoadEnglishLanguage();
            }
            else
            {
                ComboBoxLanguage.SelectedIndex = 1;
                LoadKhmerLanguage();
            }
        }
    }
}
