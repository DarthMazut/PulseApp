using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Encapsulates format(s) selected for download process.
    /// </summary>
    public class FormatSelection
    {
        private FormatInfo? _videoSelection;
        private FormatInfo? _audioSelection;

        private FormatSelection() { }

        /// <summary>
        /// Selected video format.
        /// </summary>
        public FormatInfo? VideoSelection => _videoSelection;

        /// <summary>
        /// Selected audio format.
        /// </summary>
        public FormatInfo? AudioSelection => _audioSelection;

        /// <inheritdoc/>
        public override string ToString()
        {
            if (VideoSelection is null)
            {
                return AudioSelection!.Id;
            }

            if (AudioSelection is null)
            {
                return VideoSelection!.Id;
            }

            return $"{VideoSelection.Id}+{AudioSelection.Id}";
        }

        /// <summary>
        /// Returns new instance of the <see cref="FormatSelection"/> class encapsulating given audio format.
        /// </summary>
        /// <param name="audioFormat">Selected audio format.</param>
        public static FormatSelection FromAudioFormat(FormatInfo audioFormat)
        {
            return new FormatSelection()
            {
                _audioSelection = audioFormat
            };
        }

        /// <summary>
        /// Returns new instance of the <see cref="FormatSelection"/> class encapsulating given video format.
        /// </summary>
        /// <param name="videoFormat">Selected video format.</param>
        public static FormatSelection FromVideoFormat(FormatInfo videoFormat)
        {
            return new FormatSelection()
            {
                _videoSelection = videoFormat
            };
        }

        /// <summary>
        /// Returns new instance of the <see cref="FormatSelection"/> class encapsulating given audio and video format.
        /// </summary>
        /// <param name="audioFormat">Selected audio format.</param>
        /// <param name="videoFormat">Selected video format.</param>
        public static FormatSelection FromAudioVideoFormat(FormatInfo audioFormat, FormatInfo videoFormat)
        {
            return new FormatSelection()
            {
                _audioSelection = audioFormat,
                _videoSelection = videoFormat
            };
        }
    }
}
