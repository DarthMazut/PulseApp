using Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Contains metadata regarding specific video.
    /// </summary>
    public class VideoMetadata
    {
        private string _title;
        private string _description;
        private TimeSpan _durotion;
        private string _thumbnailUrl;
        private string _videoUrl;
        private DateTimeOffset? _uploadDate;
        private string _chanelName;
        private long _views;
        private FormatTable? _formatTable;

        private VideoMetadata() { }

        /// <summary>
        /// Video title.
        /// </summary>
        public string Title => _title;

        /// <summary>
        /// Video description.
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// How long is the video.
        /// </summary>
        public TimeSpan Durotion => _durotion;

        /// <summary>
        /// A link to the video thumbnail.
        /// </summary>
        public string ThumbnailUrl => _thumbnailUrl;

        /// <summary>
        /// A link to video webpage.
        /// </summary>
        public string VideoUrl => _videoUrl;

        /// <summary>
        /// When video was uploaded.
        /// </summary>
        public DateTimeOffset? UploadDate => _uploadDate;

        /// <summary>
        /// Uploader chanel name.
        /// </summary>
        public string ChanelName => _chanelName;

        /// <summary>
        /// How many views has a video.
        /// </summary>
        public long Views => _views;

        /// <summary>
        /// Available formats for this video.
        /// </summary>
        public FormatTable? FormatTable => _formatTable;

        /// <summary>
        /// Returns new instacne of the <see cref="VideoMetadata"/> class based on provided
        /// <see cref="MetadataDto"/> object.
        /// </summary>
        /// <param name="rootMetadata">Contains data necessary for <see cref="VideoMetadata"/> instantiation.</param>
        public static VideoMetadata FromRecord(MetadataDto rootMetadata)
        {
            return new VideoMetadata()
            {
                _title = rootMetadata.title,
                _description = rootMetadata.description,
                _durotion = DurotionDoubleToTimeSpan(rootMetadata.duration),
                _thumbnailUrl = rootMetadata.thumbnail,
                _videoUrl = rootMetadata.webpage_url,
                _uploadDate = UploadDateStringToDateTimeOffset(rootMetadata.upload_date),
                _chanelName = rootMetadata.uploader,
                _views = rootMetadata.view_count,
                _formatTable = FormatTable.FromDto(rootMetadata.formats)
            };
        }

        private static DateTimeOffset? UploadDateStringToDateTimeOffset(string? upload_date)
        {
            if (upload_date is null)
            {
                return null;
            }

            return DateTimeOffset.ParseExact(upload_date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static TimeSpan DurotionDoubleToTimeSpan(double duration)
        {
            return TimeSpan.FromSeconds(duration);
        }
    }
}

