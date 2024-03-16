using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class VerticalAdapter : AdapterBase
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
            float sumHeight = 0;
            int activityCount = 0;
            for (int i = 0; i < SelfRect.childCount; i++)
            {
                if (i >= targetPos.Count) targetPos.Add(0);

                if (SelfRect.GetChild(i).gameObject.activeInHierarchy)
                {
                    activityCount++;
                    var childRect = SelfRect.GetChild(i).GetComponent<RectTransform>();
                    sumHeight += childRect.rect.height;

                    if (activityCount > 1) sumHeight += Gap;

                    targetPos[i] = sumHeight - childRect.rect.height;
                    childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, targetPos[i], childRect.rect.height);
                }
            }
            SelfRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sumHeight);
        }
    }
}