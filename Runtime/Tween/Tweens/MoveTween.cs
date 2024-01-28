using UnityEngine;


namespace F8Framework.Core
{
    public class MoveTween : BaseTween
    {
        #region PRIVATE
        private Transform obj = null;
        private Transform to = null;
        private Vector3 initialPos = Vector3.zero;
        #endregion

        #region PUBLIC
        public MoveTween(Transform obj, Transform to, float t, int id)
        {
            Init(obj, to, t);
        }

        public void Init(Transform obj, Transform to, float t)
        {
            this.obj = obj;
            this.to = to;
            this.duration = t;
            this.initialPos = obj.position;
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
                obj.position = to.position;

                CallOnUpdate();

                onComplete();
                return;
            }

            //get new value
            //Vector3 change = to.position - initialPos;
            //obj.position = Equations.ChangeVector( currentTime, initialPos, change, duration, ease );

            obj.position = EasingFunctions.ChangeVector(initialPos, to.position, currentTime / duration, ease);

            CallOnUpdate();

        }
        #endregion

        private void CallOnUpdate()
        {
            if(onUpdateVector3 != null)
                onUpdateVector3(obj.position);

            if(onUpdateVector2 != null)
                onUpdateVector2(obj.position);
        }

        public override void Reset()
        {
            base.Reset();
            obj = null;
            to = null;
            initialPos = Vector3.zero;
        }

        public override void ReplayReset()
        {
            base.ReplayReset();
            Init(obj, to, duration);
        }
    }

}

