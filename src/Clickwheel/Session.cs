using System.Collections.Generic;
using System.IO;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel
{
    class Session
    {
        IPod _iPod;
        public List<Track> DeletedTracks { get; set; }
        public List<Playlist> DeletedPlaylists { get; set; }

        public Session(IPod iPod)
        {
            _iPod = iPod;
            DeletedTracks = new List<Track>();
            DeletedPlaylists = new List<Playlist>();

            //clear out any per-session files for this iPod
            if (Directory.Exists(TempFilesPath))
            {
                Directory.Delete(TempFilesPath, true);
            }

            Directory.CreateDirectory(TempFilesPath);
        }

        /// <summary>
        /// Folder used for storing per-session temporary files
        /// </summary>
        public string TempFilesPath =>
            Path.Combine(
                Path.GetTempPath(),
                "Clickwheel",
                "Sessions",
                _iPod.DeviceInfo.SerialNumber
            );

        public void Clear()
        {
            DeletedPlaylists.Clear();
            DeletedTracks.Clear();
        }
    }
}
