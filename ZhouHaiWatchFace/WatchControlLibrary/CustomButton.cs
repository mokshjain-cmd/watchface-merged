using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Button = System.Windows.Controls.Button;

namespace WatchControlLibrary
{
    public class CustomButton : Button
    {

        public Brush BackgroundHover
        {
            get { return (Brush)GetValue(BackgroundHoverProperty); }
            set { SetValue(BackgroundHoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundHoverProperty =
            DependencyProperty.Register("BackgroundHover", typeof(Brush), typeof(CustomButton));


        public Brush BackgroundFocus
        {
            get { return (Brush)GetValue(BackgroundFocusProperty); }
            set { SetValue(BackgroundFocusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundFocusProperty =
            DependencyProperty.Register("BackgroundFocus", typeof(Brush), typeof(CustomButton));


        public CornerRadius ButtonCornerRadius
        {
            get { return (CornerRadius)GetValue(ButtonCornerRadiusProperty); }
            set { SetValue(ButtonCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonCornerRadiusProperty =
            DependencyProperty.Register("ButtonCornerRadius", typeof(CornerRadius), typeof(CustomButton));


        public Brush ButtonForeground
        {
            get { return (Brush)GetValue(ButtonForegroundProperty); }
            set { SetValue(ButtonForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonForegroundProperty =
            DependencyProperty.Register("ButtonForeground", typeof(Brush), typeof(CustomButton));
    }
}
