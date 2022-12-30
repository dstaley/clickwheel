using System;
using System.Collections.Generic;
using System.IO;
using Clickwheel.Exceptions;

namespace Clickwheel.Parsers.iTunesDB
{
    // Implements a MHLP entry in iTunesDB
    /// <summary>
    /// Class holding all playlists on the iPod
    /// </summary>
    public class PlaylistList : BaseDatabaseElement
    {
        private int _dataObjectCount;
        private List<Playlist> _childSections;
        private bool _isDirty;

        internal PlaylistList()
        {
            _requiredHeaderSize = 12;
            _childSections = new List<Playlist>();
        }

        #region IDatabaseElement Members

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhlp");

            _dataObjectCount = reader.ReadInt32();

            this.ReadToHeaderEnd(reader);

            for (var i = 0; i < _dataObjectCount; i++)
            {
                var playlist = new Playlist();
                playlist.Read(iPod, reader);
                _childSections.Add(playlist);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_childSections.Count);
            writer.Write(_unusedHeader);

            for (var i = 0; i < _childSections.Count; i++)
            {
                _childSections[i].Write(writer);
            }
            _isDirty = false;
        }

        internal override int GetSectionSize()
        {
            var size = _headerSize;
            for (var i = 0; i < _childSections.Count; i++)
            {
                size += _childSections[i].GetSectionSize();
            }
            return size;
        }

        #endregion


        internal void ResolveTracks()
        {
            foreach (var p in _childSections)
            {
                p.ResolveTracks(_iPod);
                p.UpdateSummaryData();
            }
        }

        internal void RemoveTrackFromAllPlaylists(Track track)
        {
            foreach (var p in _childSections)
            {
                p.RemoveTrack(track, true);
            }
        }

        public Playlist this[int index] => _childSections[index];

        /// <summary>
        /// Returns the playlist with the given name. If no playlists match, returns null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Playlist GetPlaylistByName(string name)
        {
            foreach (var p in _childSections)
            {
                if (p.Name == name)
                {
                    return p;
                }
            }
            return null;
        }

        internal Playlist GetPlaylistById(ulong id)
        {
            foreach (var pl in _childSections)
            {
                if (pl.Id == id)
                {
                    return pl;
                }
            }
            return null;
        }

        internal Playlist GetMasterPlaylist()
        {
            foreach (var p in _childSections)
            {
                if (p.IsMaster)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a new playlist with the specified name and returns the new playlist.
        /// If there is already a playlist with the same name OperationNotAllowedException is thrown
        /// </summary>
        /// <param name="playlistName"></param>
        /// <returns>Playlist</returns>
        public Playlist Add(string playlistName)
        {
            _iPod.AssertIsWritable();

            if (GetPlaylistByName(playlistName) != null)
            {
                throw new OperationNotAllowedException(
                    "There is already a playlist called '" + playlistName + "' on your iPod"
                );
            }

            var newPlaylist = new Playlist(_iPod);
            newPlaylist.Name = playlistName;
            if (playlistName == "Podcasts")
            {
                newPlaylist.IsPodcastPlaylist = true;
            }
            newPlaylist.Id = (ulong)new Random().Next();
            newPlaylist.UpdateSummaryData();
            _childSections.Add(newPlaylist);

            _isDirty = true;
            return newPlaylist;
        }

        /// <summary>
        /// Returns true if the iPod contains the specified playlist.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Playlist item)
        {
            return _childSections.Contains(item);
        }

        /// <summary>
        /// How many playlists are on the iPod.
        /// </summary>
        public int Count => _childSections.Count;

        /// <summary>
        /// Remove a playlist from the iPod, optionally deleting contained tracks at the same time.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="deleteTracks"></param>
        /// <returns></returns>
        public bool Remove(Playlist item, bool deleteTracks)
        {
            return Remove(item, deleteTracks, false);
        }

        internal bool Remove(Playlist item, bool deleteTracks, bool skipChecks)
        {
            _iPod.AssertIsWritable();

            if (!skipChecks)
            {
                if (item.IsMaster)
                {
                    throw new OperationNotAllowedException("You cannot remove the Master Playlist");
                }
            }

            if (deleteTracks)
            {
                var temp = new List<Track>();
                foreach (var track in item.Tracks)
                {
                    temp.Add(track);
                }
                foreach (var track in temp)
                {
                    _iPod.Tracks.Remove(track);
                }
            }

            _childSections.Remove(item);
            _iPod.Session.DeletedPlaylists.Add(item);
            _isDirty = true;

            return true;
        }

        public IEnumerator<Playlist> GetEnumerator()
        {
            foreach (var playlist in _childSections)
            {
                yield return playlist;
            }
        }

        /// <summary>
        /// To deal with different versions of iPods, there are two separate versions of the Playlists list: one with
        /// Podcasts as a normal playlist, and the other with Podcasts as a special list. Clickwheel syncs the two
        /// versions here.
        /// </summary>
        /// <param name="otherList"></param>
        internal void FollowChanges(PlaylistList otherList)
        {
            //Add new playlists from otherList to me
            foreach (var otherPlaylist in otherList)
            {
                if (GetPlaylistById(otherPlaylist.Id) == null)
                {
                    _childSections.Add(otherPlaylist);
                }
            }

            for (var count = _childSections.Count - 1; count >= 0; count--)
            {
                var thisPlaylist = _childSections[count];
                var otherPlaylist = otherList.GetPlaylistById(thisPlaylist.Id);

                if (otherPlaylist == null)
                {
                    this.Remove(thisPlaylist, false, true);
                }
                else
                {
                    if (!thisPlaylist.IsPodcastPlaylist)
                    {
                        _childSections[count] = otherPlaylist;
                    }
                    else
                    {
                        thisPlaylist.ResolveTracks(_iPod);
                        var podcastsAdapter = new PodcastListAdapter(_iPod, thisPlaylist);
                        podcastsAdapter.FollowChanges(otherPlaylist);
                    }
                }
            }
        }

        internal bool IsDirty => _isDirty;
    }
}
