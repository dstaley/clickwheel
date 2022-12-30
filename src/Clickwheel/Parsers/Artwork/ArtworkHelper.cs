using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Clickwheel.Parsers.Artwork
{
    class ArtworkHelper
    {
        public static byte[] GenerateResizedImageBytes<T>(
            Image originalImage,
            SupportedArtworkFormat format
        ) where T : unmanaged, IPixel<T>
        {
            using (var clone = originalImage.CloneAs<T>())
            {
                clone.Mutate(x => x.Resize((int)format.Width, (int)format.Height));
                var pixelArray = new byte[clone.Width * clone.Height * Unsafe.SizeOf<T>()];
                clone.CopyPixelDataTo(pixelArray);
                return pixelArray;
            }
        }

        public static byte[] GenerateResizedImageBytes(
            Image originalImage,
            SupportedArtworkFormat format
        )
        {
            switch (format.PixelFormat)
            {
                case PixelFormat.Rgb565:
                    return GenerateResizedImageBytes<Bgr565>(originalImage, format);
                default:
                    throw new Exception($"Unsupported pixel format: {format.PixelFormat}");
            }
        }
    }
}
