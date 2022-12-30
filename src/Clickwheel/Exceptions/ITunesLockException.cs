namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when an iTunesLock file exists on the iPod. This usually means iTunes has locked the iPod
    /// and is currently syncing.
    /// </summary>
    public class ITunesLockException : BaseClickwheelException
    {
        public ITunesLockException(string lockFilePath)
            : base(
                $"iTunes has locked the iPod database. Please wait for iTunes to finish synchronizing. \r\nIf iTunes is not running, delete the '{lockFilePath}' file."
            )
        {
            Category = "iPod cannot be opened";
        }
    }
}
