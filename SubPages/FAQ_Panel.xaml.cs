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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLT_Generator.SubPages
{
    /// <summary>
    /// Interaction logic for FAQ_Panel.xaml
    /// </summary>
    public partial class FAQ_Panel : UserControl
    {
        public event Action? RequestCloseDrawer;

        public FAQ_Panel()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AnimateButton(sender!);
            RequestCloseDrawer?.Invoke();
        }
        private void AnimateButton(object sender)
        {
            Button? button = sender as Button;
            if (button == null) return;

            if (button.RenderTransform == null || !(button.RenderTransform is ScaleTransform))
            {
                button.RenderTransform = new ScaleTransform(1.0, 1.0);
                button.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.5,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            button.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

            ScaleTransform? scaleTransform = button.RenderTransform as ScaleTransform;

            DoubleAnimation scaleAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.8,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            scaleTransform!.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);

            // Reverse the animation after a short delay
            DoubleAnimation reverseOpacityAnimation = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3),
                BeginTime = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation reverseScaleAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.3),
                BeginTime = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            button.BeginAnimation(UIElement.OpacityProperty, reverseOpacityAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, reverseScaleAnimation);
        }
    }
}
