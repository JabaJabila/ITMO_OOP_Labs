using System;

namespace Banks.Exceptions
{
    public class AccountException : Exception
    {
        public AccountException(string message)
            : base(message)
        {
        }
    }
}