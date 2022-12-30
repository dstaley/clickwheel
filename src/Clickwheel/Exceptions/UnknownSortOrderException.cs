namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown if a Playlist's SortOrder field is not a value enumerated by Clickwheel.
    /// </summary>
    public class UnknownSortOrderException : BaseClickwheelException
    {
        public UnknownSortOrderException(string message) : base(message)
        {
            Category = "The playlist's Sort Order value is not supported";
        }
    }
}
