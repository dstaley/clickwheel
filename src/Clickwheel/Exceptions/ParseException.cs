using System;

namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when the iPod database format could not be recognized or validated.
    /// This could occur if the iTunes file format changes or a 3rd party application has written the
    /// database in a different way to iTunes.
    /// </summary>
    public class ParseException : BaseClickwheelException
    {
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
            Category = "Your iPod database could not be read";
        }
    }
}
