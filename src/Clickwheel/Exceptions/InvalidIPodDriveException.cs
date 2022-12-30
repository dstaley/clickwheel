namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when an invalid drive is specified to a call to IPod.GetIPodByDrive()
    /// </summary>
    public class InvalidIPodDriveException : BaseClickwheelException
    {
        public InvalidIPodDriveException(string message) : base(message)
        {
            Category = "iPod Not Found";
        }
    }
}
