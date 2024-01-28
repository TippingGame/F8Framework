using UnityEngine;

namespace F8Framework.Core
{
	public class QuaternionTween : BaseTween
	{
        #region PRIVATE
        private Quaternion from = Quaternion.identity;
        private Quaternion to = Quaternion.identity;
        #endregion

        #region PUBLIC
        public QuaternionTween(Quaternion from, Quaternion to, float t, int id)
        {
            this.id = id;
            Init(from, to, t);
        }

        public void Init(Quaternion from, Quaternion to, float t)
        {
            this.from = from;
            this.to = to;
            this.duration = t;
        }

        /// <summary>
        /// called every frame
        /// </summary>
        public override void Update(float deltaTime)
        {

            if(isPause)
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

                if (onUpdateQuaternion != null)
                    onUpdateQuaternion(to);

                onComplete();
                return;
            }

            //get new value           
            float v = EasingFunctions.ChangeFloat(0.0f, 1.0f, currentTime / duration, ease);
            Quaternion value = Quaternion.SlerpUnclamped(from , to , v);

            //call update if we have it
            if (onUpdateQuaternion != null)
                onUpdateQuaternion(value);
        }
        #endregion

        public override void Reset()
        {
            base.Reset();
            from = Quaternion.identity;
            to = Quaternion.identity;
        }

        public override void ReplayReset()
        {
            base.ReplayReset();
            Init(from, to, duration);
        }
    }
}

