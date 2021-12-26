using System;

namespace Core.Domain.Tools
{
    public class ReportsAppException : Exception
    {
        public ReportsAppException(string message)
            : base(message)
        {
        }
    }
}