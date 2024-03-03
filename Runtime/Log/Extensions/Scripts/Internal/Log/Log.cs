using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F8Framework.Core
{
    public class Log : SingletonMono<Log>
    {
        public class LogData
        {
            public string message = string.Empty;
            public LogType logType = LogType.Log;
            public string stackTrace = string.Empty;
            public DateTime localTime = DateTime.Now;
            public string playTime = string.Empty;
            public string sceneName = string.Empty;
        }

        public class ReceivedLog
        {
            public ReceivedLog(string condition, string stackTrace, LogType type)
            {
                this.condition = condition;
                this.stackTrace = stackTrace;
                this.type = type;
            }

            public string condition;
            public string stackTrace;
            public LogType type;
        }

        private const float MAX_DECIMAL_SIZE = 10000F;

        private Dictionary<int, LogData> fullLogs = new Dictionary<int, LogData>();
        private Dictionary<string, List<int>> logCategories = new Dictionary<string, List<int>>();
        private List<LogData> currentLogs = new List<LogData>();
        private string currentCategory = LogConst.DEFAULT_CATEGORY_NAME;
        private string logFilter = string.Empty;
        private Dictionary<LogType, bool> currentLogTypes = new Dictionary<LogType, bool>();
        private Dictionary<LogType, int> logCountPerType = new Dictionary<LogType, int>();
        private readonly object logLock = new object();
        private Action<LogData> logNotificationCallback = null;
        private int categoryDelemiterLength = LogConst.CATEGORY_DELIMITER.Length;
        private string logFilePath = string.Empty;
        private bool filterIgnoreCase = true;
        private bool isQuit = false;

        static private Queue<ReceivedLog> receivedLogQueue = new Queue<ReceivedLog>();

        protected override void Init()
        {
            logFilePath = Path.Combine(Application.persistentDataPath, LogConst.LOGFILE_NAME);

            currentLogTypes[LogType.Log] = true;
            currentLogTypes[LogType.Warning] = true;
            currentLogTypes[LogType.Error] = true;
            currentLogTypes[LogType.Assert] = true;
            currentLogTypes[LogType.Exception] = true;

            ClearLogCountPerType();
        }

        private void OnEnable()
        {
            Application.logMessageReceivedThreaded += UnityLogReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceivedThreaded -= UnityLogReceived;
        }

        private void OnApplicationQuit()
        {
            isQuit = true;
        }

        public void SetFilterIgnoreCase(bool ignore)
        {
            lock (logLock)
            {
                filterIgnoreCase = ignore;
                RefreshCurrentLog();
            }
        }

        public int GetLogCount(LogType type)
        {
            return logCountPerType[type];
        }

        public List<LogData> GetCurrentLogs()
        {
            lock (logLock)
            {
                return currentLogs;
            }
        }

        public void SendFullLogToMail(SendCompletedEventHandler callback)
        {
            WriteFullLogToFile(exception =>
            {
                if (exception != null)
                {
                    callback(null, new AsyncCompletedEventArgs(exception, false, string.Empty));
                }
                else
                {
                    FileStream stream = null;
                    Action clearStream = () =>
                    {
                        if (stream != null)
                        {
                            stream.Close();
                            stream.Dispose();
                        }
                    };

                    try
                    {
                        stream = File.Open(logFilePath, FileMode.Open);
                        Function.Instance.SendMail(
                            string.Format(LogConst.LOGMAIL_SUBJECT_FORMAT, Application.productName,
                                DateTime.Now.ToString("G")),
                            LogConst.LOGMAIL_BODY, stream, LogConst.LOGFILE_NAME,
                            (object sender, AsyncCompletedEventArgs e) =>
                            {
                                clearStream();

                                callback(sender, e);
                            });
                    }
                    catch (Exception e)
                    {
                        clearStream();

                        callback(null, new AsyncCompletedEventArgs(e, false, string.Empty));
                    }
                }
            });
        }

        public void WriteFullLogToFile(Action<Exception> callback)
        {
            StartCoroutine(GetFullLogInformation(logInformation =>
            {
                try
                {
                    using (var stream = new FileStream(logFilePath, FileMode.Create, FileAccess.ReadWrite,
                               FileShare.ReadWrite | FileShare.Delete))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.WriteLine(SystemInformation.Instance.ToString());
                            writer.WriteLine(logInformation);
                        }
                    }

                    callback(null);
                }
                catch (Exception exception)
                {
                    callback(exception);
                }
            }));
        }

        IEnumerator GetFullLogInformation(Action<string> callback)
        {
            LogData[] logData = null;

            lock (logLock)
            {
                logData = new LogData[fullLogs.Values.Count];
                fullLogs.Values.CopyTo(logData, 0);
            }

            StringBuilder logInformation = new StringBuilder();
            LogData log = null;
            for (int index = 0; index < logData.Length; ++index)
            {
                yield return null;

                log = logData[index];
                logInformation.AppendFormat("=> [{0}] [{1}] {2} [Play time : {3}] [Scene : {4}]",
                    log.logType.ToString(), log.localTime.ToString("G"), log.message, log.playTime, log.sceneName);
                logInformation.AppendLine();
                logInformation.AppendLine(log.stackTrace);
                logInformation.AppendLine().AppendLine();
            }

            callback(logInformation.ToString());
        }

        public string MakeLogMessageWithCategory(string message, string category)
        {
            var result = new StringBuilder();

            result.AppendFormat("{0}{1}{2}{3}", LogConst.CATEGORY_DELIMITER, category, LogConst.CATEGORY_DELIMITER,
                message);

            return result.ToString();
        }

        public string[] GetCategories()
        {
            lock (logLock)
            {
                var keyCollection = logCategories.Keys;
                string[] categories = new string[keyCollection.Count];

                logCategories.Keys.CopyTo(categories, 0);

                return categories;
            }
        }

        public void SetCurrentCategory(int index)
        {
            lock (logLock)
            {
                if (index >= 0 && index < logCategories.Count)
                {
                    string newCategory = logCategories.ElementAt(index).Key;
                    if (currentCategory.Equals(newCategory, StringComparison.Ordinal) == false)
                    {
                        currentCategory = newCategory;
                        RefreshCurrentLog();
                    }
                }
            }
        }

        public void SetCurrentCategory(string category)
        {
            lock (logLock)
            {
                if (currentCategory.Equals(category, StringComparison.Ordinal) == true)
                {
                    return;
                }

                bool isCategoryChanged = false;

                if (category.Equals(LogConst.DEFAULT_CATEGORY_NAME, StringComparison.Ordinal) == true)
                {
                    isCategoryChanged = true;
                }
                else
                {
                    if (logCategories.ContainsKey(category) == true)
                    {
                        isCategoryChanged = true;
                    }
                }

                if (isCategoryChanged == true)
                {
                    currentCategory = category;
                    RefreshCurrentLog();
                }
            }
        }

        public void AddLogNotificationCallback(Action<LogData> callback)
        {
            logNotificationCallback += callback;
        }

        public void RemoveLogNotificationCallback(Action<LogData> callback)
        {
            logNotificationCallback -= callback;
        }

        public void SetLogTypeEnable(LogType logType, bool enable)
        {
            lock (logLock)
            {
                if (currentLogTypes[logType] != enable)
                {
                    currentLogTypes[logType] = enable;
                    RefreshCurrentLog();
                }
            }
        }

        public void SetFilter(string filter)
        {
            lock (logLock)
            {
                if (logFilter.Equals(filter, StringComparison.Ordinal) == false)
                {
                    logFilter = filter;

                    RefreshCurrentLog();
                }
            }
        }

        private void RefreshCurrentLog()
        {
            currentLogs.Clear();
            if (currentCategory.Equals(LogConst.DEFAULT_CATEGORY_NAME, StringComparison.Ordinal) == true)
            {
                foreach (KeyValuePair<int, LogData> kvp in fullLogs)
                {
                    if (IsCurrentLogType(kvp.Value.logType) == false)
                    {
                        continue;
                    }

                    if (IsFilteringLog(kvp.Value.message) == false)
                    {
                        continue;
                    }

                    currentLogs.Add(kvp.Value);
                }
            }
            else
            {
                List<int> logKeys = null;
                if (logCategories.TryGetValue(currentCategory, out logKeys) == true)
                {
                    int key = 0;
                    LogData data = null;
                    for (int index = 0; index < logKeys.Count; ++index)
                    {
                        key = logKeys[index];
                        data = fullLogs[key];

                        if (IsCurrentLogType(data.logType) == false)
                        {
                            continue;
                        }

                        if (IsFilteringLog(data.message) == false)
                        {
                            continue;
                        }

                        currentLogs.Add(data);
                    }
                }
            }
        }

        public void ClearLog()
        {
            lock (logLock)
            {
                fullLogs.Clear();

                foreach (KeyValuePair<string, List<int>> kvp in logCategories)
                {
                    kvp.Value.Clear();
                }

                logCategories.Clear();
                currentLogs.Clear();

                ClearLogCountPerType();
            }
        }

        private void ClearLogCountPerType()
        {
            logCountPerType.Clear();

            logCountPerType[LogType.Log] = 0;
            logCountPerType[LogType.Warning] = 0;
            logCountPerType[LogType.Error] = 0;
            logCountPerType[LogType.Assert] = 0;
            logCountPerType[LogType.Exception] = 0;
        }

        private void Update()
        {
            while (receivedLogQueue.Count > 0)
            {
                ReceivedLog receivedLog = null;
                lock (receivedLogQueue)
                {
                    if (receivedLogQueue.Count > 0)
                    {
                        receivedLog = receivedLogQueue.Dequeue();
                    }
                }

                if (receivedLog != null)
                {
                    UnityLogReceived(receivedLog);
                }
            }
        }

        private void UnityLogReceived(ReceivedLog receivedLog)
        {
            if (isQuit == true)
            {
                return;
            }

            lock (logLock)
            {
                string message = string.Empty;
                string category = string.Empty;
                ParsingCategoryInLog(receivedLog.condition, ref message, ref category);

                float realtimeSinceStartup = Time.realtimeSinceStartup;

                string playTime;
                if (realtimeSinceStartup >= MAX_DECIMAL_SIZE)
                {
                    playTime = realtimeSinceStartup.ToString("F0");
                }
                else
                {
                    playTime = realtimeSinceStartup.ToString("F");
                }

                LogData data = new LogData
                {
                    message = message,
                    stackTrace = receivedLog.stackTrace,
                    logType = receivedLog.type,
                    playTime = playTime,
                    sceneName = SceneManager.GetActiveScene().name
                };

                AddLog(data, category);
            }
        }

        private void UnityLogReceived(string condition, string stackTrace, LogType type)
        {
            if (isQuit == true)
            {
                return;
            }

            lock (receivedLogQueue)
            {
                receivedLogQueue.Enqueue(new ReceivedLog(condition, stackTrace, type));
            }
        }

        private void AddLog(LogData data, string category)
        {
            int currentLogKey = AddToFullLog(data);

            AddToCategory(data, category, currentLogKey);
            AddToCurrentLog(data, category);

            logCountPerType[data.logType] += 1;

            if (logNotificationCallback != null)
            {
                logNotificationCallback(data);
            }
        }

        private int AddToFullLog(LogData data)
        {
            int currentLogKey = fullLogs.Count;

            fullLogs[currentLogKey] = data;

            return currentLogKey;
        }

        private void AddToCategory(LogData data, string category, int logKey)
        {
            if (data.logType == LogType.Exception || data.logType == LogType.Assert)
            {
                foreach (KeyValuePair<string, List<int>> kvp in logCategories)
                {
                    kvp.Value.Add(logKey);
                }
            }
            else
            {
                if (category.Equals(LogConst.DEFAULT_CATEGORY_NAME, StringComparison.Ordinal) == false)
                {
                    List<int> logKeys = null;
                    if (logCategories.TryGetValue(category, out logKeys) == false)
                    {
                        logKeys = new List<int>();
                    }

                    logKeys.Add(logKey);
                    logCategories[category] = logKeys;
                }
            }
        }

        private void AddToCurrentLog(LogData data, string category)
        {
            if (data.logType == LogType.Exception || data.logType == LogType.Assert)
            {
                currentLogs.Add(data);
            }
            else
            {
                if (IsCurrentLogType(data.logType) == false)
                {
                    return;
                }

                bool isDefaultCategory =
                    currentCategory.Equals(LogConst.DEFAULT_CATEGORY_NAME, StringComparison.Ordinal);
                bool isCurrentCategory = currentCategory.Equals(category, StringComparison.Ordinal);
                if (isDefaultCategory == false && isCurrentCategory == false)
                {
                    return;
                }

                if (IsFilteringLog(data.message) == false)
                {
                    return;
                }

                currentLogs.Add(data);
            }
        }

        private bool IsCurrentLogType(LogType logType)
        {
            return currentLogTypes[logType];
        }

        private bool IsFilteringLog(string message)
        {
            if (string.IsNullOrEmpty(logFilter) == true)
            {
                return true;
            }
            else
            {
                if (message.IndexOf(logFilter,
                        filterIgnoreCase == true ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) != -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void ParsingCategoryInLog(string original, ref string message, ref string category)
        {
            category = LogConst.DEFAULT_CATEGORY_NAME;
            message = original;

            if (original.StartsWith(LogConst.CATEGORY_DELIMITER, StringComparison.Ordinal) == true)
            {
                int endIndex = original.IndexOf(LogConst.CATEGORY_DELIMITER, categoryDelemiterLength);

                if (endIndex != -1)
                {
                    int categoryLength = endIndex - categoryDelemiterLength;

                    category = original.Substring(categoryDelemiterLength, categoryLength);
                    message = original.Substring(endIndex + categoryDelemiterLength);
                }
            }
        }
    }
}