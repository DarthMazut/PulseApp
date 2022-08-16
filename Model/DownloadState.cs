using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Describes the status of video download process.
    /// </summary>
    public enum DownloadState
    {
        /// <summary>
        /// Download process is being prepared.
        /// </summary>
        Initializing,

        /// <summary>
        /// Downloading is in progress.
        /// </summary>
        Downloading,

        /// <summary>
        /// Downloaded files are being merged.
        /// </summary>
        Merging,

        /// <summary>
        /// Download process leftovers are being removed.
        /// </summary>
        Cleaning
    }
}
