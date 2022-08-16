using MochaCore.Behaviours;
using MochaCore.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    public class ApplicationSettings : ISettingsSection
    {
        public static readonly string ID = "ApplicationSettings";

        // GENERAL

        public string DefaultOutputFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // EXTERNAL

        public string? YtdlpPath { get; set; }

        public string? FfmpegPath { get; set; }

        // LOGGING

        //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PulseAppLogs");
        public string LogsFolder { get; set; } = BehaviourManager.Recall<object, string>("GetAppLocalFolder").Execute(new object());

        public bool AllowLogsCollecting => true;

        // NETWORK

        public bool HasDownloadSpeedLimit { get; set; } = false;

        public int DownloadSpeedLimit { get; set; } = 50;

        public async Task FillValuesAsync(string serializedData)
        {
            ApplicationSettings? deserializedSettings = await Task.Run(() => JsonConvert.DeserializeObject<ApplicationSettings>(serializedData));
            if (deserializedSettings is not null)
            {
                DefaultOutputFolder = deserializedSettings.DefaultOutputFolder;
                YtdlpPath = deserializedSettings.YtdlpPath;
                FfmpegPath = deserializedSettings.FfmpegPath;
                LogsFolder = deserializedSettings.LogsFolder;
                HasDownloadSpeedLimit = deserializedSettings.HasDownloadSpeedLimit;
                DownloadSpeedLimit = deserializedSettings.DownloadSpeedLimit;
            }
        }

        public Task<string> SerializeAsync()
        {
            return Task.Run(() => JsonConvert.SerializeObject(this));
        }
    }
}
