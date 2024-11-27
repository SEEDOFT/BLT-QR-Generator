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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLT_Generator.SubPages;

    namespace BLT_Generator.Pages
    {
    /// <summary>
        /// Interaction logic for ProfilePage.xaml
    /// </summary>
    ///
    public partial class ProfilePage : UserControl
    {
    private LetterMessage? msg;
    public ProfilePage()
    {
        InitializeComponent();
        //display card page
        SubProfile tmp = new();

        Card_Display.Children.Clear();
        Card_Display.Children.Add(tmp);
    }

    private void Btn_Msg_Click(object sender, RoutedEventArgs e)
    {
            try
            {
                var letterMessage = new LetterMessage();
                letterMessage.Owner = Window.GetWindow(this);
                letterMessage.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //msg = new LetterMessage();
            //msg.Owner = Application.Current.MainWindow;
            //msg.WindowStartupLocation = WindowStartupLocation.Manual; // Allow manual positioning
            //CenterThanksMessage();

            //Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged!;
            //Application.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;

            //msg.Closed += (s, args) =>
            //{
            //    // Detach events when message closes
            //    Application.Current.MainWindow.LocationChanged -= MainWindow_LocationChanged!;
            //    Application.Current.MainWindow.SizeChanged -= MainWindow_SizeChanged;
            //};

            //msg.Show();
    }
    private void CenterThanksMessage()
    {
        if (msg != null && msg.Owner != null)
        {
            Window mainWindow = msg.Owner;
            msg.Left = mainWindow.Left + (mainWindow.Width - msg.Width) / 2;
            msg.Top = mainWindow.Top + (mainWindow.Height - msg.Height) / 2;
        }
    }

    private void MainWindow_LocationChanged(object sender, EventArgs e)
    {
        CenterThanksMessage();
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        CenterThanksMessage();
    }
    }

}
