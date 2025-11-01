using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class AssetBeDepend
    {
        // 存储所有依赖关系
        private static Dictionary<string, List<string>> referenceCacheDic = new Dictionary<string, List<string>>();
        
        private static List<string> referenceCacheList = new List<string>();
        
        [MenuItem("Assets/（F8资产功能）/（寻找资源是否被引用）", false , 1010)]
        public static void FindReferences()
        {
            referenceCacheList.Clear();
            referenceCacheDic.Clear();
            CollectDepend();
            
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                IsBeDepend(assetPath);
            }
        
            LogF8.LogAsset("引用搜索完成");
            
            // 打开引用结果窗口
            AssetReferenceResultWindow.ShowWindow(referenceCacheList, referenceCacheDic, "初始搜索结果");
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
        
        // 判断文件是否被依赖
        private static bool IsBeDepend(string filePath)
        {
            List<string> list = null;
            if (!referenceCacheDic.TryGetValue(filePath, out list))
            {
                return false;
            }
        
            // 将依赖关系打印出来
            foreach (var file in list)
            {
                if (!referenceCacheList.Contains(file))
                {
                    referenceCacheList.Add(file);
                    LogF8.LogAsset(filePath + "---->被：<color=#FFFF00>" + file + "</color> 引用");
                }
            }
        
            return true;
        }
    }

    // 引用结果显示窗口
    public class AssetReferenceResultWindow : EditorWindow
    {
        private List<string> referenceList;
        private Dictionary<string, List<string>> referenceCacheDic;
        private Vector2 scrollPosition;
        
        // 历史记录相关
        private Stack<HistoryState> historyStack = new Stack<HistoryState>();
        private HistoryState currentState;
        
        // 历史状态类
        private class HistoryState
        {
            public List<string> references;
            public string title;
            public string targetAssetPath; // 当前查看的资源路径
            
            public HistoryState(List<string> refs, string t, string target = "")
            {
                references = new List<string>(refs);
                title = t;
                targetAssetPath = target;
            }
        }
        
        public static void ShowWindow(List<string> references, Dictionary<string, List<string>> cacheDic, string title = "资源引用结果", string targetAssetPath = "")
        {
            AssetReferenceResultWindow window = GetWindow<AssetReferenceResultWindow>("资源引用结果");
            window.referenceCacheDic = cacheDic;
            window.SetCurrentState(references, title, targetAssetPath);
            window.minSize = new Vector2(700, 500);
            window.Show();
        }
        
        private void SetCurrentState(List<string> references, string title, string targetAssetPath = "")
        {
            // 将当前状态保存到历史记录
            if (currentState != null)
            {
                historyStack.Push(currentState);
            }
            
            currentState = new HistoryState(references, title, targetAssetPath);
            referenceList = references;
            
            // 更新窗口标题
            titleContent = new GUIContent(title);
        }
        
        private void OnGUI()
        {
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
            GUILayout.Label($"{currentState.title} - 找到 {referenceList.Count} 个引用文件", EditorStyles.boldLabel);
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
            
            if (referenceList.Count == 0)
            {
                EditorGUILayout.HelpBox("没有找到任何引用", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < referenceList.Count; i++)
                {
                    DrawReferenceItem(referenceList[i], i);
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
        
        private void DrawReferenceItem(string assetPath, int index)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            EditorGUILayout.BeginHorizontal();
            
            // 显示序号
            GUILayout.Label($"{index + 1}.", GUILayout.Width(30));
            
            // 显示资源图标和路径
            GUIContent content = new GUIContent(System.IO.Path.GetFileName(assetPath), AssetDatabase.GetCachedIcon(assetPath));
            
            // 创建可点击的按钮样式
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.label);
            buttonStyle.alignment = TextAnchor.MiddleLeft;
            buttonStyle.padding = new RectOffset(5, 5, 2, 2);
            
            // 绘制可点击的资源项
            if (GUILayout.Button(content, buttonStyle, GUILayout.Height(20), GUILayout.ExpandWidth(true)))
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
            GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
            pathStyle.normal.textColor = Color.gray;
            GUILayout.Label(assetPath, pathStyle);
            EditorGUILayout.EndHorizontal();
            
            // 显示引用数量信息
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(35);
            int referenceCount = GetReferenceCount(assetPath);
            GUIStyle countStyle = new GUIStyle(EditorStyles.miniLabel);
            countStyle.normal.textColor = referenceCount > 0 ? new Color(0.2f, 0.8f, 0.2f) : Color.gray;
            GUILayout.Label($"被 {referenceCount} 个文件引用", countStyle);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(5);
        }
        
        private void SelectAndPingAsset(string assetPath)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset != null)
            {
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
        }
        
        private void OpenAsset(string assetPath)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset != null)
            {
                AssetDatabase.OpenAsset(asset);
            }
        }
        
        private void FindReferencesForThisAsset(string assetPath)
        {
            // 查找该资源的引用
            List<string> referencesForThisAsset = new List<string>();
            
            if (referenceCacheDic.TryGetValue(assetPath, out var refList))
            {
                referencesForThisAsset.AddRange(refList);
            }
            
            // 打开新的结果窗口显示该资源的引用
            if (referencesForThisAsset.Count > 0)
            {
                string newTitle = $"引用: {System.IO.Path.GetFileName(assetPath)}";
                SetCurrentState(referencesForThisAsset, newTitle, assetPath);
                LogF8.LogAsset($"找到 {assetPath} 的 {referencesForThisAsset.Count} 个引用");
            }
            else
            {
                EditorUtility.DisplayDialog("寻找索引", $"资源 {System.IO.Path.GetFileName(assetPath)} 没有被任何文件引用", "确定");
                LogF8.LogAsset($"资源 {assetPath} 没有被任何文件引用");
            }
        }
        
        private void GoBack()
        {
            if (historyStack.Count > 0)
            {
                HistoryState previousState = historyStack.Pop();
                currentState = previousState;
                referenceList = currentState.references;
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
    }
}