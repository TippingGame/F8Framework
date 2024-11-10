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
        public long StartTime = 0;
        public bool IsFinish = false;
        public bool IsFrameTimer = false;
        
        public Timer(object handle, int id, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null, bool isFrameTimer = false)
        {
            Handle = handle;
            ID = id;
            Step = step;
            Delay = delay;
            Field = field;
            OnSecond = onSecond;
            OnComplete = onComplete;
            IsFrameTimer = isFrameTimer;
        }
         
        public int Update(float dt)
        {
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
            while (elapsedTime >= Step)
            {
                elapsedTime -= Step;
                triggerCount++;
            }

            return triggerCount;
        }
    }
}