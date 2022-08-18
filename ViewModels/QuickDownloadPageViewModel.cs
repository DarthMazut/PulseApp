using CommunityToolkit.Mvvm.ComponentModel;
using MochaCore.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class QuickDownloadPageViewModel : ObservableObject, INavigatable, IOnNavigatedToAsync
    {
        public QuickDownloadPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public Navigator Navigator { get; }

        public Task OnNavigatedToAsync(NavigationData navigationData)
        {
            if (navigationData.Data is QuickDownloadNavigationData data)
            {

            }

            return Task.CompletedTask;
        }
    }
}
