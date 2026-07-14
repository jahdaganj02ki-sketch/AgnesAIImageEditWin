using System;
using System.IO;
using System.Windows.Media.Imaging;
using AgnesAIImageEdit.Services;
using Xunit;

namespace AgnesAIImageEdit.Tests.Services
{
    public class ImageHelperTests
    {
        [Fact]
        public void LoadBitmapImage_ValidPath_ReturnsFrozenBitmapImage()
        {
            var tempFile = CreateTestPng();

            try
            {
                var bmp = ImageHelper.LoadBitmapImage(tempFile);

                Assert.NotNull(bmp);
                Assert.True(bmp.IsFrozen);
                Assert.Equal(100, bmp.PixelWidth);
                Assert.Equal(100, bmp.PixelHeight);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void BytesToBitmap_ValidPngBytes_ReturnsFrozenBitmapImage()
        {
            var tempFile = CreateTestPng();
            var bytes = File.ReadAllBytes(tempFile);

            try
            {
                var bmp = ImageHelper.BytesToBitmap(bytes);

                Assert.NotNull(bmp);
                Assert.True(bmp.IsFrozen);
                Assert.Equal(100, bmp.PixelWidth);
                Assert.Equal(100, bmp.PixelHeight);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void FileToDataUri_ValidImage_ReturnsBase64DataUri()
        {
            var tempFile = CreateTestPng();

            try
            {
                var dataUri = ImageHelper.FileToDataUri(tempFile, 2048);

                Assert.StartsWith("data:image/png;base64,", dataUri);
                Assert.NotEmpty(dataUri.Substring("data:image/png;base64,".Length));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void EncodePng_BitmapSource_ReturnsPngBytes()
        {
            var tempFile = CreateTestPng();
            var src = ImageHelper.LoadBitmapImage(tempFile);

            try
            {
                var bytes = ImageHelper.EncodePng(src);

                Assert.NotNull(bytes);
                Assert.NotEmpty(bytes);
                Assert.Equal(0x89, bytes[0]);
                Assert.Equal(0x50, bytes[1]);
                Assert.Equal(0x4E, bytes[2]);
                Assert.Equal(0x47, bytes[3]);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        private string CreateTestPng()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"test_image_{Guid.NewGuid()}.png");
            var bmp = new WriteableBitmap(100, 100, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using var fs = new FileStream(tempFile, FileMode.Create);
            encoder.Save(fs);
            return tempFile;
        }
    }
}