namespace Model.Adapter
{
    /// <summary>
    /// Provides a *yt-dlp* format expressions.
    /// </summary>
    public class YtdlpCommandBuilderFormatProvider
    {
        /// <summary>
        /// Video ID.
        /// </summary>
        public string Id => "%(id)s";

        /// <summary>
        /// Video title.
        /// </summary>
        public string Title => "%(title)s";

        /// <summary>
        /// Target file extension without leading dot.
        /// </summary>
        public string Extension => "%(ext)s";

        /// <summary>
        /// Upload date in YYYYMMDD format.
        /// </summary>
        public string UploadDateUtc => "%(upload_date)s";

        /// <summary>
        /// Video duration in HH:mm:ss format.
        /// </summary>
        public string Duration => "%(duration_string)s";

        /// <summary>
        /// Video uploader name.
        /// </summary>
        public string Uploader => "%(uploader)s";
    }
}
