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
using ViewModels;
using ViewModels.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PulseApp.Controls
{
    public sealed partial class QuickDownloadDetailTable : UserControl
    {
        #region DEPENDENCY PROPERTY

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(QuickDownloadDetailsTableViewModel), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public QuickDownloadDetailsTableViewModel ViewModel
        {
            get { return (QuickDownloadDetailsTableViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        public QuickDownloadDetailTable()
        {
            this.InitializeComponent();
        }
    }

    public class QuickDownloadDetailsTableVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MediumSelection mediumSelection)
            {
                if (parameter as string == "Music")
                {
                    return mediumSelection == MediumSelection.Music ? Visibility.Visible : Visibility.Collapsed;
                }

                if (parameter as string == "Video")
                {
                    return mediumSelection == MediumSelection.Video ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToParameterValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolean)
            {
                return boolean ? parameter : null!;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
