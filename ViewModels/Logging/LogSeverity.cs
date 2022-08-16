namespace ViewModels.Logging
{
    /// <summary>
    /// Describes importance of log message.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        /// Use for spamming the logs.
        /// </summary>
        Trace,

        /// <summary>
        /// Use for debug purpose.
        /// </summary>
        Debug,

        /// <summary>
        /// Use for tracking end-user application usage.
        /// </summary>
        Info,

        /// <summary>
        /// Use when something happend that shouldn't.
        /// </summary>
        Warning,

        /// <summary>
        /// Use if some functionality failed.
        /// </summary>
        Error,

        /// <summary>
        /// Use if app crashes.
        /// </summary>
        Fatal
    }
}
