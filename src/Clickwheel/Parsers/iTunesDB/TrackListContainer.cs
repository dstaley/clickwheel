using System.IO;

namespace Clickwheel.Parsers.iTunesDB
{
    /// <summary>
    /// Implements a type 1 (Tracks list) MHSD entry in iTunesDB
    /// </summary>
    class TrackListContainer : BaseDatabaseElement
    {
        private ListContainerHeader _header;
        TrackList _childSection;

        public TrackListContainer(ListContainerHeader parent)
        {
            _header = parent;
        }

        #region IDatabaseElement Members

        internal override void Read(IPod iPod, BinaryReader reader)
        {
            base.Read(iPod, reader);
            _childSection = new TrackList();
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

        #endregion

        internal TrackList GetTrackList()
        {
            return _childSection;
        }
    }
}
