using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    class PlaylistPositionMHOD : BaseMHODElement
    {
        private byte[] _byteData;
        protected int _position;

        public int Position
        {
            get => _position;
            set => _position = value;
        }

        public PlaylistPositionMHOD() : base()
        {
            _type = MHODElementType.PlaylistPosition;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            if (_sectionSize == _headerSize) { }
            else
            {
                _position = reader.ReadInt32();
                _byteData = reader.ReadBytes(_sectionSize - (_headerSize + 4));
            }
        }

        public void Create()
        {
            _byteData = new byte[16];
        }

        internal override void Write(BinaryWriter writer)
        {
            if (writer.BaseStream.Position == 1192046) { }
            base.Write(writer);
            writer.Write(_position);
            if (_byteData != null)
            {
                writer.Write(_byteData);
            }
        }

        internal override int GetSectionSize()
        {
            var size = _headerSize + 4;
            if (_byteData != null)
            {
                size += _byteData.Length;
            }

            return size;
        }
    }
}
