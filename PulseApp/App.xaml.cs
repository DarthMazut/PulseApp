using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using MochaCore.Behaviours;
using MochaCore.DialogsEx;
using MochaCore.Dispatching;
using MochaCore.Navigation;
using MochaCore.Settings;
using MochaCoreWinUI.DialogsEx;
using MochaCoreWinUI.Dispatching;
using MochaCoreWinUI.Navigation;
using MochaCoreWinUI.Settings;
using Model.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using ViewModels;
using ViewModels.Dialogs;
using ViewModels.Logging;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using PulseApp.Dialogs;
using PulseApp.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PulseApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private static Window _mainWindow;

        public static Window MainWindow => _mainWindow;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Logger.TrySynchronize();
            };

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Logger.TryEndSession();
            };
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _mainWindow = new MainWindow();
            _mainWindow.Activate();

            NavigationManager.AddModule(AppPages.LoadingPage.Id, () => new NavigationModule(new LoadingPage()));
            NavigationManager.AddModule(AppPages.HomePage.Id, () => new NavigationModule(new HomePage()));
            NavigationManager.AddModule(AppPages.SelectMediumPage.Id, () => new NavigationModule(new SelectMediumPage()));
            NavigationManager.AddModule(AppPages.AboutPage.Id, () => new NavigationModule(new AboutPage()));
            NavigationManager.AddModule(AppPages.QuickDownloadSummaryPage.Id, () => new NavigationModule(new QuickDownloadSummaryPage()));
            NavigationManager.AddModule(AppPages.SettingsPage.Id, () => new NavigationModule(new SettingsPage()));
            NavigationManager.AddModule(AppPages.VideoFormatSelectionPage.Id, () => new NavigationModule(new VideoFormatSelectionPage()));
            NavigationManager.AddModule(AppPages.ArchivePage.Id, () => new NavigationModule(new ArchivePage()));
            NavigationManager.AddModule(AppPages.AdvancedDownloadPage.Id, () => new NavigationModule(new AdvancedDownloadPage()));
            NavigationManager.AddModule(AppPages.QuickDownloadPage.Id, () => new NavigationModule(new QuickDownloadPage()));

            DialogManager.DefineDialog("OpenFolderDialog", () => new BrowseFolderDialogModule(_mainWindow));
            DialogManager.DefineDialog("OpenFileDialog", () => new OpenFileDialogModule(_mainWindow));
            DialogManager.DefineDialog("SaveFileDialog", () => new SaveFileDialogModule(_mainWindow));
            DialogManager.DefineDialog("EnterNameDialog", () => new ContentDialogModule<EnterNameDialogProperties>(_mainWindow, new EnterNameDialog(), new EnterNameDialogViewModel()));

            SettingsManager.Register(ApplicationSettings.ID, new ApplicationSettingsSectionProvider<ApplicationSettings>());

            DispatcherManager.SetMainThreadDispatcher(new WinUIDispatcher(_mainWindow));

            BehaviourManager.Record("GetAppLocalFolder", (object o) => ApplicationData.Current.LocalFolder.Path);
        }
    }
}
