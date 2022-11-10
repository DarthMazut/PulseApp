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
    public sealed partial class QuickDownloadDetailItem : UserControl
    {
        #region DEPENDENCY PROPERTY

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(QuickDownloadDetailItem), new PropertyMetadata(null));

        public static readonly DependencyProperty ValueTextProperty =
            DependencyProperty.Register(nameof(ValueText), typeof(string), typeof(QuickDownloadDetailItem), new PropertyMetadata(null));

        public static readonly DependencyProperty ValueHintTextProperty =
            DependencyProperty.Register(nameof(ValueHintText), typeof(string), typeof(QuickDownloadDetailItem), new PropertyMetadata(null));
        
        public static readonly DependencyProperty IsValueHintVisibleProperty =
            DependencyProperty.Register(nameof(IsValueHintVisible), typeof(bool), typeof(QuickDownloadDetailItem), new PropertyMetadata(false));

        public static readonly DependencyProperty ActionButtonCommandProperty =
            DependencyProperty.Register(nameof(ActionButtonCommand), typeof(ICommand), typeof(QuickDownloadDetailItem), new PropertyMetadata(null));

        public static readonly DependencyProperty ActionButtonIconProperty =
            DependencyProperty.Register(nameof(ActionButtonIcon), typeof(string), typeof(QuickDownloadDetailItem), new PropertyMetadata(null));

        public static readonly DependencyProperty HyperlinkUriProperty =
            DependencyProperty.Register(nameof(HyperlinkUri), typeof(Uri), typeof(QuickDownloadDetailItem), new PropertyMetadata(null));

        public static readonly DependencyProperty ErrorHintTextProperty =
            DependencyProperty.Register(nameof(ErrorHintText), typeof(string), typeof(QuickDownloadDetailItem), new PropertyMetadata(null));

        public static readonly DependencyProperty IsErrorHintVisibleProperty =
            DependencyProperty.Register(nameof(IsErrorHintVisible), typeof(bool), typeof(QuickDownloadDetailItem), new PropertyMetadata(false));

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public string ValueText
        {
            get { return (string)GetValue(ValueTextProperty); }
            set { SetValue(ValueTextProperty, value); }
        }

        public string ValueHintText
        {
            get { return (string)GetValue(ValueHintTextProperty); }
            set { SetValue(ValueHintTextProperty, value); }
        }

        public bool IsValueHintVisible
        {
            get { return (bool)GetValue(IsValueHintVisibleProperty); }
            set { SetValue(IsValueHintVisibleProperty, value); }
        }

        public ICommand ActionButtonCommand
        {
            get { return (ICommand)GetValue(ActionButtonCommandProperty); }
            set { SetValue(ActionButtonCommandProperty, value); }
        }

        public string ActionButtonIcon
        {
            get { return (string)GetValue(ActionButtonIconProperty); }
            set { SetValue(ActionButtonIconProperty, value); }
        }

        public Uri HyperlinkUri
        {
            get { return (Uri)GetValue(HyperlinkUriProperty); }
            set { SetValue(HyperlinkUriProperty, value); }
        }

        public string ErrorHintText
        {
            get { return (string)GetValue(ErrorHintTextProperty); }
            set { SetValue(ErrorHintTextProperty, value); }
        }

        public bool IsErrorHintVisible
        {
            get { return (bool)GetValue(IsErrorHintVisibleProperty); }
            set { SetValue(IsErrorHintVisibleProperty, value); }
        }

        #endregion


        public QuickDownloadDetailItem()
        {
            this.InitializeComponent();
        }

    }
}
