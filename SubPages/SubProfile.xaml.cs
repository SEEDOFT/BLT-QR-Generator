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
    /// Interaction logic for SubProfile.xaml
    /// </summary>
    public partial class SubProfile : UserControl
    {
        public SubProfile()
        {
            InitializeComponent();
            Both(new SubPages.BothCard());
            Thea(new SubPages.TheaCard());
            Leng(new SubPages.LengCard());
        }

        public void Both(UserControl userControl)
        {
            Grid_Both.Children.Clear();
            Grid_Both.Children.Add(userControl);
        }

        public void Thea(UserControl userControl)
        {
            Grid_Thea.Children.Clear();
            Grid_Thea.Children.Add(userControl);
        }

        public void Leng(UserControl userControl)
        {
            Grid_Leng.Children.Clear();
            Grid_Leng.Children.Add(userControl);
        }
       
    }
}
