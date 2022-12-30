using System.IO;

namespace Clickwheel.Parsers.Artwork
{
    /// <summary>
    /// Implements a type 3 (file list) MHSD entry in ArtworkDB
    /// </summary>
    class IThmbFileListContainer : BaseDatabaseElement
    {
        private ListContainerHeader _header;
        IThmbFileList _childSection;

        public IThmbFileListContainer(ListContainerHeader parent)
        {
            _header = parent;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _childSection = new IThmbFileList();
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

        internal IThmbFileList FileList => _childSection;
    }
}
