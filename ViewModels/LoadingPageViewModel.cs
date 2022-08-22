using CommunityToolkit.Mvvm.ComponentModel;
using MochaCore.Behaviours;
using MochaCore.Navigation;
using MochaCore.Settings;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Logging;

namespace ViewModels
{
    public class LoadingPageViewModel : ObservableObject, IAppBackButtonAware, INavigatable, IOnNavigatedToAsync
    {
        public LoadingPageViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
        }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        public Navigator Navigator { get; }

        public bool CanGoBack => false;

        public Task GoBackRequested() => Task.CompletedTask;

        public async Task OnNavigatedToAsync(NavigationData navigationData)
        {
            if (!Logger.IsInitialized)
            {
                Logger.Setup(new LoggerConfiguration(BehaviourManager.Recall<object, string>("GetAppLocalFolder").Execute(new object()))
                {
                    MaximumLogFileSize = 50 * 1024 * 1024 // 50 MB
                });

                if (Logger.IsOutputFileSizeExceeded())
                {
                    await Logger.ArchiveAndClearCurrentFile();
                }

                Logger.StartSession();
            }

            ISettingsSectionProvider<ApplicationSettings> settingsProvider = SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID);
            Logger.LogInfo("About to load settings...");
            _ = await settingsProvider.LoadAsync(); // Load to cache

            await Task.Delay(2000);
            await Navigator.NavigateAsync(AppPages.HomePage.Module, new AppNavigationData() { SupressNavigationAnimation = true });
        }
    }
}
