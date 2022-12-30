using System.Collections.Generic;
using System.IO;

namespace Clickwheel.Parsers.Artwork
{
    public class ArtworkDBRoot : BaseDatabaseElement
    {
        private int _unk1,
            _unk2,
            _listContainerCount,
            _unk3;
        private uint _nextImageId;

        private List<ListContainerHeader> _childSections;

        public ArtworkDBRoot()
        {
            _requiredHeaderSize = 32;
            _childSections = new List<ListContainerHeader>();
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhfd");

            _sectionSize = reader.ReadInt32();
            _unk1 = reader.ReadInt32();
            _unk2 = reader.ReadInt32();
            _listContainerCount = reader.ReadInt32();
            _unk3 = reader.ReadInt32();
            _nextImageId = reader.ReadUInt32();

            ReadToHeaderEnd(reader);

            for (var i = 0; i < _listContainerCount; i++)
            {
                var containerHeader = new ListContainerHeader();
                containerHeader.Read(iPod, reader);
                _childSections.Add(containerHeader);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            _sectionSize = GetSectionSize();

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write(_unk1);
            writer.Write(_unk2);
            writer.Write(_childSections.Count);
            writer.Write(_unk3);
            writer.Write(_nextImageId);
            writer.Write(_unusedHeader);

            for (var i = 0; i < _childSections.Count; i++)
            {
                _childSections[i].Write(writer);
            }
        }

        internal override int GetSectionSize()
        {
            var size = _headerSize;
            for (var i = 0; i < _childSections.Count; i++)
            {
                size += _childSections[i].GetSectionSize();
            }
            return size;
        }

        internal ListContainerHeader GetChildSection(MHSDSectionType type)
        {
            for (var i = 0; i < _childSections.Count; i++)
            {
                if (_childSections[i].Type == type)
                {
                    return _childSections[i];
                }
            }
            return null;
        }

        public uint NextImageId
        {
            get => _nextImageId;
            set => _nextImageId = value;
        }
    }
}
