using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AppNavigationData
    {
        public bool SupressNavigationAnimation { get; init; }

        public bool RedirectToDependenciesSettingsTab { get; init; }
    }
}
