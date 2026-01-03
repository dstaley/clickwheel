using System;
using System.Collections.Generic;
using System.IO;
using Clickwheel.DataTypes;
using Clickwheel.Exceptions;
using Clickwheel.Parsers.Artwork;
using SixLabors.ImageSharp;

namespace Clickwheel.Parsers.iTunesDB
{
    /// <summary>
    /// Type of Media for a Track
    /// </summary>
    public enum MediaType
    {
        AudioAndVideo = 0x00000000,
        Audio = 0x00000001,
        Video = 0x00000002,
        Podcast = 0x00000004,
        VideoPodcast = 0x00000006,
        Audiobook = 0x00000008,
        MusicVideo = 0x00000020,
        TVShow = 0x00000040,
        TVAndMusic = 0x00000060,
        Ringtone = 16384
    }

    // Implements an MHIT section in iTunesDB file
    /// <summary>
    /// An iPod track. A track is either an Audio or Video file.
    /// </summary>
    public class Track : BaseDatabaseElement
    {
        private int _dataObjectCount;
        private int _id;
        private int _visible;
        private int _fileType;
        private byte[] _type;
        private byte _compilationFlag;
        private IPodRating _rating;
        private IPodDateTime _dateLastModified;
        private IPodTrackSize _fileSize;
        private IPodTrackLength _trackLength;
        private uint _trackNumber;
        private uint _albumTrackCount;
        private uint _year;
        private uint _bitrate;
        private uint _sampleRate;
        private int _volumeAdjustment;
        private byte[] _unk1;
        private int _playCount;
        private int _playCount2;
        private IPodDateTime _dateLastPlayed;
        private uint _discNumber;
        private uint _totalDiscCount;
        private int _userId;
        private IPodDateTime _dateAdded;
        private uint _bookmarkTime;
        private long _dbId;
        private bool _isChecked;
        private byte _unk2;
        private short _bpm;
        private short _artworkCount;
        private byte[] _unk3;
        private byte _hasArtwork;
        private bool _skipWhenShuffling;
        private bool _rememberPlaybackPosition;
        private bool _podcastFlag;
        private ulong _dbId2;
        bool _hasLyrics,
            _isVideoFile;
        private byte _playedMark;
        private byte _unk17;
        private byte[] _unk21;
        private int _pregap;
        private ulong _sampleCount;
        private byte[] _unk25;
        private int _postgap;
        private byte[] _unk27;
        private byte[] _unk4,
            _unk5;
        private int _mediaType;
        private uint _artworkIdLink;
        private short _hasGaplessData;

        private List<BaseMHODElement> _childSections;

        //Non-DB fields
        private bool _isDirty;
        private bool _isNew; //true if added this session
        private int _index; //index in trackslist
        private List<IPodImageFormat> _artwork = new List<IPodImageFormat>();

        internal Track()
        {
            _requiredHeaderSize = 258;

            _headerSize = 388;
            _identifier = "mhit".ToCharArray();
            _visible = 1;
            _rating = new IPodRating(0);
            _dateLastModified = new IPodDateTime(DateTime.Now);
            _fileSize = new IPodTrackSize(0);
            _trackLength = new IPodTrackLength(0);
            _dateLastPlayed = new IPodDateTime(0);
            _dateAdded = new IPodDateTime(DateTime.Now);
            _childSections = new List<BaseMHODElement>();
        }

        #region IDatabaseElement Members

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            var startOfElement = reader.BaseStream.Position;

            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhit");

