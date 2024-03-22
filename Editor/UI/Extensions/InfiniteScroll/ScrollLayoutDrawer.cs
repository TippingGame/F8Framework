#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace F8Framework.Core.Editor
{
    [CustomPropertyDrawer(typeof(ScrollLayout))]
    public class ScrollLayoutDrawer : PropertyDrawer
    {
        private Color selectedColor = new Color(84 / 255f, 178 / 255f, 255 / 255f, 1);

        private const float BOX_SIZE = 300;
        private const float ITEM_SIZE = 50;
        private const int LINE_COUNT = 3;

        private float mainPadding = 0;
        private float mainSpace = 0;

        private float crossPadding = 0;
        private float crossSpace = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            if (EditorGUIUtility.currentViewWidth < 345)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; ;
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; ;
            }
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; ;
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; ;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += BOX_SIZE;
            height += ITEM_SIZE;

            return height;
        }
        
        private float GetLineAxis(ScrollAxis axis, float contentsSize, float itemSize)
        {
            float maxLineSize = LINE_COUNT * itemSize;
            float spaceSize = (LINE_COUNT * mainSpace) - mainSpace;
            switch (axis)
            {
                case ScrollAxis.VERTICAL_CENTER:
                case ScrollAxis.HORIZONTAL_CENTER:
                    {
                        return (contentsSize - (maxLineSize + spaceSize)) * 0.5f;
                    }

                case ScrollAxis.VERTICAL_BOTTOM:
                case ScrollAxis.HORIZONTAL_RIGHT:
                    {
                        return (contentsSize - (maxLineSize + spaceSize)) - mainPadding;
                    }
            }

            return mainPadding;
        }

        private  float GetGridSizeRate(List<float> gridList)
        {
            int gridCount = gridList.Count;
            if (gridCount > 1)
            {
                float total = 0;

                for (int i = 0; i < gridCount; i++)
                {
                    total += gridList[i];
                }

                if (total > 0)
                {
                    return 1 / total;
                }
                else
                {
                    return 1 / gridCount;
                }
            }

            return 1;
        }

        public bool OnScrollGridValueGUI(Rect contentsPosition, ScrollLayout scrollLayout, ScrollAxis axis, bool leftToRight, bool topToBotton, List<float> gridList, float gridSize)
        {
            bool isUpdated = false;
            if (scrollLayout.values.Count > 1)
            {
                float BUTTON_SIZE = 20;
                float maxMainSize = BOX_SIZE - (BUTTON_SIZE * 2);

                float startX = contentsPosition.x;
                float startY = contentsPosition.y;
                if (ScrollLayout.IsVertical(axis) == true)
                {
                    if (topToBotton == false)
                    {
                        startY += contentsPosition.height - BUTTON_SIZE;
                    }

                    startX += BUTTON_SIZE;
                }
                else
                {
                    startY += BUTTON_SIZE;

                    if (leftToRight == false)
                    {
                        startX += contentsPosition.width - BUTTON_SIZE;
                    }
                }
                
                float mainPos = 0;
                float crossPos = 0;
                for (int i = 0; i < scrollLayout.values.Count; i++)
                {
                    float mainSize = gridSize * scrollLayout.values[i].value;
                    if (mainSize < 20)
                    {
                        mainSize = 20;
                    }
                    Rect valuePosition = GetAxisRect(axis, mainPos, crossPos, mainSize, BUTTON_SIZE);
                    if (ScrollLayout.IsVertical(axis) == true)
                    {
                        if (leftToRight == false)
                        {
                            valuePosition.x = maxMainSize - mainSize - mainPos;
                        }
                    }
                    else
                    {
                        if (topToBotton == false)
                        {
                            valuePosition.y = maxMainSize - mainSize - mainPos;
                        }
                    }

                    valuePosition.x += startX;
                    valuePosition.y += startY;

                    float value = EditorGUI.FloatField(valuePosition, scrollLayout.values[i].value);
                    if (scrollLayout.values[i].value != value)
                    {
                        scrollLayout.values[i].value = value;
                        isUpdated = true;
                    }

                    mainPos += mainSize;
                    if (gridList.Count > 1)
                    {
                        mainPos += crossSpace;
                    }
                }
            }
            return isUpdated;
        }

        public bool OnContentsGUI(Rect areaPosition, ScrollLayout scrollLayout, ScrollAxis axis)
        {
            bool isUpdated = false;

            float BUTTON_SIZE = 20;

            bool leftToRight = scrollLayout.leftToRight;
            bool topToBotton = scrollLayout.topToBotton;

            int gridCount = scrollLayout.values.Count;
            List<float> gridList = new List<float>();
            foreach (var v in scrollLayout.values)
            {
                gridList.Add(v.value);
            }

            GUI.Box(areaPosition, "", new GUIStyle(GUI.skin.window));

            float contentsSize = BOX_SIZE - (BUTTON_SIZE * 2);

            float lineSize = ITEM_SIZE;
            float maxLineSize = LINE_COUNT * lineSize;
            float spaceSize = (LINE_COUNT * mainSpace) - mainSpace;
            contentsSize -= crossPadding;
            Rect contentsPosition = GetAxisRect(axis, crossPadding * 0.5f, GetLineAxis(axis, contentsSize, ITEM_SIZE), contentsSize, maxLineSize + spaceSize);

            contentsPosition.x += areaPosition.x + BUTTON_SIZE;
            contentsPosition.y += areaPosition.y + BUTTON_SIZE;

            if (gridCount > 1 &&
                crossSpace != 0)
            {
                contentsSize -= crossSpace * (gridCount - 1);
            }

            float gridRateValue = contentsSize * GetGridSizeRate(gridList);
            OnScrollItemGUI(contentsPosition, axis, leftToRight, topToBotton, gridList, gridRateValue);

            Rect outPosition = contentsPosition;
            outPosition.xMin -= BUTTON_SIZE;
            outPosition.xMax += BUTTON_SIZE;
            outPosition.yMin -= BUTTON_SIZE;
            outPosition.yMax += BUTTON_SIZE;

            OnScrollGridValueGUI(outPosition, scrollLayout, axis, leftToRight, topToBotton, gridList, gridRateValue);

            Rect basePosition = new Rect(outPosition.xMin, outPosition.yMin, BUTTON_SIZE, BUTTON_SIZE);
            GUI.backgroundColor = (topToBotton && leftToRight) ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "↘"))
            {
                scrollLayout.topToBotton = true;
                scrollLayout.leftToRight = true;

                isUpdated = true;
            }

            basePosition = new Rect(outPosition.xMax - BUTTON_SIZE, outPosition.yMin, BUTTON_SIZE, BUTTON_SIZE);
            GUI.backgroundColor = (topToBotton && !leftToRight) ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "↙"))
            {
                scrollLayout.topToBotton = true;
                scrollLayout.leftToRight = false;

                isUpdated = true;
            }
            
            basePosition = new Rect(outPosition.xMin, outPosition.yMax - BUTTON_SIZE, BUTTON_SIZE, BUTTON_SIZE);
            GUI.backgroundColor = (!topToBotton && leftToRight) ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "↗"))
            {
                scrollLayout.topToBotton = false;
                scrollLayout.leftToRight = true;

                isUpdated = true;
            }

            basePosition = new Rect(outPosition.xMax - BUTTON_SIZE, outPosition.yMax - BUTTON_SIZE, BUTTON_SIZE, BUTTON_SIZE);
            GUI.backgroundColor = (!topToBotton && !leftToRight) ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "↖"))
            {
                scrollLayout.topToBotton = false;
                scrollLayout.leftToRight = false;

                isUpdated = true;
            }

            GUI.backgroundColor = Color.white;

            return isUpdated;
        }

        public void OnScrollItemGUI(Rect contentsPosition, ScrollAxis axis, bool leftToRight, bool topToBotton, List<float> gridList, float gridRateValue)
        {
            float gridPos = 0;
            float linePos = 0;

            float contentsSize = contentsPosition.width;
            if (ScrollLayout.IsVertical(axis) == true)
            {
                contentsSize = contentsPosition.width;
            }
            else
            {
                contentsSize = contentsPosition.height;
            }
            float gridSize = contentsSize;

            int gridCount = gridList.Count;

            if (gridCount > 1 &&
                crossSpace != 0)
            {
                contentsSize -= crossSpace * (gridCount - 1);
            }
            
            int itemCount = Mathf.FloorToInt(gridCount * (LINE_COUNT - 0.5f));
            if (itemCount < LINE_COUNT)
            {
                itemCount = LINE_COUNT;
            }

            float lineSize = ITEM_SIZE;

            for (int i = 0; i < itemCount; i++)
            {
                if (gridCount > 1)
                {
                    if ((i % gridCount) == 0)
                    {
                        gridPos = 0;
                    }

                    linePos = (i / gridCount) * (lineSize + mainSpace);

                    gridSize = gridRateValue * gridList[i % gridCount];
                    if (gridSize < 20)
                    {
                        gridSize = 20;
                    }
                }
                else
                {
                    gridPos = 0;
                    linePos = i * (lineSize + mainSpace);

                    gridSize = contentsSize;
                }

                Rect ItemPostion = GetAxisRect(axis, gridPos, linePos, gridSize, lineSize);

                if (leftToRight == false)
                {
                    ItemPostion.x = contentsPosition.width - ItemPostion.x - ItemPostion.width;
                }

                if (topToBotton == false)
                {
                    ItemPostion.y = contentsPosition.height - ItemPostion.y - ItemPostion.height;
                }

                ItemPostion.x += contentsPosition.x;
                ItemPostion.y += contentsPosition.y;

                GUI.Label(ItemPostion, i.ToString(), new GUIStyle(GUI.skin.button));

                gridPos += gridSize;

                if (gridCount > 1)
                {
                    gridPos += crossSpace;
                }
            }
        }

        private Rect GetAxisRect(ScrollAxis axis, float mainPos, float crossPos, float mainSize, float crossSize)
        {
            if (ScrollLayout.IsVertical(axis) == true)
            {
                return new Rect(mainPos, crossPos, mainSize, crossSize);
            }
            else
            {
                return new Rect(crossPos, mainPos, crossSize, mainSize);
            }
        }

        public bool OnScrollGUI(Rect areaPosition, ScrollLayout scrollLayout)
        {
            bool isUpdated = false;
            
            float BUTTON_SIZE = 30;
            
            ScrollAxis axis = scrollLayout.axis;
            
            Rect basePosition = new Rect(areaPosition.x + BUTTON_SIZE, areaPosition.y + BUTTON_SIZE, areaPosition.width , areaPosition.height);
            isUpdated = OnContentsGUI(basePosition, scrollLayout, axis);

            basePosition = new Rect(areaPosition.x, BUTTON_SIZE + areaPosition.y, BUTTON_SIZE, areaPosition.height / 3);
            GUI.backgroundColor = axis == ScrollAxis.VERTICAL_TOP ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "┯"))
            {
                scrollLayout.axis = ScrollAxis.VERTICAL_TOP;

                isUpdated = true;
            }
            
            basePosition = new Rect(areaPosition.x, BUTTON_SIZE + areaPosition.y + areaPosition.height / 3, BUTTON_SIZE, areaPosition.height / 3);
            GUI.backgroundColor = axis == ScrollAxis.VERTICAL_CENTER ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "┃"))
            {
                scrollLayout.axis = ScrollAxis.VERTICAL_CENTER;

                isUpdated = true;
            }

            basePosition = new Rect(areaPosition.x, BUTTON_SIZE + areaPosition.y + 2 * areaPosition.height / 3, BUTTON_SIZE, areaPosition.height / 3);
            GUI.backgroundColor = axis == ScrollAxis.VERTICAL_BOTTOM ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "┷"))
            {
                scrollLayout.axis = ScrollAxis.VERTICAL_BOTTOM;

                isUpdated = true;
            }

            basePosition = new Rect(BUTTON_SIZE + areaPosition.x, areaPosition.y, areaPosition.width / 3, BUTTON_SIZE);
            GUI.backgroundColor = axis == ScrollAxis.HORIZONTAL_LEFT ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "┠"))
            {
                scrollLayout.axis = ScrollAxis.HORIZONTAL_LEFT;

                isUpdated = true;
            }

            basePosition = new Rect(BUTTON_SIZE + areaPosition.x + areaPosition.width / 3, areaPosition.y, areaPosition.width / 3, BUTTON_SIZE);
            GUI.backgroundColor = axis == ScrollAxis.HORIZONTAL_CENTER ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "━"))
            {
                scrollLayout.axis = ScrollAxis.HORIZONTAL_CENTER;

                isUpdated = true;
            }

            basePosition = new Rect(BUTTON_SIZE + areaPosition.x + 2 * areaPosition.width / 3, areaPosition.y, areaPosition.width / 3, BUTTON_SIZE);
            GUI.backgroundColor = axis == ScrollAxis.HORIZONTAL_RIGHT ? selectedColor : Color.white;
            if (GUI.Button(basePosition, "┨"))
            {
                scrollLayout.axis = ScrollAxis.HORIZONTAL_RIGHT;

                isUpdated = true;
            }

            GUI.backgroundColor = Color.white;

            return isUpdated;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float x = position.x;
            float y = position.y;
            Object obj = property.serializedObject.targetObject;
            if (obj is InfiniteScroll)
            {
                InfiniteScroll scroll = obj as InfiniteScroll;
                ScrollLayout scrollLayout = scroll.layout;
                
                Rect uiPos = new Rect(x, y, position.width, EditorGUIUtility.singleLineHeight);

                if(scrollLayout.axis == ScrollAxis.DEFAULT)
                {
                    ScrollRect scrollRect = scroll.GetComponent<ScrollRect>();
                    if(scrollRect != null)
                    {
                        scrollLayout.CheckAxis(scrollRect);
                    }
                }

                mainPadding = scroll.GetMainPadding();
                mainSpace = scroll.GetMainSpace();

                crossPadding = scroll.GetCrossPadding();
                crossSpace = scroll.GetCrossSpace();

                var axisProp = property.FindPropertyRelative("axis");
                var topToBottonProp = property.FindPropertyRelative("topToBotton");
                var leftToRightProp = property.FindPropertyRelative("leftToRight");

                ScrollAxis axis = (ScrollAxis)axisProp.enumValueIndex;
                bool topToBotton = topToBottonProp.boolValue;
                bool leftToRight = leftToRightProp.boolValue;

                var valuesProp = property.FindPropertyRelative("values");

                scrollLayout.padding = EditorGUI.Vector2Field(uiPos, "Padding", scrollLayout.padding);
                if (position.width < 309)
                {
                    y += EditorGUIUtility.singleLineHeight;
                    y += EditorGUIUtility.standardVerticalSpacing;
                }
                y += EditorGUIUtility.singleLineHeight;
                y += EditorGUIUtility.standardVerticalSpacing;

                uiPos = new Rect(x, y, position.width, EditorGUIUtility.singleLineHeight);
                scrollLayout.space = EditorGUI.Vector2Field(uiPos, "Space", scrollLayout.space);
                if (position.width < 309)
                {
                    y += EditorGUIUtility.singleLineHeight;
                    y += EditorGUIUtility.standardVerticalSpacing;
                }
                y += EditorGUIUtility.singleLineHeight;
                y += EditorGUIUtility.standardVerticalSpacing;
                uiPos = new Rect(x, y, position.width, EditorGUIUtility.singleLineHeight);
                int arraySize = EditorGUI.IntField(uiPos, "Grid Count", valuesProp.arraySize);
                if (arraySize >= 0)
                {
                    valuesProp.arraySize = arraySize;
                }

                y += EditorGUIUtility.singleLineHeight;
                y += EditorGUIUtility.standardVerticalSpacing;
                uiPos = new Rect(x, y, position.width, EditorGUIUtility.singleLineHeight);
                GUI.Label(uiPos, "Axis", EditorStyles.boldLabel);

                x += (position.width - BOX_SIZE)* 0.5f;
                y += EditorGUIUtility.singleLineHeight;
                y += EditorGUIUtility.standardVerticalSpacing;

                Rect areaPosition = new Rect(x, y, BOX_SIZE, BOX_SIZE);
                bool bUpdated = OnScrollGUI(areaPosition, scrollLayout);
                if (bUpdated == true)
                {
                    EditorUtility.SetDirty(obj);
                }
            }
        }
    }
}

#endif