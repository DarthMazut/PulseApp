using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utils
{
    /// <summary>
    /// Provides methods supporting memory related operations.
    /// </summary>
    public static class MemorySpaceCalculator
    {
        //CalculateOrEstimateSpace(FormatSelection selection)

        /// <summary>
        /// Returns available space on requested drive or <see langword="null"/> if provided drive name is not valid.
        /// </summary>
        /// <param name="driveName">Name of drive to be examined regarding free space.</param>
        public static MemorySpace? GetDriveFreeSpace(string driveName)
        {
            DriveInfo? driveInfo = DriveInfo.GetDrives().Where(d => d.IsReady && d.Name == driveName).FirstOrDefault();
            if (driveInfo is null)
            {
                return null;
            }

            return MemorySpace.FromBytes(driveInfo.AvailableFreeSpace);
        }

        /// <summary>
        /// Returns available space on drive corresponding to given path or <see langword="null"/> if provided path is not valid.
        /// </summary>
        /// <param name="path">Path related to particular drive.</param>
        public static MemorySpace? GetPathDriveFreeSpace(string path)
        {
            string? driveName = Path.GetPathRoot(path);
            if (driveName is null)
            {
                return null;
            }

            return GetDriveFreeSpace(driveName);
        }

        /// <summary>
        /// Checks whether the amount of available space on drive corresponding to given path is greater than provided value.
        /// Returns <see langword="null"/> if that cannot be determined.
        /// </summary>
        /// <param name="path">Path corresponding to drive being inspected.</param>
        /// <param name="compareSpace">Amount of space the check is made against.</param>
        public static bool? CheckSufficientSpace(string path, MemorySpace compareSpace)
        {
            MemorySpace? driveSpace = GetPathDriveFreeSpace(path);
            if (driveSpace is null)
            {
                return null;
            }

            return driveSpace > compareSpace;
        }

        public static MemorySpace? CalculateOrEstimateFormatInfoSpace(FormatInfo formatInfo, TimeSpan duration, out bool isEstimated)
        {
            isEstimated = false;

            if (formatInfo.FileSize is not null)
            {
                return formatInfo.FileSize;
            }

            if (formatInfo.Bitrate is not null)
            {
                return new MemorySpace((long)Math.Round(formatInfo.Bitrate.Value / 8 * duration.TotalSeconds));
            }

            isEstimated = true;

            MemorySpace? audioSpace = null;
            MemorySpace? videoSpace = null;

            MemorySpace? totalSpace;
            if (audioSpace is not null)
            {
                totalSpace = audioSpace;
            }

            if (audioSpace is null && videoSpace is null)
            {
                isEstimated = false;
            }

            return new MemorySpace();
        }

        public static MemorySpace? CalculateOrEstimateFormatSelectionSpace(FormatSelection formatSelection, out bool isEstimated)
        {
            throw new NotImplementedException();
        }
    }
}
