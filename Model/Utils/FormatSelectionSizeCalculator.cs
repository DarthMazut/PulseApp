using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utils
{
    /// <summary>
    /// Provides methods for calculating or estimating size of given <see cref="FormatSelection"/> object.
    /// </summary>
    public class FormatSelectionSizeCalculator
    {
        /// <summary>
        /// Returns caluclated or estimated size of given <see cref="FormatSelection"/> instance.
        /// </summary>
        /// <param name="metadata">Metadata containing formats to be calucalted.</param>
        /// <param name="formatSelection">Contains formats to be calucalted.</param>
        /// <param name="isEstimated">Determines whether results are accurate or estimated (*true* means results are estimated).</param>
        public static MemorySpace? CalculateOrEstimateSize(VideoMetadata metadata, FormatSelection formatSelection, out bool isEstimated)
        {
            if (formatSelection.AudioSelection is null || formatSelection.VideoSelection is null)
            {
                return CalculateOrEstimateOneFormat(metadata, formatSelection.AudioSelection ?? formatSelection.VideoSelection!, out isEstimated);
            }

            if (formatSelection.AudioSelection.Id == formatSelection.VideoSelection.Id)
            {
                return CalculateOrEstimateOneFormat(metadata, formatSelection.VideoSelection, out isEstimated);
            }

            return CalculateOrEstimateBothFormats(metadata, formatSelection.AudioSelection, formatSelection.VideoSelection, out isEstimated);
        }

        private static MemorySpace? CalculateOrEstimateOneFormat(VideoMetadata metadata, FormatInfo formatInfo, out bool isEstimated)
        {
            isEstimated = false;

            if (formatInfo.FileSize is not null)
            {
                return formatInfo.FileSize;
            }

            isEstimated = true;

            if (formatInfo.Bitrate is not null)
            {
                return MemorySpace.FromKilobytes((double)(metadata.Durotion.TotalSeconds * formatInfo.Bitrate / 8));
            }

            MemorySpace? videoSize = null;
            if (formatInfo.VideoDetails?.BitRate is not null)
            {
                videoSize = MemorySpace.FromKilobytes((double)(metadata.Durotion.TotalSeconds * formatInfo.VideoDetails.BitRate));
            }

            MemorySpace? audioSize = null;
            if (formatInfo.AudioDetails?.BitRate is not null)
            {
                audioSize = MemorySpace.FromKilobytes((double)(metadata.Durotion.TotalSeconds * formatInfo.AudioDetails.BitRate / 8));
            }

            if (audioSize is not null && videoSize is not null)
            {
                return audioSize + videoSize;
            }
            
            if (audioSize is not null || videoSize is not null)
            {
                return audioSize ?? videoSize;
            }

            isEstimated = false;
            return null;
        }

        private static MemorySpace? CalculateOrEstimateBothFormats(VideoMetadata metadata, FormatInfo audioSelection, FormatInfo videoSelection, out bool isEstimated)
        {
            MemorySpace? audioSize = CalculateOrEstimateOneFormat(metadata, audioSelection, out bool isAudioEstimated);
            MemorySpace? videoSize = CalculateOrEstimateOneFormat(metadata, videoSelection, out bool isVideoEstimated);

            if (audioSize is not null && videoSize is not null)
            {
                isEstimated = isAudioEstimated || isVideoEstimated;
                return audioSize + videoSize;
            }

            if (audioSize is not null || videoSize is not null)
            {
                isEstimated = audioSize is null ? isVideoEstimated : isAudioEstimated;
                return audioSize ?? videoSize;
            }

            isEstimated = false;
            return null;
        }
    }
}
