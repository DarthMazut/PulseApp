using MochaCore.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public static class AppPages
    {
        public static PageInfo LoadingPage { get; } = new("LoadingPage");

        public static PageInfo HomePage { get; } = new("HomePage");

        public static PageInfo AboutPage { get; } = new("AboutPage");

        public static PageInfo SelectMediumPage { get; } = new("SelectMediumPage");

        public static PageInfo QuickDownloadSummaryPage { get; } = new("QuickDownloadSummaryPage");

        public static PageInfo SettingsPage { get; } = new("SettingsPage");

        public static PageInfo VideoFormatSelectionPage { get; } = new("VideoFormatSelectionPage");
    }

    public class PageInfo
    {
        public PageInfo(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public INavigationModule Module => NavigationManager.FetchModule(Id);
    }
}
