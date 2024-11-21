using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace BLT_Generator.Pages
{
    public partial class AddIcon : Window
    {
        private string assetsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets");
        private string? defaultIconsPath;
        private GeneratePage generatePage;

        public AddIcon(GeneratePage generatePage)
        {
            InitializeComponent();
            this.generatePage = generatePage;
            LoadImage();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadImage()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            assetsPath = Path.GetFullPath(assetsPath);
            defaultIconsPath = Path.Combine(assetsPath, "DefaultIcons");
            string[] files = Directory.GetFiles(defaultIconsPath);

            // Load default icons
            for (int i = 0; i < 3; i++)
            {
                using (var stream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    ImageBrush imageBrush = new ImageBrush { ImageSource = bitmap };

                    switch (i)
                    {
                        case 0:
                            Default1.Background = imageBrush;
                            Default1.Tag = files[i];
                            break;
                        case 1:
                            Default2.Background = imageBrush;
                            Default2.Tag = files[i];
                            break;
                        case 2:
                            Default3.Background = imageBrush;
                            Default3.Tag = files[i];
                            break;
                    }
                }
            }

            // Load recent images
            string imagePath = Path.Combine(exePath, App.main.dir);
            string[] supportedExtensions = { ".png", ".jpg", ".jpeg" };
            int j = 1;

            foreach (string file in Directory.GetFiles(imagePath))
            {
                if (supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                {
                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        ImageBrush imageBrush = new ImageBrush
                        {
                            ImageSource = bitmap,
                            Stretch = Stretch.UniformToFill
                        };

                        Button? targetButton = null;
                        switch (j % 5)
                        {
                            case 1:
                                targetButton = Recent1;
                                break;
                            case 2:
                                targetButton = Recent2;
                                break;
                            case 3:
                                targetButton = Recent3;
                                break;
                            case 4:
                                targetButton = Recent4;
                                break;
                            case 0:
                                targetButton = Recent5;
                                break;
                        }

                        if (targetButton != null)
                        {
                            targetButton.Background = imageBrush;
                            targetButton.Tag = file;
                        }
                        j++;
                    }
                }
            }
        }

        private void Default1_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string? imagePath = button.Tag as string;
                if (imagePath != null && generatePage != null)
                {
                    generatePage.UpdateQRCodeIcon(imagePath);
                }
            }
            this.Close();
        }

        private void Recent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string? imagePath = button.Tag as string;
                if (imagePath != null && generatePage != null)
                {
                    generatePage.UpdateQRCodeIcon(imagePath);
                }
            }
            this.Close();
        }

        //private void Default2_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button button)
        //    {
        //        string? imagePath = button.Tag as string;
        //        if (imagePath != null)
        //        {
        //            generatePage.UpdateQRCodeIcon(imagePath);
        //        }
        //    }
        //    this.Close();
        //}

        //private void Default3_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button button)
        //    {
        //        string? imagePath = button.Tag as string;
        //        if (imagePath != null)
        //        {
        //            generatePage.UpdateQRCodeIcon(imagePath);
        //        }
        //    }
        //    this.Close();
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            const int MAX_IMAGES = 5;
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "PNG file|*.png|JPG file|*.jpg|All file|*.*",
                Multiselect = false,
            };

            if (dialog.ShowDialog() == true)
            {
                string file = Path.GetFileName(dialog.FileName);
                string fullDir = Path.Combine(App.main.dir, file);

                // Get list of image files
                string[] imageFiles = Directory.GetFiles(App.main.dir, "*.*")
                    .Where(f => f.ToLower().EndsWith(".png") || f.ToLower().EndsWith(".jpg"))
                    .ToArray();

                // Delete the oldest image if we exceed the max limit
                if (imageFiles.Length >= MAX_IMAGES)
                {
                    var fileInfos = imageFiles
                        .Select(f => new FileInfo(f))
                        .OrderBy(fi => fi.CreationTime)
                        .ToList();

                    string oldestFile = fileInfos.First().FullName;
                    File.Delete(oldestFile);
                }

                // Create a copy of the file
                using (var sourceStream = File.OpenRead(dialog.FileName))
                using (var destinationStream = File.Create(fullDir))
                {
                    sourceStream.CopyTo(destinationStream);
                }

                generatePage.UpdateQRCodeIcon(fullDir);
                this.Close();
            }
        }

    }
}