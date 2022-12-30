using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    class UnknownMHOD : BaseMHODElement
    {
        private byte[] _byteData;

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            _byteData = reader.ReadBytes(_sectionSize - _headerSize);
        }

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(_byteData);
        }

        internal override int GetSectionSize()
        {
            var size = _headerSize + _byteData.Length;

            return size;
        }
    }
}
