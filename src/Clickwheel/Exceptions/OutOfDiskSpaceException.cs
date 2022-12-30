namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when adding tracks and adding new artwork to the iPod. Clickwheel will make sure there will be at least 10Mb of free space on the
    /// iPod after copying the track.
    /// </summary>
    public class OutOfDiskSpaceException : BaseClickwheelException
    {
        public OutOfDiskSpaceException(string message) : base(message)
        {
            Category = "The iPod cannot store any more files";
        }
    }
}
