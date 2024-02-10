using System;
using UnityEngine;
using Object = UnityEngine.Object;
namespace F8Framework.Core
{
    public class StandaloneDebugHelper : Utility.Debug.IDebugHelper
    {
        readonly string logFullPath;
        /// <summary>
        /// UnityDebugHelper无参构造，不输出log信息到log文件；
        /// </summary>
        public StandaloneDebugHelper() { }
        /// <summary>
        /// UnityDebugHelper构造；
        /// </summary>
        /// <param name="logFullPath">log输出的完整路径</param>
        public StandaloneDebugHelper(string logFullPath)
        {
            Utility.Text.IsStringValid(logFullPath, "LogFullPath is invalid !");
            this.logFullPath = logFullPath;
            Utility.IO.WriteTextFile(logFullPath, "Head");
            UnityEngine.Application.logMessageReceived += UnityLog;
        }
        public void LogInfo(object msg, object context)
        {
            if (context == null)
                Debug.Log($"<b><color={DebugColor.cyan}>{"[INFO]-->>"} </color></b>{msg}");
            else
                Debug.Log($"<b><color={DebugColor.cyan}>{"[INFO]-->>"}</color></b>{msg}", context as Object);
        }
        public void LogInfo(object msg, DebugColor debugColor, object context)
        {
            if (context == null)
                Debug.Log($"<b><color={debugColor}>{"[INFO]-->>"}</color></b>{msg}");
            else
                Debug.Log($"<b><color={debugColor}>{"[INFO]-->>"}</color></b>{msg}", context as Object);
        }
        public void LogError(object msg, object context)
        {
            if (context == null)
                Debug.LogError($"<b><color={DebugColor.red}>{"[ERROR]-->>"} </color></b>{msg}");
            else
                Debug.LogError($"<b><color={DebugColor.red}>{"[ERROR]-->>"}</color></b>{msg}", context as Object);
        }

        public void LogWarning(object msg, object context)
        {
            if (context == null)
                Debug.LogWarning($"<b><color={DebugColor.orange}>{"[WARNING]-->>" }</color></b>{msg}");
            else
                Debug.LogWarning($"<b><color={DebugColor.orange}>{"[WARNING]-->>" }</color></b>{msg}", context as Object);
        }
        void UnityLog(string msgStr, string stackTrace, LogType logType)
        {
            string str = null;
            string[] splitedStr = null;
            try
            {
                splitedStr = Utility.Text.StringSplit(msgStr, new string[] { "</color></b>" });
            }
            catch { }
            switch (logType)
            {
                case LogType.Error:
                    {
                        if (splitedStr.Length > 1)
                            str = $"{DateTime.Now}[ - ] > ERROR : {splitedStr[1]};{stackTrace}";
                        else
                            str = $"{DateTime.Now}[ - ] > ERROR : {msgStr};{stackTrace}";
                    }
                    break;
                case LogType.Assert:
                    {
                        str = $"{DateTime.Now}[ - ] > ASSERT : {msgStr};{stackTrace}";
                    }
                    break;
                case LogType.Warning:
                    {
                        if (splitedStr.Length > 1)
                            str = $"{DateTime.Now}[ - ] > WARN : {splitedStr[1]};{stackTrace}";
                        else
                            str = $"{DateTime.Now}[ - ] > WARN : {msgStr};{stackTrace}";
                    }
                    break;
                case LogType.Log:
                    {
                        if (splitedStr.Length > 1)
                            str = $"{DateTime.Now}[ - ] > INFO : {splitedStr[1]};{stackTrace}";
                        else
                            str = $"{DateTime.Now}[ - ] > INFO : {msgStr};{stackTrace}";
                    }
                    break;
                case LogType.Exception:
                    {
                        str = $"{DateTime.Now}[ - ] > EXCEPTION : {msgStr};{stackTrace}";
                    }
                    break;
            }
            Utility.IO.AppendWriteTextFile(logFullPath, str);
        }
    }
}