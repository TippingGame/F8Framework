using UnityEngine;

namespace F8Framework.Core
{

    public class Vector2Tween : BaseTween
    {
        #region PRIVATE
        private Vector2 from = Vector2.zero;
        private Vector2 to = Vector2.zero;
        private Vector2 tempValue = Vector2.zero;
        #endregion

        #region PUBLIC
        public Vector2Tween(Vector2 from, Vector2 to, float t, int id)
        {
            Init(from, to, t);
            this.id = id;
        }
        
        internal void Init(Vector2 from, Vector2 to, float t)
        {
            this.from = from;
            this.to = to;
            this.duration = t;
            this.PauseReset = () => this.Init(from, to, t);
        }
        
        /// <summary>
        /// called every frame
        /// </summary>
        public override void Update(float deltaTime)
        {
            if(isPause || IsComplete || IsRecycle)
                return;

            //wait a delay
            if (delay > 0.0f)
            {
                delay -= deltaTime;
                return;
            }

            base.Update(deltaTime);

            //start counting time
            currentTime += deltaTime;

            //if time ends
            if (currentTime >= duration)
            {

                if(onUpdateVector2 != null)
                    onUpdateVector2(to);

                onComplete();
                return;
            }

            //get the new value
            EasingFunctions.ChangeVector(from, to, currentTime / duration, ease, ref tempValue);

            //call update if we have it
            if(onUpdateVector2 != null)
                onUpdateVector2(tempValue);
        }

        public override void Reset()
        {
            base.Reset();
            from = Vector2.zero;
            to = Vector2.zero;
        }
        
        public override void ReplayReset()
        {
            base.ReplayReset();
            Init(from, to, duration);
        }

        #endregion
    }

}
