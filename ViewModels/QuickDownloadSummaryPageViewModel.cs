using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.DialogsEx;
using MochaCore.DialogsEx.Extensions;
using MochaCore.Navigation;
using MochaCore.Settings;
using Model;
using Model.Settings;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Dialogs;

namespace ViewModels
{
    public partial class QuickDownloadSummaryPageViewModel : ObservableObject, IAppBackButtonAware, INavigatable, IOnNavigatedToAsync
    {
        private NavigationData? _navigationData;
        private QuickDownloadNavigationData? _summaryData;

        [ObservableProperty]
        private string? _imageUrl;

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

        public QuickDownloadSummaryPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => true;

        public Task GoBackRequested()
        {
            if (_navigationData?.PreviousModule.Equals(AppPages.VideoFormatSelectionPage.Module) == true)
            {
                return Navigator.NavigateAsync(AppPages.VideoFormatSelectionPage.Module, _summaryData!);
            }

            return Navigator.NavigateAsync(AppPages.SelectMediumPage.Module, _summaryData!);
        }

        public async Task OnNavigatedToAsync(NavigationData navigationData)
        {
            _navigationData = navigationData;

            if (navigationData.Data is QuickDownloadNavigationData data)
            {
                _summaryData = data;

                Title = data.Metadata.Title;
                RawImage = await data.DownloadRawImageAsJpeg();
                MediumSelection = data.MediumSelection;
                Duration = data.Metadata.Durotion;
                Url = data.Metadata.VideoUrl;
                TargetFileName = data.Metadata.Title;
                OutputDirectory = new DirectoryInfo((await SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID).LoadAsync()).DefaultOutputFolder);

                if (data.MediumSelection == MediumSelection.Music)
                {
                    FormatInfo? musicFormat = data.Metadata.FormatTable?.GetHighestAudioQuality();
                    if (musicFormat is not null)
                    {
                        FileSize = musicFormat.FileSize;
                        SourceFormat = musicFormat.Extension;
                        TargetFormat = "mp3";
                        Codec = musicFormat.AudioDetails?.Codec;
                        Rate = musicFormat.AudioDetails?.SamplingRate;
                    }
                }
                
                if (data.MediumSelection == MediumSelection.Video)
                {
                    FormatInfo? videoFormat = data.SelectedVideoFormat;
                    FormatInfo? audioFormat = data.Metadata.FormatTable?.ResolveBestAudioFormatForExtension(data.SelectedVideoFormat.Extension);
                    if (videoFormat is not null && audioFormat is not null)
                    {
                        FileSize = videoFormat.FileSize + audioFormat.FileSize;
                        SourceFormat = videoFormat.Extension;
                        TargetFormat = videoFormat.Extension;
                        Codec = videoFormat.VideoDetails?.Codec;
                        Resolution = videoFormat.VideoDetails?.Resolution;
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Attempt was made to navigate to {GetType().Name}, " +
                    $"but no {typeof(QuickDownloadNavigationData)} navigation data was provided.");
            }
        }

        [RelayCommand]
        private async Task EditOutputFolder()
        {
            IDialogModule<BrowseFolderDialogProperties> browseFolderDialogModule = DialogManager.GetDialog<BrowseFolderDialogProperties>("OpenFolderDialog");
            browseFolderDialogModule.Properties.InitialDirectory = OutputDirectory?.FullName;
            if (await browseFolderDialogModule.ShowModalAsync(this) == true)
            {
                if(browseFolderDialogModule.Properties.SelectedPath is string selectedFolder)
                {
                    OutputDirectory = new DirectoryInfo(selectedFolder);
                }
            }
        }

        [RelayCommand]
        private async Task EditTargetFileName()
        {
            ICustomDialogModule<EnterNameDialogProperties> enterNameDialogModule = DialogManager.GetCustomDialog<EnterNameDialogProperties>("EnterNameDialog");
            enterNameDialogModule.Properties.ProvidedName = TargetFileName;
            enterNameDialogModule.Properties.Extension = TargetFormat ?? "xyz";
            await enterNameDialogModule.ShowModalAsync(this);
            if (!string.IsNullOrWhiteSpace(enterNameDialogModule.Properties.ProvidedName))
            {
                TargetFileName = enterNameDialogModule.Properties.ProvidedName;
            }
        }
    }
}
