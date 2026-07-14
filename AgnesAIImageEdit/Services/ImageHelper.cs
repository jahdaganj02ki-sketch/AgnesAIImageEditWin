using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AgnesAIImageEdit.Services
{
    public static class ImageHelper
    {
        public static string FileToDataUri(string path, int maxEdge = 2048)
        {
            var src = LoadBitmapImage(path);
            BitmapSource toEncode = src;
            if (src.PixelWidth > maxEdge || src.PixelHeight > maxEdge)
            {
                double scale = (double)maxEdge / Math.Max(src.PixelWidth, src.PixelHeight);
                toEncode = new TransformedBitmap(src, new ScaleTransform(scale, scale));
            }
            var bytes = EncodePng(toEncode);
            return "data:image/png;base64," + Convert.ToBase64String(bytes);
        }

        public static BitmapImage LoadBitmapImage(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        public static BitmapImage BytesToBitmap(byte[] pngBytes)
        {
            var bmp = new BitmapImage();
            using var ms = new MemoryStream(pngBytes);
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }

        public static byte[] EncodePng(BitmapSource source)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using var ms = new MemoryStream();
            encoder.Save(ms);
            return ms.ToArray();
        }

        public static string? MakeThumbBase64(byte[] pngBytes, int maxEdge = 256)
        {
            try
            {
                var src = BytesToBitmap(pngBytes);
                BitmapSource outp = src;
                if (src.PixelWidth > maxEdge || src.PixelHeight > maxEdge)
                {
                    double scale = (double)maxEdge / Math.Max(src.PixelWidth, src.PixelHeight);
                    outp = new TransformedBitmap(src, new ScaleTransform(scale, scale));
                }
                return Convert.ToBase64String(EncodePng(outp));
            }
            catch { return null; }
        }
    }
}
