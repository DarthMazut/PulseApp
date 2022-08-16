using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PulseApp.Controls
{
    public sealed partial class AnimatedLogo : UserControl
    {
        public AnimatedLogo()
        {
            this.InitializeComponent();
            xe_LightningGradientStop.Color = (Color)LightningColorProperty.GetMetadata(typeof(AnimatedLogo)).DefaultValue;
        }

        public static readonly DependencyProperty LightningColorProperty =
            DependencyProperty.Register(nameof(LightningColor), typeof(Color), typeof(AnimatedLogo), new PropertyMetadata(Colors.DodgerBlue, OnLightningColorChanged));



        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(AnimatedLogo), new PropertyMetadata(Colors.Transparent, OnBackgroundColorChanged));

        public Color LightningColor
        {
            get { return (Color)GetValue(LightningColorProperty); }
            set { SetValue(LightningColorProperty, value); }
        }

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        private static void OnLightningColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AnimatedLogo thisControl)
            {
                thisControl.xe_LightningGradientStop.Color = (Color)e.NewValue;
            }
        }

        private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AnimatedLogo thisControl)
            {
                thisControl.xe_BackgroundGradientStop.Color = (Color)e.NewValue;
            }
        }
    }
}
