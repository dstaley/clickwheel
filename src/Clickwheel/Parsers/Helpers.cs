using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Clickwheel.Parsers
{
    public enum CompatibilityType
    {
        Unknown,
        Compatible,
        NotWritable,
        SourceDoesntMatchOutput,
        UnsupportedNewDeviceOrFirmware
    }

    /// <summary>
    /// Static class containing some helpful functions.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Returns a DateTime from an iPod-format timestamp
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromTimeStamp(uint timestamp)
        {
            var origin = new DateTime(1904, 1, 1);
            return origin.AddSeconds(timestamp);
        }

        /// <summary>
        /// Returns iPod-format timestamp from specified DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static uint GetTimeStampFromDate(DateTime date)
        {
            var origin = new DateTime(1904, 1, 1);
            var ts = new TimeSpan(date.Ticks - origin.Ticks);
            return (uint)ts.TotalSeconds;
        }

        /// <summary>
        /// Replaces all ":" characters with "\"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string iPodPathToStandardPath(string path)
        {
            return path.Replace(':', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Replaces all "\" characters with ":"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StandardPathToiPodPath(string path)
        {
            return path.Replace('\\', ':').Replace('/', ':');
        }

        /// <summary>
        /// Returns iTunesSD int format (big-endian 3 byte)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] IntToITunesSDFormat(int value)
        {
            var bytes = new byte[3];
            bytes[0] = (byte)(value >> 16);
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value);
            return bytes;
        }

        /// <summary>
        /// Returns hh:mm:ss string from specified number of seconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetTimeString(long seconds)
        {
            var d = TimeSpan.FromSeconds(seconds);
            return $"{(int)d.TotalHours:00}:{d:mm}:{d:ss}";
        }

        /// <summary>
        /// Returns string describing the specified filesize. MB or GB will be displayed depending how large the number is
        /// </summary>
        /// <param name="fileSizeBytes"></param>
        /// <param name="decimalPoints"></param>
        /// <returns></returns>
        public static string GetFileSizeString(long fileSizeBytes, int decimalPoints)
        {
            var mbSize = (double)fileSizeBytes / 1048576;

            if (mbSize > 1024)
            {
                var gbSize = (double)fileSizeBytes / 1073741824;
                return $"{Math.Round(gbSize, 1, MidpointRounding.ToEven).ToString(CultureInfo.InvariantCulture)}GB";
            }
            else
            {
                return $"{Math.Round(mbSize, decimalPoints, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture)}MB";
            }
        }

        /// <summary>
        /// creates folders down to the given DirectoryInfo if they don't already exist
        /// </summary>
        public static void EnsureDirectoryExists(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        /// <summary>
        /// Can only be called before making changes to iPod, otherwise looses point
        /// Makes sure we can write back the iTunesDB file exactly as it was from our object model before making any changes.
        /// In some cases this will fail - eg If there are some invalid Star Ratings in the database Clickwheel will automatically reset them to 0
        /// then the write-back will be different...
        /// </summary>
        /// <returns></returns>
        internal static CompatibilityType TestCompatibility(
            string originalDBFile,
            string generatedDBFile
        )
        {
            FileStream f1 = null,
                f2 = null;
            try
            {
                f1 = new FileStream(generatedDBFile, FileMode.Open);
                f2 = new FileStream(originalDBFile, FileMode.Open);
            }
            catch (Exception) { }

            // Compare files
            int i,
                j;
            try
            {
                do
                {
                    i = f1.ReadByte();
                    j = f2.ReadByte();
                    if (i != j)
                    {
                        break;
                    }
                } while (i != -1 && j != -1);
            }
            catch (IOException ex)
            {
                DebugLogger.LogException(ex);
                return CompatibilityType.SourceDoesntMatchOutput;
            }
            finally
            {
                f1.Close();
                f2.Close();
                // File.Delete(generatedDBFile);
            }

            if (i != j)
            {
                // handle differing zlib compression efficiency
                using (var original = new FileStream(originalDBFile, FileMode.Open))
                using (var generated = new FileStream(generatedDBFile, FileMode.Open))
                {
                    var originalHeader = new byte[2];
                    var generatedHeader = new byte[2];
                    original.Seek(244, SeekOrigin.Begin);
                    generated.Seek(244, SeekOrigin.Begin);
                    original.Read(originalHeader, 0, 2);
                    generated.Read(generatedHeader, 0, 2);
                    var headersMatch = Enumerable.SequenceEqual(originalHeader, generatedHeader);

                    original.Seek(-4, SeekOrigin.End);
                    generated.Seek(-4, SeekOrigin.End);
                    var originalChecksum = new byte[4];
                    var generatedChecksum = new byte[4];
                    original.Read(originalChecksum, 0, 4);
                    generated.Read(generatedChecksum, 0, 4);
                    var checksumsMatch = Enumerable.SequenceEqual(
                        originalChecksum,
                        generatedChecksum
                    );

                    if (headersMatch && checksumsMatch)
                    {
                        return CompatibilityType.Compatible;
                    }
                }
                return CompatibilityType.SourceDoesntMatchOutput;
            }
            else
            {
                return CompatibilityType.Compatible;
            }
        }

        internal static string GetFileTypeDescription(FileInfo file)
        {
            var extension = file.Extension.ToLowerInvariant();
            switch (extension)
            {
                case ".mp4":
                case ".m4v":
                    return "MPEG-4 video file";
                case ".mp3":
                    return "MPEG audio file";
                case ".m4a":
                    return "AAC audio file";
                case ".wav":
                    return "Wav audio file";
                case ".m4r":
                    return "Ringtone";
                default:
                    return string.Empty;
            }
        }
    }
}