            _sectionSize = reader.ReadInt32();
            _dataObjectCount = reader.ReadInt32();
            _id = reader.ReadInt32();
            _visible = reader.ReadInt32();
            _fileType = reader.ReadInt32();
            _type = reader.ReadBytes(2);
            _compilationFlag = reader.ReadByte();
            _rating = new IPodRating(reader.ReadByte());
            _dateLastModified = new IPodDateTime(reader.ReadUInt32());
            _fileSize = new IPodTrackSize(reader.ReadUInt32());
            _trackLength = new IPodTrackLength(reader.ReadUInt32());
            _trackNumber = reader.ReadUInt32();
            _albumTrackCount = reader.ReadUInt32();
            _year = reader.ReadUInt32();
            _bitrate = reader.ReadUInt32();
            _sampleRate = reader.ReadUInt32();
            _volumeAdjustment = reader.ReadInt32();
            _unk1 = reader.ReadBytes(12);
            _playCount = reader.ReadInt32();
            _playCount2 = reader.ReadInt32();
            _dateLastPlayed = new IPodDateTime(reader.ReadUInt32());
            _discNumber = reader.ReadUInt32();
            _totalDiscCount = reader.ReadUInt32();
            _userId = reader.ReadInt32();
            _dateAdded = new IPodDateTime(reader.ReadUInt32());
            _bookmarkTime = reader.ReadUInt32();
            _dbId = reader.ReadInt64();

            _isChecked = reader.ReadBoolean();
            _unk2 = reader.ReadByte();
            _bpm = reader.ReadInt16();
            _artworkCount = reader.ReadInt16();
            _unk3 = reader.ReadBytes(38);

            _hasArtwork = reader.ReadByte();
            _skipWhenShuffling = reader.ReadBoolean();
            _rememberPlaybackPosition = reader.ReadBoolean();
            _podcastFlag = reader.ReadBoolean();
            _dbId2 = reader.ReadUInt64();
            _hasLyrics = reader.ReadBoolean();
            _isVideoFile = reader.ReadBoolean();
            _playedMark = reader.ReadByte();
            _unk17 = reader.ReadByte();
            _unk21 = reader.ReadBytes(4);
            _pregap = reader.ReadInt32();
            _sampleCount = reader.ReadUInt64();
            _unk25 = reader.ReadBytes(4);
            _postgap = reader.ReadInt32();
            _unk27 = reader.ReadBytes(4);
            _unk4 = reader.ReadBytes(0);

            _mediaType = reader.ReadInt32();
            _unk5 = reader.ReadBytes(44);
            _hasGaplessData = reader.ReadInt16();

            if (_headerSize > 352)
            {
                var previousPostion = reader.BaseStream.Position;
                reader.BaseStream.Seek(startOfElement + 352, SeekOrigin.Begin);
                _artworkIdLink = reader.ReadUInt32();
                reader.BaseStream.Seek(previousPostion, SeekOrigin.Begin);
            }

            this.ReadToHeaderEnd(reader);

