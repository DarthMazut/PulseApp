using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Describes type of <see cref="FormatInfo"/> object.
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// Audio and video are merged as one file.
        /// </summary>
        Merged,

        /// <summary>
        /// Format contains audio only.
        /// </summary>
        AudioOnly,

        /// <summary>
        /// Format contains video only.
        /// </summary>
        VideoOnly,

        /// <summary>
        /// Format contains images (thumbnails) of material.
        /// </summary>
        Images,

        /// <summary>
        /// Unspecified format type.
        /// </summary>
        Other
    }
}
