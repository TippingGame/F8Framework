namespace Excel.Log.Logger
{
    using Excel.Log;
    using System;

    public class NullLog : ILog, ILog<NullLog>
    {
        public void Debug(string message, params object[] formatting)
        {
        }

        public void Error(string message, params object[] formatting)
        {
        }

        public void Fatal(string message, params object[] formatting)
        {
        }

        public void Info(string message, params object[] formatting)
        {
        }

        public void InitializeFor(string loggerName)
        {
        }

        public void Warn(string message, params object[] formatting)
        {
        }
    }
}

