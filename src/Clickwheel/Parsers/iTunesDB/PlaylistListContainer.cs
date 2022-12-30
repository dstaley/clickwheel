using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    /// <summary>
    /// Implements a type 2 (Playlists list) MHSD entry in iTunesDB
    /// </summary>
    class PlaylistListContainer : BaseDatabaseElement
    {
        private ListContainerHeader _header;
        private PlaylistList _childSection;

        internal PlaylistListContainer(ListContainerHeader parent)
        {
            _header = parent;
        }

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _childSection = new PlaylistList();
            _childSection.Read(iPod, reader);
        }

        internal override void Write(BinaryWriter writer)
        {
            _childSection.Write(writer);
        }

        internal override int GetSectionSize()
        {
            return _header.HeaderSize + _childSection.GetSectionSize();
        }

        internal PlaylistList GetPlaylistsList()
        {
            return _childSection;
        }
    }
}
