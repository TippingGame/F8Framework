using UnityEngine;

namespace F8Framework.Core
{
    public class AngleAdapter : AdapterBase
    {
        [Header("间隙")]
        public float Gap = 0;
        [Header("是否每一帧都计算")]
        public bool CalculateEveryFrame = true;
        [Header("顺时针")]
        public bool Clockwise = true;
        [Header("偏移")]
        public float BiasAngle = 0;
        [Header("圆心距离")]
        public float Distance;
        private RectTransform selfRect;
        private RectTransform SelfRect
        {
            get
            {
                if (selfRect == null)
                {
                    selfRect = GetComponent<RectTransform>();
                }
                return selfRect;
            }
        }

        private void Update()
        {
            if (CalculateEveryFrame)
            {
                Adapt();
            }
        }

        public override void Adapt()
        {
            float sumGap = BiasAngle;
            for (int i = 0; i < SelfRect.childCount; i++)
            {
                var item = selfRect.GetChild(i) as RectTransform;
                item.localRotation = Quaternion.Euler(0, 0, sumGap);
                item.localPosition = new Vector2(Distance * Mathf.Cos((sumGap + 90) * Mathf.PI / 180f), Distance * Mathf.Sin((sumGap + 90) * Mathf.PI / 180f));
                if (Clockwise)
                {
                    sumGap -= Gap;
                }
                else
                {
                    sumGap += Gap;
                }
            }
        }
    }
}