using Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Contains video information for specific <see cref="FormatInfo"/> object.
    /// </summary>
    public class VideoFormatDetails
    {
        private ResolutionInfo? _resolution;
        private int? _fps;
        private string? _codec;
        private double? _bitRate;
        private string? _extension;

        private VideoFormatDetails() { }

        public ResolutionInfo? Resolution => _resolution;

        public int? Fps => _fps;

        public string? Codec => _codec;

        public double? BitRate => _bitRate;

        public string? Extension => _extension;

        public static VideoFormatDetails? FromDto(FormatDto formatDto)
        {
            ResolutionInfo? resolution = ResolutionInfo.FromDto(formatDto);

            if (resolution == null &&
                formatDto.fps == null &&
                formatDto.vcodec == null &&
                formatDto.vbr == null &&
                formatDto.video_ext == null)
            {
                return null;
            }

            return new VideoFormatDetails()
            {
                _resolution = resolution,
                _fps = formatDto.fps is null ? null : (int)Math.Round((double)formatDto.fps),
                _codec = formatDto.vcodec,
                _bitRate = formatDto.vbr,
                _extension = formatDto.video_ext
            };
        }

    }
}
