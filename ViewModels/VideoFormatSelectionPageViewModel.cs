using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.Navigation;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Logging;

namespace ViewModels
{
    public partial class VideoFormatSelectionPageViewModel : ObservableObject, IAppBackButtonAware, INavigatable, IOnNavigatedTo
    {
        private string? _selectedExtension;
        private ResolutionInfo? _selectedResolution;

        private QuickDownloadNavigationData? _quickDownloadData;

        public VideoFormatSelectionPageViewModel()
        {
            Navigator = new(this, NavigationServices.MainNavigationService);
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => true;

        [ObservableProperty]
        private IReadOnlyList<string>? _availableExtensions;

        [ObservableProperty]
        private IReadOnlyList<ResolutionInfo>? _availableResolutions;

        public string? SelectedExtension
        {
            get => _selectedExtension;
            set 
            { 
                if (SetProperty(ref _selectedExtension, value))
                {
                    OnSelectedExtensionChanged();
                }
            }
        }

        public ResolutionInfo? SelectedResolution
        {
            get => _selectedResolution;
            set
            {
                if (SetProperty(ref _selectedResolution, value))
                {
                    OnSelectedResolutionChanged();
                }
            }
        }

        [ObservableProperty]
        private bool _isAvailableResolutionsComboBoxEnabled;

        [ObservableProperty]
        private bool _isDoneButtonEnabled;

        public Task GoBackRequested()
        {
            return Navigator.NavigateAsync(AppPages.SelectMediumPage.Module, _quickDownloadData);
        }

        public void OnNavigatedTo(NavigationData navigationData)
        {
            if (navigationData.Data is QuickDownloadNavigationData data)
            {
                _quickDownloadData = data;
                AvailableExtensions = data.Metadata.FormatTable?.GetAvailableVideoExtensions();

                if (data.FormatSelection?.VideoSelection is not null)
                {
                    SelectedExtension = data.FormatSelection.VideoSelection.Extension;
                    SelectedResolution = data.FormatSelection.VideoSelection.VideoDetails?.Resolution;
                }
            }
        }

        [RelayCommand]
        private Task Done()
        {
            if (_quickDownloadData is not null)
            {
                _quickDownloadData.FormatSelection = ResolveVideoFormatSelection();
                Logger.LogInfo($"Done clicked; selected format: {_quickDownloadData.FormatSelection}");
                return Navigator.NavigateAsync(AppPages.QuickDownloadSummaryPage.Module, _quickDownloadData);
            }

            Logger.LogError($"_quickDownloadData was null in {GetType().Name}");
            return Task.CompletedTask;

        }

        private FormatSelection ResolveVideoFormatSelection()
        {
            FormatTable filteredTable = _quickDownloadData!.Metadata.FormatTable
                .GetOnlyExtensions(SelectedExtension!)
                .GetOnlyResolution(SelectedResolution!);

            FormatInfo? bestMergedFormat = filteredTable
                .GetOnlyTypes(FormatType.Merged)
                .GetHighestVideoQuality();
            
            FormatInfo? bestAudioFormat;
            FormatInfo? bestVideoFormat;

            if (bestMergedFormat is null)
            {
                bestVideoFormat = filteredTable.GetHighestVideoQuality();
                bestAudioFormat = _quickDownloadData!.Metadata.FormatTable.ResolveBestAudioFormatForExtension(SelectedExtension!);

                if (bestAudioFormat is null || bestVideoFormat is null)
                {
                    Logger.LogFatal($"Couldn't resolve best quality for given extension and resolution for: {_quickDownloadData!.Metadata.VideoUrl}");
                    throw new InvalidOperationException("Couldn't resolve best quality for given extension and resolution.");
                }

                return FormatSelection.FromAudioVideoFormat(bestAudioFormat, bestVideoFormat);
            }

            return FormatSelection.FromVideoFormat(bestMergedFormat); 
        }

        private void OnSelectedExtensionChanged()
        {
            Logger.LogInfo($"Extension selected: {SelectedExtension}");
            if (SelectedExtension is not null)
            {
                IsAvailableResolutionsComboBoxEnabled = true;
                AvailableResolutions = _quickDownloadData?.Metadata.FormatTable?.GetOnlyExtensions(SelectedExtension).GetAvailableVideoResolutions();
            }

            IsDoneButtonEnabled = SelectedResolution is not null;
        }

        private void OnSelectedResolutionChanged()
        {
            Logger.LogInfo($"Resolution selected: {SelectedResolution}");
            if (SelectedResolution is not null)
            {
                IsDoneButtonEnabled = true;
            }
        }
    }
}
