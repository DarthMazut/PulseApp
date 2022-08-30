using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Progress;

namespace Model
{
    /// <summary>
    /// Contains data describing single process of audio/video download.
    /// </summary>
    public class DownloadJob
    {
        public string Url { get; set; }

        public FormatSelection Selection { get; set; }

        public DirectoryInfo OutputDirectory { get; set; }

        public string DownloadedFileName { get; set; }

        public IProgress<DownloadProgress>? ProgressCallback { get; set; }

        public CancellationToken? CancellationToken { get; set; }

        public string? TargetFileExtension { get; set; }
    }
}
