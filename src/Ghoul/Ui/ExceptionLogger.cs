using System;
using System.Collections.Generic;
using PeanutButter.Utils;

namespace Ghoul.Ui
{
    public interface IExceptionLogger
    {
        void Log(string operation, Exception ex);
    }
    
    public class ExceptionLogger: IExceptionLogger
    {
        public void Log(string operation, Exception ex)
        {
            var finalMessage = CollectFullMessageFrom(operation, ex);
            System.Diagnostics.EventLog.WriteEntry("Ghoul", finalMessage, System.Diagnostics.EventLogEntryType.Error);
                                                   
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