            for (var i = 0; i < _dataObjectCount; i++)
            {
                var mhod = MHODFactory.ReadMHOD(iPod, reader);
                _childSections.Add(mhod);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            var startOfElement = writer.BaseStream.Position;
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write(_childSections.Count);
            writer.Write(_id);
            writer.Write(_visible);
            writer.Write(_fileType);
            writer.Write(_type);
            writer.Write(_compilationFlag);
            writer.Write(_rating.ITunesRating);
            writer.Write(_dateLastModified.TimeStamp);
            writer.Write(_fileSize.ByteCount);
            writer.Write(_trackLength.MilliSeconds);
            writer.Write(_trackNumber);
            writer.Write(_albumTrackCount);
            writer.Write(_year);
            writer.Write(_bitrate);
            writer.Write(_sampleRate);
            writer.Write(_volumeAdjustment);
            writer.Write(_unk1);
            writer.Write(_playCount);
            writer.Write(_playCount2);
            writer.Write(_dateLastPlayed.TimeStamp);
            writer.Write(_discNumber);
            writer.Write(_totalDiscCount);
            writer.Write(_userId);
            writer.Write(_dateAdded.TimeStamp);
            writer.Write(_bookmarkTime);
            writer.Write(_dbId);

            writer.Write(_isChecked);
            writer.Write(_unk2);
            writer.Write(_bpm);
            writer.Write(_artworkCount);
            writer.Write(_unk3);
            writer.Write(_hasArtwork);
            writer.Write(_skipWhenShuffling);
            writer.Write(_rememberPlaybackPosition);
            writer.Write(_podcastFlag);
            writer.Write(_dbId2);
            writer.Write(_hasLyrics);
            writer.Write(_isVideoFile);
            writer.Write(_playedMark);
            writer.Write(_unk17);
            writer.Write(_unk21);
            writer.Write(_pregap);
            writer.Write(_sampleCount);
            writer.Write(_unk25);
            writer.Write(_postgap);
            writer.Write(_unk27);
            writer.Write(_unk4);
            writer.Write(_mediaType);
            writer.Write(_unk5);
            writer.Write(_hasGaplessData);

            writer.Write(_unusedHeader);

            if (_headerSize > 352)
            {
                var currentPosition = writer.BaseStream.Position;
                writer.BaseStream.Seek(startOfElement + 352, SeekOrigin.Begin);
                writer.Write(_artworkIdLink);
                writer.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
            }

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

        #region Properties

        public string Title
        {
            get => GetDataElement(MHODElementType.Title);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new OperationNotAllowedException("The Title cannot be empty");
                }
                SetDataElement(MHODElementType.Title, value);

                if (value.StartsWith("The "))
                {
                    SetDataElement(MHODElementType.TitleSortBy, value.Substring(4));
                }
                else if (value.StartsWith("A "))
                {
                    SetDataElement(MHODElementType.TitleSortBy, value.Substring(2));
                }
            }
        }

        public string Artist
        {
            get
            {
                var artist = GetDataElement(MHODElementType.Artist);
                if (artist == "")
                {
                    artist = "Unknown Artist";
                }

                return artist;
            }
            set
            {
                SetDataElement(MHODElementType.Artist, value);
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                if (value.StartsWith("The "))
                {
                    SetDataElement(MHODElementType.ArtistSortBy, value.Substring(4));
                }
                else if (value.StartsWith("A "))
                {
                    SetDataElement(MHODElementType.ArtistSortBy, value.Substring(2));
                }
            }
        }

        public string Album
        {
            get
            {
                var album = GetDataElement(MHODElementType.Album);
                if (album == "")
                {
                    album = "Unknown Album";
                }

                return album;
            }
            set
            {
                SetDataElement(MHODElementType.Album, value);
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                if (value.StartsWith("The "))
                {
                    SetDataElement(MHODElementType.AlbumSortBy, value.Substring(4));
                }
                else if (value.StartsWith("A "))
                {
                    SetDataElement(MHODElementType.AlbumSortBy, value.Substring(2));
                }
            }
        }

        public string Comment
        {
            get => GetDataElement(MHODElementType.Comment);
            set => SetDataElement(MHODElementType.Comment, value);
        }

        public string Composer
        {
            get => GetDataElement(MHODElementType.Composer);
            set => SetDataElement(MHODElementType.Composer, value);
        }

        public string AlbumArtist
        {
            get
            {
                var artist = GetDataElement(MHODElementType.AlbumArtist);
                if (artist == "")
                {
                    artist = Artist;
                }

                return artist;
            }
            set => SetDataElement(MHODElementType.AlbumArtist, value);
        }

        /// <summary>
        /// This is only used for Podcast tracks.
        /// </summary>
        public string DescriptionText
        {
            get => GetDataElement(MHODElementType.DescriptionText);
            set => SetDataElement(MHODElementType.DescriptionText, value);
        }

