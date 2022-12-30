using System;

namespace Clickwheel.Exceptions
{
    public class BaseClickwheelException : Exception
    {
        private string _category;

        public BaseClickwheelException(string message) : base(message)
        {
            _category = "Clickwheel Exception";
        }

        public BaseClickwheelException(string message, Exception innerException)
            : base(message, innerException)
        {
            _category = "Clickwheel Exception";
        }

        public string Category
        {
            get => _category;
            set => _category = value;
        }
    }
}
