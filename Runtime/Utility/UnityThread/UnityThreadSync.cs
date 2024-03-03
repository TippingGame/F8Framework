using System;
using System.Threading;

namespace F8Framework.Core
{
    /// <summary>
    /// unity线程同步类
    /// </summary>
    public class UnityThreadSync : SingletonMono<UnityThreadSync>
    {
        SynchronizationContext synchronizationContext;

        protected override void Init()
        {
            gameObject.hideFlags = UnityEngine.HideFlags.HideInHierarchy;
            DontDestroyOnLoad(gameObject);
            synchronizationContext = SynchronizationContext.Current;
        }

        public void PostToUnityThread(Action<object> postCallback)
        {
            synchronizationContext.Post(state => postCallback.Invoke(state), null);
        }

        public void SendToUnityThread(Action<object> postCallback)
        {
            synchronizationContext.Send(state => postCallback.Invoke(state), null);
        }
    }
}