using BLT_Generator.Pages;
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
    /// Interaction logic for WIFI_Data.xaml
    /// </summary>
    public partial class WIFI_Data : UserControl
    {
        public event EventHandler<WIFI_Data>? RegenerateRequested;
        public event EventHandler<WIFI_Data>? DeleteRequested;
        public event EventHandler<bool>? PinStateChanged;
        private bool isPinned = false;

        public bool IsPinned
        {
            get => isPinned;
            private set
            {
                if (isPinned != value)
                {
                    isPinned = value;
                    UpdatePinVisual();
                }
            }
        }

        public WIFI_Data()
        {
            InitializeComponent();
            UpdatePinVisual();
        }

        private void BtnPin_Click(object sender, RoutedEventArgs e)
        {
            IsPinned = !IsPinned;
            PinStateChanged?.Invoke(this, IsPinned);
        }

        private void UpdatePinVisual()
        {
            //BtnPin.Style = (Style)FindResource(IsPinned ? "PinActive" : "Pin");
            //BtnPin.Style = (Style)FindResource(IsPinned ? "PinActive" : "Pin");
        }

        public void SetPinned(bool pinned)
        {
            IsPinned = pinned;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
            "Are you sure you want to delete this WIFI record?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DeleteRequested?.Invoke(this, this);
            }
        }

        private void BtnRegenerate_Click(object sender, RoutedEventArgs e)
        {
            RegenerateRequested?.Invoke(this, this);
        }
    }
}
