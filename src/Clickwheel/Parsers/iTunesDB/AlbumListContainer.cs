using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    /// <summary>
    /// Implements a type 4 (Album list) MHSD entry in iTunesDB
    /// </summary>
    class AlbumListContainer : BaseDatabaseElement
    {
        private ListContainerHeader _header;
        private byte[] _unk1;

        public AlbumListContainer(ListContainerHeader parent)
        {
            _header = parent;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            var length = _header.SectionSize - _header.HeaderSize;
            _unk1 = reader.ReadBytes(length);
        }

        internal override void Write(BinaryWriter writer)
        {
            writer.Write(_unk1);
        }

        internal override int GetSectionSize()
        {
            return _header.SectionSize;
        }
    }
}
