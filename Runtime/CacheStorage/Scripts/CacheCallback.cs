using System;

namespace F8Framework.Core
{
    static class CacheCallback
    {
        public static void SafeCallback<T>(this Action<T> callback, T result)
        {
            if (callback != null)
            {
                try
                {
                    object target = callback.Target;
                    if (target == null)
                    {
                        if (callback.Method?.IsStatic == false)
                        {
                            return;
                        }
                    }
                    else if (target is UnityEngine.Object obj &&
                             obj.Equals(null) == true)
                    {
                        return;
                    }

                    callback(result);
                }
                catch (Exception exception)
                {
                    LogF8.LogError(exception);
                }
            }
        }
    }
}