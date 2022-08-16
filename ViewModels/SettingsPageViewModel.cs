using CommunityToolkit.Mvvm.ComponentModel;
using MochaCore.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public partial class SettingsPageViewModel : ObservableObject, INavigatable, IAppBackButtonAware, IOnNavigatedTo
    {
        public SettingsPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => true;

        [ObservableProperty]
        private int _currentMenuTabIndex;

        public Task GoBackRequested()
        {
            return Navigator.NavigateAsync(AppPages.HomePage.Module);
        }

        public void OnNavigatedTo(NavigationData navigationData)
        {
            if (navigationData.Data is AppNavigationData appNavigationData)
            {
                if (appNavigationData.RedirectToDependenciesSettingsTab)
                {
                    CurrentMenuTabIndex = 1;
                }
            }
        }
    }
}
