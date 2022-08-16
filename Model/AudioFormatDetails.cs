using Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Contains audio information for specific <see cref="FormatInfo"/> object.
    /// </summary>
    public class AudioFormatDetails
    {
        private string? _codec;
        private double? _bitRate;
        private double? _samplingRate;
        private string? _extension;

        private AudioFormatDetails() { }

        /// <summary>
        /// Name of the audio codec in use.
        /// </summary>
        public string? Codec => _codec;

        /// <summary>
        /// Average audio bitrate in KBit/s.
        /// </summary>
        public double? BitRate => _bitRate;

        /// <summary>
        /// Audio sampling rate in Hertz.
        /// </summary>
        public double? SamplingRate => _samplingRate;

        /// <summary>
        /// Audio file extension.
        /// </summary>
        public string? Extension => _extension;

        /// <summary>
        /// Returns new instacne of the <see cref="AudioFormatDetails"/> class based on provided
        /// <see cref="FormatDto"/> object. Returns <see cref="null"/> if no audio info is aviable.
        /// </summary>
        /// <param name="formatDto">Contains data necessary for <see cref="AudioFormatDetails"/> instantiation.</param>
        public static AudioFormatDetails? FromDto(FormatDto formatDto)
        {
            if (formatDto.acodec == null ||
                formatDto.abr == null ||
                formatDto.asr == null ||
                formatDto.audio_ext == null)
            {
                return null;
            }

            return new AudioFormatDetails()
            {
                _codec = formatDto.acodec,
                _bitRate = formatDto.abr,
                _samplingRate = formatDto.asr,
                _extension = formatDto.audio_ext
            };
        }
    }
}
