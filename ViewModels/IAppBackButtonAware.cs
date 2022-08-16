namespace ViewModels
{
    public interface IAppBackButtonAware
    {
        public bool CanGoBack { get; }

        public event EventHandler<AppBackButtonAvailableChangedEventArgs>? BackButtonAvailableChanged;

        Task GoBackRequested();
    }
}