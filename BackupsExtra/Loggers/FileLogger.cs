using System;
using System.Globalization;
using System.IO;
using BackupsExtra.Tools;

namespace BackupsExtra.Loggers
{
    public class FileLogger : ILogger
    {
        public FileLogger(string pathToFile, bool timeCodeRequired = false)
        {
            PathToFile = pathToFile ?? throw new ArgumentNullException(nameof(pathToFile));
            if (!Directory.Exists(new FileInfo(pathToFile).DirectoryName))
                throw new LoggerException($"File at {pathToFile} unreachable!");

            TimeCodeRequired = timeCodeRequired;
        }

        public string PathToFile { get; }
        public bool TimeCodeRequired { get; }

        public void LogMessage(string message)
        {
            using StreamWriter sw = File.AppendText(PathToFile);
            sw.WriteLine($"{GetTimeCode()}{message}");
        }

        public void LogException(Exception exception)
        {
            using StreamWriter sw = File.AppendText(PathToFile);
            sw.WriteLine($"{GetTimeCode()}{exception.Message}");
        }

        private string GetTimeCode()
        {
            return TimeCodeRequired
                ? $"[{DateTime.Now.ToString("g", CultureInfo.CurrentCulture)}] - "
                : string.Empty;
        }
    }
}