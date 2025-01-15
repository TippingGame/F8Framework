using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace F8Framework.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("这个结构是异步/等待支持所必需的，你不应该直接使用它。")]
    public readonly struct SwquenceAwaiter : INotifyCompletion
    {
        private readonly Sequence sequence;

        internal SwquenceAwaiter(Sequence tween)
        {
            this.sequence = tween;
        }

        public bool IsCompleted => sequence.Recycle == null;  // 判断是否完成

        // 必须实现 INotifyCompletion 接口
        public void OnCompleted(Action continuation)
        {
            sequence.SetOnComplete(() =>
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
        public Sequence GetResult()
        {
            return sequence;
        }
    }
}