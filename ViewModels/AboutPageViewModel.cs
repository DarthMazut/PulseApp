using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public partial class AboutPageViewModel : ObservableObject, INavigatable, IAppBackButtonAware
    {
        private bool _canGoBack = true;

        public AboutPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => _canGoBack;

        public Task GoBackRequested()
        {
            return Navigator.NavigateAsync(AppPages.HomePage.Module);
        }
    }
}
