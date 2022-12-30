#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using Clickwheel.Exceptions;
using Clickwheel.Parsers.Artwork;

namespace Clickwheel.IPodDevice.FileSystems
{
    class XmlQueryDeviceInfo : IDeviceInfo
    {
        private readonly IPod _iPod;

        public Exception? ReadException { get; }

        internal XmlQueryDeviceInfo(IPod iPod)
        {
            _iPod = iPod;
        }

        private string SysInfoExtendedPath =>
            Path.Combine(_iPod.DriveLetter, "iPod_Control", "Device", "SysInfoExtended");

        private string ExtendedSysInfoPath =>
            Path.Combine(_iPod.DriveLetter, "iPod_Control", "Device", "ExtendedSysInfoXml");

        internal void Read()
        {
            if (File.Exists(SysInfoExtendedPath))
            {
                Trace.WriteLine("Using SysInfoExtended file");
                RawDeviceDescriptor = File.ReadAllText(SysInfoExtendedPath);
            }
            else if (File.Exists(ExtendedSysInfoPath))
            {
                Trace.WriteLine("Using ExtendedSysInfoXml file");
                RawDeviceDescriptor = File.ReadAllText(ExtendedSysInfoPath);
            }
            else
            {
                throw new ExtendedSysInfoNotFoundException();
            }

            if (RawDeviceDescriptor.Length == 0)
            {
                Trace.WriteLine("DeviceXml is empty");
                return;
            }

            try
            {
                var sysInfoXml = new XmlDocument
                {
                    XmlResolver = null
                };
                sysInfoXml.LoadXml(RawDeviceDescriptor);
                ParseDeviceXml(sysInfoXml);
            }
            catch (Exception ex)
            {
                DebugLogger.LogException(ex);
            }
        }

        /// <summary>
        /// FirewireId of the iPod
        /// </summary>
        public string? FirewireId { get; private set; }

        /// <summary>
        /// Serial number of the iPod
        /// </summary>
        public string? SerialNumber { get; private set; }

        /// <summary>
        /// Tries to return the FamilyId as an IPodFamily enum
        /// </summary>
        public IPodFamily Family
        {
            get
            {
                if (Enum.IsDefined(typeof(IPodFamily), FamilyId))
                {
                    return (IPodFamily)FamilyId;
                }
                else
                {
                    return IPodFamily.Unknown;
                }
            }
        }

        /// <summary>
        /// Returns the FamilyId as an integer reported by the iPod
        /// </summary>
        public int FamilyId { get; private set; }

        /// <summary>
        /// List of supported artwork sizes for the iPod
        /// </summary>
        public List<SupportedArtworkFormat> SupportedArtworkFormats { get; } = new();

        /// <summary>
        /// List of supported photo sizes for the iPod
        /// </summary>
        public List<SupportedArtworkFormat> SupportedPhotoFormats { get; } = new();

        /// <summary>
        /// Returns the exact data the iPod returned to Clickwheel.
        /// </summary>
        public string? RawDeviceDescriptor { get; private set; }

        public string? OSVersion { get; private set; }

