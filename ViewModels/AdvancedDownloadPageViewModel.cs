using CommunityToolkit.Mvvm.ComponentModel;
using MochaCore.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AdvancedDownloadPageViewModel : ObservableObject, IAppBackButtonAware, INavigatable
    {
        public AdvancedDownloadPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => true;

        public Task GoBackRequested()
        {
            return Navigator.NavigateAsync(AppPages.HomePage.Module);
        }
    }
}
