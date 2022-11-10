using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.Navigation;
using MochaCore.Settings;
using Model;
using Model.Adapter;
using Model.Progress;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Logging;

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
        private bool _faulted;

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

        [ObservableProperty]
        private string? _partsText;

        [ObservableProperty]
        private int _percentageValue;

        [ObservableProperty]
        private string? _percentageText;

        [ObservableProperty]
        private string? _currentDownloadSpeedText;

        [ObservableProperty]
        private string? _downloadedText;

        [ObservableProperty]
        private string? _etaText;

        [ObservableProperty]
        private string? _summaryText;

        public async Task OnNavigatedToAsync(NavigationData navigationData)
        {
            Logger.LogInfo($"Navigated to {GetType().Name}");

            if (navigationData.Data is QuickDownloadNavigationData data)
            {
                _navigationData = data;
                Title = data.TargetFileName;
                RawImage = await data.DownloadRawImageAsJpeg();

                ISettingsSectionProvider<ApplicationSettings> sectionProvider = SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID);
                ApplicationSettings settings = await sectionProvider.LoadAsync();

                YtdlpAdapter adapter = new YtdlpAdapter(settings.YtdlpPath, settings.FfmpegPath);
                
                DownloadJob downloadJob = new()
                {
                    Url = data.Metadata.VideoUrl,
                    Selection = data.FormatSelection,
                    OutputDirectory = data.OutputDirectory,
                    DownloadedFileName = data.TargetFileName,
                    CancellationToken = _cts.Token,
                    ProgressCallback = new Progress<DownloadProgress>(DownloadProgressReceived)
                };

                Stopwatch stopwatch = new();
                stopwatch.Start();

                try
                {
                    if (data.MediumSelection == MediumSelection.Video)
                    {
                        Logger.LogInfo("About to invoke DownloadVideoAsync().");
                        await adapter.DownloadVideoAsync(downloadJob);
                    }

                    if (data.MediumSelection == MediumSelection.Music)
                    {
                        Logger.LogInfo("About to invoke DownloadMusicAsync().");
                        await adapter.DownloadMusicAsync(downloadJob);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    Logger.LogInfo(ex, "Downloading operation cancelled."); 
                }
                finally
                {
                    Logger.LogInfo("Disposing _cts...");
                    _cts.Dispose();
                    Logger.LogInfo("_cts disposed successfully");
                    stopwatch.Stop();
                }

                Logger.LogInfo($"Downloading finished; faulted={_faulted}");

                SummaryText = _faulted ?
                    "Something went wrong. Please contact app provider." :
                    $"Downloading finished in: {stopwatch.Elapsed:hh\\:mm\\:ss}";

                ChangeState(_faulted ? ERROR_STATE_NAME : FINISHED_STATE_NAME);
            }
        }

        private void DownloadProgressReceived(DownloadProgress progress)
        {
            Logger.LogDebug(progress.CurrentMessage);

            if (progress.IsErrorMessage && !_faulted)
            {
                _faulted = true;
                Logger.LogInfo($"Faulted set to TRUE");
            }

            if (progress.State == DownloadState.Downloading)
            {
                ChangeState(DOWNLOADING_STATE_NAME);

                PartsText = $"{progress.CurrentPart}/{progress.Parts}";
                PercentageValue = progress.CurrentPercentage is double currentValuePercentage ? (int)Math.Round(currentValuePercentage) : 0;
                PercentageText = progress.CurrentPercentage is double currentTextPercentage ? Math.Round(currentTextPercentage).ToString() + " %" : string.Empty;
                CurrentDownloadSpeedText = progress.CurrentSpeed?.ToString() ?? string.Empty;
                DownloadedText = $"{progress.CurrentFileSize.ToString() ?? string.Empty} / {progress.TotalFileSize?.ToString() ?? string.Empty}";
                EtaText = progress.CurrentEta?.ToString() ?? string.Empty;
            }

            if (progress.State == DownloadState.Merging)
            {
                ChangeState(MERGING_STATE_NAME);
            }

            if (progress.State == DownloadState.Cleaning)
            {
                ChangeState(CLEANING_STATE_NAME);
            }
        }

        [RelayCommand]
        private Task Abort()
        {
            Logger.LogInfo("Aborted button clicked.");
            _cts.Cancel();
            return Navigator.NavigateAsync(AppPages.HomePage.Module);
        }

        [RelayCommand]
        private void OpenFolder()
        {
            string folderToBeOpened = _navigationData.OutputDirectory.FullName;
            Logger.LogInfo($"Open folder clicked: {folderToBeOpened}");

            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = folderToBeOpened,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.LogFatal(ex, "Exception thrown while trying to open folder after downloading finished." +
                    $"Folder to be opened: {folderToBeOpened}");
                throw;
            }
        }

        [RelayCommand]
        private Task Finish()
        {
            Logger.LogInfo("Finish clicked");
            return Navigator.NavigateAsync(AppPages.HomePage.Module);
        }

        [RelayCommand]
        private void DebugChangeState(string parameter)
        {
            State = parameter;
        }

        private void ChangeState(string stateName)
        {
            if (State != stateName)
            {
                State = stateName;
                Logger.LogInfo($"Downloading state changed to: {stateName}.");
            }
        }
    }
}
