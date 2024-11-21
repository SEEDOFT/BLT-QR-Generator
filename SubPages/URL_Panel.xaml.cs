using BLT_Generator.Pages;
using System;
using System.Windows.Controls;

namespace BLT_Generator.SubPages
{
    public partial class URL_Panel : UserControl
    {
        private readonly GeneratePage parentPage;

        public URL_Panel(GeneratePage parent)
        {
            InitializeComponent();
            parentPage = parent;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string url = TxtUrl.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                if (parentPage != null)
                {
                    parentPage.ClearQRCode();
                }
            }
            else
            {
                if (parentPage != null)
                {
                    parentPage.path = url;
                    parentPage.GenerateQR();
                }
            }
        }
    }
}