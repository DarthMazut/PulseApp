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
using ViewModels.Controls;
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
            DetailsTableViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(QuickDownloadDetailsTableViewModel.IsTargetFileNameExist) ||
                    e.PropertyName == nameof(QuickDownloadDetailsTableViewModel.HasSufficientSpace) ||
                    e.PropertyName == nameof(QuickDownloadDetailsTableViewModel.IsEstimatedSize))
                {
                    if (DetailsTableViewModel.IsEstimatedSize)
                    {
                        IsDownloadAvailable = !DetailsTableViewModel.IsTargetFileNameExist;
                    }
                    else
                    {
                        IsDownloadAvailable = !DetailsTableViewModel.IsTargetFileNameExist && DetailsTableViewModel.HasSufficientSpace;
                    }
                }
            };
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => true;

        // WORKAROUND FOR JUMPING TABLE - REMOVE WHEN IMAGE COMPONENT WILL GET REFACTOR
        [ObservableProperty]
        private byte[]? _rawImage;
        // WORKAROUND FOR JUMPING TABLE - REMOVE WHEN IMAGE COMPONENT WILL GET REFACTOR

        [ObservableProperty]
        private bool isDownloadAvailable;

        public QuickDownloadDetailsTableViewModel DetailsTableViewModel { get; } = new();

        public Task GoBackRequested()
        {
            if (_navigationData?.PreviousModule.Equals(AppPages.VideoFormatSelectionPage.Module) == true)
            {
                return Navigator.NavigateAsync(AppPages.VideoFormatSelectionPage.Module, _summaryData);
            }

            return Navigator.NavigateAsync(AppPages.SelectMediumPage.Module, _summaryData);
        }

        public async Task OnNavigatedToAsync(NavigationData navigationData)
        {
            _navigationData = navigationData;

            if (navigationData.Data is QuickDownloadNavigationData data)
            {
                _summaryData = data;
                RawImage = await data.DownloadRawImageAsJpeg();
                await DetailsTableViewModel.FillValues(data);
            }
        }

        [RelayCommand]
        private async Task Download()
        {
            _summaryData.TargetFileName = DetailsTableViewModel.TargetFileName;
            _summaryData.OutputDirectory = DetailsTableViewModel.OutputDirectory;
            await Navigator.NavigateAsync(AppPages.QuickDownloadPage.Module, _summaryData);
        }
    }
}
