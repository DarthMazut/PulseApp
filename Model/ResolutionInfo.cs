using Model.Dto;
using System.Text.RegularExpressions;

namespace Model
{
    public class ResolutionInfo
    {
        public ResolutionInfo(int height)
        {
            Height = height;
        }

        public ResolutionInfo(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height { get; }

        public int? Width { get; }

        public static ResolutionInfo? FromString(string? str)
        {
            if (str is null)
            {
                return null;
            }

            Match resolutionMacth = new Regex(@"\d?\d?\d\dx\d?\d?\d\d").Match(str);
            if (resolutionMacth.Success)
            {
                string[] values = resolutionMacth.Value.Split('x', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                int width = int.Parse(values[0]);
                int height = int.Parse(values[1]);
                return new ResolutionInfo(width, height);
            }

            Match standardMatch = new Regex(@"\d?\d?\d\dp").Match(str);
            if (standardMatch.Success)
            {
                string heightValue = standardMatch.Value.Split('p')[0];
                int height = int.Parse(heightValue);
                return new ResolutionInfo(height);
            }

            return null;
        }

        public static ResolutionInfo? FromDto(FormatDto format)
        {
            if (format.resolution == "audio only")
            {
                return null;
            }

            return FromString(format.resolution) ??
                   FromString($"{format.width}x{format.height}") ??
                   FromString(format.format) ??
                   FromString(format.format_id) ??
                   FromString(format.format_note);
        }

        public override string ToString()
        {
            if (Width is not null)
            {
                return $"{Width}x{Height}";
            }

            return $"{Height}p";

        }

        public override bool Equals(object? obj)
        {
            return obj is ResolutionInfo info &&
                   Width == info.Width &&
                   Height == info.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }
    }
}