using Model.Dto;
using Model.ProcessCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Progress
{
    /// <summary>
    /// Represents progress of download process.
    /// </summary>
    public class DownloadProgress
    {
        public DownloadProgress(DownloadState state, string rawMessage, DateTimeOffset timestamp) : this(state, rawMessage, timestamp, false) { }

        public DownloadProgress(DownloadState state, string rawMessage, DateTimeOffset timestamp, bool isError)
        {
            State = state;
            CurrentMessage = rawMessage;
            IsErrorMessage = isError;
            TimeStamp = timestamp;
        }

        /// <summary>
        /// Initializes a new instacne of the <see cref="DownloadProgress"/> class.
        /// </summary>
        /// <param name="progressDto"></param>
        /// <param name="parts"></param>
        /// <param name="currentPart"></param>
        /// <param name="rawMessage"></param>
        /// <param name="timestamp"></param>
        public DownloadProgress(DownloadProgressDto progressDto, int parts, int currentPart, string rawMessage, DateTimeOffset timestamp)
        {
            Parts = parts;
            CurrentPart = currentPart;
            CurrentFileSize = new MemorySpace(progressDto.downloaded);
            TotalFileSize = ResolveTotalFileSize(progressDto);
            CurrentSpeed = ResolveCurrentSpeed(progressDto);
            CurrentEta = ResolveCurrentEta(progressDto);
            CurrentMessage = rawMessage;
            IsErrorMessage = false;
            State = DownloadState.Downloading;
            TimeStamp = timestamp;
        }

        public int? Parts { get; }

        /// <summary>
        /// Number of part currently being downloaded.
        /// </summary>
        public int? CurrentPart { get; }

        /// <summary>
        /// Progress percentage value.
        /// </summary>
        public double? CurrentPercentage => CalculateCurrentPercentage();

        /// <summary>
        /// Size of already downloaded part of file.
        /// </summary>
        public MemorySpace? CurrentFileSize { get; }

        /// <summary>
        /// Total size of currently downloaded file.
        /// </summary>
        public MemorySpace? TotalFileSize { get; }

        /// <summary>
        /// Current speed of downloading process.
        /// </summary>
        public MemorySpace? CurrentSpeed { get; }

        /// <summary>
        /// Estimated time to complete the download.
        /// </summary>
        public TimeSpan? CurrentEta { get; }

        /// <summary>
        /// A yt-dlp original message, on the basis of which this instance was created.
        /// </summary>
        public string CurrentMessage { get; }

        /// <summary>
        /// Determines whether log holding by <see cref="CurrentMessage"/> indicates an error.
        /// </summary>
        public bool IsErrorMessage { get; }

        /// <summary>
        /// Determines the state of downloading process.
        /// </summary>
        public DownloadState State { get; }

        /// <summary>
        /// Timestamp of log message used for creation of this instance.
        /// </summary>
        public DateTimeOffset TimeStamp { get; }

        private double? CalculateCurrentPercentage()
        {
            if (TotalFileSize is not null && CurrentFileSize is not null)
            {
                return CurrentFileSize.Value.Bytes * 100 / TotalFileSize.Value.Bytes;
            }

            return null;
        }

        private MemorySpace? ResolveTotalFileSize(DownloadProgressDto progressDto)
        {
            if (progressDto.total is not null)
            {
                return new MemorySpace((long)progressDto.total);
            }

            return null;
        }

        private MemorySpace? ResolveCurrentSpeed(DownloadProgressDto progressDto)
        {
            if (progressDto.speed is not null)
            {
                return new MemorySpace((long)Math.Round((double)progressDto.speed));
            }

            return null;
        }

        private TimeSpan? ResolveCurrentEta(DownloadProgressDto progressDto)
        {
            if (progressDto.eta is not null)
            {
                return TimeSpan.FromSeconds((int)progressDto.eta);
            }

            return null;
        }

    }
}
