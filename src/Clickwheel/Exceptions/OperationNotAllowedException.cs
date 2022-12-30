namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when (for example) trying to add/remove tracks from a Smart Playlist.
    /// </summary>
    public class OperationNotAllowedException : BaseClickwheelException
    {
        public OperationNotAllowedException(string message) : base(message)
        {
            Category = "Operation not allowed";
        }
    }
}
