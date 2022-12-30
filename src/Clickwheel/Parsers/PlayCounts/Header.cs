using System;
using System.Collections.Generic;
using System.IO;

namespace Clickwheel.Parsers.PlayCounts
{
    class Header : BaseDatabaseElement
    {
        int _entrySize;
        int _nbrEntries;
        List<Entry> _entries;

        public Header()
        {
            _requiredHeaderSize = 16;
            _entries = new List<Entry>();
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhdp");

            _entrySize = reader.ReadInt32();
            _nbrEntries = reader.ReadInt32();

            this.ReadToHeaderEnd(reader);

            for (var i = 0; i < _nbrEntries; i++)
            {
                var entry = new Entry(_entrySize);
                entry.Read(iPod, reader);
                _entries.Add(entry);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override int GetSectionSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerable<Entry> Entries()
        {
            foreach (var e in _entries)
            {
                yield return e;
            }
        }

        public int EntryCount => _nbrEntries;
    }
}
