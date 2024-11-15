using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace BLT_Generator.Pages
{
    /// <summary>
    /// Interaction logic for LetterMessage.xaml
    /// </summary>
    public partial class LetterMessage : Window
    {
        public LetterMessage()
        {
            InitializeComponent();
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_CBRD_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://cbrd.gov.kh/",
                UseShellExecute = true,
            });
        }

        private void Btn_MPTC_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://mptc.gov.kh/",
                UseShellExecute = true,
            });
        }

        private void Btn_ANT_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "http://training.antkh.com/",
                UseShellExecute = true,
            });
        }
    }
}
