namespace F8Framework.Core
{
    public static partial class Utility
    {
        public static class Debug
        {
            public interface IDebugHelper
            {
                void LogInfo(object msg, object context);
                void LogInfo(object msg, DebugColor debugColor, object context);
                void LogWarning(object msg, object context);
                void LogError(object msg, object context);
            }
            static IDebugHelper debugHelper = null;

            static bool disableDebugLog;
            public static bool DisableDebugLog
            {
                get { return disableDebugLog; }
                set { disableDebugLog = value; }
            }
            public static void SetHelper(IDebugHelper helper)
            {
                debugHelper = helper;
            }
            public static void ClearHelper()
            {
                debugHelper = null;
            }
            public static void LogInfo(object msg, object context = null)
            {
                if (disableDebugLog)
                    return;
                debugHelper?.LogInfo(msg, context);
            }
            public static void LogInfo(object msg, DebugColor debugColor, object context = null)
            {
                if (disableDebugLog)
                    return;
                debugHelper?.LogInfo(msg, debugColor, context);
            }
            public static void LogWarning(object msg, object context = null)
            {
                if (disableDebugLog)
                    return;
                debugHelper?.LogWarning(msg, context);
            }
            public static void LogError(object o, object context = null)
            {
                if (disableDebugLog)
                    return;
                debugHelper?.LogError(o, context);
            }
        }
    }
}