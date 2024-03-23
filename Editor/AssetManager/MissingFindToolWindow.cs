using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace F8Framework.Core.Editor
{
    public class MissingFindToolWindow : EditorWindow
    {
        [MenuItem("Assets/（F8资产功能）/（全局空引用查找）", false , 1003)]
        public static void ShowWindow()
        {
            if (HasOpenInstances<MissingFindToolWindow>())
            {
                EditorWindow.GetWindow<MissingFindToolWindow>("空引用查找").Close();
            }
            else
            {
                MissingFindToolWindow window = EditorWindow.GetWindow<MissingFindToolWindow>("空引用查找");
                window.minSize = new Vector2(500f, 500f);
            }
        }

        Dictionary<GameObject, string> missGamoObjList = new Dictionary<GameObject, string>();

        private void OnEnable()
        {
            SelectMissingGameObjects(ref missGamoObjList);
        }

        private Vector2 scrollPosition = Vector2.zero;

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box("丢失引用物体列表:");
            if (GUILayout.Button("刷新列表", GUILayout.Width(150)))
            {
                SelectMissingGameObjects(ref missGamoObjList);
            }

            if (GUILayout.Button("清除无效引用（只清理组件空引用）", GUILayout.Width(210)))
            {
                RemoveMissingGameObjects(ref missGamoObjList);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if (missGamoObjList.Count == 0)
            {
                GUILayout.Label("没有组件和属性丢失引用的物体");
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var item in missGamoObjList)
            {
                GUILayout.BeginHorizontal("Box", GUILayout.Height(30));

                GUILayout.Label(item.Value, "AssetLabel", GUILayout.Width(40));
                GUILayout.Space(10);
                if (GUILayout.Button(item.Key.name, "AnimLeftPaneSeparator"))
                {
                    GameObject selectTarget = item.Key;
                    while (selectTarget.transform.parent != null)
                    {
                        selectTarget = selectTarget.transform.parent.gameObject;
                    }

                    EditorGUIUtility.PingObject(selectTarget);
                    Selection.activeObject = selectTarget;
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            GUILayout.EndScrollView();
        }

        private static void SelectMissingGameObjects(ref Dictionary<GameObject, string> gamoObjList)
        {
            gamoObjList.Clear();

            string[] paths = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);
            for (int i = 0; i < paths.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("搜索物体中···", "数量 : " + i, (float)i / paths.Length))
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }

                GameObject tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
                if (tempObj != null)
                {
                    Transform[] transArr = tempObj.GetComponentsInChildren<Transform>();
                    for (int j = 0; j < transArr.Length; j++)
                    {
                        Component[] components = transArr[j].GetComponents<Component>();
                        for (int k = 0; k < components.Length; k++)
                        {
                            if (components[k] == null) // 组件空引用
                            {
                                gamoObjList.TryAdd(transArr[j].gameObject, "组件空");
                            }
                            else // 组件的属性空引用
                            {
                                SerializedObject
                                    so = new SerializedObject(components[k]); //生成一个组件对应的S俄日阿里则对Object对象 用于遍历这个组件的所有属性
                                var iter = so.GetIterator(); //拿到迭代器
                                while (iter.NextVisible(true)) //如果有下一个属性
                                {
                                    //如果这个属性类型是引用类型的
                                    if (iter.propertyType == SerializedPropertyType.ObjectReference)
                                    {
                                        //引用对象是null 并且 引用ID不是0 说明丢失了引用
                                        if (iter.objectReferenceValue == null &&
                                            iter.objectReferenceInstanceIDValue != 0)
                                        {
                                            gamoObjList.TryAdd(transArr[j].gameObject, "属性空");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private static void RemoveMissingGameObjects(ref Dictionary<GameObject, string> gamoObjList)
        {
            if (gamoObjList.Count == 0)
                return;

            List<GameObject> objectsToRemove = new List<GameObject>();

            foreach (var item in gamoObjList)
            {
                if (item.Value == "组件空")
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(item.Key);
                    objectsToRemove.Add(item.Key);
                }
            }

            foreach (var obj in objectsToRemove)
            {
                gamoObjList.Remove(obj);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}