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
        private string currentFrameName = "";
        private string ssid = "";
        private string password = "";
        private string encryptionType = "";
        private bool isOnWIFI = false;
        private bool isOnURL = false;

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

                var tables = new[]
                {
                    @"CREATE TABLE tbl_url (
                        id         INTEGER PRIMARY KEY AUTOINCREMENT,
                        url        TEXT    NOT NULL,
                        date       DATE    NOT NULL
                    )",
                    @"CREATE TABLE tbl_wifi (
                        id               INTEGER PRIMARY KEY AUTOINCREMENT,
                        ssid             TEXT    NOT NULL,
                        password         TEXT    NOT NULL,
                        encryptionType   TEXT    NOT NULL,
                        date             DATE    NOT NULL
                    )"
                };

                using (var connection = new SQLiteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var createTable in tables)
                            {
                                using (var command = new SQLiteCommand(createTable, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }

        }

        public void SetIsOnWIFI(bool value)
        {
            isOnWIFI = value;
        }

        public void SelectWifiButton()
        {
            if (Btn_WIFI != null)
            {
                SetSelectedButton(Btn_WIFI);
                isOnWIFI = true;
            }
        }

        public void SetIsOnURL(bool value)
        {
            isOnURL = value;
        }

        public void SelectUrlButton()
        {
            if (Btn_URL != null)
            {
                SetSelectedButton(Btn_URL);
                isOnWIFI = false;
            }
        }

        public void SetWifiCredentials(string ssid, string password, string encryptionType)
        {
            this.ssid = ssid;
            this.password = password;
            this.encryptionType = encryptionType;
        }

        public void SetUrlCredentials(string url)
        {
            this.path = url;
            GenerateQR();
        }

        private URL_Panel CreateUrlPanel()
        {
            var panel = new URL_Panel(this);
            if (!string.IsNullOrWhiteSpace(path))
            {
                panel.TxtUrl.Text = path;
            }
            return panel;
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
            isOnWIFI = true;
        }

        private void Btn_URL_Click(object sender, RoutedEventArgs e)
        {
            path = "";
            SetSelectedButton(Btn_URL);
            PageCenter(CreateUrlPanel());
            isOnWIFI = false;
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
                return; 
            }
            using (QRCodeGenerator qrCodeGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(path, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(20, qrColor, qrBackColor, false);
                    int padding = 35;
                    int additionalHeight = 70;
                    Bitmap borderedQR = new Bitmap(qrCodeImage.Width + (padding * 2), qrCodeImage.Height + (padding * 2) + additionalHeight);
                    using (Graphics g = Graphics.FromImage(borderedQR))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.Clear(Color.White);
                        if (string.IsNullOrEmpty(framePath))
                        {
                            using (GraphicsPath path = new GraphicsPath())
                            {
                                int cornerRadius = 15;
                                Rectangle rect = new Rectangle(0, 0, borderedQR.Width - 1, borderedQR.Height - 1);
                                path.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                                path.AddArc(rect.Width - (cornerRadius * 2), rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                                path.AddArc(rect.Width - (cornerRadius * 2), rect.Height - (cornerRadius * 2), cornerRadius * 2, cornerRadius * 2, 0, 90);
                                path.AddArc(rect.X, rect.Height - (cornerRadius * 2), cornerRadius * 2, cornerRadius * 2, 90, 90);
                                path.CloseFigure();
                                using (Pen borderPen = new Pen(qrColor, 2))
                                {
                                    g.DrawPath(borderPen, path);
                                }
                            }
                        }
                        else if (Path.GetFileName(framePath) == "frame.png")
                        {
                            g.Clear(Color.Black);
                        }
                        g.DrawImage(qrCodeImage, padding, padding);
                        if (!isOnWIFI && !string.IsNullOrEmpty(path))
                        {
                            Font font = new Font("Arial", 16, System.Drawing.FontStyle.Regular);
                            SolidBrush brush;
                            if (Path.GetFileName(framePath) == "frame.png")
                            {
                                brush = new SolidBrush(Color.White);
                            }
                            else
                            {
                                brush = new SolidBrush(Color.Black);
                            }
                            string url = $"Text: {path}";
                            g.DrawString(url, font, brush, new PointF(padding, qrCodeImage.Height + padding + 15));
                        }
                        if (isOnWIFI && !string.IsNullOrEmpty(ssid))
                        {
                            Font font = new Font("Arial", 16, System.Drawing.FontStyle.Regular);
                            SolidBrush brush;

                            if (Path.GetFileName(framePath) == "frame.png")
                            {
                                brush = new SolidBrush(Color.White);
                            }
                            else
                            {
                                brush = new SolidBrush(Color.Black);
                            }

                            string ssidText = $"SSID: {ssid}";

                            g.DrawString(ssidText, font, brush, new PointF(padding + 10, qrCodeImage.Height + padding + 15));

                            if (!string.IsNullOrEmpty(password))
                            {
                                string passwordText = $"Password: {password}";
                                g.DrawString(passwordText, font, brush, new PointF(padding + 10, qrCodeImage.Height + padding + 40));
                            }
                        }
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
            int borderWidth = 4;

            Bitmap circularLogo = new Bitmap(logoSize + (borderWidth * 2), logoSize + (borderWidth * 2));

            Bitmap resizedLogo = new Bitmap(logo, new System.Drawing.Size(logoSize, logoSize));

            using (Graphics g = Graphics.FromImage(circularLogo))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (GraphicsPath borderPath = new GraphicsPath())
                {
                    borderPath.AddEllipse(0, 0, logoSize + (borderWidth * 2), logoSize + (borderWidth * 2));
                    g.SetClip(borderPath);

                    using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                    {
                        g.FillPath(whiteBrush, borderPath);
                    }

                    using (GraphicsPath logoPath = new GraphicsPath())
                    {
                        logoPath.AddEllipse(borderWidth, borderWidth, logoSize, logoSize);
                        g.SetClip(logoPath);
                        g.DrawImage(resizedLogo, borderWidth, borderWidth, logoSize, logoSize);
                    }
                }
            }

            Bitmap combinedImage = new Bitmap(qrCodeImage.Width, qrCodeImage.Height);
            using (Graphics g = Graphics.FromImage(combinedImage))
            {
                g.DrawImage(qrCodeImage, 0, 0);

                // Center the logo with border
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
                    qrCodeTargetWidth = 340;
                    qrCodeTargetHeight = 335;
                    posX = 310;
                    posY = 182;
                    //qrCodeTargetWidth = 346;
                    //qrCodeTargetHeight = 340;
                    //posX = 300;
                    //posY = 180;
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
            ComboBoxColor.IsEnabled = true;
            ComboBoxColor.SelectedIndex = 0;
            qrBackColor = Color.White;
            qrColor = Color.Black;
            if (sender is Button button)
            {
                var frame = button.Name;

                if (generateQRCodeWithLogo == null)
                {
                    MessageBox.Show("Please generate a QR code first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (frame != currentFrameName)
                {
                    isFrame1Clicked = false;
                    isFrame2Clicked = false;
                    isFrame3Clicked = false;
                    isFrame4Clicked = false;
                }

                switch (frame)
                {
                    case "frame1":
                        if (isFrame1Clicked)
                        {
                            ClearFrame();
                            isFrame1Clicked = false;
                            currentFrameName = "";
                        }
                        else
                        {
                            ComboBoxColor.IsEnabled = false;
                            qrBackColor = Color.Black;
                            qrColor = Color.White;
                            framePath = Path.Combine("Assets", "frame.png");
                            isFrame1Clicked = true;
                            currentFrameName = frame;
                            ApplyFrame();
                            GenerateQR();
                        }
                        break;

                    case "frame2":
                        if (isFrame2Clicked)
                        {
                            ClearFrame();
                            isFrame2Clicked = false;
                            currentFrameName = "";
                        }
                        else
                        {
                            qrBackColor = Color.White;
                            qrColor = Color.Black;
                            framePath = Path.Combine("Assets", "frameTwo.png");
                            isFrame2Clicked = true;
                            currentFrameName = frame;
                            ApplyFrame();
                            GenerateQR();
                        }
                        break;

                    case "frame3":
                        if (isFrame3Clicked)
                        {
                            ClearFrame();
                            isFrame3Clicked = false;
                            currentFrameName = "";
                        }
                        else
                        {
                            qrBackColor = Color.White;
                            qrColor = Color.Black;
                            framePath = Path.Combine("Assets", "frameThree.jpg");
                            isFrame3Clicked = true;
                            currentFrameName = frame;
                            ApplyFrame();
                            GenerateQR();
                        }
                        break;

                    case "frame4":
                        if (isFrame4Clicked)
                        {
                            ClearFrame();
                            isFrame4Clicked = false;
                            currentFrameName = "";
                        }
                        else
                        {
                            qrBackColor = Color.White;
                            qrColor = Color.Black;
                            framePath = Path.Combine("Assets", "frameFour.jpg");
                            isFrame4Clicked = true;
                            currentFrameName = frame;
                            ApplyFrame();
                            GenerateQR();
                        }
                        break;
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
            //var urlPanel = Grid_Center.Children[0] as URL_Panel;
            //if (urlPanel == null || string.IsNullOrWhiteSpace(urlPanel.TxtUrl.Text))
            //{
            //    MessageBox.Show("Please enter valid content to generate QR code.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            if(path == "បញ្ចូល URL")
            {
                path = "";
                GenerateQR();
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
                if (!isOnWIFI)
                {
                    SaveURL();
                }

                //if (isOnWIFI)
                //{
                //    SaveWIFI();
                //}
                //else
                //{
                //    SaveURL();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveURL()
        {
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

        //private void SaveWIFI()
        //{
        //    SQLiteConnection connection = new SQLiteConnection($"Data Source={databasePath}");
        //    connection.Open();
        //    string sql = "INSERT INTO tbl_wifi (date, ssid, password, encryptionType) VALUES (@date, @ssid, @password, @encryptionType);";
        //    using (var command = new SQLiteCommand(sql, connection))
        //    {
        //        command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
        //        command.Parameters.AddWithValue("@ssid", ssid);
        //        command.Parameters.AddWithValue("@password", password);
        //        command.Parameters.AddWithValue("@encryptionType", encryptionType);
        //        command.ExecuteNonQuery();
        //    }
        //    connection.Close();
        //}

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (path == "បញ្ចូល URL")
            {
                path = "";
                GenerateQR();
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

                    if (!isOnWIFI)
                    {
                        SaveURL();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GenerateQrCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string wifiQrCode = $"WIFI:S:{ssid};T:{encryptionType};P:{password};;";

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 250,
                    Width = 250,
                    Margin = 1
                }
            };

            var pixelData = writer.Write(wifiQrCode);

            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                QRCode.Source = ConvertBitmapToBitmapImage(bitmap);
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void Btn_ClearIcon_Click(object sender, RoutedEventArgs e)
        {
            selectPath = "";
            GenerateQR();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxColor.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedColor = (selectedItem.Content as string)?.Trim();

                switch (selectedColor)
                {
                    case "ព័ណ៌ខ្មៅ":
                    case "Black":
                        qrBackColor = System.Drawing.Color.White;
                        qrColor = System.Drawing.Color.Black;
                        break;
                    case "ព័ណ៌ខៀវ":
                    case "Blue":
                        qrBackColor = System.Drawing.Color.White;
                        qrColor = System.Drawing.Color.Blue;
                        break;
                    case "ព័ណ៌បៃតង":
                    case "Green":
                        qrBackColor = System.Drawing.Color.White;
                        qrColor = System.Drawing.Color.Green;
                        break;
                    case "ព័ណ៌ក្រហម":
                    case "Red":
                        qrBackColor = System.Drawing.Color.White;
                        qrColor = System.Drawing.Color.Red;
                        break;
                    default:
                        qrBackColor = System.Drawing.Color.White;
                        qrColor = System.Drawing.Color.Black;
                        break;
                }

                if (!string.IsNullOrWhiteSpace(path))
                {
                    GenerateQR();
                }
            }
        }
    }
}