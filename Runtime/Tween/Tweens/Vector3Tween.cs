using UnityEngine;

namespace F8Framework.Core
{
    public class Vector3Tween : BaseTween
    {
        private Vector3 from = Vector3.zero;
        private Vector3 to = Vector3.zero;
        private Vector3 tempValue = Vector3.zero;
        private Vector3 originalTo = Vector3.zero;
        private Vector3 originalFrom = Vector3.zero;
        
        public Vector3Tween(Vector3 from, Vector3 to, float t, int id)
        {
            this.id = id;
            Init(from, to, t);
        }

        internal void Init(Vector3 from, Vector3 to, float t)
        {
            this.from = from;
            this.to = to;
            this.originalTo = to;
            this.originalFrom = from;
            this.duration = t;
            this.PauseReset = () => this.Init(from, to, t);
        }

        /// <summary>
        /// 每帧执行的更新逻辑
        /// </summary>
        internal override void Update(float deltaTime)
        {
            if(isPause || IsComplete || IsRecycle)
                return;

            // 处理启动延迟
            if (tempDelay > 0.0f)
            {
                tempDelay -= deltaTime;
                return;
            }
            
            base.Update(deltaTime);

            currentTime += deltaTime;
            
            // 检查是否完成当前周期
            if (currentTime >= duration)
            {
                this.UpdateValue(true);
                
                bool shouldComplete = !HandleLoop();
                if (shouldComplete)
                    onComplete();
                return;
            }
            
            this.UpdateValue(false);
        }

        internal override void UpdateValue(bool isEnd = false)
        {
            base.UpdateValue(isEnd);
            if (isEnd)
            {
                if (onUpdateVector3 != null)
                    onUpdateVector3(loopType == LoopType.Yoyo ? from : to);

                if (onUpdateVector2 != null)
                    onUpdateVector2(loopType == LoopType.Yoyo ? from : to);
            }
            else
            {
                float normalizedProgress = currentTime >= duration ? 1.0f : currentTime / duration;
                // 通过曲线函数计算缓动进度
                float curveProgress = GetCurveProgress(normalizedProgress);
            
                // 基于缓动算法计算当前值
                EasingFunctions.ChangeVector(from, to, curveProgress, ease, ref tempValue);
            
                // 触发值更新回调
                if (onUpdateVector3 != null)
                    onUpdateVector3(tempValue);

                if (onUpdateVector2 != null)
                    onUpdateVector2(tempValue);
            }
        }
        
        internal override void Reset()
        {
            base.Reset();
            to = Vector3.zero;
            from = Vector3.zero;
            originalTo = Vector3.zero;
            originalFrom = Vector3.zero;
        }

        public override BaseTween ReplayReset()
        {
            base.ReplayReset();
            to = originalTo;
            from = originalFrom;
            return this;
        }
        
        public override BaseTween LoopReset()
        {
            base.LoopReset();
            return this;
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
            if (loopType == LoopType.None || tempLoopCount == 0)
            {
                return false;
            }
            else
            {
                if (tempLoopCount > 0)
                {
                    tempLoopCount -= 1;
                }
                switch (loopType)
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
                this.LoopReset();
                return tempLoopCount > 0 || tempLoopCount == -1;
            }
        }
    }
}
