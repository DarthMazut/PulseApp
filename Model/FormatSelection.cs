using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FormatSelection
    {
        private FormatInfo? _videoSelection;
        private FormatInfo? _audioSelection;

        private FormatSelection() { }

        public FormatInfo? VideoSelection => _videoSelection;

        public FormatInfo? AudioSelection => _audioSelection;

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

        public static FormatSelection FromAudioFormat(FormatInfo audioFormat)
        {
            return new FormatSelection()
            {
                _audioSelection = audioFormat
            };
        }

        public static FormatSelection FromVideoFormat(FormatInfo videoFormat)
        {
            return new FormatSelection()
            {
                _videoSelection = videoFormat
            };
        }

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
