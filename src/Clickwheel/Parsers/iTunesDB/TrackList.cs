using System;
using System.Collections.Generic;
using System.IO;
using Clickwheel.Exceptions;

namespace Clickwheel.Parsers.iTunesDB
{
    // Implements a MHLT entry in iTunesDB
    /// <summary>
    /// List of iPod tracks. This is where tracks are added/removed from the iPod.
    /// </summary>
    public class TrackList : BaseDatabaseElement
    {
        private int _trackCount;
        private List<Track> _childSections;
        private bool _isDirty;

        internal TrackList()
        {
            _requiredHeaderSize = 12;
            _childSections = new List<Track>();
        }

        #region IDatabaseElement Members

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhlt");

            _trackCount = reader.ReadInt32();

            this.ReadToHeaderEnd(reader);

            for (var i = 0; i < _trackCount; i++)
            {
                var track = new Track();
                track.Read(iPod, reader);
                _childSections.Add(track);
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

        /// <summary>
        /// Returns track matching specified Id. Returns null if no matching track found.
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public Track FindById(int trackId)
        {
            foreach (var trk in _childSections)
            {
                if (trk.Id == trackId)
                {
                    return trk;
                }
            }
            return null;
        }

        public Track Find(Predicate<Track> match)
        {
            return _childSections.Find(match);
        }

        public List<Track> FindAll(Predicate<Track> match)
        {
            return _childSections.FindAll(match);
        }

        /// <summary>
        /// Returns track matching specified DBId. Returns null if no matching track found.
        /// </summary>
        public Track FindByDBId(long dbId)
        {
            foreach (var trk in _childSections)
            {
                if (trk.DBId == dbId)
                {
                    return trk;
                }
            }
            return null;
        }

        /// <summary>
        /// Return the track at specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Track this[int index] => _childSections[index];

        /// <summary>
        /// Adds a new track to the iPod. Copies the file onto the iPod drive.
        /// Throws exception if track couldn't be added, otherwise returns a full Track object.
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public Track Add(NewTrack newItem)
        {
            _iPod.AssertIsWritable();

            if (!newItem.IsVideo.HasValue)
            {
                throw new ArgumentException("NewTrack.IsVideo property must be set");
            }

            var fileInfo = new FileInfo(newItem.FilePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(newItem.FilePath + " couldn't be found");
            }

            var existing = GetExistingTrack(newItem);
            if (existing != null)
            {
                throw new TrackAlreadyExistsException(
                    "A track with the same Title, Artist and Album already exists on your iPod",
                    existing
                );
            }

            var freespace = _iPod.FileSystem.AvailableFreeSpace;
            freespace -= fileInfo.Length;
            if (freespace <= 0)
            {
                throw new OutOfDiskSpaceException("Your iPod does not have enough free space.");
            }

            var track = new Track();
            track.Create(_iPod, newItem);

            //Try and actually copy the file onto the iPod drive
            var iPodFileName = Path.Combine(_iPod.DriveLetter, track.FilePath);

            //If the file is already in iPod's music folder, just move it to the new path, otherwise copy it
            if (
                newItem.FilePath.StartsWith(
                    _iPod.FileSystem.IPodControlPath,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                File.Move(newItem.FilePath, iPodFileName);
            }
            else
            {
                _iPod.FileSystem.CopyFileToDevice(newItem.FilePath, iPodFileName);
            }

            if (!string.IsNullOrEmpty(newItem.ArtworkFile))
            {
                try
                {
                    track.SetArtwork(newItem.ArtworkFile);
                }
                catch (Exception ex)
                {
                    DebugLogger.LogException(ex);
                }
            }

            _childSections.Add(track);

            //Add new track to Master iPod Playlist
            _iPod.Playlists.GetMasterPlaylist().AddTrack(track, -1, true);
            return track;
        }

        /// <summary>
        /// Deletes a track from the iPod. The actual file is also deleted.
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public bool Remove(Track track)
        {
            _iPod.AssertIsWritable();

            //Try and actually remove the file from the iPod drive
            var iPodFileName = Path.Combine(_iPod.DriveLetter, track.FilePath);
            if (_iPod.FileSystem.FileExists(iPodFileName))
            {
                _iPod.FileSystem.DeleteFile(iPodFileName);
            }

            _iPod.ArtworkDB.RemoveArtwork(track);

            _childSections.Remove(track);
            _iPod.Playlists.RemoveTrackFromAllPlaylists(track);
            _iPod.Session.DeletedTracks.Add(track);
            _isDirty = true;
            return true;
        }

        /// <summary>
        /// Returns true if this tracklist contains the specified track.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Track item)
        {
            return _childSections.Contains(item);
        }

        /// <summary>
        /// Returns index of specified track, if track doesnt exist, returns -1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetTrackIndex(Track item)
        {
            for (var i = 0; i < _childSections.Count; i++)
            {
                if (_childSections[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Number of tracks in this list.
        /// </summary>
        public int Count => _childSections.Count;

        /// <summary>
        /// Returns an enumerator for each track in this list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Track> GetEnumerator()
        {
            foreach (var track in _childSections)
            {
                yield return track;
            }
        }

        /// <summary>
        /// Returns a track with matching Title, artist, album, tracknumber.
        /// If no existing track is found, return null;
        /// </summary>
        /// <param name="newTrack"></param>
        /// <returns></returns>
        private Track GetExistingTrack(NewTrack newTrack)
        {
            foreach (var existing in _childSections)
            {
                if (
                    existing.Title == newTrack.Title
                    && existing.Artist == newTrack.Artist
                    && existing.Album == newTrack.Album
                    && existing.TrackNumber == newTrack.TrackNumber
                )
                {
                    return existing;
                }
            }

            return null;
        }

        internal bool IsDirty => _isDirty;
    }
}
