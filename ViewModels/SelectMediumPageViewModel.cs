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
            Logger.LogInfo("Video selected");
            _navigationData.MediumSelection = MediumSelection.Video;
            return Navigator.NavigateAsync(AppPages.VideoFormatSelectionPage.Module, _navigationData);
        }

        [RelayCommand]
        private Task AudioSelected()
        {
            Logger.LogInfo("Music selected");
            _navigationData.MediumSelection = MediumSelection.Music;
            
            FormatSelection? formatSelection = ResolveAudioFormatSelection();
            if (formatSelection is not null)
            {
                _navigationData.FormatSelection = formatSelection;
                return Navigator.NavigateAsync(AppPages.QuickDownloadSummaryPage.Module, _navigationData);
            }

            return Task.CompletedTask;
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
        }

        private FormatSelection? ResolveAudioFormatSelection()
        {
            FormatInfo? highestAudioFormat = _navigationData.Metadata.FormatTable?.GetHighestAudioQuality();
            if (highestAudioFormat is not null)
            {
                return FormatSelection.FromAudioFormat(highestAudioFormat);
            }

            Logger.LogFatal($"Couldn't resolve highest audio quality for: {_navigationData.Metadata.VideoUrl}");
            return null;
        }
    }
}
