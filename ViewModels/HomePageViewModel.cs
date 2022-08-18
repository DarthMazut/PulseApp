using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.Dispatching;
using MochaCore.Navigation;
using MochaCore.Settings;
using MochaCore.Utils;
using Model;
using Model.Exceptions;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViewModels.Logging;

namespace ViewModels
{
    public partial class HomePageViewModel : ObservableObject, INavigatable, IOnNavigatingFrom, IOnNavigatedToAsync
    {
        private YtdlpAdapter? _ytdlpAdapter;
        private readonly AsyncProperty<string> _searchText;

        public HomePageViewModel()
        {
            _searchText = new(this, nameof(SearchText))
            {
                PropertyChangedOperation = SearchTextChangedAsyncCallback,
                PropertyChangedOperationTimeout = TimeSpan.FromSeconds(20)
            };

            Navigator = new(this, NavigationServices.MainNavigationService);
        }

        [ObservableProperty]
        private ValidationTextBoxState searchState = ValidationTextBoxState.StandBy;

        [ObservableProperty]
        private string errorHintMessage = string.Empty;

        [ObservableProperty]
        private bool isAbortButtonVisible;

        [ObservableProperty]
        private bool isAbortButtonEnabled;

        [ObservableProperty]
        private bool isErrorPromptVisible;

        [ObservableProperty]
        private bool areDependenciesAvailable;

        [ObservableProperty]
        private bool isSetupRequiredVisible;

        public Navigator Navigator { get; }

        public AnimationController AnimationController { get; } = new();

        public string SearchText 
        {
            get => _searchText.Get();
            set
            {
                _searchText.Set(value);
            }
        }

        private async Task SearchTextChangedAsyncCallback(CancellationToken token, AsyncPropertyChangedEventArgs<string> e)
        {
            if (string.IsNullOrWhiteSpace(e.NewValue))
            {
                SearchState = ValidationTextBoxState.StandBy;
                return;
            }

            if (!IsLinkValid(e.NewValue))
            {
                ErrorHintMessage = "Provided address is not valid.";
                SearchState = ValidationTextBoxState.Error;
                return;
            }

            IsAbortButtonVisible = false;
            IsErrorPromptVisible = false;
            SearchState = ValidationTextBoxState.Processing;

            CancellationTokenSource showWarningCts = ScheduleShowWarning();

            VideoMetadata metadata;
            try
            {
                metadata = await _ytdlpAdapter.DownloadVideoMetadataAsync(e.NewValue, token);
                await Navigator.NavigateAsync(AppPages.SelectMediumPage.Module, new QuickDownloadNavigationData(metadata));
            }
            catch (YtdlpException ex)
            {
                ErrorHintMessage = ex.Message;
                SearchState = ValidationTextBoxState.Error;
                return;
            }
            catch (OperationCanceledException)
            {
                SearchState = ValidationTextBoxState.Error;

                if (e.IsTimedOut)
                {
                    ErrorHintMessage = "Operation timed out.";
                    IsAbortButtonEnabled = false;
                    IsAbortButtonVisible = false;
                    IsErrorPromptVisible = true;
                }
                else
                {
                    ErrorHintMessage = "Operation cancelled.";
                }

                return;
            }
            catch(Exception ex)
            {
                Logger.LogFatal(ex);
                throw;
            }
            finally
            {
                showWarningCts.Cancel();
                showWarningCts.Dispose();
            }

            SearchState = ValidationTextBoxState.StandBy;
        }

        private CancellationTokenSource ScheduleShowWarning()
        {
            CancellationTokenSource cts = new();

            _ = Task.Delay(TimeSpan.FromSeconds(10), cts.Token).ContinueWith(t =>
            {
                DispatcherManager.GetMainThreadDispatcher().EnqueueOnMainThread(() =>
                {
                    IsAbortButtonEnabled = true;
                    IsAbortButtonVisible = true;
                });
            }, cts.Token);

            return cts;
        }

        [RelayCommand]
        private void Abort()
        {
            IsAbortButtonEnabled = false;
            _searchText.CancelAsyncOperation();
        }

        [RelayCommand]
        private Task ConfigureDependencies()
        {
            return Navigator.NavigateAsync(AppPages.SettingsPage.Module, new AppNavigationData()
            {
                RedirectToDependenciesSettingsTab = true
            });
        }

        [RelayCommand]
        private Task AdvanceDownload()
        {
            return Navigator.NavigateAsync(AppPages.AdvancedDownloadPage.Module);
        }

        [RelayCommand]
        private Task Archive()
        {
            return Navigator.NavigateAsync(AppPages.ArchivePage.Module);
        }

        [RelayCommand]
        private Task Settings()
        {
            return Navigator.NavigateAsync(AppPages.SettingsPage.Module);
        }

        [RelayCommand]
        private async Task About()
        {
            await Navigator.NavigateAsync(AppPages.AboutPage.Module);
        }

        private static bool IsLinkValid(string? value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public async Task OnNavigatedToAsync(NavigationData navigationData)
        {
            bool playAnimation = navigationData.PreviousModule.Equals(AppPages.LoadingPage.Module);

            if (playAnimation)
            {
                AnimationController.PlayAnimation();
            }

            ISettingsSectionProvider<ApplicationSettings> settingsProvider = SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID);
            ApplicationSettings settings = await settingsProvider.LoadAsync();

            AreDependenciesAvailable = settings.YtdlpPath != null && settings.FfmpegPath != null;
            if (AreDependenciesAvailable)
            {
                _ytdlpAdapter = new YtdlpAdapter(settings.YtdlpPath!, settings.FfmpegPath!);
            }

            await Task.Delay(playAnimation ? 1500 : 300); // This is for UX purpose!
            IsSetupRequiredVisible = !AreDependenciesAvailable;
        }

        public void OnNavigatingFrom(NavigationData navigationData, NavigationCancelEventArgs e)
        {
            _searchText.Dispose();
            AnimationController.Dispose();
        }
    }

    public enum ValidationTextBoxState
    {
        StandBy,
        Processing,
        Error
    }
}
