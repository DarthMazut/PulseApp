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
using ViewModels.Logging;

namespace ViewModels
{
    public partial class QuickDownloadSummaryPageViewModel : ObservableObject, IAppBackButtonAware, INavigatable, IOnNavigatedToAsync
    {
        private NavigationData? _navigationData;
        private QuickDownloadNavigationData? _summaryData;

        public QuickDownloadSummaryPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => true;

        // WORKAROUND FOR JUMPING TABLE - REMOVE WHEN IMAGE COMPONENT WILL GET REFACTOR
        [ObservableProperty]
        private byte[]? _rawImage;
        // WORKAROUND FOR JUMPING TABLE - REMOVE WHEN IMAGE COMPONENT WILL GET REFACTOR

        [ObservableProperty]
        private QuickDownloadSummary? _downloadSummary;

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

                RawImage = await data.DownloadRawImageAsJpeg();

                ISettingsSectionProvider<ApplicationSettings> settingsProvider = SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID);
                Logger.LogInfo("Trying to load settings...");
                ApplicationSettings settings = await settingsProvider.LoadAsync();

                (DownloadSummary, data.FormatSelection) = await QuickDownloadSummary.FromNavigationData(data, settings);
                Logger.LogInfo($"Selected format is: {data.FormatSelection}");
            }
        }

        [RelayCommand]
        private async Task EditOutputFolder()
        {
            Logger.LogInfo("EditOutputFolder clicked.");
            IDialogModule<BrowseFolderDialogProperties> browseFolderDialogModule = DialogManager.GetDialog<BrowseFolderDialogProperties>("OpenFolderDialog");
            browseFolderDialogModule.Properties.InitialDirectory = DownloadSummary.OutputDirectory?.FullName;
            if (await browseFolderDialogModule.ShowModalAsync(this) == true)
            {
                Logger.LogInfo($"Trying to change output folder to: {browseFolderDialogModule.Properties.SelectedPath}");
                if(browseFolderDialogModule.Properties.SelectedPath is string selectedFolder)
                {
                    DownloadSummary.OutputDirectory = new DirectoryInfo(selectedFolder);
                }
            }
        }

        [RelayCommand]
        private async Task EditTargetFileName()
        {
            Logger.LogInfo("EditTargetFileName clicked.");
            ICustomDialogModule<EnterNameDialogProperties> enterNameDialogModule = DialogManager.GetCustomDialog<EnterNameDialogProperties>("EnterNameDialog");
            enterNameDialogModule.Properties.ProvidedName = DownloadSummary.TargetFileName;
            enterNameDialogModule.Properties.Extension = DownloadSummary.TargetFormat ?? "xyz";
            await enterNameDialogModule.ShowModalAsync(this);
            Logger.LogInfo($"Trying to change target file name to: {enterNameDialogModule.Properties.ProvidedName}");
            if (!string.IsNullOrWhiteSpace(enterNameDialogModule.Properties.ProvidedName))
            {
                DownloadSummary.TargetFileName = enterNameDialogModule.Properties.ProvidedName;
            }
        }

        [RelayCommand]
        private async Task Download()
        {
            Logger.LogInfo("Download clicked.");
            _summaryData!.QuickDownloadSummary = DownloadSummary;
            await Navigator.NavigateAsync(AppPages.QuickDownloadPage.Module, _summaryData!);
        }
    }
}
