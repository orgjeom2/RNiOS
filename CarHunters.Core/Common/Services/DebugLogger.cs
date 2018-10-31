using System;
using Grpc.Core.Logging;

namespace CarHunters.Core.Common.Services
{
    public class DebugLogger : ILogger
    {
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Debug(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(format, formatArgs));
        }

        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Error(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(format, formatArgs));
        }

        public void Error(Exception exception, string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(exception.StackTrace);
        }

        public ILogger ForType<T>()
        {
            return this;
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Info(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(format, formatArgs));
        }

        public void Warning(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Warning(string format, params object[] formatArgs)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(format, formatArgs));
        }

        public void Warning(Exception exception, string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(exception.StackTrace);
        }
    }
}
