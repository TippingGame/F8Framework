using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace F8Framework.Core
{
    public class BaseDirLoader : IEnumerator
    {
        public List<BaseLoader> Loaders = new List<BaseLoader>();

        public virtual bool LoaderSuccess
        {
            get
            {
                foreach (var loader in Loaders) // 检查每个 loader 是否成功
                {
                    if (!loader.LoaderSuccess)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private Action onComplete;

        public virtual void OnComplete()
        {
            onComplete?.Invoke();
            onComplete = null;
        }

        public void SetOnCompleted(Action onComplete) => this.onComplete = onComplete;
        
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
        public virtual LoaderDirAwaiter GetAwaiter()
        {
            return new LoaderDirAwaiter(this);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("这个结构是异步/等待支持所必需的，你不应该直接使用它。")]
    public readonly struct LoaderDirAwaiter : INotifyCompletion
    {
        private readonly BaseDirLoader loader;

        internal LoaderDirAwaiter(BaseDirLoader loader)
        {
            this.loader = loader;
        }

        public bool IsCompleted
        {
            get
            {
                foreach (var loader in loader.Loaders) // 检查每个 loader 是否完成
                {
                    if (!loader.LoaderSuccess)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

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

        public BaseDirLoader GetResult()
        {
            return loader;
        }
    }
}