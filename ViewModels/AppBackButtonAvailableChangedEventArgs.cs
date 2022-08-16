namespace ViewModels
{
    public class AppBackButtonAvailableChangedEventArgs : EventArgs
    {
        private bool _canGoBack;

        public AppBackButtonAvailableChangedEventArgs(bool canGoBack)
        {
            _canGoBack = canGoBack;
        }

        public bool CanGoBack => _canGoBack;
    }
}