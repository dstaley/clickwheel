namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown if artwork is added to an iPod without an ArtworkDB
    /// </summary>
    public class ArtworkDBNotFoundException : BaseClickwheelException
    {
        public ArtworkDBNotFoundException()
            : base("iPod ArtworkDB not found. You cannot add or remove artwork.")
        {
            Category = "Artwork Problem";
        }
    }
}
