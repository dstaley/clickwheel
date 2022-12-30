using System;
using System.IO;
using System.Text;

namespace Clickwheel.Parsers.iTunesDB
{
    class UnicodeMHOD : StringMHOD
    {
        private int _dataSize;
        private int _unk3;
        private int _unk4;
        private string _data;
        private byte[] _unk5;
        protected int _position;

        public override string Data
        {
            get => _data;
            set => _data = value;
        }

        public int Position
        {
            get => _position;
            set => _position = value;
        }

        public UnicodeMHOD() : base()
        {
            _position = 1;
            _dataSize = 0;
            _data = string.Empty;
            _unk3 = 1;
        }

        public UnicodeMHOD(int type) : this()
        {
            _type = type;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            _position = reader.ReadInt32();
            _dataSize = reader.ReadInt32();
            _unk3 = reader.ReadInt32();
            _unk4 = reader.ReadInt32();
            var byteData = reader.ReadBytes(_dataSize);
            _data = Encoding.Unicode.GetString(byteData);

            var extraDataLength = _sectionSize - (_dataSize + _headerSize + 16);
            if (extraDataLength > 0)
            {
                _unk5 = reader.ReadBytes(extraDataLength);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            var dataBytes = Array.Empty<byte>();
            if (_data != null)
            {
                dataBytes = Encoding.Unicode.GetBytes(_data);
            }

            writer.Write(_position);
            writer.Write(dataBytes.Length);
            writer.Write(_unk3);
            writer.Write(_unk4);

            writer.Write(dataBytes);
            if (_unk5 != null)
            {
                writer.Write(_unk5);
            }
        }

        internal override int GetSectionSize()
        {
            var size = _headerSize;
            if (_data != null)
            {
                size += Encoding.Unicode.GetByteCount(_data);
            }
            size += 16;

            if (_unk5 != null)
            {
                size += _unk5.GetLength(0);
            }
            if (size != _sectionSize) { }

            return size;
        }
    }
}
