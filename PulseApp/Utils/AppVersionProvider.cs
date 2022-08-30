using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace PulseApp.Utils
{
    public class AppVersionProvider : MarkupExtension
    {
        protected override object ProvideValue()
        {
            PackageVersion version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
