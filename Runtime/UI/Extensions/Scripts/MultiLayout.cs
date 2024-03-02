using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class MultiLayout : MonoBehaviour
    {
        [System.Serializable]
        public class Info
        {
            public bool active = true;
            public Vector2 anchoredPosition = Vector2.zero;
            public Vector2 anchorMax = Vector2.zero;
            public Vector2 anchorMin = Vector2.zero;
            public Vector2 offsetMax = Vector2.zero;
            public Vector2 offsetMin = Vector2.zero;
            public Vector2 pivot = Vector2.zero;
            public Vector2 sizeDelta = Vector2.zero;
            public Vector3 scale = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;

            public Info Clone()
            {
                Info info = new Info();

                info.active = active;
                info.anchorMin = anchorMin;
                info.anchorMax = anchorMax;
                info.pivot = pivot;
                info.offsetMin = offsetMin;
                info.offsetMax = offsetMax;
                info.anchoredPosition = anchoredPosition;
                info.sizeDelta = sizeDelta;
                info.scale = scale;
                info.rotation = rotation;

                return info;
            }

            public void Set(RectTransform rectTransform)
            {
                active = rectTransform.gameObject.activeSelf;
                anchorMin = rectTransform.anchorMin;
                anchorMax = rectTransform.anchorMax;
                pivot = rectTransform.pivot;
                offsetMin = rectTransform.offsetMin;
                offsetMax = rectTransform.offsetMax;
                anchoredPosition = rectTransform.anchoredPosition;
                sizeDelta = rectTransform.sizeDelta;
                scale = rectTransform.localScale;
                rotation = rectTransform.localRotation;
            }
        }

        [System.Serializable]
        public class Target
        {
            public RectTransform rectTransform = null;
            public List<Info> infos = new List<Info>();

            public Target(int layoutCount)
            {
                for (int i = 0; i < layoutCount; ++i)
                {
                    infos.Add(new Info());
                }
            }

            public void AddInfoByCopy(int copyIndex)
            {
                if (IsValidIndex(copyIndex) == true)
                {
                    infos.Add(infos[copyIndex].Clone());
                }
            }

            public void Save(int layoutIndex)
            {
                if (rectTransform != null && IsValidIndex(layoutIndex) == true)
                {
                    infos[layoutIndex].Set(rectTransform);
                }
            }

            public void Apply(int layoutIndex)
            {
                if (rectTransform != null && IsValidIndex(layoutIndex) == true)
                {
                    Info info = infos[layoutIndex];

                    rectTransform.gameObject.SetActive(info.active);
                    rectTransform.anchorMin = info.anchorMin;
                    rectTransform.anchorMax = info.anchorMax;
                    rectTransform.pivot = info.pivot;
                    rectTransform.offsetMin = info.offsetMin;
                    rectTransform.offsetMax = info.offsetMax;
                    rectTransform.anchoredPosition = info.anchoredPosition;
                    rectTransform.sizeDelta = info.sizeDelta;
                    rectTransform.localScale = info.scale;
                    rectTransform.localRotation = info.rotation;
                }
            }

            public void RemoveInfo(int index)
            {
                if (IsValidIndex(index) == true)
                {
                    infos.RemoveAt(index);
                }
            }

            public void SetRectTransform(RectTransform rectTransform)
            {
                if (this.rectTransform == null)
                {
                    for (int index = 0; index < infos.Count; ++index)
                    {
                        infos[index].Set(rectTransform);
                    }
                }

                this.rectTransform = rectTransform;
            }

            private bool IsValidIndex(int index)
            {
                return (index >= 0 && index < infos.Count);
            }
        }

        [System.Serializable]
        public class Layout
        {
            public List<Target> targets = new List<Target>();
            public int count = 1;
            public int current = 0;

            public void RemoveCurrentLayout()
            {
                if (count > 0)
                {
                    for (int index = 0; index < targets.Count; ++index)
                    {
                        targets[index].RemoveInfo(current);
                    }

                    --count;
                    current = count - 1;
                }
            }

            public void AddLayout()
            {
                for (int index = 0; index < targets.Count; ++index)
                {
                    targets[index].AddInfoByCopy(current);
                }

                ++current;
                ++count;
            }

            public void AddTarget()
            {
                targets.Add(new Target(count));
            }

            public void RemoveTarget(int index)
            {
                if (IsValidIndex(index) == true)
                {
                    targets.RemoveAt(index);
                }
            }

            public void SetTargetRectTransfrom(int index, RectTransform rectTransform)
            {
                if (IsValidIndex(index) == true)
                {
                    targets[index].SetRectTransform(rectTransform);
                }
            }

            public void SaveCurrentLayer()
            {
                for (int index = 0; index < targets.Count; ++index)
                {
                    targets[index].Save(current);
                }
            }

            public void SelectLayout(int layoutIndex)
            {
                if (layoutIndex >= 0 && layoutIndex < count)
                {
                    for (int index = 0; index < targets.Count; ++index)
                    {
                        targets[index].Apply(layoutIndex);
                    }

                    current = layoutIndex;
                }
            }

            private bool IsValidIndex(int index)
            {
                return (index >= 0 && index < targets.Count);
            }
        }

        [HideInInspector] public Layout layout = new Layout();

        public void SelectLayout(int layoutIndex)
        {
            layout.SelectLayout(layoutIndex);
        }
    }
}