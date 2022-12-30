using System;
using Clickwheel.Parsers;

namespace Clickwheel.DataTypes
{
    /// <summary>
    /// Wraps a track length in milliseconds and a human-readable hh:mm:ss string
    /// </summary>
    public class IPodTrackLength : IComparable
    {
        uint _trackLengthMSecs;
        string _trackLengthMinsSecs;

        public IPodTrackLength(uint trackLengthInMSecs)
        {
            _trackLengthMSecs = trackLengthInMSecs;
            _trackLengthMinsSecs = Helpers.GetTimeString(this.Seconds);
        }

        public IPodTrackLength(int trackLengthInMSecs) : this((uint)trackLengthInMSecs) { }

        public uint Seconds => _trackLengthMSecs / 1000;

        public uint MilliSeconds => _trackLengthMSecs;

        public override string ToString()
        {
            return _trackLengthMinsSecs;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return _trackLengthMSecs.CompareTo(((IPodTrackLength)obj).MilliSeconds);
        }

        #endregion
    }
}
