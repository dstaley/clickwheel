using System;
using System.Collections.Generic;
using Clickwheel.Exceptions;

namespace Clickwheel.Parsers.Artwork
{
    public enum PixelFormat : int
    {
        Unknown = -1,
        Rgb565,
        Rgb565BE,
        IYUV
    }

    /// <summary>
    /// Represents an Artwork format supported by an iPod. See IPod.DeviceInfo.SupportedArtworkFormats.
    /// </summary>
    public class SupportedArtworkFormat
    {
        private uint _formatId;
        private uint _width,
            _height,
            _iThmbBlockSize;
        private PixelFormat _pixelFormat;
        internal bool VideoOnly { get; set; }

        internal SupportedArtworkFormat(
            uint formatId,
            PixelFormat pixelFormat,
            uint iThmbBlockLength
        ) : this(formatId, pixelFormat)
        {
            _iThmbBlockSize = iThmbBlockLength;
        }

        internal SupportedArtworkFormat(uint formatId, PixelFormat pixelFormat)
        {
            _formatId = formatId;
            _pixelFormat = pixelFormat;

            GetArtworkDimensions(formatId, out _width, out _height);
        }

        internal SupportedArtworkFormat(
            uint formatId,
            PixelFormat pixelFormat,
            uint width,
            uint height
        )
        {
            _formatId = formatId;
            _pixelFormat = pixelFormat;
            _width = width;
            _height = height;
        }

        public uint FormatId => _formatId;

        public uint Height => _height;

        public uint Width => _width;

        internal uint IThmbBlockSize => _iThmbBlockSize;

        public PixelFormat PixelFormat => _pixelFormat;

        public override string ToString()
        {
            return $"{_width}x{_height} {_pixelFormat.ToString()}";
        }

        internal static SupportedArtworkFormat GetByFormatId(
            uint formatId,
            List<SupportedArtworkFormat> formats
        )
        {
            var format = formats.Find(
                delegate(SupportedArtworkFormat testformat)
                {
                    return testformat.FormatId == formatId;
                }
            );
            return format;
        }

        internal static void GetArtworkDimensions(uint formatId, out uint width, out uint height)
        {
            switch (formatId)
            {
                case 1: //full resolution image
                    width = 1000;
                    height = 1000;
                    break;
                case 1009:
                    width = 42;
                    height = 30;
                    break;
                case 1013:
                    width = 220;
                    height = 176;
                    break;
                case 1015:
                    width = 130;
                    height = 88;
                    break;
                case 1016:
                    width = 140;
                    height = 140;
                    break;
                case 1017:
                    width = 56;
                    height = 56;
                    break;
                case 1019:
                    width = 720;
                    height = 480;
                    break;
                case 1023:
                    width = 176;
                    height = 132;
                    break;
                case 1024:
                    width = 320;
                    height = 240;
                    break;
                case 1027:
                    width = 100;
                    height = 100;
                    break;
                case 1028:
                    width = 100;
                    height = 100;
                    break;
                case 1029:
                    width = 200;
                    height = 200;
                    break;
                case 1031:
                    width = 42;
                    height = 42;
                    break;
                case 1032:
                    width = 42;
                    height = 37;
                    break;
                case 1036:
                    width = 50;
                    height = 41;
                    break;
                case 1055:
                    width = 128;
                    height = 128;
                    break;
                case 1056:
                    width = 80;
                    height = 80;
                    break;
                case 1060:
                    width = 320;
                    height = 320;
                    break;
                case 1061:
                    width = 56;
                    height = 55;
                    break;
                case 1062:
                    width = 56;
                    height = 56;
                    break;
                case 1066:
                    width = 64;
                    height = 64;
                    break;
                case 1068:
                    width = 128;
                    height = 128;
                    break;
                case 1071:
                    width = 240;
                    height = 240;
                    break;
                case 1073:
                    width = 50;
                    height = 50;
                    break;
                case 1074:
                    width = 50;
                    height = 50;
                    break;
                case 1078:
                    width = 80;
                    height = 80;
                    break;
                case 1079:
                    width = 80;
                    height = 80;
                    break;
                case 1081:
                    width = 640;
                    height = 480;
                    break;
                case 1084:
                    width = 240;
                    height = 240;
                    break;
                case 1087:
                    width = 384;
                    height = 384;
                    break;
                case 3001:
                    width = 256;
                    height = 256;
                    break;
                case 3002:
                    width = 128;
                    height = 128;
                    break;
                case 3003:
                    width = 64;
                    height = 64;
                    break;
                case 3004:
                    width = 56;
                    height = 55;
                    break;
                case 3005:
                    width = 320;
                    height = 320;
                    break;
                case 3006:
                    width = 55;
                    height = 55;
                    break;
                case 3007:
                    width = 88;
                    height = 88;
                    break;
                case 3008:
                    width = 640;
                    height = 480;
                    break;
                case 3009:
                    width = 160;
                    height = 120;
                    break;
                case 3011:
                    width = 80;
                    height = 79;
                    break;
                case 9999: // for testing purposes
                    width = 4;
                    height = 4;
                    break;

                default:
                    throw new UnsupportedArtworkFormatException(formatId);
            }
        }
    }
}
