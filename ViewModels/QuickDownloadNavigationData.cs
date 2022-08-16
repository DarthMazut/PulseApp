using Model;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class QuickDownloadNavigationData : AppNavigationData
    {
        private Task<byte[]?> _downloadingTask;

        public QuickDownloadNavigationData(VideoMetadata metadata)
        {
            Metadata = metadata;
            _downloadingTask = DownloadRawImageAsJpegCore(metadata.ThumbnailUrl);
        }

        public VideoMetadata Metadata { get; }

        public MediumSelection MediumSelection { get; set; }

        public FormatInfo? SelectedVideoFormat { get; set; }

        public Task<byte[]?> DownloadRawImageAsJpeg()
        {
            return _downloadingTask;
        }

        private static async Task<byte[]?> DownloadRawImageAsJpegCore(string url)
        {
            try
            {
                byte[] bytes = await WebHelper.DownloadUrlContentAsByteArray(url);
                Image image = await Task.Run(() => Image.Load(bytes));
                using MemoryStream outputStream = new();
                await image.SaveAsJpegAsync(outputStream);
                byte[] outputArray = outputStream.ToArray();

                return outputArray;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is ImageFormatException)
            {
                return null;
            }
        }
    }

    public enum MediumSelection
    {
        Video,
        Music
    }
}
