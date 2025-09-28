using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Profiling;

namespace F8Framework.Core.Editor
{
    public class AssetBundleInspectorWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool showLoaded = true;
        private bool showUnloaded = true;
        private bool showDependencies = true;
        private string searchFilter = "";
        private Dictionary<string, AssetBundleLoader> assetBundleLoaders;
        private HashSet<string> expandedItems = new HashSet<string>();
        private float lastUpdateTime;
        [SerializeField] private float updateInterval = 0.5f;
        
        private enum TabType
        {
            Resources,
            AssetBundle,
            AssetDatabase
        }
        private TabType currentTab = TabType.AssetBundle;
        
        private Dictionary<string, ResourcesLoader> resourceLoaders;
        
        private Dictionary<string, EditorLoader> editorLoaders;
        
        [MenuItem("开发工具/资产状态检查器", false, 105)]
        public static void ShowWindow()
        {
            if (HasOpenInstances<AssetBundleInspectorWindow>())
            {
                GetWindow<AssetBundleInspectorWindow>("资产状态检查器").Close();
            }
            else
            {
                var window = GetWindow<AssetBundleInspectorWindow>("资产状态检查器");
                // 加载保存的设置
                if (F8EditorPrefs.HasKey("AssetBundleInspector_UpdateInterval"))
                {
                    window.updateInterval = F8EditorPrefs.GetFloat("AssetBundleInspector_UpdateInterval");
                }
            }
        }

        void OnEnable()
        {
            // 注册更新回调
            EditorApplication.update += UpdateWindow;
            lastUpdateTime = Time.realtimeSinceStartup;
        }

        void OnDisable()
        {
            // 取消注册更新回调
            EditorApplication.update -= UpdateWindow;
            // 保存设置
            F8EditorPrefs.SetFloat("AssetBundleInspector_UpdateInterval", updateInterval);
        }

        void UpdateWindow()
        {
            // 控制刷新频率
            if (Time.realtimeSinceStartup - lastUpdateTime > updateInterval)
            {
                lastUpdateTime = Time.realtimeSinceStartup;
                Repaint();
            }
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("此工具仅在运行模式下可用", MessageType.Info);
                return;
            }

            DrawTabs(); // 绘制页签
            DrawToolbar();
            
