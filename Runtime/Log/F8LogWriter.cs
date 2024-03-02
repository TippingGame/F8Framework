using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace F8Framework.Core
{
    [UpdateRefresh]
    public class F8LogWriter : ModuleSingleton<F8LogWriter>, IModule
    {
        private int MAX_LOG_FILE_CNT = 7;
        private float LOG_TIME = 0.5f;
        private bool isEnableLog = false;

        struct log_info
        {
            public LogType type;
            public string msg;
            public string stackTrace;

            public log_info(LogType type, string msg, string stackTrace = null)
            {
                this.type = type;
                this.msg = msg;
                this.stackTrace = stackTrace;
            }
        }

        private List<log_info> backList = new List<log_info>(100);
        private List<log_info> frontList = new List<log_info>(100);

        private List<log_info> logInfos = new List<log_info>();

        private StreamWriter writer;
        private float logTime;

        public void OnInit(object createParam)
        {
            var nowTime = DateTime.Now;
            
            if (Application.isEditor) return;
            
            Application.logMessageReceived += (LogHandler);

            var files = Directory.GetFiles(Application.persistentDataPath, "log-*.txt",
                SearchOption.TopDirectoryOnly);
            if (files.Length > MAX_LOG_FILE_CNT)
            {
                for (int i = 0; i < files.Length - MAX_LOG_FILE_CNT; i++)
                {
                    File.Delete(files[i]);
                }
            }

            var logFilePath = string.Format("{0}/log-{1}-{2}-{3}.txt", Application.persistentDataPath, nowTime.Year,
                nowTime.Month, nowTime.Day);
                
            writer = new StreamWriter(logFilePath, true, Encoding.UTF8);

            logTime = 0;
        }

        public void OnUpdate()
        {
            if (!Application.isEditor && isEnableLog)
            {
                if (Time.realtimeSinceStartup - logTime > LOG_TIME)
                {
                    lock (backList)
                    {
                        if (logInfos.Count > 0)
                        {
                            for (int i = 0; i < logInfos.Count; i++)
                            {
                                var logInfo = logInfos[i];
                                LogOneMessage(logInfo.type, logInfo.msg, logInfo.stackTrace);
                            }

                            logInfos.Clear();
                            writer.Flush();
                        }
                    }

                    lock (backList)
                    {
                        if (backList.Count > 0)
                        {
                            (frontList, backList) = (backList, frontList);
                        }
                    }

                    if (frontList.Count > 0)
                    {
                        for (int i = 0; i < frontList.Count; i++)
                        {
                            var logInfo = frontList[i];
                            LogOneMessage(logInfo.type, logInfo.msg);
                        }

                        frontList.Clear();
                        writer.Flush();
                    }

                    logTime = Time.realtimeSinceStartup;
                }
            }
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            base.Destroy();
        }
        
        public void OnEnterGame()
        {
            isEnableLog = true;
        }

        public void OnQuitGame()
        {
            isEnableLog = false;
            lock (backList)
            {
                backList.Clear();
            }

            frontList.Clear();
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }
        private void LogOneMessage(LogType type, string msg, string stackTrace = null)
        {
            var logStackTrace = false;
            var typeStr = "";
            switch (type)
            {
                case LogType.Log:
                    typeStr = "L";
                    break;
                case LogType.Warning:
                    typeStr = "W";
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    typeStr = "E";
                    logStackTrace = true;
                    break;
            }

            var str = string.Format("[LOG]|{0}|{1}|{2}\n", DateTime.Now, typeStr, msg);
            if (logStackTrace && stackTrace != null)
            {
                str += stackTrace;
            }

            writer.Write(str);
        }

        private void LogHandler(string condition, string stackTrace, LogType type)
        {
            if (Application.isEditor)
            {
                return;
            }
            if (type != LogType.Warning)
            {
                logInfos.Add(new log_info(type, condition, stackTrace));
            }
        }
        
        public void LogToMainThread(LogType type, string msg)
        {
            lock (backList)
            {
                backList.Add(new log_info(type, msg));
            }
        }

    }
}

