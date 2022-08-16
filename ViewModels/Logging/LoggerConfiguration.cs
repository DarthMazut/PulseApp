using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Logging
{
    /// <summary>
    /// Allows for configuration of global <see cref="Logger"/> object.
    /// </summary>
    public class LoggerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerConfiguration"/> class.
        /// </summary>
        /// <param name="workingDirectory">
        /// Path to the <see cref="Logger"/> working directory.
        /// Both current log file and archive file will be placed there.
        /// </param>
        public LoggerConfiguration(string workingDirectory) : this(workingDirectory, "logs.txt", "logs.archive.txt") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerConfiguration"/> class.
        /// </summary>
        /// <param name="workingDirectory">
        /// Path to the <see cref="Logger"/> working directory.
        /// Both current log file and archive file will be placed there.
        /// </param>
        /// <param name="logFileName">Name of current log file.</param>
        /// <param name="archiveFileName">Name of logs archive file.</param>
        public LoggerConfiguration(string workingDirectory, string logFileName, string archiveFileName)
        {
            WorkingDirectory = workingDirectory;
            LogFileName = logFileName;
            ArchiveFileName = archiveFileName;
            LogFilePath = Path.Combine(workingDirectory, logFileName);
            ArchiveFilePath = Path.Combine(workingDirectory, archiveFileName);
        }

        /// <summary>
        /// Path to a folder where both logs and log archive files are located.
        /// </summary>
        public string WorkingDirectory { get; }

        /// <summary>
        /// Name of current log file.
        /// </summary>
        public string LogFileName { get; }

        /// <summary>
        /// Name of log archive file.
        /// </summary>
        public string ArchiveFileName { get; }

        /// <summary>
        /// Path to current log file.
        /// </summary>
        public string LogFilePath { get; }

        /// <summary>
        /// Path to log archive file.
        /// </summary>
        public string ArchiveFilePath { get; }

        /// <summary>
        /// Template used for writing logs timestamp.
        /// </summary>
        public string TimestampTemplate { get; init; } = "yyyy-MM-dd HH:mm:ss:fff zzz";

        /// <summary>
        /// Maximum size of current log file in bytes.
        /// </summary>
        public ulong MaximumLogFileSize { get; set; } = 512 * 1024 * 1024; // 500 MB
    }
}
