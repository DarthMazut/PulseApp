using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MochaCore.DialogsEx;
using MochaCore.DialogsEx.Extensions;
using MochaCore.Settings;
using MochaCore.Utils;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Logging;

namespace ViewModels
{
    public partial class GeneralSettingsTabViewModel : ObservableObject
    {
        private readonly ISettingsSectionProvider<ApplicationSettings> _settingsProvider;
        private string? _defaultOutputPath;

        public GeneralSettingsTabViewModel()
        {
            Logger.LogInfo($"{GetType().Name}..ctor");
            _settingsProvider = SettingsManager.Retrieve<ApplicationSettings>(ApplicationSettings.ID);
        }

        public string? DefaultOutputPath 
        {
            get => _defaultOutputPath;
            set
            {
                if(SetProperty(ref _defaultOutputPath, value))
                {
                    OnDefaultOutputPathChangedCallback();
                }
            }
        }

        [ObservableProperty]
        private bool _isErrorVisible;

        [RelayCommand]
        private async Task BrowseFolder()
        {
            Logger.LogInfo("Browse folder clicked");
            IDialogModule<BrowseFolderDialogProperties> browseFolderDialogModule = DialogManager.GetDialog<BrowseFolderDialogProperties>("OpenFolderDialog");
            if (await browseFolderDialogModule.ShowModalAsync(this) == true)
            {
                DefaultOutputPath = browseFolderDialogModule.Properties.SelectedPath;
            }
        }

        [RelayCommand]
        private async Task Loaded()
        {
            DefaultOutputPath = (await _settingsProvider.LoadAsync()).DefaultOutputFolder;
            Logger.LogInfo($"DefaultOutputPath loaded ({DefaultOutputPath}).");
        }

        private async void OnDefaultOutputPathChangedCallback()
        {
            Logger.LogInfo($"Changing DefaultOutputPath to {DefaultOutputPath}");
            if (Directory.Exists(DefaultOutputPath))
            {
                IsErrorVisible = false;
                Logger.LogInfo("Trying to update settings with new DefaultOutputPath...");
                await _settingsProvider.UpdateAsync((settings) =>
                {
                    settings.DefaultOutputFolder = DefaultOutputPath;
                });
            }
            else
            {
                Logger.LogWarning($"Directory {DefaultOutputPath} does not exist.");
                IsErrorVisible = true;
            }
        }
    }
}
