using System.Configuration;
using System.Data;
using System.Windows;

namespace BLT_Generator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow main = new MainWindow();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LanguageManager.ChangeLanguage("en-US");
        }
    }

}
