using System;

namespace Shops.Tools
{
    public class PersonException : Exception
    {
        public PersonException()
        {
        }

        public PersonException(string message)
            : base(message)
        {
        }
    }
}