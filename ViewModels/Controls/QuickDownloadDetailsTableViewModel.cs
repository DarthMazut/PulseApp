using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.DialogsEx.Extensions;
using MochaCore.DialogsEx;
using MochaCore.Settings;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Dialogs;
using ViewModels.Logging;
using Model.Utils;

namespace ViewModels.Controls
{
    public partial class QuickDownloadDetailsTableViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? _title;

        [ObservableProperty]
        private byte[]? _rawImage;

        [ObservableProperty]
        private MediumSelection _mediumSelection;

        [ObservableProperty]
        private MemorySpace? _fileSize;

        [ObservableProperty]
        private bool _hasSufficientSpace;

        [ObservableProperty]
        private bool _isEstimatedSize;

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

        [ObservableProperty]
        private bool _isTargetFileNameExist;

        public async Task FillValues(QuickDownloadNavigationData navigationData)
        {
            Logger.LogInfo("Trying to load settings...");
            ISettingsSectionProvider<ApplicationSettings> settingsProvider = SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID);
            ApplicationSettings settings = await settingsProvider.LoadAsync();

            Title = navigationData.Metadata.Title;
            RawImage = await navigationData.DownloadRawImageAsJpeg();
            MediumSelection = navigationData.MediumSelection;
            Duration = navigationData.Metadata.Durotion;
            Url = navigationData.Metadata.VideoUrl;
            TargetFileName = navigationData.Metadata.Title;
            OutputDirectory = new DirectoryInfo(settings.DefaultOutputFolder);
            FileSize = FormatSelectionSizeCalculator.CalculateOrEstimateSize(navigationData.Metadata, navigationData.FormatSelection, out bool isEstimated);
            IsEstimatedSize = isEstimated;
            
            if (navigationData.MediumSelection == MediumSelection.Music)
            {
                SourceFormat = navigationData.FormatSelection?.AudioSelection?.Extension;
                TargetFormat = "mp3";
                Codec = navigationData.FormatSelection?.AudioSelection?.AudioDetails?.Codec;
                Rate = navigationData.FormatSelection?.AudioSelection?.AudioDetails?.SamplingRate;
            }

            if (navigationData.MediumSelection == MediumSelection.Video)
            {
                SourceFormat = navigationData.FormatSelection?.VideoSelection?.Extension;
                TargetFormat = navigationData.FormatSelection?.VideoSelection?.Extension;
                Codec = navigationData.FormatSelection?.VideoSelection?.VideoDetails?.Codec;
                Resolution = navigationData.FormatSelection?.VideoSelection?.VideoDetails?.Resolution;
            }

            UpdateIsTargetFileNameExists();
            UpdateHasSufficientSpace();
        }

        [RelayCommand]
        private async Task EditOutputFolder()
        {
            IDialogModule<BrowseFolderDialogProperties> browseFolderDialogModule = DialogManager.GetDialog<BrowseFolderDialogProperties>("OpenFolderDialog");
            browseFolderDialogModule.Properties.InitialDirectory = OutputDirectory?.FullName;
            if (await browseFolderDialogModule.ShowModalAsync(this) == true)
            {
                Logger.LogInfo($"Trying to change output folder to: {browseFolderDialogModule.Properties.SelectedPath}");
                if (browseFolderDialogModule.Properties.SelectedPath is string selectedFolder)
                {
                    OutputDirectory = new DirectoryInfo(selectedFolder);
                }
            }

            UpdateIsTargetFileNameExists();
            UpdateHasSufficientSpace();
        }

        [RelayCommand]
        private async Task EditTargetFileName()
        {
            ICustomDialogModule<EnterNameDialogProperties> enterNameDialogModule = DialogManager.GetCustomDialog<EnterNameDialogProperties>("EnterNameDialog");
            enterNameDialogModule.Properties.ProvidedName = TargetFileName;
            enterNameDialogModule.Properties.Extension = TargetFormat ?? "xyz";
            await enterNameDialogModule.ShowModalAsync(this);
            Logger.LogInfo($"Trying to change target file name to: {enterNameDialogModule.Properties.ProvidedName}");
            if (!string.IsNullOrWhiteSpace(enterNameDialogModule.Properties.ProvidedName))
            {
                TargetFileName = enterNameDialogModule.Properties.ProvidedName;
            }

            UpdateIsTargetFileNameExists();
        }

        private void UpdateIsTargetFileNameExists()
        {
            if (OutputDirectory is not null && TargetFileName is not null)
            {
                IsTargetFileNameExist = File.Exists(Path.Combine(OutputDirectory.FullName, $"{TargetFileName}.{TargetFormat}"));
            }
            else
            {
                IsTargetFileNameExist = false;
            }
        }

        private void UpdateHasSufficientSpace()
        {
            if (OutputDirectory is null || FileSize is null)
            {
                HasSufficientSpace = true;
            }
            else
            {
                HasSufficientSpace = MemorySpaceCalculator.CheckSufficientSpace(OutputDirectory.FullName, FileSize.Value) == true;
            } 
        }
    }
}
