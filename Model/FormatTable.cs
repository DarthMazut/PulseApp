using Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Contains available formats for particular video.
    /// Exposes fluent API for filtering format collection.
    /// </summary>
    public class FormatTable
    {
        private IReadOnlyList<FormatInfo> _aviableFormats = new List<FormatInfo>();

        private FormatTable() { }

        /// <summary>
        /// A collection of all available <see cref="FormatInfo"/> objects.
        /// </summary>
        public IReadOnlyList<FormatInfo> AvailableFormats => _aviableFormats;

        /// <summary>
        /// Returns a list of all available extensions within this <see cref="FormatTable"/>.
        /// </summary>
        public IReadOnlyList<string> GetAvailableExtensions()
        {
            return AvailableFormats
                .DistinctBy(f => f.Extension)
                .Select(f => f.Extension)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<string> GetAvailableAudioExtensions()
        {
            return AvailableFormats
                .Where(f => f.Type != FormatType.VideoOnly && f.Type != FormatType.Images)
                .DistinctBy(f => f.Extension)
                .Select(f => f.Extension)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<string> GetAvailableVideoExtensions()
        {
            return AvailableFormats
                .Where(f => f.Type != FormatType.AudioOnly && f.Type != FormatType.Images)
                .DistinctBy(f => f.Extension)
                .Select(f => f.Extension)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<ResolutionInfo> GetAvailableVideoResolutions()
        {
            return AvailableFormats
                .Where(f => f.VideoDetails?.Resolution != null)
                .Select(f => f.VideoDetails!.Resolution!)
                .Distinct()
                .ToList()
                .AsReadOnly();
        }


        public FormatTable GetOnlyTypes(FormatType type) => new FormatTable()
        {
            _aviableFormats = AvailableFormats
                .Where(f => f.Type == type)
                .ToList()
                .AsReadOnly()
        };

        public FormatTable GetOnlyExtensions(string extension) => new FormatTable()
        {
            _aviableFormats = AvailableFormats
                .Where(f =>
                    f.Extension == extension ||
                    f.AudioDetails?.Extension == extension ||
                    f.VideoDetails?.Extension == extension)
                .ToList()
                .AsReadOnly()
        };

        public FormatTable GetOnlyAudioCodec(string codec) => new FormatTable()
        {
            _aviableFormats = AvailableFormats
                .Where(f => f.AudioDetails?.Codec == codec)
                .ToList()
                .AsReadOnly()
        };

        public FormatTable GetOnlyVideoCodec(string codec) => new FormatTable()
        {
            _aviableFormats = AvailableFormats
                .Where(f => f.VideoDetails?.Codec == codec)
                .ToList()
                .AsReadOnly()
        };

        public FormatTable GetOnlyResolution(ResolutionInfo resolution) => new FormatTable()
        {
            _aviableFormats = AvailableFormats
                .Where(f => f.VideoDetails?.Resolution?.Equals(resolution) == true)
                .ToList()
                .AsReadOnly()
        };

        public FormatTable GetOnly(Func<FormatInfo, bool> predicate) => new FormatTable()
        {
            _aviableFormats = AvailableFormats
                .Where(predicate)
                .ToList()
                .AsReadOnly()
        };

        public FormatInfo? GetHighestAudioQuality()
        {
            return AvailableFormats
                .Where(f => f.Type != FormatType.VideoOnly)
                .Where(f => f.Type != FormatType.Images)
                .OrderBy(f => f.AudioDetails?.BitRate)
                .ThenBy(f => f.AudioDetails?.SamplingRate)
                .ThenBy(f => f.Bitrate)
                .ThenBy(f => f.FileSize?.Bytes)
                .ThenBy(f => f.VideoDetails?.Resolution?.Height)
                .ThenBy(f => f.VideoDetails?.Resolution?.Width)
                .LastOrDefault();
        }

        public FormatInfo? GetHighestVideoQuality()
        {
            return AvailableFormats
                .Where(f => f.Type != FormatType.AudioOnly)
                .Where(f => f.Type != FormatType.Images)
                .OrderBy(f => f.VideoDetails?.Resolution?.Height)
                .ThenBy(f => f.VideoDetails?.Resolution?.Width)
                .ThenBy(f => f.VideoDetails?.Fps)
                .ThenBy(f => f.VideoDetails?.BitRate)
                .ThenBy(f => f.Bitrate)
                .ThenBy(f => f.FileSize?.Bytes)
                .ThenBy(f => f.Quality)
                .LastOrDefault();
        }

        public FormatInfo? ResolveBestAudioFormatForExtension(string extension)
        {
            FormatInfo? bestAudioQualityWithExtension = 
                GetOnlyTypes(FormatType.AudioOnly)
               .GetOnlyExtensions(extension)
               .GetHighestAudioQuality();

            if (bestAudioQualityWithExtension is not null)
            {
                return bestAudioQualityWithExtension;
            }

            FormatTable mergedPositionsWithExtension = GetOnlyTypes(FormatType.Merged).GetOnlyExtensions(extension);
            if (mergedPositionsWithExtension.AvailableFormats.Any())
            {
                string? bestAudioCodec = mergedPositionsWithExtension.GetHighestAudioQuality()?.AudioDetails?.Codec;
                if (bestAudioCodec is not null)
                {
                    return GetOnlyAudioCodec(bestAudioCodec).GetHighestAudioQuality();
                }
            }

            return GetHighestAudioQuality();
        }

        public FormatSelection? SelectBestQualityForExtension(string extension)
        {
            FormatInfo? bestVideoQuality = GetOnlyExtensions(extension).GetHighestVideoQuality();
            FormatInfo? bestAudioQuality = ResolveBestAudioFormatForExtension(extension);

            if (bestAudioQuality is not null && bestVideoQuality is not null)
            {
                if (bestVideoQuality.Type == FormatType.Merged)
                {
                    return FormatSelection.FromVideoFormat(bestVideoQuality);
                }

                if (bestAudioQuality.Type == FormatType.Merged)
                {
                    return FormatSelection.FromAudioFormat(bestAudioQuality);
                }

                return FormatSelection.FromAudioVideoFormat(bestAudioQuality, bestVideoQuality);
            }

            if (bestAudioQuality is not null)
            {
                return FormatSelection.FromAudioFormat(bestAudioQuality);
            }

            if (bestVideoQuality is not null)
            {
                return FormatSelection.FromVideoFormat(bestVideoQuality);
            }

            return null;
        }

        public static FormatTable? FromDto(IReadOnlyList<FormatDto> formats)
        {
            if (formats?.Any() != true)
            {
                return null;
            }

            return new FormatTable()
            {
                _aviableFormats = formats.Select(f => FormatInfo.FromRecord(f)).ToList().AsReadOnly()
            };
        }
    }
}
