using System;
using System.Collections.Generic;
using System.IO;
using Clickwheel.Exceptions;

namespace Clickwheel.Parsers.iTunesDB
{
    internal enum MenuIndexType
    {
        Title = 3,
        Album_Track_Title = 4,
        Artist_Album_Track_Title = 5,
        Genre_Artist_Album_Track_Title = 7
    }

    /// <summary>
    /// Implements a Type-52 MHOD. This is a fast lookup table the iPod uses for Artist, Album, Genre menus.
    /// </summary>
    class MenuIndexMHOD : BaseMHODElement
    {
        int _indexType;
        byte[] padding = new byte[40];
        List<int> _indexes;

        public MenuIndexMHOD() : base()
        {
            _type = MHODElementType.MenuIndexTable;
        }

        public MenuIndexMHOD(MenuIndexType indexType) : this()
        {
            _indexType = (int)indexType;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            _indexType = reader.ReadInt32();
            var nbrEntries = reader.ReadInt32();
            padding = reader.ReadBytes(40);

            _indexes = new List<int>();

            for (var i = 0; i < nbrEntries; i++)
            {
                _indexes.Add(reader.ReadInt32());
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(_indexType);
            writer.Write(_indexes.Count);

            writer.Write(padding);

            foreach (var i in _indexes)
            {
                writer.Write(i);
            }
        }

        internal override int GetSectionSize()
        {
            return _headerSize + 48 + _indexes.Count * 4;
        }

        public void ReIndex(List<Track> tracks)
        {
            if (!IsSupported)
            {
                throw new BaseClickwheelException($"MenuIndexType: {_indexType} is not supported for reindexing.");
            }

            var indexType = (MenuIndexType)_indexType;

            switch (indexType)
            {
                case MenuIndexType.Title:
                    tracks.Sort(new TitleComparer());
                    break;
                case MenuIndexType.Album_Track_Title:
                    tracks.Sort(new AlbumTrackTitleComparer());
                    break;
                case MenuIndexType.Artist_Album_Track_Title:
                    tracks.Sort(new ArtistAlbumTrackTitleComparer());
                    break;
                case MenuIndexType.Genre_Artist_Album_Track_Title:
                    tracks.Sort(new GenreArtistAlbumTrackTitleComparer());
                    break;
            }

            _indexes = new List<int>();
            foreach (var t in tracks)
            {
                _indexes.Add(t.Index);
            }

            for (var i = 0; i > _indexes.Count; i++)
            {
                if (!_indexes.Contains(i)) { }
                else if (
                    _indexes
                        .FindAll(
                            delegate(int i2)
                            {
                                return i2 == i;
                            }
                        )
                        .Count > 1
                ) { }
            }
        }

        public bool IsSupported => Enum.IsDefined(typeof(MenuIndexType), _indexType);

        public int IndexType => _indexType;
    }

    internal class TitleComparer : IComparer<Track>
    {
        #region IComparer<Track> Members

        public int Compare(Track x, Track y)
        {
            var result = x.Title.CompareTo(y.Title);
            return result;
        }

        #endregion
    }

    internal class AlbumTrackTitleComparer : IComparer<Track>
    {
        #region IComparer<Track> Members

        public int Compare(Track x, Track y)
        {
            var result = (x.Album ?? string.Empty).CompareTo(y.Album);
            if (result != 0)
            {
                return result;
            }

            result = x.TrackNumber.CompareTo(y.TrackNumber);
            if (result != 0)
            {
                return result;
            }

            result = (x.Title ?? string.Empty).CompareTo(y.Title);
            return result;
        }

        #endregion
    }

    internal class ArtistAlbumTrackTitleComparer : IComparer<Track>
    {
        #region IComparer<Track> Members

        public int Compare(Track x, Track y)
        {
            var result = (x.SortArtist ?? string.Empty).CompareTo(y.SortArtist);
            if (result != 0)
            {
                return result;
            }

            result = (x.Album ?? string.Empty).CompareTo(y.Album);
            if (result != 0)
            {
                return result;
            }

            result = x.TrackNumber.CompareTo(y.TrackNumber);
            if (result != 0)
            {
                return result;
            }

            result = (x.Title ?? string.Empty).CompareTo(y.Title);
            return result;
        }

        #endregion
    }

    internal class GenreArtistAlbumTrackTitleComparer : IComparer<Track>
    {
        #region IComparer<Track> Members

        public int Compare(Track x, Track y)
        {
            var result = (x.Genre ?? string.Empty).CompareTo(y.Genre);
            if (result != 0)
            {
                return result;
            }

            result = (x.SortArtist ?? string.Empty).CompareTo(y.SortArtist);
            if (result != 0)
            {
                return result;
            }

            result = (x.Album ?? string.Empty).CompareTo(y.Album);
            if (result != 0)
            {
                return result;
            }

            result = x.TrackNumber.CompareTo(y.TrackNumber);
            if (result != 0)
            {
                return result;
            }

            result = (x.Title ?? string.Empty).CompareTo(y.Title);
            return result;
        }

        #endregion
    }
}
