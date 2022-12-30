using Clickwheel.Parsers.Artwork;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;

namespace Clickwheel.Tests.Parsers.Artwork
{
    [TestClass]
    public class ArtworkHelperTest
    {
        [TestMethod]
        [DeploymentItem(@"Fixtures/sample-image-red.png")]
        [DeploymentItem(@"Fixtures/sample-image-green.png")]
        [DeploymentItem(@"Fixtures/sample-image-blue.png")]
        public void TestGenerateResizedImage()
        {
            using (var image = Image.Load("sample-image-red.png"))
            {
                var expected = new byte[]
                {
                    0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00,
                    0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8, 0x00, 0xf8,
                };
                var format = new SupportedArtworkFormat(9999, PixelFormat.Rgb565);
                var actual = ArtworkHelper.GenerateResizedImageBytes(image, format);
                CollectionAssert.AreEqual(expected, actual);
            }

            using (var image = Image.Load("sample-image-green.png"))
            {
                var expected = new byte[]
                {
                    0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07,
                    0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07, 0xe0, 0x07
                };

                var format = new SupportedArtworkFormat(9999, PixelFormat.Rgb565);
                var actual = ArtworkHelper.GenerateResizedImageBytes(image, format);
                CollectionAssert.AreEqual(expected, actual);
            }

            using (var image = Image.Load("sample-image-blue.png"))
            {
                var expected = new byte[]
                {
                    0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00,
                    0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00, 0x1f, 0x00
                };
                var format = new SupportedArtworkFormat(9999, PixelFormat.Rgb565);
                var actual = ArtworkHelper.GenerateResizedImageBytes(image, format);
                CollectionAssert.AreEqual(expected, actual);
            }
        }
    }
}
