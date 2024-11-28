using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace BLT_Generator.SubPages
{
    public partial class URL_Data : UserControl
    {
        private bool isPinned = false;
        public event EventHandler<URL_Data>? DeleteRequested;
        public event EventHandler<bool>? PinStateChanged;

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

        public URL_Data()
        {
            InitializeComponent();
            BtnPin.Click += BtnPin_Click;
            UpdatePinVisual();
        }

        private void BtnPin_Click(object sender, RoutedEventArgs e)
        {
            IsPinned = !IsPinned;
            PinStateChanged?.Invoke(this, IsPinned);
        }

        private void UpdatePinVisual()
        {
            BtnPin.Style = (Style)FindResource(IsPinned ? "PinActive" : "Pin");
        }

        public void SetPinned(bool pinned)
        {
            IsPinned = pinned;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete this URL record?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DeleteRequested?.Invoke(this, this);
            }
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(TxbURL.Text);
                MessageBox.Show("URL copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy URL: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}