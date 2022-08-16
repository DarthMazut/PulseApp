using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.Dispatching;
using MochaCore.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Logging;

namespace ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, INavigatable
    {
        public MainWindowViewModel()
        {
            Navigator = new Navigator(this, NavigationServices.MainNavigationService);
            NavigationServices.MainNavigationService.NavigationRequested += OnNavigationRequested;
            AnimationController.RegisterAnimationWhenPropertyChanges(this, nameof(Content));
        }

        [ObservableProperty]
        private object? _content;

        [ObservableProperty]
        private bool _isBackArrowEnabled;

        public Navigator Navigator { get; }

        public AnimationController AnimationController { get; } = new();

        private void OnNavigationRequested(object? sender, NavigationData e)
        {
            if (Logger.HasActiveSession)
            {
                Logger.LogInfo($"Navigating to: {e.RequestedModule.View?.GetType().Name}");
            }

            if (e.Data is AppNavigationData appNavigationData && appNavigationData.SupressNavigationAnimation)
            {
                AnimationController.SupressSubsequentAnimation();
            }

            SetupBackButtonLogic(e);
            Content = e.RequestedModule.View;
        }

        [RelayCommand]
        private Task Loaded()
        {
            return Navigator.NavigateAsync(AppPages.LoadingPage.Module, new AppNavigationData() { SupressNavigationAnimation = true });
        }

        [RelayCommand]
        private void BackButton()
        {
            if (NavigationServices.MainNavigationService.CurrentView?.DataContext is IAppBackButtonAware currentPageVM)
            {
                currentPageVM.GoBackRequested();
            }
        }

        private void SetupBackButtonLogic(NavigationData navigationData)
        {
            if (navigationData.PreviousModule?.DataContext is IAppBackButtonAware previousPageVM)
            {
                previousPageVM.BackButtonAvailableChanged -= CanGoBackChanged;
            }

            if (navigationData.RequestedModule?.DataContext is IAppBackButtonAware requestedPageVM)
            {
                requestedPageVM.BackButtonAvailableChanged += CanGoBackChanged;
                IsBackArrowEnabled = requestedPageVM.CanGoBack;
            }
            else
            {
                IsBackArrowEnabled = false;
            }
        }

        private void CanGoBackChanged(object? sender, AppBackButtonAvailableChangedEventArgs e)
        {
            IsBackArrowEnabled = e.CanGoBack;
        }
    }
}
