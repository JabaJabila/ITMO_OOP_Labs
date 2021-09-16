using System;

namespace Shops.Tools
{
    public class UIException : Exception
    {
        public UIException()
        {
        }

        public UIException(string message)
            : base(message)
        {
        }
    }
}