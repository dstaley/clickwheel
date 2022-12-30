namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when an iPod couldn't be found during a call to GetConnectedIPod()
    /// </summary>
    public class IPodNotFoundException : BaseClickwheelException
    {
        public IPodNotFoundException(string message) : base(message)
        {
            Category = "iPod could not be found";
        }
    }
}
