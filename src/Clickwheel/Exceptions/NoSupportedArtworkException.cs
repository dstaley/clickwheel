namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when trying to add album artwork and the iPod reported no supported artwork.
    /// </summary>
    public class NoSupportedArtworkException : BaseClickwheelException
    {
        public NoSupportedArtworkException() : base("No supported artwork formats were detected")
        {
            Category = "Album art could not be added";
        }
    }
}
