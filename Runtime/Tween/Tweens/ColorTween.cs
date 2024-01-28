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
                if (onUpdateColor != null)
                    onUpdateColor(this.to);

                onComplete();
                return;
            }

            
            Color color = Color.LerpUnclamped(from, to, EasingFunctions.ChangeFloat(0.0f, 1.0f, currentTime / duration, ease));

            //call update if we have it
            if (onUpdateColor != null)
                onUpdateColor(color);
        }

        internal void Init(Color from, Color to, float t)
        {
            this.from = from;
            this.to = to;
            this.duration = t;
        }

        public override void ReplayReset()
        {
            base.ReplayReset();
            Init(from, to, duration);
        }
    }

}

