using System;
using System.IO;

namespace Clickwheel.Parsers.iTunesSD
{
    class Header : BaseDatabaseElement
    {
        private int _trackCount;
        private byte[] _unk1;
        private byte[] _headerPadding;

        public Header(IPod iPod)
        {
            _iPod = iPod;
            _unk1 = new byte[3];
            _unk1[0] = 1;
            _unk1[1] = 6;
            _unk1[2] = 0;
            _headerSize = 18;
            _headerPadding = new byte[9];

            _trackCount = _iPod.Tracks.Count;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override void Write(BinaryWriter writer)
        {
            writer.Write(Helpers.IntToITunesSDFormat(_trackCount));
            writer.Write(_unk1);
            writer.Write(Helpers.IntToITunesSDFormat(_headerSize));
            writer.Write(_headerPadding);

            foreach (var track in _iPod.Tracks)
            {
                var entry = new Entry(track);
                entry.Write(writer);
            }
        }

        internal override int GetSectionSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
