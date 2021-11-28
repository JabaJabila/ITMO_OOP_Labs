using System;
using System.Globalization;

namespace BackupsExtra.Loggers
{
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger(bool timeCodeRequired = false)
        {
            TimeCodeRequired = timeCodeRequired;
        }

        public bool TimeCodeRequired { get; }

        public void LogMessage(string message)
        {
            Console.WriteLine($"{GetTimeCode()}{message}");
        }

        public void LogException(Exception exception)
        {
            Console.WriteLine($"{GetTimeCode()}{exception.Message}");
        }

        private string GetTimeCode()
        {
            return TimeCodeRequired
                ? $"[{DateTime.Now.ToString("g", CultureInfo.CurrentCulture)}] - "
                : string.Empty;
        }
    }
}