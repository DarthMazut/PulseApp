using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Logging
{
    /// <summary>
    /// Provides event logging functionality.
    /// </summary>
    public static class Logger
    {
        private static LoggerConfiguration? _configuration;
        private static TextWriter? _streamWriter;

        /// <summary>
        /// Determines whether global <see cref="Logger"/> object has been initialized
        /// by <see cref="Setup(LoggerConfiguration)"/> method.
        /// </summary>
        public static bool IsInitialized => _configuration != null;

        /// <summary>
        /// Contains <see cref="Logger"/> configuration file.
        /// </summary>
        public static LoggerConfiguration? Configuration => _configuration;

        /// <summary>
        /// Specifies whether any logging session is currently active i.e. logs file is open.
        /// </summary>
        public static bool HasActiveSession => _streamWriter != null;

        /// <summary>
        /// Determines whether <see cref="Logger"/> is allowed to store logs.
        /// </summary>
        public static bool AllowStoreLogs { get; set; } = true;

        /// <summary>
        /// Initializes the global <see cref="Logger"/> object.
        /// </summary>
        /// <param name="configuration">Contains logger configuration data.</param>
        public static void Setup(LoggerConfiguration configuration)
        {
            _configuration = configuration;
            if (!File.Exists(configuration.LogFilePath))
            {
                File.Create(configuration.LogFilePath).Dispose();
            }
        }

        /// <summary>
        /// Determines whether logs output file has exceeded maximum size 
        /// specified in <see cref="LoggerConfiguration.MaximumLogFileSize"/> property. 
        /// </summary>
        public static bool IsOutputFileSizeExceeded()
        {
            ThrowIfNotInitialized(nameof(IsOutputFileSizeExceeded));

            FileInfo outputFileInfo = new(Configuration!.LogFilePath);
            return (ulong)outputFileInfo.Length >= Configuration!.MaximumLogFileSize;
        }

        /// <summary>
        /// Moves logs for current log file to the archive file.
        /// <para>Do not call this method during active session.</para>
        /// </summary>
        public static async Task ArchiveAndClearCurrentFile()
        {
            ThrowIfNotInitialized(nameof(ArchiveAndClearCurrentFile));
            ThrowIfActiveSession(nameof(ArchiveAndClearCurrentFile));

            await Task.Run(() => File.Copy(Configuration!.LogFilePath, Configuration.ArchiveFilePath, true));
            await File.WriteAllTextAsync(Configuration!.LogFilePath, string.Empty);
        }

        /// <summary>
        /// Copies current log file and (if present) archive file to the specified location merging them together.
        /// </summary>
        /// <param name="targetFile">Path to the output file.</param>
        public static async Task ExportAll(string targetFile)
        {
            ThrowIfNotInitialized(nameof(ExportCurrentFile));

            if (File.Exists(Configuration!.ArchiveFilePath))
            {
                await Task.Run(() =>
                {
                    File.Copy(Configuration!.ArchiveFilePath, targetFile, true);
                    using StreamReader reader = new(Configuration!.LogFilePath, new FileStreamOptions() 
                    { 
                        Mode = FileMode.Open,
                        Access = FileAccess.Read,
                        Share = FileShare.ReadWrite 
                    });
                    using StreamWriter writer = new(targetFile, true);
                    while (!reader.EndOfStream)
                    {
                        string? currentLine = reader.ReadLine();
                        writer.WriteLine(currentLine);
                    }
                });
            }
            else
            {
                await Task.Run(() => File.Copy(Configuration!.LogFilePath, targetFile, true));
            }
        }

        /// <summary>
        /// Asynchronously copies current log file into specified location.
        /// </summary>
        /// <param name="targetFile">Path to the output file.</param>
        public static Task ExportCurrentFile(string targetFile)
        {
            ThrowIfNotInitialized(nameof(ExportCurrentFile));

            return Task.Run(() => File.Copy(Configuration!.LogFilePath, targetFile, true));
        }

        /// <summary>
        /// Starts logging session by opening the output file, making it ready for capturing incoming logs.
        /// <para>This method is <b>NOT thread-safe</b> and should be called from UI thread.</para>
        /// </summary>
        public static void StartSession()
        {
            ThrowIfNotInitialized(nameof(StartSession));
            ThrowIfActiveSession(nameof(StartSession));

            _streamWriter = TextWriter.Synchronized(new StreamWriter(Configuration!.LogFilePath, true));
            MarkSessionStart();
        }

        #region LOGGING

        public static void LogTrace(string message) => LogCore(LogSeverity.Trace, null, message);
        public static void LogTrace(Exception ex) => LogCore(LogSeverity.Trace, ex, null);
        public static void LogTrace(Exception? ex, string? message) => LogCore(LogSeverity.Trace, ex, message);


        public static void LogDebug(string message) => LogCore(LogSeverity.Debug, null, message);
        public static void LogDebug(Exception ex) => LogCore(LogSeverity.Debug, ex, null);
        public static void LogDebug(Exception? ex, string? message) => LogCore(LogSeverity.Debug, ex, message);


        public static void LogInfo(string message) => LogCore(LogSeverity.Info, null, message);
        public static void LogInfo(Exception ex) => LogCore(LogSeverity.Info, ex, null);
        public static void LogInfo(Exception? ex, string? message) => LogCore(LogSeverity.Info, ex, message);


        public static void LogWarning(string message) => LogCore(LogSeverity.Warning, null, message);
        public static void LogWarning(Exception ex) => LogCore(LogSeverity.Warning, ex, null);
        public static void LogWarning(Exception? ex, string? message) => LogCore(LogSeverity.Warning, ex, message);


        public static void LogError(string message) => LogCore(LogSeverity.Error, null, message);
        public static void LogError(Exception ex) => LogCore(LogSeverity.Error, ex, null);
        public static void LogError(Exception? ex, string? message) => LogCore(LogSeverity.Error, ex, message);


        public static void LogFatal(string message) => LogCore(LogSeverity.Fatal, null, message);
        public static void LogFatal(Exception ex) => LogCore(LogSeverity.Fatal, ex, null);
        public static void LogFatal(Exception? ex, string? message) => LogCore(LogSeverity.Fatal, ex, message);

        private static void LogCore(LogSeverity severity, Exception? ex, string? message)
        {
            if (AllowStoreLogs)
            {
                if (!IsInitialized)
                {
                    throw new InvalidOperationException("Cannot log because logger hasn't been initialized.");
                }

                if (!HasActiveSession)
                {
                    throw new InvalidOperationException("Cannot log because there is no active logging session.");
                }

                string messageLog = message ?? string.Empty;
                if (ex is not null)
                {
                    messageLog += Environment.NewLine + ex.ToString();
                }

                LogBase(severity, messageLog);
            }
        }

        private static void LogBase(LogSeverity severity, string message)
        {
            _streamWriter!.WriteLine($"[{DateTimeOffset.Now.ToString(Configuration!.TimestampTemplate)}] {severity.ToString().ToUpper()}:\t {message}");
        }

        #endregion

        /// <summary>
        /// Flushes the current stream if possible.
        /// </summary>
        public static void TrySynchronize()
        {
            _streamWriter?.Flush();
        }

        /// <summary>
        /// Tries to end current logging session.
        /// Flushes the current logs buffer if necessary.
        /// </summary>
        public static void TryEndSession()
        {
            if (HasActiveSession)
            {
                MarkSessionEnd();
                _streamWriter?.Dispose();
                _streamWriter = null;
            }
        }

        private static void MarkSessionStart()
        {
            if (AllowStoreLogs)
            {
                string sessionStart = Environment.NewLine +
                "====================" + Environment.NewLine +
                "SESSION STARTED" + Environment.NewLine +
                DateTimeOffset.Now.ToString(Configuration!.TimestampTemplate) + Environment.NewLine +
                "====================" + Environment.NewLine +
                Environment.NewLine;

                _streamWriter!.Write(sessionStart);
                _streamWriter!.Flush(); // Make sure whatever happens, we at least log app started event.
            }
        }

        private static void MarkSessionEnd()
        {
            if (AllowStoreLogs)
            {
                string sessionEnd = Environment.NewLine +
                "====================" + Environment.NewLine +
                "SESSION ENDED" + Environment.NewLine +
                DateTimeOffset.Now.ToString(Configuration!.TimestampTemplate) + Environment.NewLine +
                "====================" + Environment.NewLine +
                Environment.NewLine;

                _streamWriter!.Write(sessionEnd);
            }
        }

        private static void ThrowIfActiveSession(string methodName)
        {
            if (HasActiveSession)
            {
                throw new InvalidOperationException($"Cannot call *{methodName}()* during active logging session.");
            }
        }

        private static void ThrowIfNotInitialized(string methodName)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException($"Cannot call *{methodName}()* before logger initialization. Call *Setup()* first.");
            }
        }

        private static void ThrowIfNoActiveSession(string methodName)
        {
            if (!HasActiveSession)
            {
                throw new InvalidOperationException($"Cannot call *{methodName}()* because there is no active logging session. Call *StartSession()* first.");
            }
        }
    }
}
