namespace Clickwheel
{
    /// <summary>
    /// Used to add new tracks to the iPod
    /// </summary>
    public class NewTrack
    {
        /// <summary>
        /// Title of the track. Cannot be empty.
        /// </summary>
        public string Title;
        public string Artist;
        public string Album;
        public string Comments;

        /// <summary>
        /// Full path of the file to import. Can not be empty.
        /// </summary>
        public string FilePath;
        public string Genre;

        /// <summary>
        /// Length of track in milliseconds
        /// </summary>
        public uint Length;

        /// <summary>
        /// Bitrate in kb (e.g. 192)
        /// </summary>
        public uint Bitrate;
        public string Composer;

        /// <summary>
        /// Only displayed on the iPod for Podcast tracks
        /// </summary>
        public string DescriptionText;
        public uint TrackNumber;
        public uint Year;

        /// <summary>
        /// How many tracks in the album
        /// </summary>
        public uint AlbumTrackCount;

        /// <summary>
        /// Number of disc in the set
        /// </summary>
        public uint DiscNumber;

        /// <summary>
        /// How many discs in the set
        /// </summary>
        public uint TotalDiscCount;

        public string AlbumArtist;

        /// <summary>
        /// True if this item contains a video stream, otherwise false. Cannot be null.
        /// </summary>
        public bool? IsVideo;

        /// <summary>
        /// Path to an image file which will be used for the track's album art. Can be null.
        /// </summary>
        public string ArtworkFile;
    }
}
