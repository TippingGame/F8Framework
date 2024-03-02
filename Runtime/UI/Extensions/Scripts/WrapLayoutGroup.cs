using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Abstract base class for HorizontalLayoutGroup and VerticalLayoutGroup.
/// </summary>
public class WrapLayoutGroup : LayoutGroup
{
    [SerializeField]
    protected Vector2 m_Spacing;

    [SerializeField]
    protected bool m_IsVertical;

    [SerializeField]
    protected bool m_MainInverse;

    [SerializeField]
    protected bool m_CrossInverse;

    private List<float> mainSizeList = new List<float>();
    private List<float> crossSizeList = new List<float>();

    public Vector2 spacing
    {
        get
        {
            return this.m_Spacing;
        }
        set
        {
            base.SetProperty<Vector2>(ref this.m_Spacing, value);
        }
    }

    public bool isVertical
    {
        get
        {
            return this.m_IsVertical;
        }
        set
        {
            base.SetProperty<bool>(ref this.m_IsVertical, value);
        }
    }

    public bool mainInverse
    { 
        get
        {
            return this.m_MainInverse;
        }
        set
        {
            base.SetProperty<bool>(ref this.m_MainInverse, value);
        }
    }
    public bool crossInverse
    {
        get
        {
            return this.m_CrossInverse;
        }
        set
        {
            base.SetProperty<bool>(ref this.m_CrossInverse, value);
        }
    }

    /// <summary>
    /// Calculate the layout element properties for this layout element along the given axis.
    /// </summary>
    /// <param name="axis">The axis to calculate for. 0 is horizontal and 1 is vertical.</param>
    /// 
    protected void CalcAlongAxis(int axis)
    {
        mainSizeList.Clear();
        crossSizeList.Clear();

        int crossAxis = 1 - axis;
        float mainSize = base.rectTransform.rect.size[axis];
        float mainMaxSize = mainSize - (axis == 0 ? padding.horizontal : padding.vertical);

        float lineMainSize = 0;
        float lineCrossSize = 0f;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];

            float childSize = child.sizeDelta[axis];
            float childCrossSize = child.sizeDelta[crossAxis];

            if (lineMainSize + childSize >= mainMaxSize)
            {
                mainSizeList.Add(lineMainSize - this.spacing[axis]);
                crossSizeList.Add(lineCrossSize);

                lineMainSize = 0;
                lineCrossSize = 0f;
            }

            lineMainSize += childSize + this.spacing[axis];
            lineCrossSize = Mathf.Max(lineCrossSize, childCrossSize);
        }

        if (lineMainSize > 0)
        { 
            mainSizeList.Add(lineMainSize - this.spacing[axis]);
            crossSizeList.Add(lineCrossSize);
        }
    }

    /// <summary>
    /// Set the positions and sizes of the child layout elements for the given axis.
    /// </summary>
    /// <param name="axis">The axis to handle. 0 is horizontal and 1 is vertical.</param>
    /// <param name="isVertical">Is this group a vertical group?</param>
    protected void SetChildrenAlongAxis(int axis, bool isVertical)
    {
        int mainAxis = 0;
        if (isVertical == true)
        {
            mainAxis = 1;
        }

        int crossAxis = 1 - mainAxis;

        bool alongOtherAxis = axis != mainAxis;

        if (mainSizeList.Count > 0)
        {
            float mainSize = base.rectTransform.rect.size[mainAxis];

            float crossSize = base.rectTransform.rect.size[crossAxis];

            float crossPerferredSize = GetTotalPreferredSize(crossAxis);

            float mainMaxSize = mainSize - (mainAxis == 0 ? padding.horizontal : padding.vertical);

            float mainOffset = 0;
            float crossOffset = 0;

            int lineIndex = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];

                float childMainSize = child.sizeDelta[mainAxis];
                float childCrossSize = child.sizeDelta[crossAxis];

                if (mainOffset + childMainSize >= mainMaxSize)
                {
                    mainOffset = 0;

                    crossOffset += crossSizeList[lineIndex] + this.spacing[crossAxis];

                    lineIndex++;
                }

                if (alongOtherAxis == true)
                {
                    float crossOffsetSize = childCrossSize - crossSizeList[lineIndex];
                    float crossStartOffset = GetStartOffset(crossAxis, crossOffsetSize + crossPerferredSize);

                    float crossPosition = crossOffset;
                    if (crossInverse == true)
                    {
                        crossPosition = crossPerferredSize - crossSizeList[lineIndex] - crossOffset;
                    }
                    base.SetChildAlongAxis(child, crossAxis, crossStartOffset + crossPosition);
                }
                else
                {
                    float maxOffsetSize = Mathf.Min(mainMaxSize, mainSizeList[lineIndex]);
                    float inlineStartOffset = GetStartOffset(mainAxis, maxOffsetSize);

                    float mainPosition = mainOffset;
                    if (mainInverse == true)
                    {
                        mainPosition = (maxOffsetSize - mainOffset - childMainSize);
                    }
                    base.SetChildAlongAxis(child, mainAxis, inlineStartOffset + mainPosition);
                }

                mainOffset += childMainSize + this.spacing[mainAxis];
            }
        }
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        int mainAxis = 0;
        if (isVertical == true)
        {
            mainAxis = 1;
        }
        else
        {
            mainAxis = 0;
        }

        CalcAlongAxis(mainAxis);

        int crossAxis = 1 - mainAxis;

        float maxMain = 0;
        for (int i = 0; i < mainSizeList.Count; i++)
        {
            maxMain = Mathf.Max(maxMain, mainSizeList[i]);
        }
        maxMain += (mainAxis == 0 ? padding.horizontal : padding.vertical);

        float sumCross = 0;
        for (int i = 0; i < crossSizeList.Count; i++)
        {
            sumCross += crossSizeList[i] + this.spacing[crossAxis];
        }
        if (crossSizeList.Count > 0)
        {
            sumCross -= this.spacing[crossAxis];
        }
        sumCross += (crossAxis == 0 ? padding.horizontal : padding.vertical);

        base.SetLayoutInputForAxis(maxMain, maxMain, 0, mainAxis);
        base.SetLayoutInputForAxis(sumCross, sumCross, 0, crossAxis);
    }

    /// <summary>
    /// Called by the layout system.
    /// </summary>
    public override void CalculateLayoutInputVertical()
    {
    }

    /// <summary>
    /// Called by the layout system.
    /// </summary>
    public override void SetLayoutHorizontal()
    {
        SetChildrenAlongAxis(0, this.isVertical);
    }

    /// <summary>
    /// Called by the layout system.
    /// </summary>
    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1, this.isVertical);
    }
}

