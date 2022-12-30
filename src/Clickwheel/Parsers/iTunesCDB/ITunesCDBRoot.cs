using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Parsers.iTunesCDB
{
    /// <summary>
    /// Implements an MHBD section in the iTunesCDB file. The only difference between this file and the regular iTunesDB is the
    /// content is zlib compressed.
    /// </summary>
    class ITunesCDBRoot : iTunesDBRoot
    {
        private List<Track> _dirtyTracks;
        private List<Playlist> _dirtyPlaylists;

        public ITunesCDBRoot() : base() { }

        #region IDatabaseElement Members

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            _iPod = iPod;
            _identifier = reader.ReadChars(4);
            _headerSize = reader.ReadInt32();

            ValidateHeader("mhbd");

            _sectionSize = reader.ReadInt32();
            _unk1 = reader.ReadInt32();
            _versionNumber = reader.ReadInt32();
            _listContainerCount = reader.ReadInt32();
            _id = reader.ReadUInt64();
            _unk2 = reader.ReadBytes(16);
            _hashingScheme = reader.ReadInt16();

            ReadToHeaderEnd(reader);

            var data = reader.ReadBytes(_sectionSize - _headerSize);

            using (var decompressed = new MemoryStream())
            using (var compressed = new MemoryStream(data))
            using (var decompressor = new ZLibStream(compressed, CompressionMode.Decompress))
            {
                decompressor.CopyTo(decompressed);
                data = decompressed.ToArray();
            }

            var contentsReader = new BinaryReader(new MemoryStream(data));

            while (contentsReader.BaseStream.Position != contentsReader.BaseStream.Length)
            {
                var containerHeader = new ListContainerHeader();
                containerHeader.Read(iPod, contentsReader);
                _childSections.Add(containerHeader);
            }

            contentsReader.Close();
            reader.Close();

            //Get all dirty tracks and register callback for successful iTunesDB write.
            _iPod.ITunesDB.DatabaseWritten += new EventHandler(ITunesDB_DatabaseWritten);
        }

        internal override void Write(BinaryWriter writer)
        {
            if (_iPod.Tracks != null)
            {
                _dirtyTracks = new List<Track>();
                foreach (var t in _iPod.Tracks)
                {
                    if (t.IsDirty)
                    {
                        _dirtyTracks.Add(t);
                    }
                }
                _dirtyPlaylists = new List<Playlist>();
                foreach (var p in _iPod.Playlists)
                {
                    if (p.IsDirty)
                    {
                        _dirtyPlaylists.Add(p);
                    }
                }
            }

            byte[] data;
            var contents = new MemoryStream();
            var contentsWriter = new BinaryWriter(contents);

            for (var i = 0; i < _childSections.Count; i++)
            {
                _childSections[i].Write(contentsWriter);
            }

            using (var compressed = new MemoryStream())
            {
                using (var compressor = new ZLibStream(compressed, CompressionLevel.Fastest))
                {
                    compressor.Write(contents.GetBuffer(), 0, (int)contents.Length);
                }
                data = compressed.ToArray();
            }
            contentsWriter.Close();

            _sectionSize = GetSectionSize() + data.Length;

            writer.Write(_identifier);
            writer.Write(_headerSize);
            writer.Write(_sectionSize);
            writer.Write(_unk1);
            writer.Write(_versionNumber);
            writer.Write(_listContainerCount);
            //really this should be _childSections.Count, but have observed some
            //dbs with wrong count from iTunes, updating it means we fail compatibility test with SourceDoesntMatchOutput

            writer.Write(_id);
            writer.Write(_unk2);
            writer.Write(_hashingScheme);
            writer.Write(_unusedHeader);
            writer.Write(data);
        }

        void ITunesDB_DatabaseWritten(object sender, EventArgs e)
        {
            SqliteTables sqlTables = null;

            switch (_iPod.DeviceInfo.Family)
            {
                case IPodFamily.iPod_Nano_Gen5:
                    sqlTables = new SqliteTables_Nano5G(_iPod);
                    break;
            }

            sqlTables.UpdateTracks(_dirtyTracks);
            sqlTables.UpdatePlaylists(_dirtyPlaylists);
            sqlTables.Save();
            sqlTables.UpdateLocationsCbk();
        }

        internal override int GetSectionSize()
        {
            return _headerSize;
        }

        #endregion
    }
}
