using Model.Dto;
using Model.ProcessCommunication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Handles output from <i>yt-dlp</i> download process and
    /// parses it to the <see cref="DownloadProgress"/> object.
    /// This class encloses state of download process.
    /// </summary>
    public class DownloadProgressCoordinator
    {
        private DownloadState _state = DownloadState.Initializing;
        private int _partsCount;
        private int _currentPart;

        /// <summary>
        /// Produces <see cref="DownloadProgress"/> object based on provided
        /// <see cref="ProcessCommandPartialOutput"/> value and current state of this object.
        /// </summary>
        /// <param name="partialOutput">Output from <i>yt-dlp</i> process.</param>
        public DownloadProgress ParseOutput(ProcessCommandPartialOutput partialOutput)
        {
            string rawMessage = partialOutput.Message;
            bool isError = partialOutput.IsError;
            DateTimeOffset timestamp = partialOutput.Timestamp;

            if (rawMessage.StartsWith("[info]") && rawMessage.Contains("format(s):"))
            {
                string formatString = rawMessage.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Last();
                _partsCount = formatString.Contains('+') ? 2 : 1;
            }
            
            if (rawMessage.StartsWith("[download] Destination:"))
            {
                _state = DownloadState.Downloading;
                _currentPart++;
            }

            if (rawMessage.StartsWith("[Merger]"))
            {
                _state = DownloadState.Merging;
            }

            if (rawMessage.StartsWith("Deleting original file"))
            {
                _state = DownloadState.Cleaning;
            }

            if (rawMessage.StartsWith("[JSONDownload]"))
            {
                _state = DownloadState.Downloading;
                string rawJson = rawMessage.Substring("[JSONDownload]".Length);
                rawJson = rawJson.Replace(": NA", ": null");
                DownloadProgressDto? progressDto;
                
                try
                {
                    progressDto = JsonConvert.DeserializeObject<DownloadProgressDto>(rawJson);
                    if (progressDto is null)
                    {
                        throw new JsonSerializationException("Object was deserialized into *null*.");
                    }
                }
                catch (JsonException ex)
                {
                    return new DownloadProgress(_state, ex.ToString(), timestamp, true);
                }

                return new DownloadProgress(progressDto, _partsCount, _currentPart, rawMessage, timestamp);
            }

            return new DownloadProgress(_state, rawMessage, timestamp, isError);
        }
    }
}
