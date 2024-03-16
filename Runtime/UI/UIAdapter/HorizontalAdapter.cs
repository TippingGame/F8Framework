using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class HorizontalAdapter : AdapterBase
    {
        [Header("间隙")]
        public float Gap = 0;
        [Header("是否每一帧都计算")]
        public bool CalculateEveryFrame = true;
        private List<float> targetPos = new List<float>();
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
            float sumWidth = 0;
            int activityCount = 0;
            for (int i = 0; i < SelfRect.childCount; i++)
            {
                if (i >= targetPos.Count) targetPos.Add(0);

                if (SelfRect.GetChild(i).gameObject.activeInHierarchy)
                {
                    activityCount++;
                    var childRect = SelfRect.GetChild(i).GetComponent<RectTransform>();
                    sumWidth += childRect.rect.width;

                    if (activityCount > 1) sumWidth += Gap;

                    targetPos[i] = sumWidth - childRect.rect.width;
                    childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, targetPos[i], childRect.rect.width);
                }
            }
            SelfRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sumWidth);
        }
    }
}