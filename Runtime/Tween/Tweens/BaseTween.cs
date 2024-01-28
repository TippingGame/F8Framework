using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F8Framework.Core
{
    public enum UpdateMode
    {
        Update,
        LateUpdate,
        FixedUpdate
    }


    /// <summary>
    /// Base tween class
    /// </summary>
    
    public abstract class BaseTween : IRecyclable<BaseTween>
    {
        #region PROTECTED
        protected int id = 0;
        protected float delay = 0.0f;
        protected float startTime = 0.0f;
        protected float duration = 0.0f;
        protected float currentTime = 0.0f;
        protected Ease ease = Ease.Linear;
        protected bool isPause = false;
        protected bool isComplete = false;
        protected UpdateMode updateMode = UpdateMode.Update;
        protected GameObject owner = null;
        private float timeSinceStart = 0.0f;
        #endregion

        public int ID { get { return id; } }
        public bool IsComplete 
        { 
            get { return isComplete; }
            set { isComplete = value; }
        }

        #region EVENTS
        protected Action onComplete = null;
        public Action onUpdate = null;
        protected Action<Vector3> onUpdateVector3 = null;
        protected Action<float> onUpdateFloat = null;
        protected Action<Color> onUpdateColor = null;
        protected Action<Vector2> onUpdateVector2 = null;
        protected Action<Quaternion> onUpdateQuaternion = null;
        protected List<TimeEvent> events = new List<TimeEvent>();
        public Action<BaseTween> Recycle { get; set; }
        #endregion

        public GameObject Owner { get { return owner; } }
        public UpdateMode UpdateMode { get { return updateMode; } }
        public bool HandleBySequence { get; set; }

        public Action PauseReset = null;

        public BaseTween()
        {
            onComplete += CheckIfTweenIsComplete;
        }

        private void CheckIfTweenIsComplete()
        {
            if (!HandleBySequence)
            {
                //Recycle(this);
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

        public virtual BaseTween SetIsPause(bool value)
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
            isComplete = false;
            isPause = false;
            currentTime = 0.0f;
            onComplete = null;
        }

        /// <summary>
        /// Set ease type
        /// </summary>
        /// <param name="ease"></param>
        public virtual BaseTween SetEase(Ease ease)
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
        public virtual BaseTween SetOnComplete(Action action)
        {
            onComplete += action;
            return this;
        }

        /// <summary>
        /// set a delay
        /// </summary>
        /// <param name="t">delay in seconds </param>
        /// <returns></returns>
        public virtual BaseTween SetDelay(float t)
        {
            delay = t;

            return this;
        }

        public virtual BaseTween SetOnUpdate(Action action)
        {
            onUpdate += action;
            return this;
        }

        public virtual BaseTween SetOnUpdateVector2(Action<Vector2> action)
        {
            onUpdateVector2 += action;
            return this;
        }

        /// <summary>
        /// Set Callback for OnUpdate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual BaseTween SetOnUpdateVector3(Action<Vector3> action)
        {
            onUpdateVector3 += action;
            return this;
        }

        public virtual BaseTween SetOnUpdateColor(Action<Color> action)
        {
            onUpdateColor += action;
            return this;
        }

        /// <summary>
        /// Set Callback for OnUpdate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual BaseTween SetOnUpdateFloat(Action<float> action)
        {
            onUpdateFloat += action;
            return this;
        }

        /// <summary>
        /// Set Callback for OnUpdate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual BaseTween SetOnUpdateQuaternion(Action<Quaternion> action)
        {
            onUpdateQuaternion += action;
            return this;
        }

        public virtual BaseTween SetOwner(GameObject owner)
        {
            this.owner = owner;
            Tween.Instance.ProcessConnection(this);
            return this;
        }

        public virtual BaseTween SetUpdateMode(UpdateMode updateMode)
        {
            this.updateMode = updateMode;
            return this;
        }

        /// <summary>
        /// Restore all fields to default
        /// </summary>
        public virtual void Reset()
        {
            id = 0;
            delay = 0.0f;
            startTime = 0.0f;
            duration = 0.0f;
            currentTime = 0.0f;
            ease = Ease.Linear;
            updateMode = UpdateMode.Update;
            owner = null;
            timeSinceStart = 0.0f;
            isComplete = false;
            isPause = false;
            HandleBySequence = false;

            onComplete = null;
            onUpdate = null;
            onUpdateVector3 = null;
            onUpdateFloat = null;
            onUpdateColor = null;
            onUpdateVector2 = null;
            onUpdateQuaternion = null;
            PauseReset = null;
            events.Clear();

            onComplete += CheckIfTweenIsComplete;
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

