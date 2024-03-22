using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public enum ScrollAxis
    {
        DEFAULT = 0,
        VERTICAL_TOP,
        VERTICAL_CENTER,
        VERTICAL_BOTTOM,
        HORIZONTAL_LEFT,
        HORIZONTAL_CENTER,
        HORIZONTAL_RIGHT,
    }
     [Serializable]
    public class ScrollLayout
    {
        [Serializable]
        public class LayoutValue
        {
            public enum ValueType
            {
                DEFAULT,
                RATE,
            }

            [HideInInspector]
            public ValueType valueType;
            public float value;
        }

        
        public ScrollAxis axis;

        public Vector2 padding;
        public Vector2 space;

        public bool topToBotton = true;
        public bool leftToRight = true;
        public List<LayoutValue> values = new List<LayoutValue>();

        static public bool IsVertical(ScrollAxis axis)
        {
            return axis == ScrollAxis.VERTICAL_TOP ||
                   axis == ScrollAxis.VERTICAL_CENTER ||
                   axis == ScrollAxis.VERTICAL_BOTTOM;
        }

        public void CheckAxis(ScrollRect scrollRect)
        {
            if (axis == ScrollAxis.DEFAULT)
            {
                if (scrollRect.vertical == true)
                {
                    axis = ScrollAxis.VERTICAL_TOP;
                }
                else
                {
                    axis = ScrollAxis.HORIZONTAL_LEFT;
                }
            }

            if (IsVertical() == true)
            {
                scrollRect.vertical = true;
            }
            else
            {
                scrollRect.horizontal = true;
            }
        }

        

        public bool IsVertical()
        {
            return IsVertical(axis);
        }

        public Vector2 GetAxisPivot()
        {
            Vector2 pivot = Vector2.zero;
            if (axis == ScrollAxis.VERTICAL_TOP)
            {
                pivot = new Vector2(0.5f, 1);
            }
            else if (axis == ScrollAxis.VERTICAL_CENTER)
            {
                pivot = new Vector2(0.5f, 0.5f);
            }
            else if (axis == ScrollAxis.VERTICAL_BOTTOM)
            {
                pivot = new Vector2(0.5f, 0);
            }
            else if (axis == ScrollAxis.DEFAULT ||
                     axis == ScrollAxis.HORIZONTAL_LEFT)
            {
                pivot = new Vector2(0, 0.5f);
            }
            else if (axis == ScrollAxis.HORIZONTAL_CENTER)
            {
                pivot = new Vector2(0.5f, 0.5f);
            }
            else if (axis == ScrollAxis.HORIZONTAL_RIGHT)
            {
                pivot = new Vector2(1.0f, 0.5f);
            }
            return pivot;
        }

        public void SetDefaults()
        {
            foreach (LayoutValue layoutValue in values)
            {
                if (layoutValue.valueType == LayoutValue.ValueType.DEFAULT)
                {
                    layoutValue.valueType = LayoutValue.ValueType.RATE;
                    layoutValue.value = 1;
                }
            }
        }

        public float GetAxisPosition(RectTransform content)
        {
            if (IsVertical() == true)
            {
                return topToBotton ? content.offsetMax.y : -content.offsetMin.y;
            }
            else
            {
                return leftToRight ? -content.offsetMin.x : content.offsetMax.x;
            }
        }

        public float GetAxisPostionFromOffset(float offset)
        {
            if (IsVertical() == true)
            {
                return topToBotton ? offset : -offset;
            }
            else
            {
                return leftToRight ? -offset : offset;
            }
        }

        public void FitItemSize(RectTransform rectTransform, int itemIndex, float size)
        {
            rectTransform.sizeDelta = GetAxisVector(size);
        }

        public void FitItemInlinePosition(RectTransform rectTransform, int itemIndex, float crossSize)
        {
            float min = 0;
            float max = 1;
            if (IsGrid() == true)
            {
                float inlineMaxSize = 0;
                foreach (LayoutValue layoutValue in values)
                {
                    inlineMaxSize += layoutValue.value;
                }

                float crossSapce = space[IsVertical() ? 0 : 1];
                if(crossSapce != 0)
                {
                    crossSapce = crossSapce/ crossSize;
                    inlineMaxSize += (crossSapce * (values.Count - 1));
                }

                int inlineIndex = itemIndex % values.Count;

                float inlinePos = 0;
                float inlineSize = values[inlineIndex].value;

                if (inlineMaxSize > 0 &&
                    inlineSize > 0)
                {
                    for (int index = 0; index < inlineIndex; index++)
                    {
                        inlinePos += values[index].value + crossSapce;
                    }
                    inlinePos = inlinePos / inlineMaxSize;
                    inlineSize = inlineSize / inlineMaxSize;

                    if ((IsVertical() == true && leftToRight == false) ||
                        (IsVertical() == false && topToBotton == true))
                    {
                        inlinePos = 1 - inlinePos - inlineSize;
                    }

                    min = inlinePos;
                    max = inlinePos + inlineSize;
                }
                else
                {
                    min = 0;
                    max = 0;
                }
            }

            Vector2 anchorMin = rectTransform.anchorMin;
            Vector2 anchorMax = rectTransform.anchorMax;
            if (IsVertical() == true)
            {
                anchorMin.x = min;
                anchorMax.x = max;
            }
            else
            {
                anchorMin.y = min;
                anchorMax.y = max;
            }
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        public int GridCount()
        {
            return values.Count;
        }

        public bool IsGrid()
        {
            if (GridCount() > 1)
            {
                return true;
            }

            return false;
        }

        public Vector2 GetItemPivot()
        {
            return new Vector2(leftToRight ? 0.0f : 1.0f, topToBotton ? 1.0f : 0.0f);
        }

        public Rect GetItemAnchor()
        {
            Vector2 pivot = GetItemPivot();

            Rect anchor = Rect.MinMaxRect(0, 0, 1, 1);
            if (IsVertical() == true)
            {
                anchor.yMin = pivot[1];
                anchor.yMax = pivot[1];
            }
            else
            {
                anchor.xMin = pivot[0];
                anchor.xMax = pivot[0];
            }

            return anchor;
        }

        public Vector2 GetAxisVector(float value)
        {
            return GetAxisVector(Vector2.zero, value);
        }

        public Vector2 GetAxisVector(Vector2 vector, float value)
        {
            if (IsVertical() == true)
            {
                vector.y = value;
            }
            else
            {
                vector.x = value;
            }

            return vector;
        }

        public float GetMainSize(RectTransform transform)
        {
            return GetMainSize(transform.rect);
        }

        public float GetMainSize(Rect rect)
        {
            return IsVertical() == true ? rect.height : rect.width;
        }
        
        public float GetCrossSize(Rect rect)
        {
            return IsVertical() == true ? rect.width : rect.height;
        }
        public float GetMainSize(Vector2 delta)
        {
            return IsVertical() == true ? delta.y : delta.x;
        }

        public float GetCrossSize(Vector2 delta)
        {
            return IsVertical() == true ? delta.x : delta.y;
        }

        public int GetMainIndex()
        {
            return IsVertical() ? 1 : 0;
        }

        public int GetCrossIndex()
        {
            return IsVertical() ? 0 : 1;
        }

        public float MainPadding
        {
            get
            {
                return padding[GetMainIndex()];
            }

            set
            {
                padding[GetMainIndex()] = value;
            }
        }

        public float CrossPadding
        {
            get
            {
                return padding[GetCrossIndex()];
            }

            set
            {
                padding[GetCrossIndex()] = value;
            }
        }

        public float MainSpace
        {
            get
            {
                return space[GetMainIndex()];
            }

            set
            {
                space[GetMainIndex()] = value;
            }
        }

        public float CrossSpace
        {
            get
            {
                return space[GetCrossIndex()];
            }

            set
            {
                space[GetCrossIndex()] = value;
            }
        }
    }
}