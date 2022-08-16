using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PulseApp.Controls
{
    public sealed partial class CompositeFontIcon : UserControl
    {
        public CompositeFontIcon()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FontIconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(string), typeof(CompositeFontIcon), new PropertyMetadata(null));

        public static readonly DependencyProperty BackgroundFontIconProperty =
            DependencyProperty.Register(nameof(BackgroundIcon), typeof(string), typeof(CompositeFontIcon), new PropertyMetadata(null));

        public string Icon
        {
            get { return (string)GetValue(FontIconProperty); }
            set { SetValue(FontIconProperty, value); }
        }

        public string BackgroundIcon
        {
            get { return (string)GetValue(BackgroundFontIconProperty); }
            set { SetValue(BackgroundFontIconProperty, value); }
        }
    }
}
