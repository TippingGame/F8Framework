using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace F8Framework.Core
{
    public enum UpdateMode : byte
    {
        Update,
        LateUpdate,
        FixedUpdate
    }
    
    public enum LoopType : byte
    {
        None = 0,
        // 到达结束值后，回到起始值并重新播放
        Restart = 1,
        // 到达结束值后，起始值和结束值互换并重新播放
        Flip = 2,
        // 到达结束值后，给结束值增加(结束值-起始值)后继续播放
        Incremental = 4,
        // 到达结束值后，将反向播放以返回起始值并重新播放（往返一次是完整的动画周期）
        Yoyo = 3
    }
    
    /// <summary>
    /// Base tween class
    /// </summary>
    public abstract class BaseTween : IEnumerator
    {
        #region PROTECTED
        protected int id = 0;
        protected float delay = 0.0f;
        protected float duration = 0.0f;
        protected float currentTime = 0.0f;
        protected Ease ease = Ease.EaseOutQuad;
        protected bool isPause = false;
        protected bool isComplete = false;
        protected UpdateMode updateMode = UpdateMode.Update;
        protected GameObject owner = null;
        private float timeSinceStart = 0.0f;
        protected LoopType loopType = LoopType.None;
        protected int loopCount = 0;
        protected int tempLoopCount = 0;
        #endregion

        public int ID
        {
            get => id;
            set => id = value;
        }
        public bool IsComplete 
        { 
            get => isComplete;
            set => isComplete = value;
        }
        
        #region EVENTS
        protected Action onComplete = null;
        protected Action onCompleteSequence = null;
        protected Action onUpdate = null;
        protected Action<Vector3> onUpdateVector3 = null;
        protected Action<float> onUpdateFloat = null;
        protected Action<Color> onUpdateColor = null;
        protected Action<Vector2> onUpdateVector2 = null;
        protected Action<Quaternion> onUpdateQuaternion = null;
        protected List<TimeEvent> events = new List<TimeEvent>();
        #endregion

        public GameObject Owner { get { return owner; } }
        public UpdateMode UpdateMode { get { return updateMode; } }
        public Action PauseReset = null;
        public bool CanRecycle = true;
        public bool IsRecycle = false;
        
        public BaseTween()
        {
            onComplete = FinishTween;
        }
        
        private void FinishTween()
        {
            IsComplete = true;
            // 可以回收
            if (CanRecycle)
            {
                IsRecycle = true;
            }
            else
            {
                this.tempLoopCount = this.loopCount;
                onCompleteSequence?.Invoke();
            }
        }
        
        /// <summary>
        /// Called to update this tween
        /// </summary>
        public virtual void Update(float deltaTime)
        {
            timeSinceStart += deltaTime;

            if (events.Count > 0 && timeSinceStart >= events[0].Time)
            {
                events[0].Action();
                events.RemoveAt(0);
            }

            if(onUpdate != null)
                onUpdate();
        }

        public BaseTween SetIsPause(bool value)
        {
            if (isPause && !value)
            {
                if(PauseReset != null)
                    PauseReset.Invoke();
            }

            isPause = value;
            return this;
        }

        public virtual void ReplayReset()
        {
            IsComplete = false;
            isPause = false;
            currentTime = 0.0f;
        }

        public void ClearOnCompleteSequence()
        {
            onCompleteSequence = null;
        }
        
        /// <summary>
        /// Set ease type
        /// </summary>
        /// <param name="ease"></param>
        public BaseTween SetEase(Ease ease)
        {
            this.ease = ease;
            return this;
        }

        public BaseTween SetEvent(Action action, float t)
        {
            events.Add(new TimeEvent(action, t));

            //sort this list
            if (events.Count > 1)
                events = events.OrderBy(o => o.Time).ToList();


            return this;
        }

        /// <summary>
        /// Set callback for onComplete
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BaseTween SetOnComplete(Action action)
        {
            onComplete += action;
            return this;
        }

        public BaseTween SetOnCompleteSequence(Action action)
        {
            onCompleteSequence += action;
            return this;
        }
        
        /// <summary>
        /// set a delay
        /// </summary>
        /// <param name="t">delay in seconds </param>
        /// <returns></returns>
        public BaseTween SetDelay(float t)
        {
            delay = t;
            return this;
        }

        public BaseTween SetOnUpdate(Action action)
        {
            onUpdate += action;
            return this;
        }

        public BaseTween SetOnUpdateVector2(Action<Vector2> action)
        {
            onUpdateVector2 += action;
            return this;
        }

        /// <summary>
        /// Set Callback for OnUpdate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BaseTween SetOnUpdateVector3(Action<Vector3> action)
        {
            onUpdateVector3 += action;
            return this;
        }

        public BaseTween SetOnUpdateColor(Action<Color> action)
        {
            onUpdateColor += action;
            return this;
        }

        /// <summary>
        /// Set Callback for OnUpdate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BaseTween SetOnUpdateFloat(Action<float> action)
        {
            onUpdateFloat += action;
            return this;
        }

        /// <summary>
        /// Set Callback for OnUpdate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BaseTween SetOnUpdateQuaternion(Action<Quaternion> action)
        {
            onUpdateQuaternion += action;
            return this;
        }

        public BaseTween SetOwner(GameObject owner)
        {
            this.owner = owner;
            Tween.Instance.ProcessConnection(this);
            return this;
        }

        public BaseTween SetUpdateMode(UpdateMode updateMode)
        {
            this.updateMode = updateMode;
            return this;
        }

        public BaseTween SetLoopType(LoopType loopType, int loopCount = -1)
        {
            this.loopType = loopType;
            this.loopCount = loopCount;
            this.tempLoopCount = loopCount;
            CanRecycle = false;
            return this;
        }
        
        /// <summary>
        /// Restore all fields to default
        /// </summary>
        public virtual void Reset()
        {
            id = 0;
            delay = 0.0f;
            duration = 0.0f;
            currentTime = 0.0f;
            ease = Ease.EaseOutQuad;
            updateMode = UpdateMode.Update;
            owner = null;
            timeSinceStart = 0.0f;
            loopType = LoopType.None;
            loopCount = 0;
            tempLoopCount = 0;
            IsComplete = false;
            isPause = false;
            CanRecycle = true;
            
            onUpdate = null;
            onUpdateVector3 = null;
            onUpdateFloat = null;
            onUpdateColor = null;
            onUpdateVector2 = null;
            onUpdateQuaternion = null;
            PauseReset = null;
            events.Clear();
            IsRecycle = false;
            onCompleteSequence = null;
            
            onComplete = FinishTween;
        }
        
        /// <summary>使用此方法在协程中等待Tween。</summary>
        /// <example><code>
        /// IEnumerator Coroutine() {
        ///     yield return gameObject.Move(Vector3.one, 1f);
        /// }
        /// </code></example>
        bool IEnumerator.MoveNext() {
            return !IsRecycle;
        }

        object IEnumerator.Current {
            get {
                if (IsRecycle)
                {
                    LogF8.LogError("已回收的Tween无法访问当前值");
                }
                return null;
            }
        }

        void IEnumerator.Reset() => throw new NotSupportedException();
        
        /// <summary>此方法是异步/等待支持所必需的。不要直接使用它。</summary>
        /// <example><code>
        /// async void Coroutine() {
        ///     await gameObject.Move(Vector3.one, 1f);
        /// }
        /// </code></example>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TweenAwaiter GetAwaiter() {
            return new TweenAwaiter(this);
        }
    }

    public class TimeEvent
    {
        public Action Action;
        public float Time;

        public TimeEvent(Action action, float t)
        {
            Action = action;
            Time = t;
        }
    }

}

