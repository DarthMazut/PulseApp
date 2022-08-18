using CommunityToolkit.Mvvm.ComponentModel;
using MochaCore.Settings;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public partial class QuickDownloadSummary : ObservableObject
    {
        private QuickDownloadSummary() { }

        [ObservableProperty]
        private string? _title;

        [ObservableProperty]
        private byte[]? _rawImage;

        [ObservableProperty]
        private MediumSelection _mediumSelection;

        [ObservableProperty]
        private MemorySpace? _fileSize;

        [ObservableProperty]
        private string? _sourceFormat;

        [ObservableProperty]
        private string? _targetFormat;

        [ObservableProperty]
        private ResolutionInfo? _resolution;

        [ObservableProperty]
        private double? _rate;

        [ObservableProperty]
        private TimeSpan _duration;

        [ObservableProperty]
        private string? _codec;

        [ObservableProperty]
        private string? _url;

        [ObservableProperty]
        private DirectoryInfo? _outputDirectory;

        [ObservableProperty]
        private string? _targetFileName;

        public static async Task<(QuickDownloadSummary, FormatSelection?)> FromNavigationData(QuickDownloadNavigationData navigationData, ApplicationSettings settings)
        {
            QuickDownloadSummary newSummary = new();
            FormatSelection? formatSelection = null;

            await FillDownloadSummaryBase(newSummary, navigationData, settings);

            if (navigationData.MediumSelection == MediumSelection.Music)
            {
                formatSelection = FillDownloadSummaryForMusic(newSummary, navigationData);
            }

            if (navigationData.MediumSelection == MediumSelection.Video)
            {
                formatSelection = FillDownloadSummaryForVideo(newSummary, navigationData);
            }

            return (newSummary, formatSelection);
        }

        private static async Task FillDownloadSummaryBase(QuickDownloadSummary newSummary, QuickDownloadNavigationData navigationData, ApplicationSettings settings)
        {
            newSummary.Title = navigationData.Metadata.Title;
            newSummary.RawImage = await navigationData.DownloadRawImageAsJpeg();
            newSummary.MediumSelection = navigationData.MediumSelection;
            newSummary.Duration = navigationData.Metadata.Durotion;
            newSummary.Url = navigationData.Metadata.VideoUrl;
            newSummary.TargetFileName = navigationData.Metadata.Title;
            newSummary.OutputDirectory = new DirectoryInfo(settings.DefaultOutputFolder);
        }

        private static FormatSelection? FillDownloadSummaryForVideo(QuickDownloadSummary newSummary, QuickDownloadNavigationData navigationData)
        {
            FormatInfo? videoFormat = navigationData.SelectedVideoFormat;
            FormatInfo? audioFormat = navigationData.Metadata.FormatTable?.ResolveBestAudioFormatForExtension(navigationData.SelectedVideoFormat.Extension);
            if (videoFormat is not null && audioFormat is not null)
            {
                FormatSelection formatSelection = FormatSelection.FromAudioVideoFormat(audioFormat, videoFormat);

                newSummary.FileSize = videoFormat.FileSize + audioFormat.FileSize;
                newSummary.SourceFormat = videoFormat.Extension;
                newSummary.TargetFormat = videoFormat.Extension;
                newSummary.Codec = videoFormat.VideoDetails?.Codec;
                newSummary.Resolution = videoFormat.VideoDetails?.Resolution;

                return formatSelection;
            }

            return null;
        }

        private static FormatSelection? FillDownloadSummaryForMusic(QuickDownloadSummary newSummary, QuickDownloadNavigationData navigationData)
        {
            FormatInfo? musicFormat = navigationData.Metadata.FormatTable?.GetHighestAudioQuality();
            if (musicFormat is not null)
            {
                FormatSelection formatSelection = FormatSelection.FromAudioFormat(musicFormat);

                newSummary.FileSize = musicFormat.FileSize;
                newSummary.SourceFormat = musicFormat.Extension;
                newSummary.TargetFormat = "mp3";
                newSummary.Codec = musicFormat.AudioDetails?.Codec;
                newSummary.Rate = musicFormat.AudioDetails?.SamplingRate;

                return formatSelection;
            }

            return null;
        }
    }
}
