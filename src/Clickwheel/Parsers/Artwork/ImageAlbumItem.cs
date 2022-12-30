using System.Collections.Generic;
using System.IO;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Parsers.Artwork
{
    // Implements a MHIA entry in ArtworkDB
    /// <summary>
    /// An Image Album
    /// </summary>
    public class ImageAlbumItem : BaseDatabaseElement
    {
        int _unk1;

        private List<BaseMHODElement> _dataObjects = new List<BaseMHODElement>();
        private List<ImageAlbum> _images = new List<ImageAlbum>();
        internal uint ImageId { get; set; }
        internal IPodImage Artwork { get; set; }

        internal ImageAlbumItem()
        {
            _requiredHeaderSize = 20;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);

            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhia");

            _sectionSize = reader.ReadInt32();
            _unk1 = reader.ReadInt32();
            ImageId = reader.ReadUInt32();

            base.ReadToHeaderEnd(reader);
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write(_unk1);
            writer.Write(ImageId);
            writer.Write(_unusedHeader);
        }

        internal override int GetSectionSize()
        {
            return _sectionSize;
        }
    }
}
