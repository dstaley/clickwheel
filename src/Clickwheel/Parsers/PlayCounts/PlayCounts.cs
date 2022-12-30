using System.Diagnostics;
using System.IO;
using Clickwheel.DataTypes;

namespace Clickwheel.Parsers.PlayCounts
{
    class PlayCounts
    {
        Header _header;
        MusicDatabase _iTunesDB;

        public PlayCounts(MusicDatabase iTunesDB)
        {
            _iTunesDB = iTunesDB;
            var playCountsPath = _iTunesDB.iPod.FileSystem.PlayCountsPath;
            if (!_iTunesDB.iPod.FileSystem.FileExists(playCountsPath))
            {
                return;
            }

            var fs = _iTunesDB.iPod.FileSystem.OpenFile(playCountsPath, FileAccess.Read);
            var br = new BinaryReader(fs);
            if (br.BaseStream.Length < 16)
            {
                br.Close();
                return;
            }

            _header = new Header();
            _header.Read(_iTunesDB.iPod, br);
            br.Close();
        }

        public void MergeChanges()
        {
            //If we didnt read the file, cant merge any changes.
            if (_header == null)
            {
                return;
            }

            if (_header.EntryCount != _iTunesDB.TracksList.Count)
            {
                return;
            }

            var currentIndex = 0;

            foreach (var entry in _header.Entries())
            {
                var track = _iTunesDB.TracksList[currentIndex];
                if (entry.PlayCount > 0)
                {
                    Debug.WriteLine("Updated playcount for " + track.Artist + " " + track.Title);
                    track.PlayCount += entry.PlayCount;
                    track.DateLastPlayed = new IPodDateTime(entry.DateLastPlayed);
                }
                if (track.Rating.StarRating != entry.Rating)
                {
                    track.Rating = new IPodRating(entry.Rating);
                }

                currentIndex++;
            }

            var playCountsPath = _iTunesDB.iPod.FileSystem.PlayCountsPath;
            _iTunesDB.iPod.FileSystem.DeleteFile(playCountsPath);
        }
    }
}
