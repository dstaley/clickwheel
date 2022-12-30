using Clickwheel.Parsers;

namespace Clickwheel.Exceptions
{
    /// <summary>
    /// Thrown when the iPod database version is below 0x14. iTunes 7.1 and above create 0x14(+) databases.
    /// If the version is below 0x14, Clickwheel will try and read it, but will not enable modifications.
    /// </summary>
    public class UnsupportedITunesVersionException : BaseClickwheelException
    {
        private CompatibilityType _compatibility;

        public UnsupportedITunesVersionException(string message, CompatibilityType compatibility)
            : base(message)
        {
            Category = "iPod database not supported";
            _compatibility = compatibility;
        }

        public CompatibilityType Compatibility => _compatibility;
    }
}
