using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BLT_Generator.Helpers
{
    public static class ToggleButtonHelper
    {
        public static readonly DependencyProperty CheckedIconProperty =
        DependencyProperty.RegisterAttached("CheckedIcon", typeof(string), typeof(ToggleButtonHelper), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty UncheckedIconProperty =
            DependencyProperty.RegisterAttached("UncheckedIcon", typeof(string), typeof(ToggleButtonHelper), new PropertyMetadata(default(string)));

        public static void SetCheckedIcon(UIElement element, string value) => element.SetValue(CheckedIconProperty, value);
        public static string GetCheckedIcon(UIElement element) => (string)element.GetValue(CheckedIconProperty);

        public static void SetUncheckedIcon(UIElement element, string value) => element.SetValue(UncheckedIconProperty, value);
        public static string GetUncheckedIcon(UIElement element) => (string)element.GetValue(UncheckedIconProperty);
    }
}
