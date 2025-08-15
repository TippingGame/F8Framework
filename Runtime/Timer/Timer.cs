using System;

namespace F8Framework.Core
{
    public class Timer
    {
        private float elapsedTime = 0f;
        private bool isDelayCompleted = false;
        public object Handle = null;
        public int ID = 0;
        public float Step = 1f;
        public float Delay = 0f;
        public int Field = 0;
        public Action OnSecond = null;
        public Action OnComplete = null;
        public bool IsFinish = false;
        public bool IsFrameTimer = false;
        public bool IsPaused = false;
        public bool IgnoreTimeScale = false;
        
        // 存储初始值以便重置
        private float _initialStep;
        private float _initialDelay;
        private int _initialField;
        
        public void Init(object handle, int id, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null, bool ignoreTimeScale = false, bool isFrameTimer = false)
        {
            Handle = handle;
            ID = id;
            Step = step;
            Delay = delay;
            Field = field;
            OnSecond = onSecond;
            OnComplete = onComplete;
            IsFrameTimer = isFrameTimer;
            IgnoreTimeScale = ignoreTimeScale;
            // 保存初始值
            _initialStep = step;
            _initialDelay = delay;
            _initialField = field;
        }
         
        public int Update(float dt)
        {
            if (IsPaused)
                return 0;
            
            int triggerCount = 0; // 记录触发次数

            if (!isDelayCompleted)
            {
                Delay -= dt;
                if (Delay <= 0f)
                {
                    isDelayCompleted = true;
                    elapsedTime = -Delay; // 保留超出部分时间
                    triggerCount++;
                    Delay = 0f;
                }
                else
                {
                    return triggerCount;
                }
            }
            else
            {
                elapsedTime += dt;
            }

            // 计算需要触发的次数
            if (elapsedTime >= Step)
            {
                float stepsFloat = elapsedTime / Step;
                int steps = UnityEngine.Mathf.FloorToInt(stepsFloat);
                triggerCount += steps;
                elapsedTime -= steps * Step;
            }

            return triggerCount;
        }
        
        // 重置计时器到初始状态
        public void Reset()
        {
            IsPaused = false;
            elapsedTime = 0f;
            IsFinish = false;
            isDelayCompleted = false;
            
            // 恢复初始值
            Step = _initialStep;
            Delay = _initialDelay;
            Field = _initialField;
        }
    }
}