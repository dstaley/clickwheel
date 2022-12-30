using System;
using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    internal class IdGenerator
    {
        private int _lastTrackId;
        private long _lastDBId;
        private int _lastPodcastGroupId = 1;

        private IPod _iPod;

        public IdGenerator(IPod iPod)
        {
            _iPod = iPod;

            _lastTrackId = 0;

            for (var i = 0; i < _iPod.Tracks.Count; i++)
            {
                if (_iPod.Tracks[i].Id > _lastTrackId)
                {
                    _lastTrackId = _iPod.Tracks[i].Id;
                }

                if (_iPod.Tracks[i].DBId > _lastDBId)
                {
                    _lastDBId = _iPod.Tracks[i].DBId;
                }
            }

            var listHeader = _iPod.ITunesDB.DatabaseRoot.GetChildSection(
                MHSDSectionType.PlaylistsV2
            );
            if (listHeader != null)
            {
                var podcastsContainer = (PlaylistListV2Container)listHeader.GetListContainer();
                var podcastsList = podcastsContainer.GetPlaylistsList();

                var podcastsPlaylist = podcastsList.GetPlaylistByName("Podcasts");
                if (podcastsPlaylist != null)
                {
                    foreach (var item in podcastsPlaylist.Items())
                    {
                        if (item.GroupId > _lastPodcastGroupId)
                        {
                            _lastPodcastGroupId = item.GroupId;
                        }
                    }
                }
            }
        }

        public int GetNewTrackId()
        {
            _lastTrackId++;
            return _lastTrackId;
        }

        public long GetNewDBId()
        {
            _lastDBId++;
            return _lastDBId;
        }

        public string GetNewIPodFilePath(Track track, string fileExtension)
        {
            var r = new Random();
            var folderNumber = "F" + r.Next(49).ToString("00");

            string path;
            while (true)
            {
                if (track.MediaType == MediaType.Ringtone)
                {
                    path = Path.Combine(_iPod.FileSystem.IPodControlPath, "Ringtones");
                }
                else
                {
                    path = Path.Combine(
                        Path.Combine(_iPod.FileSystem.IPodControlPath, "Music"),
                        folderNumber
                    );
                }
                path = Path.Combine(path, GetNewRandomFileName() + fileExtension);
                if (!_iPod.FileSystem.FileExists(path))
                {
                    break;
                }
            }

            return path.Substring(_iPod.DriveLetter.Length);
        }

        private string GetNewRandomFileName()
        {
            var path = "";
            var r = new Random();
            for (var i = 0; i < 4; i++)
            {
                var c = (char)r.Next(65, 90);
                var s = Convert.ToString(c);
                path += s;
            }
            path = "SP" + path;
            return path;
        }

        public uint GetNewArtworkId()
        {
            var newArtworkId = _iPod.ArtworkDB.NextImageId;
            _iPod.ArtworkDB.NextImageId++;
            return newArtworkId;
        }

        public int GetNewPodcastGroupId()
        {
            _lastPodcastGroupId++;
            return _lastPodcastGroupId;
        }
    }
}