            // 根据当前页签绘制不同内容
            switch (currentTab)
            {
                case TabType.Resources:
                    DrawResourcesList();
                    break;
                case TabType.AssetBundle:
                    DrawAssetBundleList();
                    break;
                case TabType.AssetDatabase:
                    DrawAssetDatabaseList();
                    break;
            }
        }

        // 新增方法：绘制页签
        private void DrawTabs()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Toggle(currentTab == TabType.Resources, "Resources检查器", EditorStyles.toolbarButton))
                    currentTab = TabType.Resources;
                
                if (GUILayout.Toggle(currentTab == TabType.AssetBundle, "AssetBundle检查器", EditorStyles.toolbarButton))
                    currentTab = TabType.AssetBundle;
                
                if (GUILayout.Toggle(currentTab == TabType.AssetDatabase, "AssetDatabase检查器（编辑器模式）", EditorStyles.toolbarButton))
                    currentTab = TabType.AssetDatabase;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
        }
        
        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                searchFilter =
                    EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(200));

                showLoaded = GUILayout.Toggle(showLoaded, "已加载", EditorStyles.toolbarButton);
                showUnloaded = GUILayout.Toggle(showUnloaded, "已卸载", EditorStyles.toolbarButton);
                showDependencies = GUILayout.Toggle(showDependencies, "依赖关系", EditorStyles.toolbarButton);

                GUILayout.FlexibleSpace();

                // 添加刷新间隔设置
                EditorGUIUtility.labelWidth = 80;
                updateInterval = EditorGUILayout.FloatField("刷新间隔(秒)", updateInterval, GUILayout.Width(150));
                updateInterval = Mathf.Clamp(updateInterval, 0.1f, 5f); // 限制在0.1-5秒之间
                EditorGUIUtility.labelWidth = 0; // 重置为默认值

                if (GUILayout.Button("刷新", EditorStyles.toolbarButton))
                {
                    expandedItems.Clear();
                    Repaint();
                }
            }
            GUILayout.EndHorizontal();
        }

        // 新增方法：绘制Resources列表
        private void DrawResourcesList()
        {
            // 获取Resources加载器数据
            resourceLoaders = ResourcesManager.Instance.GetResourceLoaders();
            if (resourceLoaders == null || resourceLoaders.Count == 0)
            {
                EditorGUILayout.HelpBox("没有通过Resources加载的资源", MessageType.Info);
                return;
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                var sortedLoaders = resourceLoaders.OrderBy(pair => pair.Key).ToList();
                int itemIndex = 0;

                foreach (var pair in sortedLoaders)
                {
                    string resourcePath = pair.Key;
                    ResourcesLoader loader = pair.Value;

                    if (!string.IsNullOrEmpty(searchFilter) && !resourcePath.ToLower().Contains(searchFilter.ToLower()))
                        continue;

                    bool isUnloaded = loader.loadType == ResourcesLoader.LoaderType.NONE &&
                                      loader.resourceLoadState == ResourcesLoader.LoaderState.NONE;
                    bool isLoaded = loader.IsLoadFinished && !isUnloaded;

                    // 根据筛选条件过滤
                    if ((isLoaded && !showLoaded) || (isUnloaded && !showUnloaded))
                        continue;
                    
                    DrawResourceItem(resourcePath, loader, itemIndex);
                    itemIndex++;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawResourceItem(string resourcePath, ResourcesLoader loader, int index)
        {
            bool isUnloaded = loader.loadType == ResourcesLoader.LoaderType.NONE &&
                              loader.resourceLoadState == ResourcesLoader.LoaderState.NONE;
            bool isLoaded = loader.IsLoadFinished && !isUnloaded;
            bool isLoading = loader.resourceLoadState == ResourcesLoader.LoaderState.WORKING;

            // 获取整行矩形
            Rect rowRect = EditorGUILayout.BeginHorizontal();
            {
                // 绘制背景
                Color bgColor = index % 2 == 0 ? new Color(0.3f, 0.3f, 0.3f, 0.2f) : new Color(0.4f, 0.4f, 0.4f, 0.2f);
                EditorGUI.DrawRect(rowRect, bgColor);

                // 1. 折叠箭头
                bool isExpanded = expandedItems.Contains(resourcePath);
                Rect foldoutRect = new Rect(rowRect.x + 4, rowRect.y + (rowRect.height - 16) / 2, 16, 16);
                isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, GUIContent.none);
                GUILayout.Space(20);

                // 2. 资源路径
                EditorGUILayout.LabelField(resourcePath, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));

                // 3. 加载状态
                string stateText = isUnloaded ? "已卸载" :
                    isLoaded ? "已加载" :
                    isLoading ? "加载中..." : "未知状态";

                Color originalColor = GUI.color;
                if (isUnloaded) GUI.color = Color.red;
                else if (isLoading) GUI.color = Color.yellow;
                else if (isLoaded) GUI.color = Color.green;

                EditorGUILayout.LabelField(stateText, GUILayout.Width(80));
                GUI.color = originalColor;

                // 4. 内存占用
                if (isLoaded)
                {
                    long memorySize = 0;

                    // 计算主资源内存
                    if (loader.ResouceObject != null)
                    {
                        memorySize += Profiler.GetRuntimeMemorySizeLong(loader.ResouceObject);
                    }

                    // 计算子资源内存（如果有）
                    if (loader.GetAllAssetObject() != null && loader.GetAllAssetObject().Count > 0)
                    {
                        foreach (var obj in loader.GetAllAssetObject().Values)
                        {
                            if (obj != null)
                            {
                                memorySize += Profiler.GetRuntimeMemorySizeLong(obj);
                            }
                        }
                    }

                    EditorGUILayout.LabelField(EditorUtility.FormatBytes(memorySize), EditorStyles.miniLabel,
                        GUILayout.Width(80));
                }
                else
                {
                    GUILayout.Space(80); // 保持布局一致
                }

                // 整行点击检测
                if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
                {
                    if (expandedItems.Contains(resourcePath))
                        expandedItems.Remove(resourcePath);
                    else
                        expandedItems.Add(resourcePath);
                    Event.current.Use();
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();

            // 展开内容
            if (expandedItems.Contains(resourcePath))
            {
                EditorGUI.indentLevel++;
                DrawResourceDetails(loader);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawResourceDetails(ResourcesLoader loader)
        {
            EditorGUILayout.Space(2);

            // 基本信息
            EditorGUILayout.LabelField($"加载状态: {loader.resourceLoadState}");
            EditorGUILayout.LabelField($"加载类型: {loader.loadType}");

            // 主资源显示
            if (loader.ResouceObject != null)
            {
                EditorGUILayout.LabelField("主资源:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.ObjectField(loader.ResouceObject, typeof(Object), false);
                EditorGUI.indentLevel--;

                // 显示主资源内存占用
                long mainMemory = Profiler.GetRuntimeMemorySizeLong(loader.ResouceObject);
                EditorGUILayout.LabelField($"主资源内存: {EditorUtility.FormatBytes(mainMemory)}");
            }

            // 所有资源显示
            if (loader.GetAllAssetObject() != null && loader.GetAllAssetObject().Count > 0)
            {
                EditorGUILayout.LabelField("所有子资源:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                long totalSubMemory = 0;
                foreach (var assetPair in loader.GetAllAssetObject())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(assetPair.Key);

                    if (assetPair.Value != null)
                    {
                        EditorGUILayout.ObjectField(assetPair.Value, typeof(Object), false, GUILayout.Width(200));
                        long subMemory = Profiler.GetRuntimeMemorySizeLong(assetPair.Value);
                        EditorGUILayout.LabelField(EditorUtility.FormatBytes(subMemory), GUILayout.Width(80));
                        totalSubMemory += subMemory;
                    }
                    else
                    {
                        EditorGUILayout.LabelField("(null)");
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.LabelField($"子资源总内存: {EditorUtility.FormatBytes(totalSubMemory)}");
                EditorGUI.indentLevel--;
            }

            // 操作按钮
            EditorGUILayout.BeginHorizontal();
            {
                if (loader.IsLoadFinished && GUILayout.Button("卸载", EditorStyles.miniButton))
                {
                    LogF8.Log($"卸载Resources资源 {loader.resourcePath}");
                    loader.Clear();
                }

                if (!loader.IsLoadFinished && GUILayout.Button("重新加载", EditorStyles.miniButton))
                {
                    LogF8.Log($"加载Resources资源 {loader.resourcePath}");
                    loader.Load();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawAssetDatabaseList()
        {
            // 获取AssetDatabase加载器数据
            editorLoaders = AssetDatabaseManager.Instance.GetEditorLoaders();
            if (editorLoaders == null || editorLoaders.Count == 0)
            {
                EditorGUILayout.HelpBox("没有通过AssetDatabase加载的资源", MessageType.Info);
                return;
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                var sortedLoaders = editorLoaders.OrderBy(pair => pair.Key).ToList();
                int itemIndex = 0;
            
                foreach (var pair in sortedLoaders)
                {
                    string assetPath = pair.Key;
                    EditorLoader loader = pair.Value;
            
                    if (!string.IsNullOrEmpty(searchFilter) && !assetPath.ToLower().Contains(searchFilter.ToLower()))
                        continue;
            
                    bool isUnloaded = !loader.LoaderSuccess;
                    bool isLoaded = loader.LoaderSuccess;

                    // 根据筛选条件过滤
                    if ((isLoaded && !showLoaded) || (isUnloaded && !showUnloaded))
                        continue;
                    
                    DrawAssetDatabaseItem(assetPath, loader, itemIndex);
                    itemIndex++;
                }
            }
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawAssetDatabaseItem(string assetPath, EditorLoader loader, int index)
        {
            bool isUnloaded = !loader.LoaderSuccess;
            bool isLoaded = loader.LoaderSuccess;

            // 获取整行矩形
            Rect rowRect = EditorGUILayout.BeginHorizontal();
            {
                // 绘制背景
                Color bgColor = index % 2 == 0 ? new Color(0.3f, 0.3f, 0.3f, 0.2f) : new Color(0.4f, 0.4f, 0.4f, 0.2f);
                EditorGUI.DrawRect(rowRect, bgColor);

                // 1. 折叠箭头
                bool isExpanded = expandedItems.Contains(assetPath);
                Rect foldoutRect = new Rect(rowRect.x + 4, rowRect.y + (rowRect.height - 16) / 2, 16, 16);
                isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, GUIContent.none);
                GUILayout.Space(20);

                // 2. 资源路径
                EditorGUILayout.LabelField(assetPath, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));

                // 3. 加载状态
                string stateText = isUnloaded ? "已卸载" :
                    isLoaded ? "已加载" : "未知状态";

                Color originalColor = GUI.color;
                if (isUnloaded) GUI.color = Color.red;
                else if (isLoaded) GUI.color = Color.green;

                EditorGUILayout.LabelField(stateText, GUILayout.Width(80));
                GUI.color = originalColor;

                // 4. 内存占用
                if (isLoaded)
                {
                    long memorySize = 0;

                    // 计算主资源内存
                    if (loader.Asset != null)
                    {
                        memorySize += Profiler.GetRuntimeMemorySizeLong(loader.Asset);
                    }

                    // 计算子资源内存（如果有）
                    if (loader.GetAllAssetObject() != null && loader.GetAllAssetObject().Count > 0)
                    {
                        foreach (var obj in loader.GetAllAssetObject().Values)
                        {
                            if (obj != null)
                            {
                                memorySize += Profiler.GetRuntimeMemorySizeLong(obj);
                            }
                        }
                    }

                    EditorGUILayout.LabelField(EditorUtility.FormatBytes(memorySize), EditorStyles.miniLabel,
                        GUILayout.Width(80));
                }
                else
                {
                    GUILayout.Space(80); // 保持布局一致
                }

                // 整行点击检测
                if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
                {
                    if (expandedItems.Contains(assetPath))
                        expandedItems.Remove(assetPath);
                    else
                        expandedItems.Add(assetPath);
                    Event.current.Use();
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();

            // 展开内容
            if (expandedItems.Contains(assetPath))
            {
                EditorGUI.indentLevel++;
                DrawAssetDatabaseDetails(loader);
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawAssetDatabaseDetails(EditorLoader loader)
        {
            EditorGUILayout.Space(2);

            // 基本信息
            EditorGUILayout.LabelField($"加载状态: {loader.LoaderSuccess}");

            // 主资源显示
            if (loader.Asset != null)
            {
                EditorGUILayout.LabelField("主资源:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.ObjectField(loader.Asset, typeof(Object), false);
                EditorGUI.indentLevel--;

                // 显示主资源内存占用
                long mainMemory = Profiler.GetRuntimeMemorySizeLong(loader.Asset);
                EditorGUILayout.LabelField($"主资源内存: {EditorUtility.FormatBytes(mainMemory)}");
            }

            // 所有资源显示
            if (loader.GetAllAssetObject() != null && loader.GetAllAssetObject().Count > 0)
            {
                EditorGUILayout.LabelField("所有子资源:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                long totalSubMemory = 0;
                foreach (var assetPair in loader.GetAllAssetObject())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(assetPair.Key);

                    if (assetPair.Value != null)
                    {
                        EditorGUILayout.ObjectField(assetPair.Value, typeof(Object), false, GUILayout.Width(200));
                        long subMemory = Profiler.GetRuntimeMemorySizeLong(assetPair.Value);
                        EditorGUILayout.LabelField(EditorUtility.FormatBytes(subMemory), GUILayout.Width(80));
                        totalSubMemory += subMemory;
                    }
                    else
                    {
                        EditorGUILayout.LabelField("(null)");
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.LabelField($"子资源总内存: {EditorUtility.FormatBytes(totalSubMemory)}");
                EditorGUI.indentLevel--;
            }
        }
        
        private void DrawAssetBundleList()
        {
            assetBundleLoaders = AssetBundleManager.Instance.GetAssetBundleLoaders();
            if (assetBundleLoaders == null)
            {
                return;
            }

            if (assetBundleLoaders.Count == 0)
            {
                EditorGUILayout.HelpBox("没有加载的AssetBundle", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                var sortedLoaders = assetBundleLoaders.OrderBy(pair => pair.Key).ToList();
                int itemIndex = 0;

                foreach (var pair in sortedLoaders)
                {
                    string abName = pair.Key;
                    AssetBundleLoader loader = pair.Value;

                    if (!string.IsNullOrEmpty(searchFilter) && !abName.ToLower().Contains(searchFilter.ToLower()))
                        continue;

                    // 更精确的状态判断
                    bool isUnloaded = loader.assetBundleUnloadState == AssetBundleLoader.LoaderState.FINISHED ||
                                      (loader.assetBundleLoadState == AssetBundleLoader.LoaderState.NONE &&
                                       loader.assetBundleUnloadState != AssetBundleLoader.LoaderState.WORKING);
                    bool isLoaded = loader.assetBundleLoadState == AssetBundleLoader.LoaderState.FINISHED &&
                                    !isUnloaded;

                    // 根据筛选条件过滤
                    if ((isLoaded && !showLoaded) || (isUnloaded && !showUnloaded))
                        continue;

                    // 绘制背景
                    Rect bgRect = EditorGUILayout.BeginVertical();
                    Color bgColor = itemIndex % 2 == 0
                        ? new Color(0.3f, 0.3f, 0.3f, 0.2f)
                        : new Color(0.4f, 0.4f, 0.4f, 0.2f);
                    EditorGUI.DrawRect(bgRect, bgColor);

                    // 绘制内容
                    DrawAssetBundleItem(abName, loader, itemIndex);

                    EditorGUILayout.EndVertical();

                    itemIndex++;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawAssetBundleItem(string abName, AssetBundleLoader loader, int index)
        {
            bool isUnloaded = loader.assetBundleUnloadState == AssetBundleLoader.LoaderState.FINISHED ||
                              (loader.assetBundleLoadState == AssetBundleLoader.LoaderState.NONE &&
                               loader.assetBundleUnloadState != AssetBundleLoader.LoaderState.WORKING);
            bool isUnloading = loader.assetBundleUnloadState == AssetBundleLoader.LoaderState.WORKING;
            bool isLoaded = loader.assetBundleLoadState == AssetBundleLoader.LoaderState.FINISHED && !isUnloaded;
            bool isLoading = loader.assetBundleLoadState == AssetBundleLoader.LoaderState.WORKING;
            bool isFullUnloaded = isUnloaded && loader.assetBundleLoadState == AssetBundleLoader.LoaderState.NONE;

            // 获取整行矩形
            Rect rowRect = EditorGUILayout.BeginHorizontal();
            {
                // 绘制背景
                Color bgColor = index % 2 == 0 ? new Color(0.3f, 0.3f, 0.3f, 0.2f) : new Color(0.4f, 0.4f, 0.4f, 0.2f);
                EditorGUI.DrawRect(rowRect, bgColor);

                // 1. 折叠箭头（使用绝对定位）
                bool isExpanded = expandedItems.Contains(abName);
                Rect foldoutRect = new Rect(rowRect.x + 4, rowRect.y + (rowRect.height - 16) / 2, 16, 16);
                isExpanded = EditorGUI.Foldout(foldoutRect, isExpanded, GUIContent.none);

                // 为折叠箭头留出空间
                GUILayout.Space(20);

                // 2. AB包名称（自动扩展）
                EditorGUILayout.LabelField(abName, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));

                // 3. 加载状态（固定宽度80）
                string stateText = isFullUnloaded ? "已完全卸载" :
                    isLoaded ? "已加载" :
                    isLoading ? "加载中..." :
                    isUnloading ? "卸载中..." :
                    isUnloaded ? "已卸载" : "未知状态";

                Color originalColor = GUI.color;
                if (isFullUnloaded) GUI.color = Color.red;
                else if (isLoading) GUI.color = Color.yellow;
                else if (isLoaded) GUI.color = Color.green;
                else if (isUnloading) GUI.color = new Color(1f, 0.5f, 0f);
                else if (isUnloaded) GUI.color = Color.red;

                EditorGUILayout.LabelField(stateText, GUILayout.Width(80));
                GUI.color = originalColor;

                // 4. 文件大小（固定宽度80）
                if (isLoaded && loader.AssetBundleContent != null)
                {
                    long memorySize = Profiler.GetRuntimeMemorySizeLong(loader.AssetBundleContent);
                    EditorGUILayout.LabelField(EditorUtility.FormatBytes(memorySize), EditorStyles.miniLabel,
                        GUILayout.Width(80));
                }
                else
                {
                    GUILayout.Space(80); // 保持布局一致
                }

                // 整行点击检测
                if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
                {
                    if (expandedItems.Contains(abName))
                        expandedItems.Remove(abName);
                    else
                        expandedItems.Add(abName);
                    Event.current.Use();
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();

            // 展开内容
            if (expandedItems.Contains(abName))
            {
                EditorGUI.indentLevel++;
                DrawAssetBundleDetails(loader);
                EditorGUI.indentLevel--;
            }
        }

        private void DrawAssetBundleDetails(AssetBundleLoader loader)
        {
            EditorGUILayout.Space(2);

            // 基本信息
            EditorGUILayout.LabelField($"加载状态: {loader.assetBundleLoadState}");
            EditorGUILayout.LabelField($"卸载状态: {loader.assetBundleUnloadState}");
            EditorGUILayout.LabelField($"加载类型: {loader.loadType}");
            EditorGUILayout.LabelField($"卸载类型: {loader.unloadType}");

            // 主资源显示
            if (loader.GetAssetObject() != null)
            {
                EditorGUILayout.LabelField("主资源:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.ObjectField(loader.GetAssetObject(), typeof(Object), false);
                EditorGUI.indentLevel--;
            }

            // 所有资源显示
            if (loader.GetAllAssetObject() != null && loader.GetAllAssetObject().Count > 0)
            {
                EditorGUILayout.LabelField("所有资源:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var assetPair in loader.GetAllAssetObject())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(assetPair.Key);
                    EditorGUILayout.ObjectField(assetPair.Value, typeof(Object), false, GUILayout.Width(200));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel--;
            }

            // 依赖关系
            if (showDependencies)
            {
                EditorGUILayout.LabelField("依赖关系:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                if (loader.dependentNames.Count > 0)
                {
                    EditorGUILayout.LabelField("依赖的AB包:");
                    foreach (var dep in loader.dependentNames)
                    {
                        EditorGUILayout.LabelField($"- {dep.Key}");
                    }
                }

                if (loader.parentBundleNames.Count > 0)
                {
                    EditorGUILayout.LabelField("被以下AB包依赖:");
                    foreach (var parent in loader.parentBundleNames)
                    {
                        EditorGUILayout.LabelField($"- {parent}");
                    }
                }

                EditorGUI.indentLevel--;
            }

            // 操作按钮
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("重新加载", EditorStyles.miniButton))
                {
                    LogF8.Log($"重新加载AB包 {loader.abName} 中的资源");
                    if (loader.AssetBundleLoadRequest?.assetBundle)
                    {
                        loader.AssetBundleLoadRequest?.assetBundle.Unload(false);
                    }

                    loader.Load();

                    for (int i = 0; i < loader.assetPaths.Count; i++)
                    {
                        loader.Expand(loader.assetPaths[i], null);
                    }
                }

                if (loader.assetBundleLoadState == AssetBundleLoader.LoaderState.FINISHED &&
                    GUILayout.Button("卸载", EditorStyles.miniButton))
                {
                    LogF8.Log($"卸载AB包 {loader.abName}");
                    loader.Unload(false);
                }

                if (loader.assetBundleLoadState == AssetBundleLoader.LoaderState.FINISHED &&
                    GUILayout.Button("完全卸载", EditorStyles.miniButton))
                {
                    LogF8.Log($"完全卸载AB包 {loader.abName}");
                    loader.Unload(true);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}