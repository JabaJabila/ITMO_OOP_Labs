using System;

namespace Shops.Tools
{
    public class UiException : Exception
    {
        public UiException()
        {
        }

        public UiException(string message)
            : base(message)
        {
        }
    }
}