using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    internal enum MHSDSectionType
    {
        Tracks = 1,
        Playlists = 2,
        PlaylistsV2 = 3,
        Albums = 4,
        Type5,
        Type6,
        Unknown = 999
    }

    /// <summary>
    /// Implements a generic MHSD entry in iTunesDB
    /// </summary>
    class ListContainerHeader : BaseDatabaseElement
    {
        protected MHSDSectionType _type;
        protected BaseDatabaseElement _childElement;

        public ListContainerHeader()
        {
            _requiredHeaderSize = 16;
        }

        internal int HeaderSize => _headerSize;

        internal int SectionSize => _sectionSize;

        internal MHSDSectionType Type => _type;

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhsd");

            _sectionSize = reader.ReadInt32();
            _type = (MHSDSectionType)reader.ReadInt32();
            ReadToHeaderEnd(reader);

            switch (_type)
            {
                case MHSDSectionType.Albums:
                    _childElement = new AlbumListContainer(this);
                    break;
                case MHSDSectionType.Tracks:
                    _childElement = new TrackListContainer(this);
                    break;
                case MHSDSectionType.Playlists:
                    _childElement = new PlaylistListContainer(this);
                    break;
                case MHSDSectionType.PlaylistsV2:
                    _childElement = new PlaylistListV2Container(this);
                    break;
                default:
                    _childElement = new UnknownListContainer(this);
                    break;
            }
            _childElement.Read(iPod, reader);
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write((int)_type);
            writer.Write(_unusedHeader);

            _childElement.Write(writer);
        }

        internal override int GetSectionSize()
        {
            return _childElement.GetSectionSize();
        }

        public BaseDatabaseElement GetListContainer()
        {
            return _childElement;
        }
    }
}
