using Model.Dto;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;

namespace Model
{
    /// <summary>
    /// Provides details regarding particular format available for specific material.
    /// </summary>
    [DebuggerDisplay("{Type} | {Name}")]
    public class FormatInfo
    {
        private FormatType _type;
        private string _id;
        private string _extension;
        private string _name;
        private string _description;
        private MemorySpace? _fileSize;
        private string _protocol;
        private string _comment;
        private string _url;
        private int? _quality;
        private bool? _hasDrm;
        private double? _bitrate;
        private AudioFormatDetails? _audioDetails;
        private VideoFormatDetails? _videoDetails;

        private FormatInfo() { }

        /// <summary>
        /// Describes type of this instance.
        /// </summary>
        public FormatType Type => _type;

        /// <summary>
        /// Material identifier.
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Material file extension.
        /// </summary>
        public string Extension => _extension;

        /// <summary>
        /// A human-readable description of the format.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Additional info about the format.
        /// </summary>
        public string Description => _description;

        /// <summary>
        /// The size of format, if known in advance.
        /// </summary>
        public MemorySpace? FileSize => _fileSize;

        /// <summary>
        /// The protocol that will be used for the actual download.
        /// </summary>
        public string Protocol => _protocol;

        /// <summary>
        /// Additional info about the format.
        /// </summary>
        public string Comment => _comment;

        /// <summary>
        /// Material URL.
        /// </summary>
        public string Url => _url;

        /// <summary>
        /// The quality of the format.
        /// </summary>
        public int? Quality => _quality;

        /// <summary>
        /// Determines whether material is protected by Digital Rights Managment system.
        /// </summary>
        public bool? HasDrm => _hasDrm;

        /// <summary>
        /// Average bitrate of audio and video in KBit/s.
        /// </summary>
        public double? Bitrate => _bitrate;

        /// <summary>
        /// Contains audio information for this instance.
        /// </summary>
        public AudioFormatDetails? AudioDetails => _audioDetails;

        /// <summary>
        /// Contains video information for this instance.
        /// </summary>
        public VideoFormatDetails? VideoDetails => _videoDetails;

        /// <summary>
        /// Returns new instance of <see cref="FormatInfo"/> based on provided <see cref="FormatDto"/> object.
        /// </summary>
        /// <param name="format">Contains data necessary for <see cref="FormatInfo"/> instantiation.</param>
        public static FormatInfo FromRecord(FormatDto format)
        {
            return new FormatInfo()
            {
                _type = ResolveFormatType(format),
                _id = format.format_id,
                _extension = format.ext,
                _name = format.format,
                _description = format.format_note,
                _fileSize = format.filesize is not null ? new MemorySpace((long)format.filesize) : null,
                _protocol = format.protocol,
                _comment = format.format_note,
                _url = format.url,
                _quality = format.quality,
                _hasDrm = format.has_drm,
                _bitrate = format.tbr,
                _audioDetails = AudioFormatDetails.FromDto(format),
                _videoDetails = VideoFormatDetails.FromDto(format)
            };

        }

        private static FormatType ResolveFormatType(FormatDto format)
        {
            if (format.format_note == "storyboard")
            {
                return FormatType.Images;
            }

            if (format.resolution == "audio only")
            {
                return FormatType.AudioOnly;
            }

            if (format.acodec == "none" && format.audio_ext == "none")
            {
                return FormatType.VideoOnly;
            }

            if (format.resolution != null)
            {
                return FormatType.Merged;
            }

            return FormatType.Other;
        }
    }
}