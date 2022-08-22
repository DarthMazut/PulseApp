using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.DialogsEx;
using MochaCore.DialogsEx.Extensions;
using MochaCore.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Logging;

namespace ViewModels
{
    public partial class LoggingSettingsTabViewModel : ObservableObject
    {
        private static Task? _exportingTask;

        [ObservableProperty]
        private bool _isExportingLogs;

        [RelayCommand]
        private async Task Loaded()
        {
            Logger.LogInfo($"{GetType().Name} loaded");
            if (_exportingTask != null)
            {
                Logger.LogInfo("Exporting logs in progress...");
                IsExportingLogs = true;
                await _exportingTask;
                IsExportingLogs = false;

                _exportingTask = null;
            }
        }

        [RelayCommand]
        private async Task ExportLogs()
        {
            Logger.LogInfo("ExportLogs clicked,");
            IDialogModule<SaveFileDialogProperties> saveFileDialog = DialogManager.GetDialog<SaveFileDialogProperties>("SaveFileDialog");
            saveFileDialog.Properties.Filters = new List<ExtensionFilter>()
            {
                new ExtensionFilter("Text file", "txt"),
                new ExtensionFilter("Any file", "")
            };

            if (await saveFileDialog.ShowModalAsync(this) == true)
            {
                IsExportingLogs = true;
                Logger.LogInfo($"About to export logs to {saveFileDialog.Properties.SelectedPath!}");
                _exportingTask = Logger.ExportAll(saveFileDialog.Properties.SelectedPath!);
                await _exportingTask;
                IsExportingLogs = false;
                _exportingTask = null;
            } 
        }
    }
}
