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
    /// Interaction logic for URL_History.xaml
    /// </summary>
    public partial class URL_History : UserControl
    {
        private URL_Data? _data;
        public URL_History()
        {
            InitializeComponent();

            _data = new URL_Data();

            Data1.Children.Add(_data);
        }
    }
}