        private void ParseDeviceXml(XmlDocument document)
        {
            var node = document.SelectSingleNode(
                "/plist/dict/key[text()='SerialNumber']/following-sibling::*[1]"
            );
            if (node != null) {
                SerialNumber = node.InnerText;
            }

            node = document.SelectSingleNode(
                "/plist/dict/key[text()='FireWireGUID']/following-sibling::*[1]"
            );
            if (node != null) {
                FirewireId = node.InnerText;
                Trace.WriteLine("iPod FirewireId: " + FirewireId);
            }

            node = document.SelectSingleNode(
                "/plist/dict/key[text()='FamilyID']/following-sibling::*[1]"
            );
            if (node != null) {
                FamilyId = int.Parse(node.InnerText, CultureInfo.InvariantCulture);
                Trace.WriteLine("iPod Family: " + FamilyId);
            }

            node = document.SelectSingleNode(
                "/plist/dict/key[text()='VisibleBuildID']/following-sibling::*[1]"
            );
            if (node != null) {
                OSVersion = node.InnerText;
                Trace.WriteLine("iPod OS Version: " + OSVersion);
            }

            if (FamilyId == (int)IPodFamily.iPod_Nano_Gen5)
            {
                // Nano 5G doesn't report supported formats :(
                SupportedArtworkFormats.Add(
                    new SupportedArtworkFormat(1056, PixelFormat.Rgb565, 128, 128)
                );
                SupportedArtworkFormats.Add(new SupportedArtworkFormat(1078, PixelFormat.Rgb565));
                SupportedArtworkFormats.Add(
                    new SupportedArtworkFormat(1073, PixelFormat.Rgb565, 240, 240)
                );
                SupportedArtworkFormats.Add(new SupportedArtworkFormat(1074, PixelFormat.Rgb565));

                SupportedPhotoFormats.Add(new SupportedArtworkFormat(1087, PixelFormat.Rgb565));
                SupportedPhotoFormats.Add(new SupportedArtworkFormat(1079, PixelFormat.Rgb565));
                SupportedPhotoFormats.Add(new SupportedArtworkFormat(1066, PixelFormat.Rgb565));
                return;
            }

            node = document.SelectSingleNode(
                "/plist/dict/key[text()='AlbumArt']/following-sibling::*[1]"
            );
            ReadArtworkNode(node, SupportedArtworkFormats, false);

            node = document.SelectSingleNode(
                "/plist/dict/key[text()='ImageSpecifications']/following-sibling::*[1]"
            );
            ReadArtworkNode(node, SupportedPhotoFormats, true);
        }

        private static string? GetNextSiblingInnerText(XmlNode? node)
        {
            return node?.NextSibling?.InnerText;
        }

        private static void ReadArtworkNode(
            XmlNode? node,
            List<SupportedArtworkFormat> artwork,
            bool useReportedSize
        )
        {
            if (node == null)
            {
                return;
            }

            var albumArtNodes = node.SelectNodes("dict");
            if (albumArtNodes == null)
            {
                return;
            }

            foreach (XmlNode albumArtNode in albumArtNodes)
            {
                var formatId = GetNextSiblingInnerText(albumArtNode.SelectSingleNode("key[text()='FormatId']"));
                var widthText = GetNextSiblingInnerText(albumArtNode.SelectSingleNode("key[text()='RenderWidth']"));
                var heightText = GetNextSiblingInnerText(albumArtNode.SelectSingleNode("key[text()='RenderHeight']"));
                if (formatId == null || widthText == null || heightText == null)
                {
                    return;
                }
                var pixelFormat = GetNextSiblingInnerText(albumArtNode.SelectSingleNode("key[text()='PixelFormat']"));
                if (pixelFormat == "4C353635")
                {
                    var width = uint.Parse(widthText, CultureInfo.InvariantCulture);
                    var height = uint.Parse(heightText, CultureInfo.InvariantCulture);
                    Trace.WriteLine($"Supported artwork format: {formatId} {width}x{height}, format {pixelFormat}");

                    try
                    {
                        if (!artwork.Exists(format => format.Width == width && format.Height == height))
                        {
                            if (useReportedSize)
                            {
                                artwork.Add(
                                    new SupportedArtworkFormat(
                                        uint.Parse(formatId, CultureInfo.InvariantCulture),
                                        PixelFormat.Rgb565,
                                        width,
                                        height
                                    )
                                );
                            }
                            else
                            {
                                artwork.Add(new SupportedArtworkFormat(uint.Parse(formatId, CultureInfo.InvariantCulture), PixelFormat.Rgb565));
                            }
                        }
                        else
                        {
                            Trace.WriteLine("Format ignored.");
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugLogger.LogException(ex);
                    }
                }
                else
                {
                    Trace.WriteLine($"Unknown artwork format: {formatId} {widthText}x{heightText} {pixelFormat}");
                }
            }
        }
    }
}
