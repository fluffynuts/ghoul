using System;
using System.Collections.Generic;
using PeanutButter.Utils;
using System.Diagnostics;

namespace Ghoul.Ui
{
    public interface ILogger
    {
        void LogError(string message, Exception ex);
        void LogInfo(string message);
        void LogWarning(string message);
    }
    
    public class Logger: ILogger
    {
        public void LogError(string message, Exception ex)
        {
            var finalMessage = CollectFullMessageFrom(message, ex);
            Log(EventLogEntryType.Error, finalMessage);
        }

        public void LogInfo(string message)
        {
            Log(EventLogEntryType.Information, message);
        }

        public void LogWarning(string message)
        {
            Log(EventLogEntryType.Warning, message);
        }

        private void Log(EventLogEntryType type, string message)
        {
            EventLog.WriteEntry("Ghoul", message, type);
        }

        private string CollectFullMessageFrom(string operation, Exception exception)
        {
            var lines = new List<string>
            {
                $"Exception encountered: {operation}:"
            };
            lines.AddRange(CollectExceptionMessage(exception));
            return lines.JoinWith(Environment.NewLine);
        }

        private IEnumerable<string> CollectExceptionMessage(Exception exception)
        {
            var lines = new List<string>
            {
                exception.Message,
                "Stacktrace:",
                exception.StackTrace
            };
            if (exception.InnerException != null)
            {
                lines.AddRange(CollectExceptionMessage(exception.InnerException));
            }
            return lines;
        }
    }
}