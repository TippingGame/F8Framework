using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace F8Framework.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("这个结构是异步/等待支持所必需的，你不应该直接使用它。")]
    public readonly struct TweenAwaiter : INotifyCompletion
    {
        private readonly BaseTween tween;

        internal TweenAwaiter(BaseTween tween)
        {
            this.tween = tween;
        }

        public bool IsCompleted => tween.IsRecycle;  // 判断是否完成

        // 必须实现 INotifyCompletion 接口
        public void OnCompleted(Action continuation)
        {
            tween.SetOnComplete(() =>
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
        public BaseTween GetResult()
        {
            return tween;
        }
    }
}