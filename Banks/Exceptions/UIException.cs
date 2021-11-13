using System;

namespace Banks.Exceptions
{
    public class UIException : Exception
    {
        public UIException(string message)
            : base(message)
        {
        }
    }
}