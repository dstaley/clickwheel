using System.IO;
using System.Text;

namespace Clickwheel.Parsers.iTunesDB
{
    enum StringEncodingType
    {
        Ascii = 0,
        UTF8 = 1,
        Unicode = 2
    }

    /// <summary>
    /// Implements an MHOD used in the ArtworkDB and PhotoDB
    /// </summary>
    class ArtworkStringMHOD : StringMHOD
    {
        private ushort _unk1x;
        private int _padding;
        private StringEncodingType _stringType;
        private int _unk3;
        private string _data;

        private int _actualPadding;

        internal ArtworkStringMHOD()
        {
            _requiredHeaderSize = 24;

            _headerSize = 24;
            _identifier = "mhod".ToCharArray();
            _type = MHODElementType.Album; //3
            _stringType = StringEncodingType.Unicode;
        }

        #region IDatabaseElement Members

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            var startOfElement = reader.BaseStream.Position;

            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhod");

            _sectionSize = reader.ReadInt32();
            _type = reader.ReadUInt16();
            _unk1x = reader.ReadUInt16();

            _unk2 = reader.ReadInt32();
            _padding = reader.ReadInt32();

            ReadToHeaderEnd(reader);

            var dataLength = reader.ReadInt32();
            _stringType = (StringEncodingType)reader.ReadInt32();
            _unk3 = reader.ReadInt32();
            var bytes = reader.ReadBytes(dataLength);

            _data = StringEncoding.GetString(bytes);

            _actualPadding = (int)((startOfElement + _sectionSize) - reader.BaseStream.Position);
            //Jump over padding section
            if (_actualPadding != 0)
            {
                reader.BaseStream.Seek(startOfElement + _sectionSize, SeekOrigin.Begin);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            var bytes = StringEncoding.GetBytes(_data);

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write((ushort)_type);
            writer.Write(_unk1x);
            writer.Write(_unk2);
            writer.Write(_padding);
            writer.Write(_unusedHeader);
            writer.Write(bytes.Length);
            writer.Write((int)_stringType);
            writer.Write(_unk3);
            writer.Write(bytes);

            //Jump over padding section
            writer.BaseStream.Seek(_actualPadding, SeekOrigin.Current);
        }

        internal override int GetSectionSize()
        {
            var dataLength = StringEncoding.GetByteCount(_data);
            return _headerSize + 12 + dataLength + _actualPadding;
        }

        #endregion

        public override string Data
        {
            get => _data;
            set
            {
                if (!value.StartsWith(":"))
                {
                    _data = ":" + value;
                }
                else
                {
                    _data = value;
                }
            }
        }

        internal void Create(string data)
        {
            _unusedHeader = new byte[_headerSize - _requiredHeaderSize];
            Data = data;
        }

        private Encoding StringEncoding
        {
            get
            {
                if (_stringType == StringEncodingType.Unicode)
                {
                    return Encoding.Unicode;
                }
                else if (_stringType == StringEncodingType.Ascii)
                {
                    return Encoding.ASCII;
                }
                else
                {
                    return Encoding.UTF8;
                }
            }
        }
    }
}
