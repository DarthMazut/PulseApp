using Model.Dto;
using Model.Exceptions;
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
    /// Provides a way to communicate with external yt-dlp process.
    /// </summary>
    public class YtdlpAdapter
    {
        private readonly ProcessMessenger _messenger;
        private readonly string _ffmpegPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="YtdlpAdapter"/> class.
        /// </summary>
        /// <param name="ytdlpPath">Location of *yt-dlp* executable file.</param>
        /// <param name="ffmpegPath">Path to *FFmpeg* executable.</param>
        public YtdlpAdapter(string ytdlpPath, string ffmpegPath)
        {
            _messenger = new ProcessMessenger(ytdlpPath, Path.GetDirectoryName(ytdlpPath));
            _ffmpegPath = ffmpegPath;
        }

        /// <summary>
        /// Asynchronously downloads metadata for specific video.
        /// Throws if metadata couldn't be downloaded for whatever reason.
        /// </summary>
        /// <param name="url">Target video URL.</param>
        /// <exception cref="YtdlpException"/>
        public Task<VideoMetadata> DownloadVideoMetadataAsync(string url) => DownloadVideoMetadataAsync(url, default);

        /// <summary>
        /// Asynchronously downloads metadata for specific video.
        /// Throws if metadata couldn't be downloaded for whatever reason.
        /// </summary>
        /// <param name="url">Target video URL.</param>
        /// <param name="token">Cancellation token used for cancelling this operation.</param>
        /// <exception cref="YtdlpException"/>
        public async Task<VideoMetadata> DownloadVideoMetadataAsync(string url, CancellationToken token)
        {
            string getMetadataCommand = new YtdlpCommandBuilder(url).GetMetadata().AsCommand();

            ProcessCommandOutput output = await _messenger.SendCommandAndWaitForResponseAsync(getMetadataCommand, null, token);
            
            if (output.ErrorPartialResults.Any() && !output.PartialResults.Any())
            {
                throw new YtdlpException(
                    "Could not download video metadata.",
                    null,
                    output.ErrorPartialResults.Select(o => o.Message).ToList());
            }

            string? lastResponse = output.PartialResults.LastOrDefault()?.Message;
            if (lastResponse is not null)
            {
                try
                {
                    MetadataDto? metadataDto = await Task.Run(() => JsonConvert.DeserializeObject<MetadataDto>(lastResponse));
                    if (metadataDto is not null)
                    {
                        return VideoMetadata.FromRecord(metadataDto);
                    }
                    
                }
                catch (JsonException ex)
                {
                    throw new YtdlpException(
                        "Cannot deserialize output from Yt-dlp.",
                        ex,
                        output.ErrorPartialResults.Select(o => o.Message).ToList());
                } 
            }

            throw new YtdlpException("No response received from Yt-dlp.");
        }

        /// <summary>
        /// Asynchronously downloads video specified within provided <see cref="DownloadJob"/> object.
        /// </summary>
        /// <param name="downloadJob">Contains details of video to be downloaded.</param>
        public Task DownloadVideoAsync(DownloadJob downloadJob)
        {
            return DownloadVideoBase(downloadJob,
                YtdlpCommandBuilder
                    .Download(downloadJob.Url)
                    .SetTargetFormats(downloadJob.Selection)
                    .SetOutputFolder(provider => downloadJob.OutputDirectory.FullName)
                    .SetOutputName(provider => $"{downloadJob.DownloadedFileName}.{provider.Extension}")
                    .SetFfmpegPath(_ffmpegPath)
                    .AsCommand());
        }

        /// <summary>
        /// Asynchronously downloads audio specified within provided <see cref="DownloadJob"/> object.
        /// </summary>
        /// <param name="downloadJob">Contains details of audio to be downloaded.</param>
        public Task DownloadMusicAsync(DownloadJob downloadJob)
        {
            return DownloadVideoBase(downloadJob,
                YtdlpCommandBuilder
                .Download(downloadJob.Url)
                .SetTargetFormats(downloadJob.Selection)
                .SetOutputFolder(provider => downloadJob.OutputDirectory.FullName)
                .SetOutputName(provider => $"{downloadJob.DownloadedFileName}.{provider.Extension}")
                .SetFfmpegPath(_ffmpegPath)
                .ExtractAudio(AudioFormat.MP3, 4)
                .AsCommand());
        }

        private Task DownloadVideoBase(DownloadJob downloadJob, string command)
        {
            DownloadProgressCoordinator progressCoordinator = new();

            return _messenger.SendCommandAndWaitForResponseAsync(command,
                new Progress<ProcessCommandPartialOutput>(o =>
                {
                    downloadJob.ProgressCallback?.Report(progressCoordinator.ParseOutput(o));
                }), downloadJob.CancellationToken ?? default);
        }
    }
}
