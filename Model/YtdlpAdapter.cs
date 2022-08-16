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
        private const string JSON_PROGRESS_TEMPLATE = @"[JSONDownload]{\""downloaded\"": %(progress.downloaded_bytes)s,\""total\"": %(progress.total_bytes)s,\""elapsed\"": %(progress.elapsed)s,\""speed\"": %(progress.speed)s,\""eta\"": %(progress.eta)s}";

        private readonly ProcessMessenger _messenger;

        /// <summary>
        /// Initializes a new instance of the <see cref="YtdlpAdapter"/> class.
        /// </summary>
        /// <param name="path">Location of *yt-dlp* executable file.</param>
        public YtdlpAdapter(string path)
        {
            _messenger = new ProcessMessenger(path, Path.GetDirectoryName(path));
        }

        /// <summary>
        /// Asynchronously downloads metadata for specific video.
        /// Throws if metadata couldn't be downloaded for whatever reason.
        /// </summary>
        /// <param name="url">Target video URL.</param>
        /// <exception cref="YtdlpException"/>
        public Task<VideoMetadata> DownloadVideoMetadataAsync(string url)
        {
            return DownloadVideoMetadataAsync(url, default);
        }

        /// <summary>
        /// Asynchronously downloads metadata for specific video.
        /// Throws if metadata couldn't be downloaded for whatever reason.
        /// </summary>
        /// <param name="url">Target video URL.</param>
        /// <param name="token">Cancellation token used for cancelling this operation.</param>
        /// <exception cref="YtdlpException"/>
        public async Task<VideoMetadata> DownloadVideoMetadataAsync(string url, CancellationToken token)
        {
            ProcessCommandOutput output = await _messenger.SendCommandAndWaitForResponseAsync(CreateGetMetadataCommand(url), null, token);
            
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
                catch (JsonSerializationException ex)
                {
                    throw new YtdlpException(
                        "Cannot deserialize output from Yt-dlp.",
                        ex,
                        output.ErrorPartialResults.Select(o => o.Message).ToList());
                } 
            }

            throw new YtdlpException("No response received from Yt-dlp.");
        }


        public Task DownloadVideoAsync(DownloadJob downloadJob)
        {
            DownloadProgressCoordinator progressCoordinator = new();

            return _messenger.SendCommandAndWaitForResponseAsync(
                CreateDownloadVideoCommand(
                    downloadJob.Url,
                    downloadJob.Selection.ToString(),
                    downloadJob.OutputDirectory.FullName,
                    downloadJob.DownloadedFileName),
                new Progress<ProcessCommandPartialOutput>(o => 
                {
                    downloadJob.ProgressCallback?.Report(progressCoordinator.ParseOutput(o));
                }), downloadJob.CancellationToken ?? default);
        }

        private string CreateGetMetadataCommand(string url)
        {
            return $@"--print ""%()j"" -- {url}";
        }

        private string CreateDownloadVideoCommand(string url, string selectedFormats, string outputFolderPath, string outputFileName)
        {
            return $"-f {selectedFormats} -o \"{Path.Combine(outputFolderPath, outputFileName)}.%(ext)s\" --newline --progress-template \"{JSON_PROGRESS_TEMPLATE}\" -- {url} ";
        }
    }
}
