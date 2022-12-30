namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when the iPod is not supported. This can be achieved by setting iPod.IsWritable=false.
    ///
    /// </summary>
    public class UnsupportedIPodException : BaseClickwheelException
    {
        public UnsupportedIPodException(string message) : base(message)
        {
            Category = "iPod not fully supported";
        }
    }
}
