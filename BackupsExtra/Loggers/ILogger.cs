using System;

namespace BackupsExtra.Loggers
{
    public interface ILogger
    {
        void LogMessage(string message);
        void LogException(Exception exception);
    }
}