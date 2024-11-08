using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BLT_Generator
{
    public class LanguageManager
    {
        public static void ChangeLanguage(string cultureName)
        {
            // Change the culture of the application
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);

            // Create ResourceDictionary for the new language
            ResourceDictionary dict = new ResourceDictionary();
            switch (cultureName)
            {
                case "km-KH":
                    dict.Source = new Uri("Languages/Strings.km-KH.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Languages/Strings.xaml", UriKind.Relative);
                    break;
            }

            // Clear and add the new ResourceDictionary
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
