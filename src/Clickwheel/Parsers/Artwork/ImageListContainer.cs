using System.IO;

namespace Clickwheel.Parsers.Artwork
{
    /// <summary>
    /// Implements a type 1 (Image list) MHSD entry in ArtworkDB
    /// </summary>
    class ImageListContainer : BaseDatabaseElement
    {
        private ListContainerHeader _header;
        ImageList _childSection;

        public ImageListContainer(ListContainerHeader parent)
        {
            _header = parent;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _childSection = new ImageList();
            _childSection.Read(iPod, reader);
        }

        internal override void Write(BinaryWriter writer)
        {
            _childSection.Write(writer);
        }

        internal override int GetSectionSize()
        {
            return _header.HeaderSize + _childSection.GetSectionSize();
        }

        internal ImageList ImageList => _childSection;
    }
}
