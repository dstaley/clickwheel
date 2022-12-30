using System.IO;

namespace Clickwheel.Parsers.Artwork
{
    internal enum MHSDSectionType
    {
        Images = 1,
        Albums = 2,
        Files = 3
    }

    /// <summary>
    /// Implements a generic MHSD entry in ArtworkDB
    /// </summary>
    class ListContainerHeader : BaseDatabaseElement
    {
        protected MHSDSectionType _type;
        protected BaseDatabaseElement _childSection;

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
                case MHSDSectionType.Images:
                    _childSection = new ImageListContainer(this);
                    break;
                case MHSDSectionType.Files:
                    _childSection = new IThmbFileListContainer(this);
                    break;
                case MHSDSectionType.Albums:
                    _childSection = new ImageAlbumListContainer(this);
                    break;
                default:
                    _childSection = new UnknownListContainer(this);
                    break;
            }
            _childSection.Read(iPod, reader);
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write((int)_type);
            writer.Write(_unusedHeader);

            _childSection.Write(writer);
        }

        internal override int GetSectionSize()
        {
            return _childSection.GetSectionSize();
        }

        public BaseDatabaseElement GetListContainer()
        {
            return _childSection;
        }
    }
}
