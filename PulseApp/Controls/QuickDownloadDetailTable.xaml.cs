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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PulseApp.Controls
{
    public sealed partial class QuickDownloadDetailTable : UserControl
    {
        #region DEPENDENCY PROPERTY

        /*
        public static readonly DependencyProperty OutputFileNameProperty =
            DependencyProperty.Register(nameof(OutputFileName), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty OutputFolderPathProperty =
            DependencyProperty.Register(nameof(OutputFolderPath), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register(nameof(Link), typeof(Uri), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty RateTextProperty =
            DependencyProperty.Register(nameof(RateText), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty CodecNameProperty =
            DependencyProperty.Register(nameof(CodecName), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty ResolutionTextProperty =
            DependencyProperty.Register(nameof(ResolutionText), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty TargetFormatNameProperty =
            DependencyProperty.Register(nameof(TargetFormatName), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty SourceFormatNameProperty =
            DependencyProperty.Register(nameof(SourceFormatName), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty DurationTextProperty =
            DependencyProperty.Register(nameof(DurationText), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty FileSizeTextProperty =
            DependencyProperty.Register(nameof(FileSizeText), typeof(string), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty MediumTypeProperty =
            DependencyProperty.Register(nameof(MediumType), typeof(MediumSelection), typeof(QuickDownloadDetailTable), new PropertyMetadata(MediumSelection.Video));

        public string OutputFileName
        {
            get { return (string)GetValue(OutputFileNameProperty); }
            set { SetValue(OutputFileNameProperty, value); }
        }

        public string OutputFolderPath
        {
            get { return (string)GetValue(OutputFolderPathProperty); }
            set { SetValue(OutputFolderPathProperty, value); }
        }

        public Uri Link
        {
            get { return (Uri)GetValue(LinkProperty); }
            set { SetValue(LinkProperty, value); }
        }

        public string RateText
        {
            get { return (string)GetValue(RateTextProperty); }
            set { SetValue(RateTextProperty, value); }
        }

        public string CodecName
        {
            get { return (string)GetValue(CodecNameProperty); }
            set { SetValue(CodecNameProperty, value); }
        }

        public string ResolutionText
        {
            get { return (string)GetValue(ResolutionTextProperty); }
            set { SetValue(ResolutionTextProperty, value); }
        }

        public string TargetFormatName
        {
            get { return (string)GetValue(TargetFormatNameProperty); }
            set { SetValue(TargetFormatNameProperty, value); }
        }

        public string SourceFormatName
        {
            get { return (string)GetValue(SourceFormatNameProperty); }
            set { SetValue(SourceFormatNameProperty, value); }
        }

        public string DurationText
        {
            get { return (string)GetValue(DurationTextProperty); }
            set { SetValue(DurationTextProperty, value); }
        }

        public string FileSizeText
        {
            get { return (string)GetValue(FileSizeTextProperty); }
            set { SetValue(FileSizeTextProperty, value); }
        }

        public MediumSelection MediumType
        {
            get { return (MediumSelection)GetValue(MediumTypeProperty); }
            set { SetValue(MediumTypeProperty, value); }
        }

        */

        public static readonly DependencyProperty EditOutputFileNameCommandProperty =
            DependencyProperty.Register(nameof(EditOutputFileNameCommand), typeof(ICommand), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty EditOutputFolderPathCommandProperty =
            DependencyProperty.Register(nameof(EditOutputFolderPathCommand), typeof(ICommand), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public static readonly DependencyProperty TableDataProperty =
            DependencyProperty.Register(nameof(TableData), typeof(QuickDownloadSummary), typeof(QuickDownloadDetailTable), new PropertyMetadata(null));

        public ICommand EditOutputFileNameCommand
        {
            get { return (ICommand)GetValue(EditOutputFileNameCommandProperty); }
            set { SetValue(EditOutputFileNameCommandProperty, value); }
        }

        public ICommand EditOutputFolderPathCommand
        {
            get { return (ICommand)GetValue(EditOutputFolderPathCommandProperty); }
            set { SetValue(EditOutputFolderPathCommandProperty, value); }
        }
        
        public QuickDownloadSummary TableData
        {
            get { return (QuickDownloadSummary)GetValue(TableDataProperty); }
            set { SetValue(TableDataProperty, value); }
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
}
