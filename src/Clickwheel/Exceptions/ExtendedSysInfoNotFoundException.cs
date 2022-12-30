namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown if ExtendedSysInfo was not found
    /// </summary>
    public class ExtendedSysInfoNotFoundException : BaseClickwheelException
    {
        public ExtendedSysInfoNotFoundException() : base("ExtendedSysInfo file not found on iPod.")
        {
            Category = "ExtendedSysInfo not found";
        }
    }
}
