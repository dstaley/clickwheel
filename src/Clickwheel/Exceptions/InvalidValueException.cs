namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when an invalid value is specified for a track or playlist property
    /// </summary>
    public class InvalidValueException : BaseClickwheelException
    {
        public InvalidValueException(string message) : base(message)
        {
            Category = "Invalid Value Specified";
        }
    }
}
