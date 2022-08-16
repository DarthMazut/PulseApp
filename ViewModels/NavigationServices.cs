using MochaCore.Navigation;

namespace ViewModels
{
    public static class NavigationServices
    {
        public static INavigationService MainNavigationService { get; } = new NavigationService();
    }
}
