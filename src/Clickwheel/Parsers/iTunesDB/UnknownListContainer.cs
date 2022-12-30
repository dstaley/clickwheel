using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    /// <summary>
    /// Implements any unknown type MHSD entry in iTunesDB
    /// Simply reads to the end of the list, ignoring the contents.
    /// Role is to protect against future iTunesDB changes.
    /// </summary>
    class UnknownListContainer : BaseDatabaseElement
    {
        private ListContainerHeader _header;
        private byte[] _unk1;

        public UnknownListContainer(ListContainerHeader parent)
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
