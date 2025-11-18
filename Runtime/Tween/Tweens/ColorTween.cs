using UnityEngine;

namespace F8Framework.Core
{
    public class ColorTween : BaseTween
    {
        private Color from = Color.white;
        private Color to = Color.white;

        public ColorTween(Color from, Color to, float t, int id)
        {
            this.from = from;
            this.to = to;
            this.duration = t;
            this.id = id;
        }

        internal void Init(Color from, Color to, float t)
        {
            this.from = from;
            this.to = to;
            this.duration = t;
            this.PauseReset = () => this.Init(from, to, t);
        }
        
        /// <summary>
        /// 每帧执行的更新逻辑
        /// </summary>
        public override void Update(float deltaTime)
        {
            if(isPause || IsComplete || IsRecycle)
                return;
            
            // 处理启动延迟
            if (delay > 0.0f)
            {
                delay -= deltaTime;
                return;
            }

            base.Update(deltaTime);

            currentTime += deltaTime;
            
            // 检查是否完成当前周期
            if (currentTime >= duration)
            {
                if (onUpdateColor != null)
                    onUpdateColor(to);
                bool shouldComplete = !HandleLoop();
                if (shouldComplete)
                    onComplete();
                return;
            }
            
            float normalizedProgress = currentTime >= duration ? 1.0f : currentTime / duration;
            // 通过曲线函数计算缓动进度
            float curveProgress = GetCurveProgress(normalizedProgress);
            
            // 基于缓动算法计算当前值
            Color color = Color.LerpUnclamped(from, to, EasingFunctions.ChangeFloat(0f, 1f, curveProgress, ease));
            
            // 触发值更新回调
            if (onUpdateColor != null)
                onUpdateColor(color);
        }

        public override void Reset()
        {
            base.Reset();
            from = Color.white;
            to = Color.white;
        }

        public override void ReplayReset()
        {
            base.ReplayReset();
            Init(from, to, duration);
        }
        
        private float GetCurveProgress(float normalizedProgress)
        {
            switch (loopType)
            {
                case LoopType.Yoyo:
                    // 使用平滑的往返曲线 (0→1→0)
                    return Mathf.PingPong(normalizedProgress * 2, 1);
                default:
                    return normalizedProgress;
            }
        }
        
        private bool HandleLoop()
        {
            if (this.loopType == LoopType.None || this.tempLoopCount == 0)
            {
                return false;
            }
            else
            {
                if (this.tempLoopCount > 0)
                {
                    this.tempLoopCount -= 1;
                }
                switch (this.loopType)
                {
                    case LoopType.Restart:
                        break;
                    case LoopType.Flip:
                        (from, to) = (to, from);
                        break;
                    case LoopType.Incremental:
                    {
                        var delta = to - from;
                        from = to;
                        to += delta;
                        break;
                    }
                    case LoopType.Yoyo:
                        break;
                }
                this.ReplayReset();
                return this.tempLoopCount > 0 || this.tempLoopCount == -1;
            }
        }
    }
}

