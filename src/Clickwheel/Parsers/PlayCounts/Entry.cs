using System;
using System.IO;

namespace Clickwheel.Parsers.PlayCounts
{
    class Entry : BaseDatabaseElement
    {
        private int _playCount;
        private DateTime _lastPlayed;
        private int _bookmarkPosition;
        private int _rating;

        public Entry(int entrySize)
        {
            _headerSize = entrySize;
            _requiredHeaderSize = 16;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);

            _playCount = reader.ReadInt32();
            _lastPlayed = Helpers.GetDateTimeFromTimeStamp(reader.ReadUInt32());
            _bookmarkPosition = reader.ReadInt32();
            _rating = reader.ReadInt32();

            ReadToHeaderEnd(reader);
        }

        internal override void Write(BinaryWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override int GetSectionSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal int PlayCount => _playCount;

        internal DateTime DateLastPlayed => _lastPlayed;

        internal int Rating => _rating / 20;
    }
}
