using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class MissingReferencesWindow : EditorWindow
    {
        private Dictionary<GameObject, List<MissingReferenceInfo>> _results;
        private Dictionary<GameObject, List<MissingReferenceInfo>> _missingScriptsResults;
        private Dictionary<GameObject, List<MissingReferenceInfo>> _nullRefsResults;
        private Vector2 _scrollPosition;
        private readonly HashSet<string> _selectedItems = new();
        private bool _selectAll;
        private string _lastActionResult = "";
        private float _lastActionTime;
        private readonly Dictionary<GameObject, bool> _gameObjectFoldouts = new();
        
        // 页签相关
        private enum TabType
        {
            All,
            MissingScripts,
            NullReferences
        }
        
        private TabType _currentTab = TabType.All;
        private static readonly string[] TabNames = { "All Issues", "Missing Scripts", "Null References" };

        public static void ShowWindow(Dictionary<GameObject, List<MissingReferenceInfo>> results)
        {
            if (HasOpenInstances<MissingReferencesWindow>())
            {
                EditorWindow.GetWindow<MissingReferencesWindow>("Missing References").Close();
            }
            else
            {
                MissingReferencesWindow window = EditorWindow.GetWindow<MissingReferencesWindow>("Missing References");
                window.minSize = new Vector2(800, 500);
                window.Setup(results);
                window.Show();
            }
        }

        private void Setup(Dictionary<GameObject, List<MissingReferenceInfo>> results)
        {
            _results = SortResults(results);
            
            // 分离出缺失脚本和空引用
            _missingScriptsResults = new Dictionary<GameObject, List<MissingReferenceInfo>>();
            _nullRefsResults = new Dictionary<GameObject, List<MissingReferenceInfo>>();
            
            foreach (var kvp in _results)
            {
                var missingScripts = kvp.Value.Where(info => info.IsScriptMissing).ToList();
                var nullRefs = kvp.Value.Where(info => !info.IsScriptMissing).ToList();
                
                if (missingScripts.Count > 0)
                    _missingScriptsResults[kvp.Key] = missingScripts;
                
                if (nullRefs.Count > 0)
                    _nullRefsResults[kvp.Key] = nullRefs;
            }
            
            ResetFoldouts();
            _selectedItems.Clear();
            _selectAll = false;
        }

        private Dictionary<GameObject, List<MissingReferenceInfo>> SortResults(Dictionary<GameObject, List<MissingReferenceInfo>> results)
        {
            var sortedResults = new Dictionary<GameObject, List<MissingReferenceInfo>>();
            var sortedKeys = results.Keys.OrderBy(static go => go.name).ToList();

            foreach (GameObject key in sortedKeys)
            {
                sortedResults[key] = results[key];
            }

            return sortedResults;
        }

        private void ResetFoldouts()
        {
            _gameObjectFoldouts.Clear();
            var currentResults = GetCurrentResults();
            
            foreach (GameObject key in currentResults.Keys)
            {
                _gameObjectFoldouts[key] = true;
            }
        }

        private void OnGUI()
        {
            if (_results == null)
            {
                EditorGUILayout.LabelField("Loading...", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            DrawTabs();
            DrawHeader();
            DrawActionBar();
            DrawContent();
            DrawActionResult();
        }

        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            var newTab = (TabType)GUILayout.Toolbar((int)_currentTab, TabNames, GUILayout.Width(400));
            
            if (newTab != _currentTab)
            {
                _currentTab = newTab;
                ResetFoldouts();
                _selectedItems.Clear();
                _selectAll = false;
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var currentResults = GetCurrentResults();
            int totalProblems = currentResults.Values.Sum(static list => list.Count);
            int missingScripts = currentResults.Values.Sum(static list => list.Count(static info => info.IsScriptMissing));
            int nullRefs = totalProblems - missingScripts;

            string headerText;
            
            if (_currentTab == TabType.All)
            {
                headerText = $"{totalProblems} Issue(s) Found • {missingScripts} Missing Scripts • {nullRefs} Null References";
            }
            else if (_currentTab == TabType.MissingScripts)
            {
                headerText = $"{totalProblems} Missing Script(s) Found";
            }
            else
            {
                headerText = $"{totalProblems} Null Reference(s) Found";
            }

            if (totalProblems == 0)
                headerText = "🎉 No Issues Found!";

            EditorGUILayout.LabelField(headerText, EditorStyles.boldLabel);
            
            // 显示统计信息
            if (_currentTab == TabType.All && totalProblems > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                var style = new GUIStyle(EditorStyles.miniLabel);
                style.normal.textColor = Color.gray;
                
                string statsText = $"Objects: {currentResults.Count} ";
                statsText += $"• Missing Scripts: {missingScripts} ";
                statsText += $"• Null Refs: {nullRefs}";
                
                EditorGUILayout.LabelField(statsText, style);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawActionBar()
        {
            var currentResults = GetCurrentResults();
            
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // 刷新按钮
            if (GUILayout.Button("Refresh", EditorStyles.miniButton, GUILayout.Width(80)))
            {
                RefreshResults();
            }

            GUILayout.Space(10);

            if (currentResults.Count == 0)
            {
                EditorGUILayout.EndHorizontal();
                return;
            }

            EditorGUI.BeginChangeCheck();
            _selectAll = EditorGUILayout.ToggleLeft("Select All", _selectAll, GUILayout.Width(80));

            if (EditorGUI.EndChangeCheck())
            {
                if (_selectAll)
                {
                    SelectAllItems();
                }
                else
                {
                    _selectedItems.Clear();
                }
            }

            GUILayout.Space(10);

            int selectedCount = _selectedItems.Count;

            using (new EditorGUI.DisabledScope(selectedCount == 0))
            {
                if (_currentTab != TabType.MissingScripts)
                {
                    if (GUILayout.Button($"Remove Components ({selectedCount})", EditorStyles.miniButton, GUILayout.Width(160)))
                    {
                        RemoveSelectedComponents();
                    }
                }

                if (_currentTab != TabType.NullReferences)
                {
                    if (GUILayout.Button($"Remove Scripts ({selectedCount})", EditorStyles.miniButton, GUILayout.Width(140)))
                    {
                        RemoveSelectedMissingScripts();
                    }
                }
            }

            GUILayout.FlexibleSpace();

            if (_currentTab != TabType.MissingScripts)
            {
                int totalNullRefs = CountNullReferences(currentResults);
                if (totalNullRefs > 0 && GUILayout.Button($"Remove All Null ({totalNullRefs})", EditorStyles.miniButton, GUILayout.Width(140)))
                {
                    RemoveAllComponentsWithNullRefs();
                }
            }

            if (_currentTab != TabType.NullReferences)
            {
                int totalMissingScripts = CountMissingScripts(currentResults);
                if (totalMissingScripts > 0 && GUILayout.Button($"Remove All Missing ({totalMissingScripts})", EditorStyles.miniButton, GUILayout.Width(150)))
                {
                    RemoveAllMissingScripts();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void DrawContent()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            var currentResults = GetCurrentResults();
            
            if (currentResults.Count == 0)
            {
                string message = _currentTab switch
                {
                    TabType.MissingScripts => "🎉 No missing scripts found!",
                    TabType.NullReferences => "🎉 No null references found!",
                    _ => "🎉 Scene is clean! No issues found."
                };
                
                EditorGUILayout.HelpBox(message, MessageType.Info, true);
            }
            else
            {
                foreach (KeyValuePair<GameObject, List<MissingReferenceInfo>> kvp in currentResults)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    DrawResultCard(kvp.Key, kvp.Value);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(5);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawResultCard(GameObject go, List<MissingReferenceInfo> infos)
        {
            EditorGUILayout.BeginHorizontal();

            string objectDisplayName = $"{go.name} ({infos.Count} issue(s))";
            _gameObjectFoldouts[go] = EditorGUILayout.Foldout(_gameObjectFoldouts[go], objectDisplayName, true);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(60)))
            {
                Selection.activeGameObject = go;
                EditorGUIUtility.PingObject(go);
            }

            EditorGUILayout.EndHorizontal();

            if (_gameObjectFoldouts[go])
            {
                EditorGUI.indentLevel++;

                if (_currentTab == TabType.All)
                {
                    IEnumerable<IGrouping<string, MissingReferenceInfo>> groupedByComponent = infos
                        .GroupBy(static info => info.IsScriptMissing ? "Missing Script" : info.ComponentName);

                    foreach (IGrouping<string, MissingReferenceInfo> group in groupedByComponent)
                    {
                        if (group.Key == "Missing Script")
                        {
                            DrawMissingScriptGroup(go, group.ToList());
                        }
                        else
                        {
                            DrawNullReferenceGroup(go, group);
                        }
                    }
                }
                else if (_currentTab == TabType.MissingScripts)
                {
                    // 只显示缺失脚本
                    DrawMissingScriptGroup(go, infos);
                }
                else // TabType.NullReferences
                {
                    // 只显示空引用
                    IEnumerable<IGrouping<string, MissingReferenceInfo>> groupedByComponent = infos
                        .GroupBy(static info => info.ComponentName);
                    
                    foreach (IGrouping<string, MissingReferenceInfo> group in groupedByComponent)
                    {
                        DrawNullReferenceGroup(go, group);
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        private void DrawMissingScriptGroup(GameObject go, List<MissingReferenceInfo> infos)
        {
            EditorGUILayout.BeginHorizontal();

            string itemKey = $"{go.GetInstanceID()}_missing_script";
            bool isSelected = _selectedItems.Contains(itemKey);

            string countText = infos.Count > 1 ? $" ({infos.Count})" : "";
            isSelected = EditorGUILayout.ToggleLeft($"❌ Missing Script{countText}", isSelected, EditorStyles.label);
            UpdateSelection(itemKey, isSelected);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.Width(60)))
            {
                if (RemoveMissingScriptFromObject(go))
                {
                    // 立即刷新当前对象的结果
                    UpdateResultsAfterRemoval(go, true);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawNullReferenceGroup(GameObject go, IGrouping<string, MissingReferenceInfo> group)
        {
            EditorGUILayout.LabelField($"📜 Script: {group.Key}", EditorStyles.boldLabel);

            foreach (MissingReferenceInfo info in group)
            {
                EditorGUILayout.BeginHorizontal();

                string itemKey = $"{go.GetInstanceID()}_{info.ComponentName}_{info.FieldName}";
                bool isSelected = _selectedItems.Contains(itemKey);

                EditorGUI.indentLevel++;
                isSelected = EditorGUILayout.ToggleLeft($"⚠️ Field: {info.FieldName}", isSelected, EditorStyles.label);
                UpdateSelection(itemKey, isSelected);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Remove", EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    if (RemoveComponentFromObject(go, info))
                    {
                        // 立即刷新当前对象的结果
                        UpdateResultsAfterRemoval(go, false);
                    }
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndHorizontal();
            }
        }

        private void UpdateSelection(string itemKey, bool isSelected)
        {
            if (isSelected)
            {
                _selectedItems.Add(itemKey);
            }
            else
            {
                _selectedItems.Remove(itemKey);
                _selectAll = false;
            }
        }

        private void SelectAllItems()
        {
            _selectedItems.Clear();
            var currentResults = GetCurrentResults();

            foreach (KeyValuePair<GameObject, List<MissingReferenceInfo>> kvp in currentResults)
            {
                GameObject go = kvp.Key;
                List<MissingReferenceInfo> infos = kvp.Value;

                if (_currentTab == TabType.All)
                {
                    bool hasMissingScript = infos.Any(static info => info.IsScriptMissing);

                    if (hasMissingScript)
                    {
                        _selectedItems.Add($"{go.GetInstanceID()}_missing_script");
                    }

                    foreach (MissingReferenceInfo info in infos)
                    {
                        if (!info.IsScriptMissing)
                        {
                            _selectedItems.Add($"{go.GetInstanceID()}_{info.ComponentName}_{info.FieldName}");
                        }
                    }
                }
                else if (_currentTab == TabType.MissingScripts)
                {
                    if (infos.Count > 0) // 在MissingScripts标签下，所有都是缺失脚本
                    {
                        _selectedItems.Add($"{go.GetInstanceID()}_missing_script");
                    }
                }
                else // TabType.NullReferences
                {
                    foreach (MissingReferenceInfo info in infos)
                    {
                        _selectedItems.Add($"{go.GetInstanceID()}_{info.ComponentName}_{info.FieldName}");
                    }
                }
            }
        }

        private int CountNullReferences(Dictionary<GameObject, List<MissingReferenceInfo>> results)
        {
            return results.Values.Sum(static list => list.Count(static info => !info.IsScriptMissing));
        }

        private int CountMissingScripts(Dictionary<GameObject, List<MissingReferenceInfo>> results)
        {
            return results.Values.Sum(static list => list.Count(static info => info.IsScriptMissing));
        }

        private Dictionary<GameObject, List<MissingReferenceInfo>> GetCurrentResults()
        {
            return _currentTab switch
            {
                TabType.MissingScripts => _missingScriptsResults,
                TabType.NullReferences => _nullRefsResults,
                _ => _results
            };
        }

        private void RemoveSelectedComponents()
        {
            int removedCount = 0;
            var itemsToRemove = new List<string>();
            var processedObjects = new HashSet<GameObject>();

            foreach (string itemKey in _selectedItems)
            {
                if (!itemKey.Contains("_missing_script", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = itemKey.Split('_');

                    if (parts.Length >= 3)
                    {
                        int instanceId = int.Parse(parts[0]);
#if UNITY_6000_3_OR_NEWER
                        var go = EditorUtility.EntityIdToObject(instanceId) as GameObject;
#else
                        var go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
#endif

                        if (go)
                        {
                            string componentName = parts[1];

                            string fieldName = "";

                            for (int i = 2; i < parts.Length; i++)
                            {
                                if (i > 2)
                                {
                                    fieldName += "_";
                                }

                                fieldName += parts[i];
                            }

                            var info = new MissingReferenceInfo
                            {
                                ComponentName = componentName,
                                FieldName = fieldName,
                                IsScriptMissing = false
                            };

                            if (RemoveComponentFromObject(go, info))
                            {
                                removedCount++;
                                itemsToRemove.Add(itemKey);
                                processedObjects.Add(go);
                            }
                        }
                    }
                }
            }

            foreach (string item in itemsToRemove)
            {
                _selectedItems.Remove(item);
            }

            // 刷新所有处理过的对象
            foreach (var go in processedObjects)
            {
                UpdateResultsAfterRemoval(go, false);
            }

            ShowActionResult($"✅ Removed {removedCount} component(s)");
        }

        private void RemoveSelectedMissingScripts()
        {
            int fixedCount = 0;
            var itemsToRemove = new List<string>();
            var processedObjects = new HashSet<GameObject>();

            foreach (string itemKey in _selectedItems)
            {
                if (itemKey.Contains("_missing_script", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = itemKey.Split('_');
                    int instanceId = int.Parse(parts[0]);
#if UNITY_6000_3_OR_NEWER
                    var go = EditorUtility.EntityIdToObject(instanceId) as GameObject;
#else
                    var go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
#endif

                    if (go && RemoveMissingScriptFromObject(go))
                    {
                        fixedCount++;
                        itemsToRemove.Add(itemKey);
                        processedObjects.Add(go);
                    }
                }
            }

            foreach (string item in itemsToRemove)
            {
                _selectedItems.Remove(item);
            }

            // 刷新所有处理过的对象
            foreach (var go in processedObjects)
            {
                UpdateResultsAfterRemoval(go, true);
            }

            ShowActionResult($"✅ Removed {fixedCount} missing script(s)");
        }

        private void RemoveAllComponentsWithNullRefs()
        {
            int removedCount = 0;
            var currentResults = _currentTab == TabType.All ? _nullRefsResults : GetCurrentResults();
            var processedObjects = new HashSet<GameObject>();

            foreach (KeyValuePair<GameObject, List<MissingReferenceInfo>> kvp in currentResults)
            {
                GameObject go = kvp.Key;
                List<MissingReferenceInfo> infos = kvp.Value;

                var processedComponents = new HashSet<string>();

                foreach (MissingReferenceInfo info in infos)
                {
                    if (!info.IsScriptMissing && !processedComponents.Contains(info.ComponentName) && RemoveComponentFromObject(go, info))
                    {
                        removedCount++;
                        processedComponents.Add(info.ComponentName);
                        processedObjects.Add(go);
                    }
                }
            }

            // 刷新所有处理过的对象
            foreach (var go in processedObjects)
            {
                UpdateResultsAfterRemoval(go, false);
            }

            ShowActionResult($"✅ Removed {removedCount} component(s) with null references");
        }

        private void RemoveAllMissingScripts()
        {
            int removedCount = 0;
            var currentResults = _currentTab == TabType.All ? _missingScriptsResults : GetCurrentResults();
            var processedObjects = new HashSet<GameObject>();

            foreach (KeyValuePair<GameObject, List<MissingReferenceInfo>> kvp in currentResults)
            {
                GameObject go = kvp.Key;
                List<MissingReferenceInfo> infos = kvp.Value;

                bool hasMissingScript = infos.Any(static info => info.IsScriptMissing);

                if (hasMissingScript && RemoveMissingScriptFromObject(go))
                {
                    removedCount++;
                    processedObjects.Add(go);
                }
            }

            // 刷新所有处理过的对象
            foreach (var go in processedObjects)
            {
                UpdateResultsAfterRemoval(go, true);
            }

            ShowActionResult($"✅ Removed missing scripts from {removedCount} object(s)");
        }

        private static bool RemoveComponentFromObject(GameObject go, MissingReferenceInfo info)
        {
            try
            {
                MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour component in components)
                {
                    if (component && component.GetType().Name == info.ComponentName)
                    {
                        if (EditorUtility.DisplayDialog("Remove Component", $"Remove component '{info.ComponentName}' from '{go.name}'?\n\nThis action cannot be undone.", "Remove",
                                "Cancel"))
                        {
                            DestroyImmediate(component);
                            EditorUtility.SetDirty(go);

                            return true;
                        }

                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                LogF8.LogWarning($"Failed to remove component {info.ComponentName} from {go.name}: {e.Message}");

                return false;
            }

            return false;
        }

        private static bool RemoveMissingScriptFromObject(GameObject go)
        {
            try
            {
                Component[] components = go.GetComponents<Component>();
                int removedCount = 0;

                for (int i = components.Length - 1; i >= 0; i--)
                {
                    if (!components[i])
                    {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        removedCount++;

                        break;
                    }
                }

                return removedCount > 0;
            }
            catch (Exception e)
            {
                LogF8.LogWarning($"Failed to remove missing script from {go.name}: {e.Message}");

                return false;
            }
        }

        private void ShowActionResult(string message)
        {
            _lastActionResult = message;
            _lastActionTime = Time.realtimeSinceStartup;
            Repaint();
        }

        private void DrawActionResult()
        {
            if (!string.IsNullOrEmpty(_lastActionResult) && Time.realtimeSinceStartup - _lastActionTime < 3f)
            {
                EditorGUILayout.LabelField(_lastActionResult, EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void RefreshResults()
        {
            ShowActionResult("🔄 Refreshing...");
            EditorApplication.delayCall += () =>
            {
                MethodInfo scanMethod = typeof(ToolbarFindMissingReferences).GetMethod("StartScanCoroutine", BindingFlags.NonPublic | BindingFlags.Static);

                if (scanMethod != null)
                {
                    scanMethod.Invoke(null, null);
                }
                else
                {
                    Repaint();
                }
            };
        }

        /// <summary>
        /// 删除组件后更新结果
        /// </summary>
        private void UpdateResultsAfterRemoval(GameObject go, bool isMissingScriptRemoved)
        {
            // 重新扫描该对象的组件
            EditorApplication.delayCall += () =>
            {
                // 重新获取该对象的缺失引用信息
                var newResults = RescanGameObject(go);
                
                // 更新主结果集
                if (_results.ContainsKey(go))
                {
                    if (newResults.Count > 0)
                    {
                        _results[go] = newResults;
                    }
                    else
                    {
                        _results.Remove(go);
                    }
                }
                
                // 更新缺失脚本结果集
                if (isMissingScriptRemoved)
                {
                    var missingScripts = newResults.Where(info => info.IsScriptMissing).ToList();
                    if (missingScripts.Count > 0)
                    {
                        _missingScriptsResults[go] = missingScripts;
                    }
                    else
                    {
                        _missingScriptsResults.Remove(go);
                    }
                }
                
                // 更新空引用结果集
                var nullRefs = newResults.Where(info => !info.IsScriptMissing).ToList();
                if (nullRefs.Count > 0)
                {
                    _nullRefsResults[go] = nullRefs;
                }
                else
                {
                    _nullRefsResults.Remove(go);
                }
                
                // 更新折叠状态
                if (_results.ContainsKey(go))
                {
                    _gameObjectFoldouts[go] = true;
                }
                else
                {
                    _gameObjectFoldouts.Remove(go);
                }
                
                // 清除选择
                _selectedItems.Clear();
                _selectAll = false;
                
                Repaint();
            };
        }

        /// <summary>
        /// 重新扫描单个游戏对象
        /// </summary>
        private List<MissingReferenceInfo> RescanGameObject(GameObject go)
        {
            var results = new List<MissingReferenceInfo>();
            
            try
            {
                MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
                
                foreach (MonoBehaviour component in components)
                {
                    if (!component)
                    {
                        // 缺失脚本
                        results.Add(new MissingReferenceInfo
                        {
                            ComponentName = "Missing Script",
                            FieldName = "",
                            IsScriptMissing = true
                        });
                        continue;
                    }
                    
                    // 检查空引用
                    Type componentType = component.GetType();
                    FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    foreach (FieldInfo field in fields)
                    {
                        if (field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) || field.FieldType == typeof(UnityEngine.Object))
                        {
                            object value = field.GetValue(component);
                            if (value == null || value.Equals(null))
                            {
                                results.Add(new MissingReferenceInfo
                                {
                                    ComponentName = componentType.Name,
                                    FieldName = field.Name,
                                    IsScriptMissing = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogF8.LogWarning($"Failed to rescan {go.name}: {e.Message}");
            }
            
            return results;
        }
    }
}