using System;

namespace BackupsExtra.Loggers
{
    public class EmptyLogger : ILogger
    {
        public void LogMessage(string message)
        {
        }

        public void LogException(Exception exception)
        {
        }
    }
}