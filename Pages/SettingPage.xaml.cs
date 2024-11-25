using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLT_Generator.Pages
{
/// <summary>
    /// Interaction logic for SettingPage.xaml
/// </summary>
public partial class SettingPage : UserControl
{
private bool isDrawerOpen = false;
private void OpenPage(UserControl page)
{
Grid_Display.Children.Clear();
Grid_Display.Children.Add(page);
}
public SettingPage()
{
InitializeComponent();
Btn_Language.IsChecked = true;
SetSelectedButton(Btn_Language);

OpenPage(new SubPages.Language_Panel());

var faqPanel = new BLT_Generator.SubPages.FAQ_Panel();
faqPanel.RequestCloseDrawer += HandleDrawerToggle;

Grid_FAQ.Children.Clear();
Grid_FAQ.Children.Add(faqPanel);
}

private void Btn_Language_Click(object sender, RoutedEventArgs e)
{
SetSelectedButton(Btn_Language);
OpenPage(new SubPages.Language_Panel());
}

private void Btn_Theme_Click(object sender, RoutedEventArgs e)
{
SetSelectedButton(Btn_Theme);
OpenPage(new SubPages.Theme_Panel());
}


private void Btn_FAQ_Click(object sender, RoutedEventArgs e)
{
HandleDrawerToggle();
}

private void HandleDrawerToggle()
{
DoubleAnimation animation = new DoubleAnimation
{
Duration = TimeSpan.FromSeconds(0.3),
To = isDrawerOpen ? 0 : 210,
EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
};

Grid_FAQ.BeginAnimation(WidthProperty, animation);

isDrawerOpen = !isDrawerOpen;
}

private void SetSelectedButton(ToggleButton selectedButton)
{
foreach (var child in ((Grid)Btn_Language.Parent).Children)
{
if (child is ToggleButton button && button != selectedButton)
{
button.IsChecked = false;
}
}

selectedButton.IsChecked = true;
}
}
}