        /// <summary>
        /// Path to the actual file on the iPod drive. This does not contain the iPod drive letter.
        /// (e.g. "ipod_control\music\f00\1234.mp3")
        /// </summary>
        public string FilePath
        {
            get
            {
                var path = GetDataElement(MHODElementType.FilePath);
                if (path.StartsWith(":"))
                {
                    path = path.Substring(1);
                }

                return Helpers.iPodPathToStandardPath(path);
            }
            internal set
            {
                var path = Helpers.StandardPathToiPodPath(value);
                if (!path.StartsWith(":"))
                {
                    path = ":" + path;
                }

                SetDataElement(MHODElementType.FilePath, path);
                var mhod = (UnicodeMHOD)GetChildByType(MHODElementType.FilePath);
                mhod.Position = 1; //!HACK: Magic value, otherwise file wont play(?)
            }
        }

        public string FileType
        {
            get => GetDataElement(MHODElementType.FileType);
            set => SetDataElement(MHODElementType.FileType, value);
        }

        public string Genre
        {
            get
            {
                var genre = GetDataElement(MHODElementType.Genre);
                if (genre == "")
                {
                    genre = "Unknown Genre";
                }

                return genre;
            }
            set => SetDataElement(MHODElementType.Genre, value);
        }

        public int Id => _id;

        public long DBId
        {
            get => _dbId;
            internal set
            {
                _dbId = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// True if song is part of a Compilation. Will appear under Compilations on iPod if Settings > Compilations is turned on.
        /// </summary>
        public bool IsCompilation
        {
            get => _compilationFlag == 1;
            set
            {
                if (value)
                {
                    _compilationFlag = 1;
                }
                else
                {
                    _compilationFlag = 0;
                }

                _isDirty = true;
            }
        }

        public IPodRating Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                _isDirty = true;
            }
        }

        public IPodDateTime DateLastModified
        {
            get => _dateLastModified;
            set
            {
                _dateLastModified = value;
                _isDirty = true;
            }
        }

        public IPodTrackSize FileSize => _fileSize;

        public IPodTrackLength Length
        {
            get => _trackLength;
            set
            {
                _trackLength = value;
                _isDirty = true;
            }
        }

