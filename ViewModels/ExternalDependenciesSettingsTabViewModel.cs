using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.DialogsEx;
using MochaCore.DialogsEx.Extensions;
using MochaCore.Settings;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Logging;

namespace ViewModels
{
    public partial class ExternalDependenciesSettingsTabViewModel : ObservableObject
    {
        private readonly ISettingsSectionProvider<ApplicationSettings> _settingsProvider;

        private string? _ytdlpPath;
        private string? _ffmpegPath;

        public ExternalDependenciesSettingsTabViewModel()
        {
            Logger.LogInfo($"{GetType().Name}..ctor");
            _settingsProvider = SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID);
        }

        [ObservableProperty]
        private string? _ytdlpErrorText;

        [ObservableProperty]
        private string? _ffmpegErrorText;

        public string? YtdlpPath 
        {
            get => _ytdlpPath;
            set
            {
                if(SetProperty(ref _ytdlpPath, value))
                {
                    Logger.LogInfo($"Changing Yt-dlp path to {value}");
                    OnPathChanged("ytdlp");
                }
            }
        }

        public string? FfmpegPath
        {
            get => _ffmpegPath;
            set
            {
                if (SetProperty(ref _ffmpegPath, value))
                {
                    Logger.LogInfo($"Changing FFmpeg path to {value}");
                    OnPathChanged("ffmpeg");
                }
            }
        }

        [RelayCommand]
        private async Task Loaded()
        {
            ApplicationSettings settings = await _settingsProvider.LoadAsync();

            YtdlpPath = settings.YtdlpPath;
            OnPathChanged("ytdlp");

            FfmpegPath = settings.FfmpegPath;
            OnPathChanged("ffmpeg");

            Logger.LogInfo($"Yt-dlp path loaded ({YtdlpPath})");
            Logger.LogInfo($"FFmpeg path loaded ({FfmpegPath})");
        }

        [RelayCommand]
        private async Task OpenFile(string dependency)
        {
            Logger.LogInfo($"Open file clicked for {dependency}");
            IDialogModule<OpenFileDialogProperties> openFileDialogModule = DialogManager.GetDialog<OpenFileDialogProperties>("OpenFileDialog");
            openFileDialogModule.Properties.Filters.Add(new ExtensionFilter("Executable", "exe"));
            if (await openFileDialogModule.ShowModalAsync(this) == true)
            {
                string selectedPath = openFileDialogModule.Properties.SelectedPaths.First();
                Logger.LogInfo($"Selected path is: {selectedPath}");
                if (dependency == "Ytdlp")
                {
                    YtdlpPath = selectedPath;
                }

                if (dependency == "Ffmpeg")
                {
                    FfmpegPath = selectedPath;
                }
            }
        }

        private async void OnPathChanged(string dependencyName)
        {
            Logger.LogInfo($"Path changed for {dependencyName}");
            Action<string?> assignDelegate = dependencyName == "ytdlp" ? (s) => YtdlpErrorText = s: (s) => FfmpegErrorText = s;
            string? value = dependencyName == "ytdlp" ? YtdlpPath : FfmpegPath;

            if (string.IsNullOrWhiteSpace(value))
            {
                assignDelegate("No path provided!");
                return;
            }

            if (File.Exists(value) && Path.GetExtension(value) == ".exe")
            {
                assignDelegate(null);
                Logger.LogInfo($"About to update settings with value: {value}");
                await _settingsProvider.UpdateAsync((settings) =>
                {
                    settings.YtdlpPath = YtdlpPath;
                    settings.FfmpegPath = FfmpegPath;
                });
            }
            else
            {
                Logger.LogInfo("Path is not valid");
                assignDelegate("Provided path is not valid!");
            }

        }
    }
}
