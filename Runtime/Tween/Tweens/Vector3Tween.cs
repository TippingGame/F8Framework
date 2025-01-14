using UnityEngine;

namespace F8Framework.Core
{

    public class Vector3Tween : BaseTween
    {
        #region PRIVATE
        private Vector3 from = Vector3.zero;
        private Vector3 to = Vector3.zero;
        private Vector3 tempValue = Vector3.zero;
        #endregion

        #region PUBLIC
        public Vector3Tween(Vector3 from, Vector3 to, float t, int id)
        {
            Init(from, to, t);
            this.id = id;
        }

        internal void Init(Vector3 from, Vector3 to, float t)
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

                if (onUpdateVector3 != null)
                    onUpdateVector3(to);

                if (onUpdateVector2 != null)
                    onUpdateVector2(to);

                onComplete();
                return;
            }

            //get new value           
            EasingFunctions.ChangeVector(from, to, currentTime / duration, ease, ref tempValue);

            //call update if we have it
            if (onUpdateVector3 != null)
                onUpdateVector3(tempValue);

            if (onUpdateVector2 != null)
                onUpdateVector2(tempValue);
        }

        public override void Reset()
        {
            base.Reset();
            from = Vector3.zero;
            to = Vector3.zero;
        }

        public override void ReplayReset()
        {
            base.ReplayReset();
            Init(from, to, duration);
        }
        #endregion
    }

}
