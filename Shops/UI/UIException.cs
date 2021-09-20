using System;

namespace Shops.UI
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