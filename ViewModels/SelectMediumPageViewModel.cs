using CommunityToolkit.Mvvm.Input;
using MochaCore.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public partial class SelectMediumPageViewModel : IAppBackButtonAware, INavigatable, IOnNavigatedTo
    {
        private QuickDownloadNavigationData _navigationData;

        public SelectMediumPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => true;

        [RelayCommand]
        private Task VideoSelected()
        {
            _navigationData.MediumSelection = MediumSelection.Video;
            return Navigator.NavigateAsync(AppPages.VideoFormatSelectionPage.Module, _navigationData);
        }

        [RelayCommand]
        private Task AudioSelected()
        {
            _navigationData.MediumSelection = MediumSelection.Music;
            return Navigator.NavigateAsync(AppPages.QuickDownloadSummaryPage.Module, _navigationData);
        }

        public Task GoBackRequested()
        {
            return Navigator.NavigateAsync(AppPages.HomePage.Module);
        }

        public void OnNavigatedTo(NavigationData navigationData)
        {
            if (navigationData.Data is QuickDownloadNavigationData data)
            {
                _navigationData = data;
            }
            else
            {
                throw new ArgumentException($"Attempt was made to navigate to {GetType().Name}, " +
                    $"but no {typeof(QuickDownloadNavigationData)} navigation data was provided.");
            }
        }
    }
}
