using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Clickwheel.Parsers.Artwork;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Tests
{
    public class Utilities
    {
        public static void InflateFixture(string archive)
        {
            ZipFile.ExtractToDirectory(archive, Directory.GetCurrentDirectory(), true);
        }

        public static string DumpTrack(Track track)
        {
            var properties = new string[]
            {
                track.Title,
                track.Artist,
                track.Album,
                track.AlbumArtist,
                track.Composer,
                track.Genre,
                track.Year.ToString(),
                track.TrackNumber.ToString(),
                track.AlbumTrackCount.ToString(),
                track.DiscNumber.ToString(),
                track.TotalDiscCount.ToString(),
                track.IsCompilation.ToString(),
                track.Rating.ToString(),
                track.PlayCount.ToString(),
                track.Comment,
                track.SortAlbum,
                track.SortArtist,
                track.SortTitle,
                track.FileType,
                track.Bitrate.ToString(),
                track.SampleRate.ToString(),
                track.Length.ToString(),
                track.FileSize.ToString(),
                track.DateAdded.DateTime.ToString("o"),
                track.IsVideo.ToString(),
                track.DateLastModified.DateTime.ToString("o"),
                track.VolumeAdjustment.ToString(),
                track.PodcastFlag.ToString(),
                track.IsNew.ToString()
            };
            return string.Join("\t", properties);
        }

        public static string DumpTracks(IEnumerable<Track> tracks)
        {
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb)
            {
                NewLine = "\n"
            };
            foreach (var t in tracks)
            {
                sw.WriteLine(DumpTrack(t));
            }

            return sb.ToString();
        }

        public static string DumpTracks(TrackList tracks)
        {
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb)
            {
                NewLine = "\n"
            };
            foreach (var t in tracks)
            {
                sw.WriteLine(DumpTrack(t));
            }

            return sb.ToString();
        }

        public static string DumpPlaylist(Playlist playlist)
        {
            var properties = new string[]
            {
                playlist.Name,
                playlist.LengthSummary,
                playlist.SizeSummary,
                playlist.IsMaster.ToString(),
                playlist.ItemCount.ToString(),
                playlist.TrackCount.ToString()
            };
            var trackList = DumpTracks(playlist.Tracks);
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb)
            {
                NewLine = "\n"
            };
            sw.WriteLine(string.Join("\t", properties));
            sw.WriteLine(trackList);
            return sb.ToString();
        }

        public static string DumpPlaylists(PlaylistList playlists)
        {
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb)
            {
                NewLine = "\n"
            };
            foreach (var p in playlists)
            {
                sw.WriteLine(DumpPlaylist(p));
            }

            return sb.ToString();
        }

        public static string DumpArtworkFormat(SupportedArtworkFormat format)
        {
            var properties = new string[]
            {
                format.ToString(),
                format.Height.ToString(),
                format.Width.ToString(),
                format.FormatId.ToString(),
                format.PixelFormat.ToString()
            };
            return string.Join("\t", properties);
        }

        public static string DumpArtworkFormats(List<SupportedArtworkFormat> artworkFormats)
        {
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb)
            {
                NewLine = "\n"
            };
            foreach (var f in artworkFormats)
            {
                sw.WriteLine(DumpArtworkFormat(f));
            }

            return sb.ToString();
        }

        public static string DumpIpod(IPod ipod)
        {
            var properties = new string[]
            {
                ipod.DatabaseVersion.ToString(),
                ipod.DeviceInfo.FirewireId,
                ipod.DeviceInfo.SerialNumber,
                ipod.DeviceInfo.OSVersion,
                ipod.DeviceInfo.Family.ToString(),
                ipod.DeviceInfo.FamilyId.ToString(),
            };

            var sb = new StringBuilder();
            using var sw = new StringWriter(sb)
            {
                NewLine = "\n"
            };
            sw.WriteLine(string.Join("\t", properties));
            sw.WriteLine(DumpArtworkFormats(ipod.DeviceInfo.SupportedArtworkFormats));
            sw.WriteLine(DumpArtworkFormats(ipod.DeviceInfo.SupportedPhotoFormats));
            return sb.ToString();
        }

        public static string DumpAlbum(ImageAlbum album)
        {
            var properties = new string[]
            {
                album.Title,
                album.ImageCount.ToString()
            };
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb)
            {
                NewLine = "\n"
            };
            sw.WriteLine(string.Join("\t", properties));
            foreach (var photo in album.Images)
            {
                var imageProperties = new string[]
                {
                    photo.Id.ToString(),
                    photo.LargestFormat.FileName,
                    photo.LargestFormat.Height.ToString(),
                    photo.LargestFormat.Width.ToString(),
                    photo.LargestFormat.FormatId.ToString(),
                    photo.LargestFormat.IsFullResolution.ToString(),
                    photo.SmallestFormat.FileName,
                    photo.SmallestFormat.Height.ToString(),
                    photo.SmallestFormat.Width.ToString(),
                    photo.SmallestFormat.FormatId.ToString(),
                    photo.SmallestFormat.IsFullResolution.ToString()
                };
                sw.WriteLine(string.Join("\t", imageProperties));
            }

            return sb.ToString();
        }

        public static string DumpPhotos(ImageAlbumList photos)
        {
            var sb = new StringBuilder();
            using var sw = new StringWriter(sb)
            {
                NewLine = "\n"
            };
            foreach (var album in photos.Albums)
            {
                sw.WriteLine(DumpAlbum(album));
            }

            return sb.ToString();
        }

        public static void PrintByteArray(ReadOnlySpan<byte> bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.AppendFormat("0x{0:x2}, ", b);
            }
            sb.Append('}');
            Console.WriteLine(sb.ToString());
        }
    }
}