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
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PulseApp.Controls
{
    public sealed partial class TransitionButton : UserControl
    {
        private const string NORMAL_STATE_NAME = "Normal";
        private const string HOVERED_STATE_NAME = "Hovered";

        public TransitionButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(TransitionButton), new PropertyMetadata(null, OnImagePropertyChanged));

        public static readonly DependencyProperty FontIconProperty =
            DependencyProperty.Register(nameof(FontIcon), typeof(CompositeFontIcon), typeof(TransitionButton), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(TransitionButton), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(TransitionButton), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(TransitionButton), new PropertyMetadata(null));


        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public CompositeFontIcon FontIcon
        {
            get { return (CompositeFontIcon)GetValue(FontIconProperty); }
            set { SetValue(FontIconProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void OnImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TransitionButton thisControl = d as TransitionButton;
            thisControl.xe_Image.Visibility = thisControl.xe_Image != null ? Visibility.Visible : Visibility.Collapsed;
            thisControl.xe_FontIconViewBox.Visibility = thisControl.xe_Image != null ? Visibility.Collapsed : Visibility.Visible;
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            VisualStateManager.GoToState(this, HOVERED_STATE_NAME, true);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            VisualStateManager.GoToState(this, NORMAL_STATE_NAME, true);
        }
    }

    public class TransitionButtonOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolean)
            {
                return boolean ? 1.0 : 0.2;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
