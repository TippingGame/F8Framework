namespace Excel.Log
{
    using System;
    using System.Collections.Generic;

    public static class LogManager
    {
        private static readonly Dictionary<string, ILog> _dictionary = new Dictionary<string, ILog>();
        private static object _sync = new object();

        public static ILog Log(string objectName)
        {
            ILog loggerFor = null;
            if (_dictionary.ContainsKey(objectName))
            {
                loggerFor = _dictionary[objectName];
            }
            if (loggerFor == null)
            {
                lock (_sync)
                {
                    if (_dictionary.ContainsKey(objectName))
                    {
                        loggerFor = _dictionary[objectName];
                    }
                    else
                    {
                        loggerFor = Excel.Log.Log.GetLoggerFor(objectName);
                        _dictionary.Add(objectName, loggerFor);
                    }
                    loggerFor = _dictionary[objectName];
                }
            }
            return loggerFor;
        }

        public static ILog Log<T>(T type) => 
            Log(typeof(T).FullName);
    }
}

