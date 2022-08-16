using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;

namespace PulseApp.Converters
{
    public class RawImageToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is byte[] bytes)
            {
                BitmapImage bitmapImage = new();
                bitmapImage.SetSource(new MemoryStream(bytes).AsRandomAccessStream());
                return bitmapImage;

            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
