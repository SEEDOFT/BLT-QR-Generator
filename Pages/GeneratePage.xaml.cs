using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using QRCoder;
using System.Drawing.Imaging;
using ZXing;
using ZXing.QrCode;
using System.Diagnostics;
using System.Xml.Linq;
using BLT_Generator.SubPages;
using ZXing.Common;
using System.Data.SQLite;

namespace BLT_Generator.Pages
{
    /// <summary>
    /// Interaction logic for GeneratePage.xaml
    /// </summary>
    public partial class GeneratePage : UserControl
    {
        private AddIcon? icon;

        private Bitmap? generateQRCodeWithLogo;
        private string? selectPath;
        //public string dir = "Data";
        public string path = "";
        System.Drawing.Color qrColor = System.Drawing.Color.Black;
        System.Drawing.Color qrBackColor = System.Drawing.Color.White;
        private bool isFrameAdded = false;
        private Bitmap? currentFrame = null;
        public string framePath = "";
        private bool isFrame1Clicked = false;
        private bool isFrame2Clicked = false;
        private bool isFrame3Clicked = false;
        private bool isFrame4Clicked = false;
        public string databasePath = "qrcode.sqlite";

        public void PageCenter(UserControl Page)
        {
            Grid_Center.Children.Clear();
            Grid_Center.Children.Add(Page);
        }

        public GeneratePage()
        {
            InitializeComponent();
            Btn_URL.IsChecked = true;
            SetSelectedButton(Btn_URL);
            PageCenter(CreateUrlPanel());

            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);

                SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}");
                connection.Open();
                string createTable = @"
                CREATE TABLE tbl_url (
                    id         INTEGER PRIMARY KEY AUTOINCREMENT,
                    url        TEXT    NOT NULL,
                    date       DATE    NOT NULL
                );";
                var command = new SQLiteCommand(createTable, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }

        }

        private URL_Panel CreateUrlPanel()
        {
            return new SubPages.URL_Panel(this);
        }

        private void SetSelectedButton(ToggleButton selectedButton)
        {
            foreach (var child in ((StackPanel)Btn_URL.Parent).Children)
            {
                if (child is ToggleButton button && button != selectedButton)
                {
                    button.IsChecked = false;
                }
            }
            selectedButton.IsChecked = true;
        }

        private void Btn_WIFI_Click(object sender, RoutedEventArgs e)
        {
            QRCode.Source = null;
            SetSelectedButton(Btn_WIFI);
            PageCenter(new SubPages.WIFI_Panel(this));
        }

        private void Btn_URL_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedButton(Btn_URL);
            PageCenter(CreateUrlPanel());
        }

        public void UpdateQRCodeIcon(string iconPath)
        {
            selectPath = iconPath;
            GenerateQR();
        }

        private void Btn_AddIcon_Click(object sender, RoutedEventArgs e)
        {
            icon = new AddIcon(this);
            icon.Owner = Application.Current.MainWindow;
            icon.WindowStartupLocation = WindowStartupLocation.Manual;
            CenterAddIcon();

            Application.Current.MainWindow.LocationChanged += MainWindow_LocationChanged!;
            Application.Current.MainWindow.SizeChanged += MainWindow_SizeChanged;

            icon.Closed += (s, args) =>
            {
                Application.Current.MainWindow.LocationChanged -= MainWindow_LocationChanged!;
                Application.Current.MainWindow.SizeChanged -= MainWindow_SizeChanged;
            };

            icon.ShowDialog();
        }

        private void CenterAddIcon()
        {
            if (icon != null && icon.Owner != null)
            {
                Window mainWindow = icon.Owner;
                icon.Left = mainWindow.Left + (mainWindow.Width - icon.Width) / 2;
                icon.Top = mainWindow.Top + (mainWindow.Height - icon.Height) / 2;
            }
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            CenterAddIcon();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterAddIcon();
        }

        public void ClearQRCode()
        {
            path = "";
            QRCode.Source = null;
            generateQRCodeWithLogo = null;
            isFrameAdded = false;
            currentFrame = null;
            framePath = "";
        }

        public void GenerateQR()
        {
            // Clear everything if path is empty or whitespace
            if (string.IsNullOrWhiteSpace(path))
            {
                QRCode.Source = null;
                generateQRCodeWithLogo = null;
                isFrameAdded = false;
                currentFrame = null;
                framePath = "";
                return;  // Important: return here to prevent further execution
            }

            // If it's all digits, format as phone number
            if (path.All(char.IsDigit))
            {
                path = $"tel:+{path}";
            }

            using (QRCodeGenerator qrCodeGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(path, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(20, qrColor, qrBackColor, false);

                    int padding = 35;
                    Bitmap borderedQR = new Bitmap(qrCodeImage.Width + (padding * 2), qrCodeImage.Height + (padding * 2));

                    using (Graphics g = Graphics.FromImage(borderedQR))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.Clear(Color.Transparent);

                        if (Path.GetFileName(framePath) == "frame.png")
                        {
                            g.Clear(Color.Black);
                        }

                        using (GraphicsPath path = new GraphicsPath())
                        {
                            int cornerRadius = 15;
                            Rectangle rect = new Rectangle(0, 0, borderedQR.Width - 1, borderedQR.Height - 1);

                            path.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                            path.AddArc(rect.Width - (cornerRadius * 2), rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                            path.AddArc(rect.Width - (cornerRadius * 2), rect.Height - (cornerRadius * 2), cornerRadius * 2, cornerRadius * 2, 0, 90);
                            path.AddArc(rect.X, rect.Height - (cornerRadius * 2), cornerRadius * 2, cornerRadius * 2, 90, 90);
                            path.CloseFigure();

                            g.SetClip(path);
                        }

                        g.DrawImage(qrCodeImage, padding, padding);
                    }

                    if (!string.IsNullOrEmpty(selectPath))
                    {
                        Bitmap logo = new Bitmap(selectPath);
                        generateQRCodeWithLogo = CombineLogo(borderedQR, logo);
                    }
                    else
                    {
                        generateQRCodeWithLogo = borderedQR;
                    }

                    if (isFrameAdded)
                    {
                        ApplyFrame();
                    }
                    else
                    {
                        QRCode.Source = BitmapToImage(generateQRCodeWithLogo);
                    }
                }
            }
        }

        private Bitmap CombineLogo(Bitmap qrCodeImage, Bitmap logo)
        {
            int logoSize = qrCodeImage.Width / 5;
            Bitmap resizedLogo = new Bitmap(logo, new System.Drawing.Size(logoSize, logoSize));

            Bitmap circularLogo = new Bitmap(logoSize, logoSize);
            using (Graphics g = Graphics.FromImage(circularLogo))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, logoSize, logoSize);
                    g.SetClip(path);

                    g.DrawImage(resizedLogo, 0, 0, logoSize, logoSize);
                }
            }

            Bitmap combinedImage = new Bitmap(qrCodeImage.Width, qrCodeImage.Height);
            using (Graphics g = Graphics.FromImage(combinedImage))
            {
                g.DrawImage(qrCodeImage, 0, 0);

                int centerX = (qrCodeImage.Width - circularLogo.Width) / 2;
                int centerY = (qrCodeImage.Height - circularLogo.Height) / 2;

                g.DrawImage(circularLogo, centerX, centerY);
            }

            return combinedImage;
        }

        private BitmapImage BitmapToImage(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void BtnChoseIMG_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), App.main.dir);

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Multiselect = false,
                InitialDirectory = folderPath,
            };
            if (dialog.ShowDialog() == true)
            {
                selectPath = dialog.FileName;
                GenerateQR();
            }
        }

        private void ApplyFrame()
        {
            if (generateQRCodeWithLogo == null)
            {
                return;
            }

            if (!File.Exists(framePath))
            {
                MessageBox.Show("Frame image not found in Assets folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (Bitmap frameImage = new Bitmap(framePath))
            {
                int qrCodeTargetWidth = 350;
                int qrCodeTargetHeight = 340;
                int posX = 300;
                int posY = 180;

                if (Path.GetFileName(framePath) == "frame.png")
                {
                    qrCodeTargetWidth = 346;
                    qrCodeTargetHeight = 340;
                    posX = 302;
                    posY = 180;
                }
                else if (Path.GetFileName(framePath) == "frameTwo.png")
                {
                    qrCodeTargetWidth = 600;
                    qrCodeTargetHeight = 600;
                    posX = 278;
                    posY = 180;
                }
                else if (Path.GetFileName(framePath) == "frameThree.jpg")
                {
                    qrCodeTargetWidth = 440;
                    qrCodeTargetHeight = 450;
                    posX = 170;
                    posY = 290;
                }
                else if (Path.GetFileName(framePath) == "frameFour.jpg")
                {
                    qrCodeTargetWidth = 350;
                    qrCodeTargetHeight = 350;
                    posX = 290;
                    posY = 275;
                }

                Bitmap combinedWithFrame = new Bitmap(frameImage);
                Bitmap resizedQRCode = new Bitmap(generateQRCodeWithLogo, new System.Drawing.Size(qrCodeTargetWidth, qrCodeTargetHeight));

                using (Graphics g = Graphics.FromImage(combinedWithFrame))
                {
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.DrawImage(resizedQRCode, posX, posY, qrCodeTargetWidth, qrCodeTargetHeight);
                }

                generateQRCodeWithLogo = new Bitmap(combinedWithFrame);
                QRCode.Source = BitmapToImage(generateQRCodeWithLogo);
            }
            isFrameAdded = true;
        }

        private void frame1_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (qrBackColor == Color.Black && qrColor == Color.White)
                {
                    qrBackColor = Color.White;
                    qrColor = Color.Black;
                }

                var frame = button.Name;

                switch (frame)
                {
                    case "frame1":
                        qrBackColor = Color.Black;
                        qrColor = Color.White;
                        framePath = Path.Combine("Assets", "frame.png");
                        if (isFrame1Clicked)
                        {
                            ClearFrame();
                            isFrame1Clicked = false;
                        }
                        else
                        {
                            isFrame1Clicked = true;
                            ApplyFrame();
                            GenerateQR();
                        }
                        break;
                    case "frame2":
                        framePath = Path.Combine("Assets", "frameTwo.png");
                        if (isFrame2Clicked)
                        {
                            ClearFrame();
                            isFrame2Clicked = false;
                        }
                        else
                        {
                            isFrame2Clicked = true;
                            ApplyFrame();
                            GenerateQR();
                        }
                        break;
                    case "frame3":
                        framePath = Path.Combine("Assets", "frameThree.jpg");
                        if (isFrame3Clicked)
                        {
                            ClearFrame();
                            isFrame3Clicked = false;
                        }
                        else
                        {
                            isFrame3Clicked = true;
                            ApplyFrame();
                            GenerateQR();
                        }
                        break;
                    case "frame4":
                        framePath = Path.Combine("Assets", "frameFour.jpg");
                        if (isFrame4Clicked)
                        {
                            ClearFrame();
                            isFrame4Clicked = false;
                        }
                        else
                        {
                            isFrame4Clicked = true;
                            ApplyFrame();
                            GenerateQR();
                        }
                        break;
                }
                if (generateQRCodeWithLogo == null)
                {
                    MessageBox.Show("QR Code is not generated.");
                    return;
                }
            }
        }

        private void ClearFrame()
        {
            qrBackColor = Color.White;
            qrColor = Color.Black;
            framePath = "";
            isFrameAdded = false;
            currentFrame = null;
            GenerateQR();
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            var urlPanel = Grid_Center.Children[0] as URL_Panel;
            if (urlPanel == null || string.IsNullOrWhiteSpace(urlPanel.TxtUrl.Text))
            {
                MessageBox.Show("Please enter valid content to generate QR code.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (generateQRCodeWithLogo == null)
            {
                MessageBox.Show("Please enter valid content to generate QR code.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Rest of your copy code...
                using (Bitmap finalImage = new Bitmap(generateQRCodeWithLogo.Width, generateQRCodeWithLogo.Height))
                {
                    using (Graphics g = Graphics.FromImage(finalImage))
                    {
                        g.Clear(System.Drawing.Color.White);
                        g.DrawImage(generateQRCodeWithLogo, 0, 0, generateQRCodeWithLogo.Width, generateQRCodeWithLogo.Height);
                    }

                    using (MemoryStream memory = new MemoryStream())
                    {
                        finalImage.Save(memory, ImageFormat.Png);
                        memory.Position = 0;

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();

                        Clipboard.SetImage(bitmapImage);
                    }
                }

                SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}");
                connection.Open();
                string sql = $"INSERT INTO tbl_url (url, date) VALUES (@url, @date);";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@url", path);
                    command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var urlPanel = Grid_Center.Children[0] as URL_Panel;
            if (urlPanel == null || string.IsNullOrWhiteSpace(urlPanel.TxtUrl.Text))
            {
                MessageBox.Show("Please enter valid content to generate QR code.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (generateQRCodeWithLogo == null)
            {
                MessageBox.Show("Please enter valid content to generate QR code.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "BLT_QR_Code",
                DefaultExt = ".png",
                Filter = "PNG file|*.png|JPG file|*.jpg|JPEG file|*.jpeg|Bitmap file|*.bmp",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string extension = Path.GetExtension(filePath).ToLower();

                if (string.IsNullOrEmpty(extension))
                {
                    filePath += ".png";
                }

                try
                {
                    if (filePath.EndsWith(".png"))
                    {
                        generateQRCodeWithLogo.Save(filePath, ImageFormat.Png);
                    }
                    else if (filePath.EndsWith(".jpg") || filePath.EndsWith(".jpeg"))
                    {
                        generateQRCodeWithLogo.Save(filePath, ImageFormat.Jpeg);
                    }
                    else if (filePath.EndsWith(".bmp"))
                    {
                        generateQRCodeWithLogo.Save(filePath, ImageFormat.Bmp);
                    }
                    else
                    {
                        MessageBox.Show("Invalid File Extension", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
