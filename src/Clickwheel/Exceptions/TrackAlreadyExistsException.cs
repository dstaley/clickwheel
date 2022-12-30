using Clickwheel.Parsers.iTunesDB;

namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when trying to add a track to the iPod which already exists. (Same Title, Artist, Album, TrackNumber)
    /// </summary>
    public class TrackAlreadyExistsException : BaseClickwheelException
    {
        private Track _existingTrack;

        public TrackAlreadyExistsException(string message, Track existingTrack) : base(message)
        {
            Category = "This track already exists on your iPod";
            _existingTrack = existingTrack;
        }

        /// <summary>
        /// Track that is on the iPod already
        /// </summary>
        public Track ExistingTrack => _existingTrack;
    }
}
