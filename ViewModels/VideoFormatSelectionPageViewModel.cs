using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.Navigation;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [ObservableProperty]
        private IReadOnlyList<ResolutionInfo>? _availableResolutions;

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

                if (data.SelectedVideoFormat is not null)
                {
                    SelectedExtension = data.SelectedVideoFormat.Extension;
                    SelectedResolution = data.SelectedVideoFormat.VideoDetails?.Resolution;
                }
            }
        }

        [RelayCommand]
        private Task Done()
        {
            _quickDownloadData.SelectedVideoFormat = _quickDownloadData.Metadata.FormatTable?
                .GetOnlyExtensions(SelectedExtension!)
                .GetOnlyResolution(SelectedResolution!)
                .GetHighestVideoQuality();

            return Navigator.NavigateAsync(AppPages.QuickDownloadSummaryPage.Module, _quickDownloadData);
        }

        private void OnSelectedExtensionChanged()
        {
            if (SelectedExtension is not null)
            {
                IsAvailableResolutionsComboBoxEnabled = true;
                AvailableResolutions = _quickDownloadData?.Metadata.FormatTable?.GetOnlyExtensions(SelectedExtension).GetAvailableVideoResolutions();
            }
        }

        private void OnSelectedResolutionChanged()
        {
            if (SelectedResolution is not null)
            {
                IsDoneButtonEnabled = true;
            }
        }
    }
}