        public uint TrackNumber
        {
            get => _trackNumber;
            set
            {
                _trackNumber = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Number of tracks in the album
        /// </summary>
        public uint AlbumTrackCount
        {
            get => _albumTrackCount;
            set
            {
                _albumTrackCount = value;
                _isDirty = true;
            }
        }

        public uint Year
        {
            get => _year;
            set
            {
                _year = value;
                _isDirty = true;
            }
        }

        public uint Bitrate
        {
            get => _bitrate;
            set
            {
                _bitrate = value;
                _isDirty = true;
            }
        }

        public uint SampleRate => _sampleRate / 0x10000;

        public ulong SampleCount
        {
            get => _sampleCount;
            set
            {
                _sampleCount = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Volume Adjustment can be between -255 and 255. 0 is default. Use to make songs louder or quieter than normal.
        /// </summary>
        public int VolumeAdjustment
        {
            get => _volumeAdjustment;
            set
            {
                if (value < -255 || value > 255)
                {
                    throw new InvalidValueException(
                        "The Volume Adjustment field should be between -255 and 255"
                    );
                }

                _volumeAdjustment = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// How many times the track has been played
        /// </summary>
        public int PlayCount
        {
            get => _playCount;
            set
            {
                _playCount = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// When the track was last played
        /// </summary>
        public IPodDateTime DateLastPlayed
        {
            get => _dateLastPlayed;
            set
            {
                _dateLastPlayed = value;
                _isDirty = true;
            }
        }

        public uint DiscNumber
        {
            get => _discNumber;
            set
            {
                _discNumber = value;
                _isDirty = true;
            }
        }

        public uint TotalDiscCount
        {
            get => _totalDiscCount;
            set
            {
                _totalDiscCount = value;
                _isDirty = true;
            }
        }

        public IPodDateTime DateAdded
        {
            get => _dateAdded;
            set
            {
                _dateAdded = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Type of Media this track is. (Audio, Movie, TV Show etc)
        /// </summary>
        public MediaType MediaType
        {
            get => (MediaType)_mediaType;
            set
            {
                _mediaType = (int)value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Whether the iPod should resume or start again when playing this track.
        /// </summary>
        public bool RememberPlaybackPosition
        {
            get => _rememberPlaybackPosition;
            set
            {
                _rememberPlaybackPosition = value;
                _isDirty = true;
            }
        }

        public bool PodcastFlag
        {
            get => _podcastFlag;
            set => _podcastFlag = value;
        }

        public uint ArtworkIdLink
        {
            get => _artworkIdLink;
            set => _artworkIdLink = value;
        }

        /// <summary>
        /// List of all album art formats attached to this track.
        /// To add/remove artwork, use the SetArtwork() and RemoveArtwork() methods.
        /// </summary>
        public List<IPodImageFormat> Artwork => _artwork;

        /// <summary>
        /// Adds or updates the artwork associated with this track
        /// </summary>
        /// <param name="filename"></param>
        public void SetArtwork(string filename)
        {
            using (var image = Image.Load(filename))
            {
                SetArtwork(image);
            }
        }

        /// <summary>
        /// Adds or updates the artwork associated with this track
        /// </summary>
        public void SetArtwork(Image image)
        {
            _iPod.ArtworkDB.SetArtwork(this, image);
            _hasArtwork = 1;
            _artworkCount = 1;
            _isDirty = true;
        }

        /// <summary>
        /// Removes the artwork associated with this track
        /// </summary>
        public void RemoveArtwork()
        {
            _iPod.ArtworkDB.RemoveArtwork(this);
            _hasArtwork = 2; //no artwork
            _artworkCount = 0;
            _artworkIdLink = 0;
            _isDirty = true;
        }

        #endregion



        internal StringMHOD GetChildByType(int type)
        {
            for (var i = 0; i < _childSections.Count; i++)
            {
                if (_childSections[i] is StringMHOD && _childSections[i].Type == type)
                {
                    return (StringMHOD)_childSections[i];
                }
            }
            return null;
        }

        private string GetDataElement(int type)
        {
            var mhod = GetChildByType(type);
            if (mhod != null)
            {
                return mhod.Data;
            }
            else
            {
                return string.Empty;
            }
        }

        private void SetDataElement(int type, string data)
        {
            var mhod = GetChildByType(type);
            if (mhod != null)
            {
                if (string.IsNullOrEmpty(data))
                {
                    _childSections.Remove(mhod);
                    return;
                }
                mhod.Data = data;
            }
            else
            {
                if (string.IsNullOrEmpty(data))
                {
                    return;
                }

                StringMHOD newSection = new UnicodeMHOD(type);
                newSection.Data = data;
                _childSections.Add(newSection);
            }
            _isDirty = true;
        }

        public bool IsNew => _isNew;

        internal void Create(IPod iPod, NewTrack newTrack)
        {
            _iPod = iPod;

            if (string.IsNullOrEmpty(newTrack.Title))
            {
                throw new OperationNotAllowedException("You must provide a non-empty Title");
            }

            if (string.IsNullOrEmpty(newTrack.FilePath))
            {
                throw new OperationNotAllowedException("You must provide a valid file name");
            }

            var fileInfo = new FileInfo(newTrack.FilePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(newTrack.FilePath + " couldn't be found");
            }

            this.Title = newTrack.Title;
            if (!string.IsNullOrEmpty(newTrack.Album))
            {
                this.Album = newTrack.Album;
            }

            if (!string.IsNullOrEmpty(newTrack.Artist))
            {
                this.Artist = newTrack.Artist;
            }

            if (!string.IsNullOrEmpty(newTrack.Comments))
            {
                this.Comment = newTrack.Comments;
            }

            if (!string.IsNullOrEmpty(newTrack.Genre))
            {
                this.Genre = newTrack.Genre;
            }

            if (!string.IsNullOrEmpty(newTrack.Composer))
            {
                this.Composer = newTrack.Composer;
            }

            if (!string.IsNullOrEmpty(newTrack.AlbumArtist))
            {
                this.AlbumArtist = newTrack.AlbumArtist;
            }

            if (!string.IsNullOrEmpty(newTrack.DescriptionText))
            {
                this.DescriptionText = newTrack.DescriptionText;
            }

            _year = newTrack.Year;
            _bitrate = newTrack.Bitrate;
            _trackLength = new IPodTrackLength(newTrack.Length);
            _fileSize = new IPodTrackSize((uint)fileInfo.Length);
            _trackNumber = newTrack.TrackNumber;
            _albumTrackCount = newTrack.AlbumTrackCount;
            _discNumber = newTrack.DiscNumber;
            _totalDiscCount = newTrack.TotalDiscCount;

            this.MediaType = newTrack.IsVideo.Value ? MediaType.Video : MediaType.Audio;

            if (newTrack.FilePath.EndsWith(".m4r", StringComparison.InvariantCultureIgnoreCase))
            {
                MediaType = MediaType.Ringtone;
            }
            else if (
                newTrack.FilePath.EndsWith(".m4b", StringComparison.InvariantCultureIgnoreCase)
            )
            {
                MediaType = MediaType.Audiobook;
            }

            _isVideoFile = newTrack.IsVideo.Value;
            this.FileType = Helpers.GetFileTypeDescription(fileInfo);

            if (this.MediaType == MediaType.Video)
            {
                _rememberPlaybackPosition = true;
            }

            _dbId = _iPod.IdGenerator.GetNewDBId();

            string iPodFileName;
            while (true)
            {
                _id = _iPod.IdGenerator.GetNewTrackId();
                this.FilePath = _iPod.IdGenerator.GetNewIPodFilePath(this, fileInfo.Extension);

                iPodFileName = Path.Combine(iPod.DriveLetter, this.FilePath);
                if (_iPod.FileSystem.FileExists(iPodFileName) == false)
                {
                    break;
                }
            }
            _iPod.FileSystem.CreateDirectory(Path.GetDirectoryName(iPodFileName));

            _unk1 = new byte[12];
            _unk3 = new byte[38];
            _playedMark = 0x01;
            _unk17 = 0;
            _unk21 = new byte[4];
            _pregap = 0;
            _sampleCount = newTrack.SampleCount;
            _unk25 = new byte[4];
            _postgap = 0;
            _unk27 = new byte[4];
            _unk4 = new byte[0];
            _unk5 = new byte[44];
            _unusedHeader = new byte[_headerSize - _requiredHeaderSize];
            _type = new byte[2];
            _hasGaplessData = 1; //tell iTunes not to figure out gapless data

            _isNew = true;
            _isDirty = true;
        }

        internal List<BaseMHODElement> Children => _childSections;

        public override string ToString()
        {
            return $"Title: {Title}\r\nArtist: {Artist}\r\nAlbum: {Album}\r\nGenre: {Genre}\r\nRating: {Rating}";
        }

        internal bool IsDirty => _isDirty;

        internal int Index => _index;

        internal void ReIndex()
        {
            _index = _iPod.Tracks.GetTrackIndex(this);
        }

        public string SortTitle
        {
            get
            {
                var title = GetDataElement(MHODElementType.TitleSortBy);
                if (title != string.Empty)
                {
                    return title;
                }

                return Title;
            }
        }
        public string SortAlbum
        {
            get
            {
                var album = GetDataElement(MHODElementType.AlbumSortBy);
                if (album != string.Empty)
                {
                    return album;
                }

                return Album;
            }
        }
        public string SortArtist
        {
            get
            {
                var artist = GetDataElement(MHODElementType.ArtistSortBy);
                if (artist != string.Empty)
                {
                    return artist;
                }

                return Artist;
            }
        }

        public bool IsVideo => _isVideoFile;
    }
}
