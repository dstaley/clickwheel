namespace Clickwheel.Exceptions
{
    public class UnsupportedArtworkFormatException : BaseClickwheelException
    {
        public UnsupportedArtworkFormatException(uint imageSize)
            : base($"The artwork format (size {imageSize}) is not currently supported.")
        {
            Category = "Unsupported Artwork format.";
        }
    }
}
