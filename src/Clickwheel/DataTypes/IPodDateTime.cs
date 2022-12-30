using System;
using Clickwheel.Parsers;

namespace Clickwheel.DataTypes
{
    /// <summary>
    /// Wraps a .NET DateTime and an iPod-format timestamp.
    /// </summary>
    public class IPodDateTime : IComparable
    {
        uint _timeStamp;
        DateTime _dateTime;

        public IPodDateTime(uint timestamp)
        {
            _dateTime = Helpers.GetDateTimeFromTimeStamp(timestamp);
            _timeStamp = timestamp;
        }

        public IPodDateTime(DateTime date)
        {
            _dateTime = date;
            _timeStamp = Helpers.GetTimeStampFromDate(_dateTime);
        }

        public DateTime DateTime => _dateTime;

        public uint TimeStamp => _timeStamp;

        public override string ToString()
        {
            if (_timeStamp == 0)
            {
                return string.Empty;
            }
            else
            {
                return _dateTime.ToString();
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return _timeStamp.CompareTo(((IPodDateTime)obj).TimeStamp);
        }

        #endregion
    }
}
