using System;

namespace BackupsExtra.Tools
{
    public class LoggerException : Exception
    {
        public LoggerException(string message)
            : base(message)
        {
        }
    }
}