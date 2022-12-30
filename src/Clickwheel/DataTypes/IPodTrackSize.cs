using System;
using Clickwheel.Parsers;

namespace Clickwheel.DataTypes
{
    /// <summary>
    /// Wraps a file size in bytes and a human-readable string describing the size.
    /// </summary>
    public class IPodTrackSize : IComparable
    {
        uint _trackSize;
        string _trackSizeMB;

        public IPodTrackSize(uint trackSizeInBytes)
        {
            _trackSize = trackSizeInBytes;
            _trackSizeMB = Helpers.GetFileSizeString(trackSizeInBytes, 1);
        }

        public uint ByteCount => _trackSize;

        public override string ToString()
        {
            return _trackSizeMB;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return _trackSize.CompareTo(((IPodTrackSize)obj).ByteCount);
        }

        #endregion
    }
}
