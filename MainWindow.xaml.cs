using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using BLT_Generator.SubPages;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLT_Generator.Pages;
using System.IO;

namespace BLT_Generator
{
public partial class MainWindow : Window
{
public string dir = "Data";

// Remove this property as it's causing the conflict
// public ToggleButton Btn_Generate => this.Btn_Generate;

    public void PageBig(UserControl page)
    {
    Grid_Big.Children.Clear();
    Grid_Big.Children.Add(page);
    }

    public MainWindow()
    {
    InitializeComponent();
    Btn_Generate.IsChecked = true;
    SetSelectedButton(Btn_Generate);
    PageBig(new Pages.GeneratePage());
    Directory.CreateDirectory(dir);
    }

    public void SetSelectedButton(ToggleButton selectedButton)
    {
    var stackPanel = (StackPanel)Btn_Generate.Parent;
    foreach (var child in stackPanel.Children)
    {
    if (child is ToggleButton button && button != selectedButton)
    {
    button.IsChecked = false;
    }
    }
    selectedButton.IsChecked = true;
    }

    private void Btn_Generate_Click(object sender, RoutedEventArgs e)
    {
    SetSelectedButton(Btn_Generate);
    PageBig(new Pages.GeneratePage());
    }

    private void Btn_Profile_Click(object sender, RoutedEventArgs e)
    {
    SetSelectedButton(Btn_Profile);
    PageBig(new Pages.ProfilePage());
    }

    private void Btn_Setting_Click(object sender, RoutedEventArgs e)
    {
    SetSelectedButton(Btn_Setting);
    PageBig(new Pages.SettingPage());
    }

    private void Btn_History_Click(object sender, RoutedEventArgs e)
    {
    SetSelectedButton(Btn_History);
    PageBig(new Pages.HistoryPage());
    }

    private void Btn_Close_Click(object sender, RoutedEventArgs e)
    {
    this.Close();
    }
    private void Btn_Minimize_Click(object sender, RoutedEventArgs e)
    {
    this.WindowState = WindowState.Minimized;
    }

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
    if (e.ChangedButton == MouseButton.Left)
    {
    this.DragMove();
    }
    }

    private void Btn_Maximize_Click(object sender, RoutedEventArgs e)
    {
    if (this.WindowState == WindowState.Normal)
    {
    this.WindowState = WindowState.Maximized;
    }
    else
    {
    this.WindowState = WindowState.Normal;
    }
    }
    }
    }