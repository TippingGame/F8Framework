using UnityEngine;
using System.Diagnostics;
using System.Net;
using System;
using System.Collections.Generic;
using System.Text;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

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
        
        // 调色板
        private static readonly Color color1 = new Color(.14f, .65f, 1.00f);
        private static readonly Color color2 = new Color(.89f, .12f, .12f);
        private static readonly Color color3 = new Color(.09f, .85f, .43f);
        private static readonly Color color4 = new Color(.80f, .24f, 1.00f);
        private static readonly Color color5 = new Color(1.00f, .79f, .0f);
        private static readonly Color color6 = new Color(.09f, .80f, .85f);
        private static readonly Color color7 = new Color(.66f, .85f, .09f);
        private static readonly Color color8 = new Color(1.0f, 0.63f, 0.66f);
        private static readonly Color color9 = new Color(1.0f, 0.62f, 0.35f);
        private static readonly Color color10 = new Color(0.86f, 0.55f, 0.92f);
        private static readonly Color color11 = new Color(1.0f, 0.90f, 0.40f);
        private static readonly Color color12 = new Color(1.0f, 0.9215f, 0.0156f);
        private static readonly Color color13 = new Color(0.0f, 1.0f, 1.0f);
        private static readonly Color color14 = new Color(0.0f, 1.0f, 0.0f);
        
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
        
        public static void Log(string s, Object context)
        {
            sb.Clear();
            sb.Append(s);

            Debug.Log(sb.ToString(), context);
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

        public static void LogUtil(string s, Object context)
        {
            LogColor("[工具日志]", color10, s, context);
        }
        
        public static void LogUtil(string s, params object[] p)
        {
            LogColor("[工具日志]", color10, s, p);
        }
        
        public static void LogUtil(object o)
        {
            LogColor("[工具日志]", color10, o);
        }
        
        public static void LogVersion(string s, Object context)
        {
            LogColor("[版本日志]", color9, s, context);
        }
        
        public static void LogVersion(string s, params object[] p)
        {
            LogColor("[版本日志]", color9, s, p);
        }
        
        public static void LogVersion(object o)
        {
            LogColor("[版本日志]", color9, o);
        }
        
        public static void LogSDK(string s, Object context)
        {
            LogColor("[SDK日志]", color8, s, context);
        }
        
        public static void LogSDK(string s, params object[] p)
        {
            LogColor("[SDK日志]", color8, s, p);
        }
        
        public static void LogSDK(object o)
        {
            LogColor("[SDK日志]", color8, o);
        }
        
        public static void LogModule(string s, Object context)
        {
            LogColor("[模块日志]", color1, s, context);
        }
        
        public static void LogModule(string s, params object[] p)
        {
            LogColor("[模块日志]", color1, s, p);
        }
        
        public static void LogModule(object o)
        {
            LogColor("[模块日志]", color1, o);
        }
        
        public static void LogNet(string s, Object context)
        {
            LogColor("[网络日志]", color5, s, context);
        }
        
        public static void LogNet(string s, params object[] p)
        {
            LogColor("[网络日志]", color5, s, p);
        }
        
        public static void LogNet(object o)
        {
            LogColor("[网络日志]", color5, o);
        }
        
        public static void LogConfig(string s, Object context)
        {
            LogColor("[配置日志]", color7, s, context);
        }
        
        public static void LogConfig(string s, params object[] p)
        {
            LogColor("[配置日志]", color7, s, p);
        }
        
        public static void LogConfig(object o)
        {
            LogColor("[配置日志]", color7, o);
        }
        
        public static void LogView(string s, Object context)
        {
            LogColor("[视图日志]", color12, s, context);
        }
        
        public static void LogView(string s, params object[] p)
        {
            LogColor("[视图日志]", color12, s, p);
        }
        
        public static void LogView(object o)
        {
            LogColor("[视图日志]", color12, o);
        }
        
        public static void LogEvent(string s, Object context)
        {
            LogColor("[事件日志]", color13, s, context);
        }
        
        public static void LogEvent(string s, params object[] p)
        {
            LogColor("[事件日志]", color13, s, p);
        }
        
        public static void LogEvent(object o)
        {
            LogColor("[事件日志]", color13, o);
        }

        public static void LogEntity(string s, Object context)
        {
            LogColor("[实体日志]", color3, s, context);
        }
        
        public static void LogEntity(string s, params object[] p)
        {
            LogColor("[实体日志]", color3, s, p);
        }
        
        public static void LogEntity(object o)
        {
            LogColor("[实体日志]", color3, o);
        }
        
        public static void LogAsset(string s, Object context)
        {
            LogColor("[资产日志]", color14, s, context);
        }
        
        public static void LogAsset(string s, params object[] p)
        {
            LogColor("[资产日志]", color14, s, p);
        }
        
        public static void LogAsset(object o)
        {
            LogColor("[资产日志]", color14, o);
        }
        
        private static void LogColor(string logName, Color color, string s, Object context)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(color));
            sb.Append(DateTime.Now);
            sb.Append(logName);
            sb.Append("</color>");
            sb.Append(s);
            Debug.Log(sb.ToString(), context);
        }
        
        private static void LogColor(string logName, Color color, string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(color));
            sb.Append(DateTime.Now);
            sb.Append(logName);
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }
        
        private static void LogColor(string logName, Color color, object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(color));
            sb.Append(DateTime.Now);
            sb.Append(logName);
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }
        
        public static void LogToMainThread(string s, params object[] p)
        {
            string msg = (p != null && p.Length > 0 ? string.Format(s, p) : s);
            F8LogWriter.Instance.LogToMainThread(LogType.Log, msg);
        }
        
        public static void LogException(Exception exception, Object context)
        {
            Debug.LogException(exception, context);
        }
        
        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
        
        public static void LogAssertion(string s, Object context)
        {
            Debug.LogAssertion(s, context);
        }
        
        public static void LogAssertion(string s, params object[] p)
        {
            Debug.LogAssertion((p != null && p.Length > 0 ? string.Format(s, p) : s));
        }
        
        public static void LogAssertion(object o)
        {
            Debug.LogAssertion(o);
        }

        public static void LogWarning(string s, Object context)
        {
            Debug.LogWarning(s, context);
        }
        
        public static void LogWarning(string s, params object[] p)
        {
            Debug.LogWarning((p != null && p.Length > 0 ? string.Format(s, p) : s));
        }
        
        public static void LogWarning(object o)
        {
            Debug.LogWarning(o);
        }
        
        public static void LogError(string s, Object context)
        {
            Debug.LogError(s, context);
        }
        
        public static void LogError(string s, params object[] p)
        {
            Debug.LogError((p != null && p.Length > 0 ? string.Format(s, p) : s));
        }
        
        public static void LogError(object o)
        {
            Debug.LogError(o);
        }
        
        public static void LogErrorToMainThread(string s, params object[] p)
        {
            string msg = (p != null && p.Length > 0 ? string.Format(s, p) : s);
            F8LogWriter.Instance.LogToMainThread(LogType.Error, msg);
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
            if (e.Error != null)
            {
                LogF8.LogError(e.Error.Message);
            }
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