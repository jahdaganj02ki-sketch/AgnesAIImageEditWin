using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AgnesAIImageEdit.Converters
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrEmpty(s))
            {
                try
                {
                    var bytes = System.Convert.FromBase64String(s);
                    var bmp = new BitmapImage();
                    using var ms = new MemoryStream(bytes);
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = ms;
                    bmp.EndInit();
                    bmp.Freeze();
                    return bmp;
                }
                catch { }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null!;
    }
}
