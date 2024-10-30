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
    /// Interaction logic for WIFI_Panel.xaml
/// </summary>
    public partial class WIFI_Panel : UserControl
    {
        public WIFI_Panel()
        {
            InitializeComponent();
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            Txb_WIFI.Text = string.Empty;
            Txb_Password.Text = string.Empty;
        }
    }
}
