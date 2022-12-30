using System;
using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    internal static class MHODElementType
    {
        public const int Id = 0;
        public const int Title = 1;
        public const int FilePath = 2;
        public const int Album = 3;
        public const int Artist = 4;
        public const int Genre = 5;
        public const int FileType = 6;
        public const int Comment = 8;
        public const int Composer = 12;
        public const int DescriptionText = 14;
        public const int PodcastFileUrl = 15;
        public const int PodcastRSSUrl = 16;
        public const int ChapterData = 17;
        public const int AlbumArtist = 22;
        public const int ArtistSortBy = 23;
        public const int TitleSortBy = 27;
        public const int AlbumSortBy = 28;
        public const int AlbumArtistSortBy = 29;
        public const int SmartPlaylistData = 50;
        public const int SmartPlaylistRule = 51;
        public const int MenuIndexTable = 52;
        public const int LetterJumpTable = 53;
        public const int PlaylistPosition = 100;
    }

    class BaseMHODElement : BaseDatabaseElement
    {
        protected int _type;
        protected int _unk1;
        protected int _unk2;

        internal int HeaderSize => _headerSize;
        internal int SectionSize => _sectionSize;

        internal int Unk1 => _unk1;
        internal int Unk2 => _unk2;

        public int Type
        {
            get => _type;
            set => _type = value;
        }

        internal BaseMHODElement()
        {
            _requiredHeaderSize = 24;

            _headerSize = 24;
            _identifier = "mhod".ToCharArray();

            _type = MHODElementType.Id;
        }

        internal BaseMHODElement(int type) : this()
        {
            _type = type;
        }

        #region IDatabaseElement Members

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhod");

            _sectionSize = reader.ReadInt32();
            _type = reader.ReadInt32();
            _unk1 = reader.ReadInt32();
            _unk2 = reader.ReadInt32();
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write((int)_type);
            writer.Write(_unk1);
            writer.Write(_unk2);
        }

        internal override int GetSectionSize()
        {
            throw new NotImplementedException();
        }

        #endregion

        internal void SetHeader(BaseMHODElement header)
        {
            _headerSize = header.HeaderSize;
            _sectionSize = header.SectionSize;
            _unk1 = header.Unk1;
            _unk2 = header.Unk2;
            _type = header.Type;
        }
    }
}
