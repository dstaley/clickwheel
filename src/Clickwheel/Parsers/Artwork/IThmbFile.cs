using System.IO;

namespace Clickwheel.Parsers.Artwork
{
    // Implements a MHIF element in ArtworkDB
    /// <summary>
    ///
    /// </summary>
    class IThmbFile : BaseDatabaseElement
    {
        private uint _unk1,
            _formatId,
            _imageSize;

        public IThmbFile()
        {
            _requiredHeaderSize = 24;
            _headerSize = 124;
            _sectionSize = 124;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhif");
            _sectionSize = reader.ReadInt32();
            _unk1 = reader.ReadUInt32();
            _formatId = reader.ReadUInt32();
            _imageSize = reader.ReadUInt32();

            ReadToHeaderEnd(reader);
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write(_unk1);
            writer.Write(_formatId);
            writer.Write(_imageSize);
            writer.Write(_unusedHeader);
        }

        internal override int GetSectionSize()
        {
            return _sectionSize;
        }

        public uint FormatId => _formatId;

        public uint ImageSize => _imageSize;

        internal void Create(uint imageSize, uint formatId)
        {
            _identifier = "mhif".ToCharArray();
            _unusedHeader = new byte[_headerSize - _requiredHeaderSize];
            _imageSize = imageSize;
            _formatId = formatId;
        }
    }
}
