using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.Navigation;
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
    public partial class QuickDownloadPageViewModel : ObservableObject, INavigatable, IOnNavigatedToAsync
    {
        private readonly string INITIALIZING_STATE_NAME = "Initializing";
        private readonly string DOWNLOADING_STATE_NAME = "Downloading";
        private readonly string MERGING_STATE_NAME = "Merging";
        private readonly string CLEANING_STATE_NAME = "Cleaning";
        private readonly string FINISHED_STATE_NAME = "Finished";
        private readonly string ERROR_STATE_NAME = "Error";

        private QuickDownloadNavigationData? _navigationData;
        private CancellationTokenSource _cts = new();

        public QuickDownloadPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public Navigator Navigator { get; }

        [ObservableProperty]
        private string? _state;

        [ObservableProperty]
        private string? _title;

        [ObservableProperty]
        private byte[]? _rawImage;

        public async Task OnNavigatedToAsync(NavigationData navigationData)
        {
            if (navigationData.Data is QuickDownloadNavigationData data && data.QuickDownloadSummary is not null)
            {
                _navigationData = data;
                Title = data.QuickDownloadSummary.TargetFileName;
                RawImage = await data.DownloadRawImageAsJpeg();

                ISettingsSectionProvider<ApplicationSettings> sectionProvider = SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID);
                ApplicationSettings settings = await sectionProvider.LoadAsync();

                YtdlpAdapter adapter = new YtdlpAdapter(settings.YtdlpPath, settings.FfmpegPath);
                
                DownloadJob downloadJob = new()
                {
                    Url = data.QuickDownloadSummary.Url,
                    Selection = data.FormatSelection,
                    OutputDirectory = data.QuickDownloadSummary.OutputDirectory,
                    DownloadedFileName = data.QuickDownloadSummary.TargetFileName,
                    CancellationToken = _cts.Token,
                    ProgressCallback = new Progress<DownloadProgress>(DownloadProgressReceived)
                };

                if (data.MediumSelection == MediumSelection.Video)
                {
                    //await adapter.DownloadVideoAsync(downloadJob);
                }

                if (data.MediumSelection == MediumSelection.Music)
                {
                    //await adapter.DownloadMusicAsync(downloadJob);
                }
            }
        }

        private void DownloadProgressReceived(DownloadProgress progress)
        {
            // TODO
        }

        [RelayCommand]
        private Task Abort()
        {
            _cts.Cancel();
            return Navigator.NavigateAsync(AppPages.QuickDownloadSummaryPage.Module, _navigationData);
        }

        [RelayCommand]
        private void DebugChangeState(string parameter)
        {
            State = parameter;
        }
    }
}
