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

        private class ScrollLayoutDrawState
        {
            public ScrollAxis axis;
            public Vector2 padding;
            public Vector2 space;
            public bool topToBotton;
            public bool leftToRight;
            public readonly List<float> values = new List<float>();
        }

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

        private bool OnScrollGridValueGUI(Rect contentsPosition, ScrollLayoutDrawState scrollLayout, ScrollAxis axis, bool leftToRight, bool topToBotton, List<float> gridList, float gridSize)
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
                    float mainSize = gridSize * scrollLayout.values[i];
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

                    float value = EditorGUI.FloatField(valuePosition, scrollLayout.values[i]);
                    if (Mathf.Approximately(scrollLayout.values[i], value) == false)
                    {
                        scrollLayout.values[i] = value;
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

        private bool OnContentsGUI(Rect areaPosition, ScrollLayoutDrawState scrollLayout, ScrollAxis axis)
        {
            bool isUpdated = false;

            float BUTTON_SIZE = 20;

            bool leftToRight = scrollLayout.leftToRight;
            bool topToBotton = scrollLayout.topToBotton;

            int gridCount = scrollLayout.values.Count;
            List<float> gridList = new List<float>();
            foreach (var v in scrollLayout.values)
            {
                gridList.Add(v);
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

        private void OnScrollItemGUI(Rect contentsPosition, ScrollAxis axis, bool leftToRight, bool topToBotton, List<float> gridList, float gridRateValue)
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

        private bool OnScrollGUI(Rect areaPosition, ScrollLayoutDrawState scrollLayout)
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

        private static ScrollLayoutDrawState CreateState(SerializedProperty axisProp, SerializedProperty paddingProp, SerializedProperty spaceProp, SerializedProperty topToBottonProp, SerializedProperty leftToRightProp, SerializedProperty valuesProp)
        {
            ScrollLayoutDrawState state = new ScrollLayoutDrawState();
            state.axis = (ScrollAxis)axisProp.enumValueIndex;
            state.padding = paddingProp.vector2Value;
            state.space = spaceProp.vector2Value;
            state.topToBotton = topToBottonProp.boolValue;
            state.leftToRight = leftToRightProp.boolValue;

            for (int index = 0; index < valuesProp.arraySize; index++)
            {
                SerializedProperty valueProp = valuesProp.GetArrayElementAtIndex(index).FindPropertyRelative("value");
                state.values.Add(valueProp.floatValue);
            }

            return state;
        }

        private static void WriteState(ScrollLayoutDrawState state, SerializedProperty axisProp, SerializedProperty paddingProp, SerializedProperty spaceProp, SerializedProperty topToBottonProp, SerializedProperty leftToRightProp, SerializedProperty valuesProp)
        {
            axisProp.enumValueIndex = (int)state.axis;
            paddingProp.vector2Value = state.padding;
            spaceProp.vector2Value = state.space;
            topToBottonProp.boolValue = state.topToBotton;
            leftToRightProp.boolValue = state.leftToRight;

            if (valuesProp.arraySize != state.values.Count)
            {
                valuesProp.arraySize = state.values.Count;
            }

            for (int index = 0; index < state.values.Count; index++)
            {
                SerializedProperty valueProp = valuesProp.GetArrayElementAtIndex(index).FindPropertyRelative("value");
                valueProp.floatValue = state.values[index];
            }
        }

        private static void InitializeLayoutValue(SerializedProperty elementProp)
        {
            SerializedProperty valueTypeProp = elementProp.FindPropertyRelative("valueType");
            SerializedProperty valueProp = elementProp.FindPropertyRelative("value");
            valueTypeProp.enumValueIndex = (int)ScrollLayout.LayoutValue.ValueType.RATE;
            valueProp.floatValue = 1.0f;
        }

        private static float GetMainValue(ScrollAxis axis, Vector2 vector)
        {
            return vector[ScrollLayout.IsVertical(axis) ? 1 : 0];
        }

        private static float GetCrossValue(ScrollAxis axis, Vector2 vector)
        {
            return vector[ScrollLayout.IsVertical(axis) ? 0 : 1];
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float x = position.x;
            float y = position.y;
            Object obj = property.serializedObject.targetObject;
            if (obj is InfiniteScroll)
            {
                InfiniteScroll scroll = obj as InfiniteScroll;
                
                Rect uiPos = new Rect(x, y, position.width, EditorGUIUtility.singleLineHeight);

                var axisProp = property.FindPropertyRelative("axis");
                var paddingProp = property.FindPropertyRelative("padding");
                var spaceProp = property.FindPropertyRelative("space");
                var topToBottonProp = property.FindPropertyRelative("topToBotton");
                var leftToRightProp = property.FindPropertyRelative("leftToRight");

                ScrollAxis axis = (ScrollAxis)axisProp.enumValueIndex;
                if(axis == ScrollAxis.DEFAULT)
                {
                    ScrollRect scrollRect = scroll.GetComponent<ScrollRect>();
                    if(scrollRect != null)
                    {
                        axis = scrollRect.vertical ? ScrollAxis.VERTICAL_TOP : ScrollAxis.HORIZONTAL_LEFT;
                        axisProp.enumValueIndex = (int)axis;
                        GUI.changed = true;
                    }
                }

                var valuesProp = property.FindPropertyRelative("values");

                Vector2 padding = paddingProp.vector2Value;
                Vector2 space = spaceProp.vector2Value;

                mainPadding = GetMainValue(axis, padding);
                mainSpace = GetMainValue(axis, space);

                crossPadding = GetCrossValue(axis, padding);
                crossSpace = GetCrossValue(axis, space);

                EditorGUI.BeginChangeCheck();
                padding = EditorGUI.Vector2Field(uiPos, "Padding", padding);
                if (EditorGUI.EndChangeCheck())
                {
                    paddingProp.vector2Value = padding;
                    GUI.changed = true;
                }

                if (position.width < 309)
                {
                    y += EditorGUIUtility.singleLineHeight;
                    y += EditorGUIUtility.standardVerticalSpacing;
                }
                y += EditorGUIUtility.singleLineHeight;
                y += EditorGUIUtility.standardVerticalSpacing;

                uiPos = new Rect(x, y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginChangeCheck();
                space = EditorGUI.Vector2Field(uiPos, "Space", space);
                if (EditorGUI.EndChangeCheck())
                {
                    spaceProp.vector2Value = space;
                    GUI.changed = true;
                }

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
                    int oldArraySize = valuesProp.arraySize;
                    valuesProp.arraySize = arraySize;
                    for (int index = oldArraySize; index < arraySize; index++)
                    {
                        InitializeLayoutValue(valuesProp.GetArrayElementAtIndex(index));
                    }
                }

                y += EditorGUIUtility.singleLineHeight;
                y += EditorGUIUtility.standardVerticalSpacing;
                uiPos = new Rect(x, y, position.width, EditorGUIUtility.singleLineHeight);
                GUI.Label(uiPos, "Axis", EditorStyles.boldLabel);

                x += (position.width - BOX_SIZE)* 0.5f;
                y += EditorGUIUtility.singleLineHeight;
                y += EditorGUIUtility.standardVerticalSpacing;

                Rect areaPosition = new Rect(x, y, BOX_SIZE, BOX_SIZE);
                ScrollLayoutDrawState state = CreateState(axisProp, paddingProp, spaceProp, topToBottonProp, leftToRightProp, valuesProp);
                EditorGUI.BeginChangeCheck();
                bool bUpdated = OnScrollGUI(areaPosition, state);
                if (EditorGUI.EndChangeCheck() == true || bUpdated == true)
                {
                    WriteState(state, axisProp, paddingProp, spaceProp, topToBottonProp, leftToRightProp, valuesProp);
                    GUI.changed = true;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}

#endif
