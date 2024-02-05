using UnityEngine;
using System.Diagnostics;
using System.Net;
using System;
using System.Collections.Generic;
using System.Text;
using F8Framework.Core;
using Debug = UnityEngine.Debug;

namespace F8Framework.Core
{
    public static class LogF8
    {
        private static Stopwatch watch = new Stopwatch();
        private static WebClient m_webClient = new WebClient();
        private static List<string> m_errorList = new List<string>();
        private static bool m_canTakeError = true;
        private static bool m_isInit = false;
        private static int counter = 0;
        private static StringBuilder sb = new StringBuilder();
        
        public static void EnabledLog()
        {
            UnityEngine.Debug.unityLogger.logEnabled = true;
            Debug.unityLogger.filterLogType = LogType.Error | LogType.Assert | LogType.Warning | LogType.Log | LogType.Exception;
        }

        public static void DisableLog()
        {
            UnityEngine.Debug.unityLogger.logEnabled = false;
            Debug.unityLogger.filterLogType = LogType.Error;
        }
        
        public static void GetCrashErrorMessage()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string traceInfo = new StackTrace((Exception)(e.ExceptionObject)).ToString();
            AddError(traceInfo);
        }
        
        public static void Log(string s, params object[] p)
        {
            sb.Clear();
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);

            Debug.Log(sb.ToString());
        }
        
        public static void Log(object o)
        {
            Debug.Log(o);
        }

        public static void LogNet(string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.yellow));
            sb.Append(DateTime.Now);
            sb.Append("[网络日志]");
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }
        
        public static void LogNet(object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.yellow));
            sb.Append(DateTime.Now);
            sb.Append("[网络日志]");
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }
        
        public static void LogConfig(string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.grey));
            sb.Append(DateTime.Now);
            sb.Append("[配置日志]");
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }
        
        public static void LogConfig(object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.grey));
            sb.Append(DateTime.Now);
            sb.Append("[配置日志]");
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }
        
        public static void LogView(string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.magenta));
            sb.Append(DateTime.Now);
            sb.Append("[视图日志]");
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }
        
        public static void LogView(object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.magenta));
            sb.Append(DateTime.Now);
            sb.Append("[视图日志]");
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }
        
        public static void LogEvent(string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.cyan));
            sb.Append(DateTime.Now);
            sb.Append("[事件日志]");
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }
        
        public static void LogEvent(object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.cyan));
            sb.Append(DateTime.Now);
            sb.Append("[事件日志]");
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }

        public static void LogEntity(string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.blue));
            sb.Append(DateTime.Now);
            sb.Append("[实体日志]");
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }
        
        public static void LogEntity(object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.blue));
            sb.Append(DateTime.Now);
            sb.Append("[实体日志]");
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }
        
        public static void LogAsset(string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.green));
            sb.Append(DateTime.Now);
            sb.Append("[资产日志]");
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }
        
        public static void LogAsset(object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(Color.green));
            sb.Append(DateTime.Now);
            sb.Append("[资产日志]");
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }
        
        public static void LogColor(Color color, string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(color));
            sb.Append(DateTime.Now);
            sb.Append("[颜色日志]");
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }
        public static void LogColor(Color color, object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(color));
            sb.Append(DateTime.Now);
            sb.Append("[颜色日志]");
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }
        public static void LogToMainThread(string s, params object[] p)
        {
            string msg = (p != null && p.Length > 0 ? string.Format(s, p) : s);
            F8LogHelper.Instance.LogToMainThread(LogType.Log, msg);
        }

        public static void Assert(bool condition, string s, params object[] p)
        {
            if (condition)
            {
                return;
            }

            LogError("Assert failed! Message:\n" + s, p);
        }

        public static void LogError(string s, params object[] p)
        {
#if UNITY_EDITOR
            Debug.LogError((p != null && p.Length > 0 ? string.Format(s, p) : s));
#endif
        }
        
        public static void LogError(object o)
        {
#if UNITY_EDITOR
            Debug.LogError(o);
#endif
        }
        
        public static void LogErrorToMainThread(string s, params object[] p)
        {
            string msg = (p != null && p.Length > 0 ? string.Format(s, p) : s);
            F8LogHelper.Instance.LogToMainThread(LogType.Error, msg);
        }


        public static void LogStackTrace(string str)
        {
            StackFrame[] stacks = new StackTrace().GetFrames();
            string result = str + "\n\n";

            if (stacks != null)
            {
                for (int i = 0; i < stacks.Length; i++)
                {
                    result += string.Format("{0} {1}\n\n", stacks[i].GetFileName(), stacks[i].GetMethod().ToString());
                }
            }

            LogError(result);
        }

        public static void AddError(string errorStr)
        {
            if (!string.IsNullOrEmpty(errorStr))
            {
                m_errorList.Add(errorStr);
            }

            CheckReportError();
        }

        private static void SendToHttpSvr(string postData)
        {
            if (!string.IsNullOrEmpty(postData))
            {
                if (!m_isInit)
                {
                    m_webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(OnUploadStringCompleted);
                    m_isInit = true;
                }

                if (string.IsNullOrEmpty(URLSetting.REPORT_ERROR_URL))
                {
                    Debug.LogError("REPORT_ERROR_URL is empty");
                    return;
                }
                m_webClient.UploadStringAsync(new Uri(URLSetting.REPORT_ERROR_URL), "POST", postData);
            }
        }

        public static void CheckReportError()
        {
            counter++;

            if (counter % 5 == 0 && m_canTakeError)
            {
                DealWithReportError();
                counter = (counter > 1000) ? 0 : counter;
            }
        }

        private static void DealWithReportError()
        {
            sb.Clear();
            int errorCount = m_errorList.Count;
            if (errorCount > 0)
            {
                m_canTakeError = false;
                counter = 0;
                sb.Length = 0;
                for (int i = 0; i < errorCount; i++)
                {
                    sb.Append(m_errorList[i] + " | \n");
                }

                m_errorList.Clear();
                SendToHttpSvr(sb.ToString());
            }
        }

        private static void OnUploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            m_canTakeError = true;
        }

        public static void Watch()
        {
#if UNITY_EDITOR
            watch.Reset();
            watch.Start();
#endif
        }

        public static long UseTime
        {
            get
            {
#if UNITY_EDITOR
            return watch.ElapsedMilliseconds;
#else
            return 0;
#endif
            }
        }

        public static string UseMemory
        {
            get
            {
#if UNITY_EDITOR
            #if UNITY_5_6_OR_NEWER
                return (UnityEngine.Profiling.Profiler.usedHeapSizeLong / 1024).ToString() + " kb";
            #elif UNITY_5_5_OR_NEWER
                return (UnityEngine.Profiling.Profiler.usedHeapSize / 1024).ToString() + " kb";
            #else
                return (Profiler.usedHeapSize / 1024).ToString() + " kb";
            #endif
#else
            return "0";
#endif
            }
        }
    }
}