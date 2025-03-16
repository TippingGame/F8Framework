using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    public class BaseLoader : IEnumerator
    {
        public virtual bool LoaderSuccess => false;
        
        private Action onComplete;
        
        protected virtual void OnComplete()
        {
            onComplete?.Invoke();
            onComplete = null;
        }
        public void SetOnCompleted(Action onComplete) => this.onComplete = onComplete;
        
        public virtual T GetAssetObject<T>(string subAssetName = null) where T : Object
        {
            return null;
        }

        public virtual Object GetAssetObject(string subAssetName = null)
        {
            return null;
        }
        
        public virtual Dictionary<string, TObject> GetAllAssetObject<TObject>() where TObject : Object
        {
            return null;
        }
        
        public virtual Dictionary<string, Object> GetAllAssetObject()
        {
            return null;
        }
        
        bool IEnumerator.MoveNext()
        {
            return !LoaderSuccess;
        }

        object IEnumerator.Current
        {
            get
            {
                if (LoaderSuccess)
                {
                    LogF8.LogError("加载已完成，请使用GetAssetObject方法获取资产！");
                }

                return null;
            }
        }

        void IEnumerator.Reset() => throw new NotSupportedException();
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual LoaderAwaiter GetAwaiter() {
            return new LoaderAwaiter(this);
        }
    }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("这个结构是异步/等待支持所必需的，你不应该直接使用它。")]
    public readonly struct LoaderAwaiter : INotifyCompletion
    {
        private readonly BaseLoader loader;

        internal LoaderAwaiter(BaseLoader loader)
        {
            this.loader = loader;
        }

        public bool IsCompleted => loader.LoaderSuccess;  // 判断是否完成

        // 必须实现 INotifyCompletion 接口
        public void OnCompleted(Action continuation)
        {
            loader.SetOnCompleted(() =>
            {
                try
                {
                    continuation?.Invoke();
                }
                catch (Exception e)
                {
                    LogF8.LogException(e);
                }
            });
        }

        // GetResult 是必须实现的方法，用来返回最终的结果
        public BaseLoader GetResult()
        {
            return loader;
        }
    }
}
