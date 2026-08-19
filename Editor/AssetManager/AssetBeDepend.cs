using System.Collections.Generic;
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class AssetBeDepend
    {
        public class ReferenceEntry
        {
            public string AssetPath;
            public List<string> HierarchyPaths = new List<string>();

            public ReferenceEntry(string assetPath)
            {
                AssetPath = assetPath;
            }
        }

        public class ReferenceGroup
        {
            public string TargetAssetPath;
            public List<ReferenceEntry> References = new List<ReferenceEntry>();

            public ReferenceGroup(string targetAssetPath)
            {
                TargetAssetPath = targetAssetPath;
            }
        }

        // 存储所有依赖关系
        private static Dictionary<string, List<string>> referenceCacheDic = new Dictionary<string, List<string>>();

        public static List<ReferenceGroup> BuildReferenceGroupsForTest(string[] targetAssetPaths, Dictionary<string, List<string>> cacheDic)
        {
            return BuildReferenceGroups(targetAssetPaths, cacheDic, false);
        }

        public static List<string> FindPrefabReferenceHierarchyPathsForTest(string prefabPath, string targetAssetPath)
        {
            return FindPrefabReferenceHierarchyPaths(prefabPath, targetAssetPath);
        }

        internal static List<ReferenceGroup> BuildReferenceGroupsForWindow(string[] targetAssetPaths, Dictionary<string, List<string>> cacheDic)
        {
            return BuildReferenceGroups(targetAssetPaths, cacheDic, true);
        }

        private static List<ReferenceGroup> BuildReferenceGroups(string[] targetAssetPaths, Dictionary<string, List<string>> cacheDic, bool includeHierarchy)
        {
            List<ReferenceGroup> groups = new List<ReferenceGroup>();
            for (int i = 0; i < targetAssetPaths.Length; i++)
            {
                ReferenceGroup group = new ReferenceGroup(targetAssetPaths[i]);
                if (cacheDic.TryGetValue(targetAssetPaths[i], out List<string> references))
                {
                    for (int j = 0; j < references.Count; j++)
                    {
                        group.References.Add(BuildReferenceEntry(targetAssetPaths[i], references[j], includeHierarchy));
                    }
                }

                groups.Add(group);
            }

            return groups;
        }

        [MenuItem("Assets/（F8资产功能）/（寻找资源是否被引用）", false , 1010)]
        public static void FindReferences()
        {
            referenceCacheDic.Clear();
            CollectDepend();

            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            string[] assetPaths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                // 将 GUID 转换为 路径
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            List<ReferenceGroup> groups = BuildReferenceGroups(assetPaths, referenceCacheDic, true);
            LogReferenceGroups(groups);
            LogF8.Log("引用搜索完成");

            // 打开引用结果窗口
            AssetReferenceResultWindow.ShowWindow(groups, referenceCacheDic, "初始搜索结果");
        }

        // 收集项目中所有依赖关系
        private static void CollectDepend()
        {
            int count = 0;
            // 获取 AssetBundles 文件夹下所有资源
            string[] uiDirs = { System.IO.Path.Combine("Assets") };
            string[] guids = AssetDatabase.FindAssets("", uiDirs);
            foreach (string guid in guids)
            {
                // 将 GUID 转换为路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 获取文件所有直接依赖的资源
                string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);

                foreach (var filePath in dependencies)
                {
                    // dependency 被 assetPath 依赖了
                    // 将所有依赖关系存储到字典中
                    List<string> list = null;
                    if (!referenceCacheDic.TryGetValue(filePath, out list))
                    {
                        list = new List<string>();
                        referenceCacheDic[filePath] = list;
                    }

                    list.Add(assetPath);
                }

                count++;
                EditorUtility.DisplayProgressBar("引用查找", "引用查找中",
                    (float)(count * 1.0f / guids.Length));
            }

            EditorUtility.ClearProgressBar();
        }

        private static ReferenceEntry BuildReferenceEntry(string targetAssetPath, string referenceAssetPath, bool includeHierarchy)
        {
            ReferenceEntry entry = new ReferenceEntry(referenceAssetPath);
            if (includeHierarchy && IsPrefabPath(referenceAssetPath))
            {
                entry.HierarchyPaths.AddRange(FindPrefabReferenceHierarchyPaths(referenceAssetPath, targetAssetPath));
            }

            return entry;
        }

        private static bool IsPrefabPath(string assetPath)
        {
            return assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase);
        }

        private static void LogReferenceGroups(List<ReferenceGroup> groups)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                ReferenceGroup group = groups[i];
                for (int j = 0; j < group.References.Count; j++)
                {
                    LogF8.Log(group.TargetAssetPath + "---->被：<color=#FFFF00>" + group.References[j].AssetPath + "</color> 引用");
                }
            }
        }

        private static List<string> FindPrefabReferenceHierarchyPaths(string prefabPath, string targetAssetPath)
        {
            List<string> hierarchyPaths = new List<string>();
            string targetGuid = AssetDatabase.AssetPathToGUID(targetAssetPath);
            if (string.IsNullOrEmpty(targetGuid))
            {
                return hierarchyPaths;
            }

            GameObject prefabRoot = null;
            try
            {
                prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
                if (prefabRoot == null)
                {
                    return hierarchyPaths;
                }

                Transform rootTransform = prefabRoot.transform;
                Transform[] transforms = prefabRoot.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < transforms.Length; i++)
                {
                    if (!GameObjectReferencesTarget(transforms[i].gameObject, targetGuid))
                    {
                        continue;
                    }

                    string hierarchyPath = GetHierarchyPath(transforms[i], rootTransform);
                    if (!hierarchyPaths.Contains(hierarchyPath))
                    {
                        hierarchyPaths.Add(hierarchyPath);
                    }
                }
            }
            catch (Exception exception)
            {
                LogF8.LogWarning("读取 Prefab 层级引用失败: " + prefabPath + "\n" + exception.Message);
            }
            finally
            {
                if (prefabRoot != null)
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }

            return hierarchyPaths;
        }

        private static bool GameObjectReferencesTarget(GameObject gameObject, string targetGuid)
        {
            if (SerializedObjectReferencesTarget(gameObject, targetGuid))
            {
                return true;
            }

            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                {
                    continue;
                }

                if (SerializedObjectReferencesTarget(component, targetGuid))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool SerializedObjectReferencesTarget(UnityEngine.Object owner, string targetGuid)
        {
            SerializedObject serializedObject = new SerializedObject(owner);
            SerializedProperty iterator = serializedObject.GetIterator();

            // 通过序列化属性查对象引用，才能覆盖组件上的材质、贴图、Prefab 字段等资源引用。
            while (iterator.Next(true))
            {
                if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                {
                    continue;
                }

                UnityEngine.Object referencedObject = iterator.objectReferenceValue;
                if (referencedObject == null)
                {
                    continue;
                }

                string referencePath = AssetDatabase.GetAssetPath(referencedObject);
                if (string.IsNullOrEmpty(referencePath))
                {
                    continue;
                }

                if (AssetDatabase.AssetPathToGUID(referencePath) == targetGuid)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetHierarchyPath(Transform node, Transform root)
        {
            List<string> pathParts = new List<string>();
            Transform current = node;
            while (current != null)
            {
                pathParts.Add(current.name);
                if (current.parent == null || current.parent == root)
                {
                    break;
                }

                current = current.parent;
            }

            pathParts.Reverse();
            return string.Join("/", pathParts);
        }
    }

    // 引用结果显示窗口
    public class AssetReferenceResultWindow : EditorWindow
    {
        private List<AssetBeDepend.ReferenceGroup> referenceGroups;
        private Dictionary<string, List<string>> referenceCacheDic;
        private Vector2 scrollPosition;

        // 历史记录相关
        private Stack<HistoryState> historyStack = new Stack<HistoryState>();
        private HistoryState currentState;
        private static GUIStyle s_ReferenceButtonStyle;
        private static GUIStyle s_PathStyle;
        private static GUIStyle s_TargetPathStyle;
        private static GUIStyle s_ReferencedCountStyle;
        private static GUIStyle s_EmptyCountStyle;
        private static GUIStyle s_HierarchyStyle;

        private static GUIStyle ReferenceButtonStyle
        {
            get
            {
                if (s_ReferenceButtonStyle == null)
                {
                    s_ReferenceButtonStyle = new GUIStyle(GUI.skin.label);
                    s_ReferenceButtonStyle.alignment = TextAnchor.MiddleLeft;
                    s_ReferenceButtonStyle.padding = new RectOffset(5, 5, 2, 2);
                }

                return s_ReferenceButtonStyle;
            }
        }

        private static GUIStyle PathStyle
        {
            get
            {
                if (s_PathStyle == null)
                {
                    s_PathStyle = new GUIStyle(EditorStyles.miniLabel);
                    s_PathStyle.normal.textColor = Color.gray;
                }

                return s_PathStyle;
            }
        }

        private static GUIStyle TargetPathStyle
        {
            get
            {
                if (s_TargetPathStyle == null)
                {
                    s_TargetPathStyle = new GUIStyle(EditorStyles.miniLabel);
                    s_TargetPathStyle.normal.textColor = new Color(0.1f, 0.5f, 0.9f);
                }

                return s_TargetPathStyle;
            }
        }

        private static GUIStyle ReferencedCountStyle
        {
            get
            {
                if (s_ReferencedCountStyle == null)
                {
                    s_ReferencedCountStyle = new GUIStyle(EditorStyles.miniLabel);
                    s_ReferencedCountStyle.normal.textColor = new Color(0.2f, 0.8f, 0.2f);
                }

                return s_ReferencedCountStyle;
            }
        }

        private static GUIStyle EmptyCountStyle
        {
            get
            {
                if (s_EmptyCountStyle == null)
                {
                    s_EmptyCountStyle = new GUIStyle(EditorStyles.miniLabel);
                    s_EmptyCountStyle.normal.textColor = Color.gray;
                }

                return s_EmptyCountStyle;
            }
        }

        private static GUIStyle HierarchyStyle
        {
            get
            {
                if (s_HierarchyStyle == null)
                {
                    s_HierarchyStyle = new GUIStyle(EditorStyles.miniLabel);
                    s_HierarchyStyle.normal.textColor = new Color(0.9f, 0.65f, 0.2f);
                }

                return s_HierarchyStyle;
            }
        }

        // 历史状态类
        private class HistoryState
        {
            public List<AssetBeDepend.ReferenceGroup> groups;
            public string title;
            public string targetAssetPath; // 当前查看的资源路径

            public HistoryState(List<AssetBeDepend.ReferenceGroup> refs, string t, string target = "")
            {
                groups = new List<AssetBeDepend.ReferenceGroup>(refs);
                title = t;
                targetAssetPath = target;
            }
        }

        public static void ShowWindow(List<AssetBeDepend.ReferenceGroup> references, Dictionary<string, List<string>> cacheDic, string title = "资源引用结果", string targetAssetPath = "")
        {
            AssetReferenceResultWindow window = GetWindow<AssetReferenceResultWindow>("资源引用结果");
            window.referenceCacheDic = cacheDic;
            window.SetCurrentState(references, title, targetAssetPath);
            window.minSize = new Vector2(700, 500);
            window.Show();
        }

        private void SetCurrentState(List<AssetBeDepend.ReferenceGroup> references, string title, string targetAssetPath = "")
        {
            // 将当前状态保存到历史记录
            if (currentState != null)
            {
                historyStack.Push(currentState);
            }

            currentState = new HistoryState(references, title, targetAssetPath);
            referenceGroups = references;

            // 更新窗口标题
            titleContent = new GUIContent(title);
        }

        private void OnGUI()
        {
            if (currentState == null || referenceGroups == null)
            {
                EditorGUILayout.HelpBox("暂无引用搜索结果", MessageType.Info);
                return;
            }

            GUILayout.Space(10);

            // 显示标题和导航信息
            EditorGUILayout.BeginVertical(GUI.skin.box);

            // 显示当前查看的资源信息（如果有）
            if (!string.IsNullOrEmpty(currentState.targetAssetPath))
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("当前查看:", GUILayout.Width(60));
                GUIStyle pathStyle = new GUIStyle(EditorStyles.label);
                pathStyle.normal.textColor = new Color(0.1f, 0.5f, 0.9f);
                GUILayout.Label(currentState.targetAssetPath, pathStyle);
                EditorGUILayout.EndHorizontal();
            }

            // 显示标题和统计信息
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"{currentState.title} - {referenceGroups.Count} 个资源分组，找到 {GetTotalReferenceCount()} 个引用文件", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // 导航按钮
            EditorGUILayout.BeginHorizontal();

            // 返回上一步按钮
            GUI.enabled = historyStack.Count > 0;
            if (GUILayout.Button("← 返回上一步", GUILayout.Width(100)))
            {
                GoBack();
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 显示引用列表
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (referenceGroups.Count == 0)
            {
                EditorGUILayout.HelpBox("没有找到任何引用", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < referenceGroups.Count; i++)
                {
                    DrawReferenceGroup(referenceGroups[i], i);
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            // 操作按钮
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("重新收集依赖", GUILayout.Width(100)))
            {
                Close();
                // 重新收集所有依赖关系
                AssetBeDepend.FindReferences();
            }

            if (GUILayout.Button("关闭", GUILayout.Width(80)))
            {
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawReferenceGroup(AssetBeDepend.ReferenceGroup group, int index)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"{index + 1}.", GUILayout.Width(30));
            GUIContent content = new GUIContent(System.IO.Path.GetFileName(group.TargetAssetPath), AssetDatabase.GetCachedIcon(group.TargetAssetPath));
            GUILayout.Label(content, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            GUILayout.Label($"{group.References.Count} 个引用", EditorStyles.miniBoldLabel, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(35);
            GUILayout.Label(group.TargetAssetPath, TargetPathStyle);
            EditorGUILayout.EndHorizontal();

            if (group.References.Count == 0)
            {
                EditorGUILayout.HelpBox("该资源没有找到任何引用", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < group.References.Count; i++)
                {
                    DrawReferenceItem(group.References[i], i);
                }
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(8);
        }

        private void DrawReferenceItem(AssetBeDepend.ReferenceEntry entry, int index)
        {
            string assetPath = entry.AssetPath;
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            // 显示序号
            GUILayout.Label($"{index + 1}.", GUILayout.Width(30));

            // 显示资源图标和路径
            GUIContent content = new GUIContent(System.IO.Path.GetFileName(assetPath), AssetDatabase.GetCachedIcon(assetPath));

            // 绘制可点击的资源项
            if (GUILayout.Button(content, ReferenceButtonStyle, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {
                // 点击时在Project窗口中定位并选中该资源
                SelectAndPingAsset(assetPath);
            }

            // 添加选择按钮
            if (GUILayout.Button("选择", GUILayout.Width(40)))
            {
                SelectAndPingAsset(assetPath);
            }

            // 添加打开按钮
            if (GUILayout.Button("打开", GUILayout.Width(40)))
            {
                OpenAsset(assetPath);
            }

            // 添加寻找索引按钮
            if (GUILayout.Button("寻找索引", GUILayout.Width(60)))
            {
                FindReferencesForThisAsset(assetPath);
            }

            EditorGUILayout.EndHorizontal();

            // 显示完整路径（较小字体）
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(35);
            GUILayout.Label(assetPath, PathStyle);
            EditorGUILayout.EndHorizontal();

            // 显示引用数量信息
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(35);
            int referenceCount = GetReferenceCount(assetPath);
            GUILayout.Label($"被 {referenceCount} 个文件引用", referenceCount > 0 ? ReferencedCountStyle : EmptyCountStyle);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < entry.HierarchyPaths.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(35);
                GUILayout.Label("层级: " + entry.HierarchyPaths[i], HierarchyStyle);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(5);
        }

        private void SelectAndPingAsset(string assetPath)
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (asset != null)
            {
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
        }

        private void OpenAsset(string assetPath)
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (asset != null)
            {
                AssetDatabase.OpenAsset(asset);
            }
        }

        private void FindReferencesForThisAsset(string assetPath)
        {
            List<AssetBeDepend.ReferenceGroup> groups = AssetBeDepend.BuildReferenceGroupsForWindow(new[] { assetPath }, referenceCacheDic);
            int referenceCount = groups.Count > 0 ? groups[0].References.Count : 0;

            // 打开新的结果窗口显示该资源的引用
            if (referenceCount > 0)
            {
                string newTitle = $"引用: {System.IO.Path.GetFileName(assetPath)}";
                SetCurrentState(groups, newTitle, assetPath);
                LogF8.Log($"找到 {assetPath} 的 {referenceCount} 个引用");
            }
            else
            {
                EditorUtility.DisplayDialog("寻找索引", $"资源 {System.IO.Path.GetFileName(assetPath)} 没有被任何文件引用", "确定");
                LogF8.Log($"资源 {assetPath} 没有被任何文件引用");
            }
        }

        private void GoBack()
        {
            if (historyStack.Count > 0)
            {
                HistoryState previousState = historyStack.Pop();
                currentState = previousState;
                referenceGroups = currentState.groups;
                titleContent = new GUIContent(currentState.title);

                // 重绘窗口
                Repaint();
            }
        }

        private int GetReferenceCount(string assetPath)
        {
            if (referenceCacheDic.TryGetValue(assetPath, out var refList))
            {
                return refList.Count;
            }
            return 0;
        }

        private int GetTotalReferenceCount()
        {
            int count = 0;
            for (int i = 0; i < referenceGroups.Count; i++)
            {
                count += referenceGroups[i].References.Count;
            }

            return count;
        }
    }
}
#endif
