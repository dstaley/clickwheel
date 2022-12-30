using System;
using System.Collections.Generic;
using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    /// <summary>
    /// Implements an MHBD section in the iTunesDB file
    /// </summary>
    public class iTunesDBRoot : BaseDatabaseElement
    {
        protected int _unk1;
        protected int _versionNumber;
        protected int _listContainerCount;
        protected ulong _id;
        protected byte[] _unk2;
        protected short _hashingScheme;

        internal List<ListContainerHeader> _childSections;

        public int VersionNumber => _versionNumber;

        internal int HashingScheme => _hashingScheme;

        public iTunesDBRoot()
        {
            _requiredHeaderSize = 50;
            _childSections = new List<ListContainerHeader>();
        }

        #region IDatabaseElement Members

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
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

            this.ReadToHeaderEnd(reader);

            while (reader.BaseStream.Position != reader.BaseStream.Length)
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
            writer.Write(_versionNumber);
            writer.Write(_listContainerCount);
            //really this should be _childSections.Count, but have observed some
            //dbs with wrong count from iTunes, updating it means we fail compatibility test with SourceDoesntMatchOutput

            writer.Write(_id);
            writer.Write(_unk2);
            writer.Write(_hashingScheme);
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

        #endregion

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

        /// <summary>
        /// Gets the PlaylistList, or PlaylistV2 if Playlist container doesn't exist
        /// </summary>
        /// <returns></returns>
        public PlaylistList GetPlaylistList()
        {
            if (GetChildSection(MHSDSectionType.Playlists) != null)
            {
                var playlistsContainer = (PlaylistListContainer)GetChildSection(
                        MHSDSectionType.Playlists
                    )
                    .GetListContainer();
                return playlistsContainer.GetPlaylistsList();
            }

            if (GetChildSection(MHSDSectionType.PlaylistsV2) != null)
            {
                var playlistsContainer = (PlaylistListV2Container)GetChildSection(
                        MHSDSectionType.PlaylistsV2
                    )
                    .GetListContainer();
                return playlistsContainer.GetPlaylistsList();
            }

            throw new Exception("iTunesDB Playlist container not found");
        }
    }
}
