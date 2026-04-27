using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace F8Framework.Core.Editor
{
    public class BatchRenameTool : EditorWindow
    {
        static BatchRenameTool()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            appliedAssetRenameBatchCount = Mathf.Clamp(AssetRenameUndoState.instance.appliedBatchCount, 0, AssetRenameHistory.Count);
        }

        [Serializable]
        private class RenamePresetData
        {
            public string presetName = "";
            public int selectedTab;
            public string baseName = "";
            public string replaceOld = "";
            public string replaceNew = "";
            public int startIndex = 1;
            public int incrementValue = 1;
            public string prefix = "";
            public string suffix = "";
            public bool useUIComponentNameAsPrefix;
            public int numberPadding;
            public int sequencePosition;
            public int trimFrontCount;
            public int trimBackCount;
            public bool useRegexReplace;
            public int caseMode;
        }

        [Serializable]
        private class RenamePresetCollection
        {
            public List<RenamePresetData> presets = new List<RenamePresetData>();
        }

        private enum RenameCaseMode
        {
            None = 0,
            Upper = 1,
            Lower = 2,
            Title = 3
        }

        private enum SelectionSource
        {
            Hierarchy = 0,
            Project = 1
        }

        [Serializable]
        private class AssetRenameRecord
        {
            public string assetGuid = "";
            public string oldName = "";
            public string newName = "";
        }

        [Serializable]
        private class AssetRenameBatch
        {
            public List<AssetRenameRecord> records = new List<AssetRenameRecord>();
        }

        private class AssetRenameUndoState : ScriptableSingleton<AssetRenameUndoState>
        {
            public int appliedBatchCount;
        }

        private const string PresetStorageKey = "F8Framework.BatchRenameTool.Presets";

        private UnityEngine.Object[] targetObjects;
        private SelectionSource currentSelectionSource = SelectionSource.Hierarchy;
        private bool lockSelection = true;
        private string baseName = "";
        private string replaceOld = "";
        private string replaceNew = "";
        private int startIndex = 1;
        private int incrementValue = 1;
        private int selectedTab = 0;
        private string[] tabNames = { "按序号重命名", "查找替换", "添加前后缀", "删除前后字符", "大小写转换" };
        private readonly string[] sequencePositionOptions = { "序号在后", "序号在前" };
        private readonly string[] caseModeOptions = { "不转换", "全部大写", "全部小写", "单词首字母大写" };

        // 前缀后缀专用
        private string prefix = "";
        private string suffix = "";
        private bool useUIComponentNameAsPrefix = false;
        private int numberPadding = 0;
        private int sequencePosition = 0;
        private int trimFrontCount = 0;
        private int trimBackCount = 0;
        private bool useRegexReplace = false;
        private string regexValidationMessage = "";
        private int caseMode = 0;
        private string presetName = "";
        private int selectedPresetIndex = 0;
        private List<RenamePresetData> presets = new List<RenamePresetData>();
        private bool hasLoadedPresets = false;

        private Vector2 scrollPosition;
        private Vector2 windowScrollPosition;
        private static readonly List<AssetRenameBatch> AssetRenameHistory = new List<AssetRenameBatch>();
        private static int appliedAssetRenameBatchCount;

        // ============ Hierarchy右键菜单 ============

        [MenuItem("GameObject/（F8资产功能）/（批量重命名）", false, 0)]
        private static void RenameFromHierarchy()
        {
            UnityEngine.Object[] selectedObjects = GetHierarchyOrderedSelection();

            if (selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请先选择需要改名的物体", "确定");
                return;
            }

            ShowWindow(selectedObjects, SelectionSource.Hierarchy);
        }

        // 验证菜单项
        [MenuItem("GameObject/（F8资产功能）/（批量重命名）", true)]
        private static bool ValidateHierarchyMenu()
        {
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem("Assets/（F8资产功能）/（批量重命名）", false, 1014)]
        private static void RenameFromProject()
        {
            UnityEngine.Object[] selectedObjects = GetProjectOrderedSelection();

            if (selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请先在 Project 窗口选择需要改名的资源", "确定");
                return;
            }

            ShowWindow(selectedObjects, SelectionSource.Project);
        }

        [MenuItem("Assets/（F8资产功能）/（批量重命名）", true)]
        private static bool ValidateProjectMenu()
        {
            return Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0;
        }

        // ============ 窗口逻辑 ============

        private static void ShowWindow(UnityEngine.Object[] objects, SelectionSource selectionSource)
        {
            BatchRenameTool window = GetWindow<BatchRenameTool>("批量重命名");
            window.currentSelectionSource = selectionSource;
            window.targetObjects = SortObjectsBySelectionOrder(objects, selectionSource);
            window.minSize = new Vector2(450, 750);
            window.maxSize = new Vector2(800, 800);
            window.Show();
            window.Focus();
        }

        private void OnGUI()
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                EditorGUILayout.HelpBox($"没有选中的{GetTargetLabel()}，请关闭窗口重新选择", MessageType.Warning);
                if (GUILayout.Button("关闭窗口"))
                {
                    Close();
                }

                return;
            }

            // 标题
            EditorGUILayout.Space(10);
            windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);
            EditorGUILayout.LabelField("批量重命名", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"当前来源: {GetSelectionSourceLabel()}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"选中{GetTargetLabel()}数量: {targetObjects.Length}", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            DrawPresetSection();
            EditorGUILayout.Space(10);

            // 刷新按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新当前选择", GUILayout.Width(120)))
            {
                RefreshSelectedObjects();
            }

            if (GUILayout.Button("从当前入口获取最新选择", GUILayout.Width(200)))
            {
                UpdateFromCurrentSelection();
            }

            EditorGUILayout.EndHorizontal();
            lockSelection = EditorGUILayout.ToggleLeft($"锁定当前待重命名{GetTargetLabel()}（避免随当前来源选择变化）", lockSelection);

            EditorGUILayout.Space(10);

            // 显示选中的物体列表
            EditorGUILayout.LabelField($"将要改名的{GetTargetLabel()}:", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));

            for (int i = 0; i < Mathf.Min(targetObjects.Length, 50); i++) // 最多显示50个
            {
                if (targetObjects[i] == null) continue;

                EditorGUILayout.BeginHorizontal();

                // 序号
                EditorGUILayout.LabelField($"{i + 1}.", GUILayout.Width(30));

                // 物体引用
                EditorGUILayout.ObjectField(targetObjects[i], typeof(UnityEngine.Object), true, GUILayout.Width(150));

                // 原名称
                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.normal.textColor = Color.gray;
                EditorGUILayout.LabelField(GetObjectDisplayName(targetObjects[i]), style, GUILayout.Width(120));

                // 预览新名称
                string preview = GetPreviewName(i);
                GUIStyle previewStyle = new GUIStyle(EditorStyles.boldLabel);
                previewStyle.normal.textColor = Color.green;
                if (preview != GetObjectDisplayName(targetObjects[i]))
                {
                    EditorGUILayout.LabelField($"→ {preview}", previewStyle);
                }
                else
                {
                    // Keep the IMGUI control count stable so text field focus is not lost
                    // when preview content toggles between empty/non-empty while typing.
                    EditorGUILayout.LabelField(string.Empty, previewStyle);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (targetObjects.Length > 50)
            {
                EditorGUILayout.HelpBox($"还有 {targetObjects.Length - 50} 个{GetTargetLabel()}未显示...", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            // 标签页选择
            EditorGUILayout.LabelField("重命名模式:", EditorStyles.boldLabel);
            int newSelectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            if (newSelectedTab != selectedTab)
            {
                selectedTab = newSelectedTab;
                GUI.FocusControl(null);
                EditorGUIUtility.editingTextField = false;
            }

            EditorGUILayout.Space(10);

            // 分隔线
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            switch (selectedTab)
            {
                case 0:
                    DrawIndexRenameTab();
                    break;
                case 1:
                    DrawReplaceRenameTab();
                    break;
                case 2:
                    DrawPrefixSuffixTab();
                    break;
                case 3:
                    DrawTrimTab();
                    break;
                case 4:
                    DrawCaseTab();
                    break;
            }

            EditorGUILayout.Space(20);

            // 按钮区域
            EditorGUILayout.BeginHorizontal();

            // 执行重命名按钮
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("执行重命名", GUILayout.Height(40)))
            {
                ExecuteRename();
            }

            GUI.backgroundColor = Color.white;

            // 重置按钮
            if (GUILayout.Button("重置参数", GUILayout.Height(40)))
            {
                ResetParameters();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 撤销按钮
            if (GUILayout.Button("撤销重命名（Ctrl+Z）", GUILayout.Height(30)))
            {
                Undo.PerformUndo();
                Repaint();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawIndexRenameTab()
        {
            EditorGUILayout.LabelField("序号重命名", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("基础名称:", GUILayout.Width(80));
            GUI.SetNextControlName("BatchRename_BaseName");
            baseName = EditorGUILayout.TextField(baseName);
            EditorGUILayout.EndHorizontal();

            useUIComponentNameAsPrefix = EditorGUILayout.ToggleLeft("使用UI组件名作为前缀", useUIComponentNameAsPrefix);
            if (useUIComponentNameAsPrefix)
            {
                EditorGUILayout.HelpBox(
                    "勾选后会按物体上的 UI 组件类型自动加前缀；如果同一物体上有多个 UI 组件，会组合多个类型名，例如 Button_Image_Item_1。非 UI 物体会保持不加此前缀。",
                    MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("起始序号:", GUILayout.Width(80));
            startIndex = EditorGUILayout.IntField(startIndex);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("增量:", GUILayout.Width(80));
            incrementValue = EditorGUILayout.IntField(incrementValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("补零位数:", GUILayout.Width(80));
            numberPadding = Mathf.Max(0, EditorGUILayout.IntField(numberPadding));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("序号位置:", GUILayout.Width(80));
            sequencePosition = EditorGUILayout.Popup(sequencePosition, sequencePositionOptions);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                "示例: 输入“Item”且序号在后 → Item_1, Item_2...\n切到序号在前 → 1_Item, 2_Item。\n补零位数设为 3 时会变成 001_Item 或 Item_001。",
                MessageType.Info);
        }

        private void DrawReplaceRenameTab()
        {
            EditorGUILayout.LabelField("查找替换", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("查找文本:", GUILayout.Width(80));
            replaceOld = EditorGUILayout.TextField(replaceOld);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("替换为:", GUILayout.Width(80));
            replaceNew = EditorGUILayout.TextField(replaceNew);
            EditorGUILayout.EndHorizontal();

            useRegexReplace = EditorGUILayout.ToggleLeft("使用正则表达式", useRegexReplace);

            if (useRegexReplace)
            {
                regexValidationMessage = ValidateRegexPattern(replaceOld);
                if (!string.IsNullOrEmpty(regexValidationMessage))
                {
                    EditorGUILayout.HelpBox(regexValidationMessage, MessageType.Warning);
                }
            }
            else
            {
                regexValidationMessage = "";
            }

            string replaceHelp = useRegexReplace
                ? "正则模式下支持分组替换。\n示例: 查找“^Btn_(.*)$” 替换为“Button_$1”"
                : "支持部分文本替换，区分大小写。\n示例: 查找“Btn” 替换为“Button”";
            EditorGUILayout.HelpBox(replaceHelp, MessageType.Info);
        }

        private void DrawPrefixSuffixTab()
        {
            EditorGUILayout.LabelField("添加前后缀", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("前缀:", GUILayout.Width(80));
            GUI.SetNextControlName("BatchRename_Prefix");
            prefix = EditorGUILayout.TextField(prefix);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("后缀:", GUILayout.Width(80));
            suffix = EditorGUILayout.TextField(suffix);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("在原名前后添加文本\n示例: 原名“Button” → “prefix_Button_suffix”", MessageType.Info);
        }

        private void DrawTrimTab()
        {
            EditorGUILayout.LabelField("删除前后字符", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("前删字符数:", GUILayout.Width(80));
            trimFrontCount = Mathf.Max(0, EditorGUILayout.IntField(trimFrontCount));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("后删字符数:", GUILayout.Width(80));
            trimBackCount = Mathf.Max(0, EditorGUILayout.IntField(trimBackCount));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("按原名称裁剪字符。\n示例: 原名“CHairA”，前删 1、后删 1 → “Hair”", MessageType.Info);
        }

        private void DrawCaseTab()
        {
            EditorGUILayout.LabelField("大小写转换", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("转换方式:", GUILayout.Width(80));
            caseMode = EditorGUILayout.Popup(caseMode, caseModeOptions);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("对当前名称整体做大小写转换。\n示例: hello_world → HELLO_WORLD / hello_world / Hello_World",
                MessageType.Info);
        }

        private string GetPreviewName(int index)
        {
            if (targetObjects == null || index >= targetObjects.Length || targetObjects[index] == null)
                return "";

            string originalName = GetObjectDisplayName(targetObjects[index]);

            switch (selectedTab)
            {
                case 0: // 序号重命名
                    if (string.IsNullOrEmpty(baseName) && !useUIComponentNameAsPrefix)
                        return originalName;
                    return BuildIndexedName(targetObjects[index], index);

                case 1: // 查找替换
                    if (string.IsNullOrEmpty(replaceOld))
                        return originalName;
                    return GetReplacedName(originalName);

                case 2: // 添加前后缀
                    return $"{prefix}{originalName}{suffix}";

                case 3: // 删除前后字符
                    return GetTrimmedName(originalName);

                case 4: // 大小写转换
                    return ApplyCaseMode(originalName);

                default:
                    return originalName;
            }
        }

        private void ExecuteRename()
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", $"没有需要重命名的{GetTargetLabel()}", "确定");
                return;
            }

            // 检查是否有有效的重命名参数
            bool hasValidParams = false;
            switch (selectedTab)
            {
                case 0:
                    hasValidParams = !string.IsNullOrEmpty(baseName) || useUIComponentNameAsPrefix;
                    break;
                case 1:
                    hasValidParams = !string.IsNullOrEmpty(replaceOld) && string.IsNullOrEmpty(regexValidationMessage);
                    break;
                case 2:
                    hasValidParams = !string.IsNullOrEmpty(prefix) || !string.IsNullOrEmpty(suffix);
                    break;
                case 3:
                    hasValidParams = trimFrontCount > 0 || trimBackCount > 0;
                    break;
                case 4:
                    hasValidParams = caseMode != (int)RenameCaseMode.None;
                    break;
            }

            if (!hasValidParams)
            {
                EditorUtility.DisplayDialog("提示", "请先设置重命名参数", "确定");
                return;
            }

            // 询问确认
            if (!EditorUtility.DisplayDialog("确认重命名",
                    $"即将重命名 {targetObjects.Length} 个{GetTargetLabel()}，是否继续？\n\n可以按 Ctrl+Z 撤销操作",
                    "确认", "取消"))
            {
                return;
            }

            // 记录操作以便撤销
            Undo.RecordObjects(targetObjects, "批量重命名");

            Dictionary<string, int> nameCount = new Dictionary<string, int>();
            int renamedCount = 0;
            List<string> conflicts = new List<string>();
            List<string> failures = new List<string>();
            AssetRenameBatch assetRenameBatch = currentSelectionSource == SelectionSource.Project
                ? new AssetRenameBatch()
                : null;

            for (int i = 0; i < targetObjects.Length; i++)
            {
                if (targetObjects[i] == null)
                    continue;

                string newName = "";

                switch (selectedTab)
                {
                    case 0: // 序号重命名
                        newName = BuildIndexedName(targetObjects[i], i);
                        break;

                    case 1: // 查找替换
                        newName = GetReplacedName(GetObjectDisplayName(targetObjects[i]));
                        break;

                    case 2: // 添加前后缀
                        newName = $"{prefix}{GetObjectDisplayName(targetObjects[i])}{suffix}";
                        break;

                    case 3: // 删除前后字符
                        newName = GetTrimmedName(GetObjectDisplayName(targetObjects[i]));
                        break;

                    case 4: // 大小写转换
                        newName = ApplyCaseMode(GetObjectDisplayName(targetObjects[i]));
                        break;
                }

                string originalName = GetObjectDisplayName(targetObjects[i]);
                if (!string.IsNullOrEmpty(newName) && newName != originalName)
                {
                    string finalName = newName;

                    // 处理重名情况
                    if (nameCount.ContainsKey(newName))
                    {
                        nameCount[newName]++;
                        finalName = $"{newName}_{nameCount[newName]}";
                        conflicts.Add($"重名处理: {originalName} → {finalName}");
                    }
                    else
                    {
                        nameCount[newName] = 0;
                    }

                    if (RenameTargetObject(targetObjects[i], finalName, out string errorMessage))
                    {
                        renamedCount++;
                        EditorUtility.SetDirty(targetObjects[i]);
                        if (assetRenameBatch != null)
                        {
                            assetRenameBatch.records.Add(new AssetRenameRecord
                            {
                                assetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(targetObjects[i])),
                                oldName = originalName,
                                newName = finalName
                            });
                        }
                    }
                    else if (!string.IsNullOrEmpty(errorMessage))
                    {
                        failures.Add($"{originalName} → {finalName}: {errorMessage}");
                    }
                }
            }

            // 刷新资源
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            RegisterAssetRenameUndo(assetRenameBatch);

            // 显示结果
            string message = $"成功重命名了 {renamedCount} 个{GetTargetLabel()}";
            if (conflicts.Count > 0)
            {
                message += $"\n\n有 {conflicts.Count} 个重名冲突已自动处理";
                if (conflicts.Count <= 5)
                {
                    message += ":\n" + string.Join("\n", conflicts);
                }
            }

            if (failures.Count > 0)
            {
                message += $"\n\n有 {failures.Count} 个{GetTargetLabel()}重命名失败";
                if (failures.Count <= 5)
                {
                    message += ":\n" + string.Join("\n", failures);
                }
            }

            EditorUtility.DisplayDialog("完成", message, "确定");

            // 重新绘制窗口
            Repaint();

            // 刷新Hierarchy视图
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();
            RefreshSelectedObjects();
        }

        private void ResetParameters()
        {
            baseName = "";
            replaceOld = "";
            replaceNew = "";
            startIndex = 1;
            incrementValue = 1;
            numberPadding = 0;
            sequencePosition = 0;
            prefix = "";
            suffix = "";
            useUIComponentNameAsPrefix = false;
            trimFrontCount = 0;
            trimBackCount = 0;
            useRegexReplace = false;
            regexValidationMessage = "";
            caseMode = (int)RenameCaseMode.None;
        }

        private void RefreshSelectedObjects()
        {
            targetObjects = GetCurrentOrderedSelection();

            Repaint();
        }

        private void UpdateFromCurrentSelection()
        {
            targetObjects = GetCurrentOrderedSelection();

            EditorUtility.DisplayDialog("更新成功", $"已更新选择，当前 {targetObjects.Length} 个{GetTargetLabel()}", "确定");
            Repaint();
        }

        private void OnSelectionChange()
        {
            if (lockSelection)
            {
                return;
            }

            targetObjects = GetCurrentOrderedSelection();

            Repaint();
        }

        private UnityEngine.Object[] GetCurrentOrderedSelection()
        {
            return GetOrderedSelection(currentSelectionSource);
        }

        private static UnityEngine.Object[] GetOrderedSelection(SelectionSource selectionSource)
        {
            switch (selectionSource)
            {
                case SelectionSource.Project:
                    return GetProjectOrderedSelection();
                default:
                    return GetHierarchyOrderedSelection();
            }
        }

        private static UnityEngine.Object[] GetHierarchyOrderedSelection()
        {
            return SortObjectsByHierarchyOrder(Selection.gameObjects);
        }

        private static UnityEngine.Object[] GetProjectOrderedSelection()
        {
            string[] assetGuids = Selection.assetGUIDs;
            if (assetGuids == null || assetGuids.Length == 0)
            {
                return Array.Empty<UnityEngine.Object>();
            }

            List<UnityEngine.Object> selectedAssets = new List<UnityEngine.Object>(assetGuids.Length);
            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (asset != null)
                {
                    selectedAssets.Add(asset);
                }
            }

            return SortObjectsBySelectionOrder(selectedAssets.ToArray(), SelectionSource.Project);
        }

        private static UnityEngine.Object[] SortObjectsBySelectionOrder(UnityEngine.Object[] objects, SelectionSource selectionSource)
        {
            if (objects == null || objects.Length <= 1)
            {
                return objects ?? Array.Empty<UnityEngine.Object>();
            }

            switch (selectionSource)
            {
                case SelectionSource.Project:
                    return SortObjectsByProjectOrder(objects);
                default:
                    List<GameObject> gameObjects = new List<GameObject>(objects.Length);
                    foreach (UnityEngine.Object obj in objects)
                    {
                        if (obj is GameObject gameObject)
                        {
                            gameObjects.Add(gameObject);
                        }
                    }

                    return SortObjectsByHierarchyOrder(gameObjects.ToArray());
            }
        }

        private static UnityEngine.Object[] SortObjectsByHierarchyOrder(GameObject[] objects)
        {
            if (objects == null || objects.Length <= 1)
            {
                return objects == null ? Array.Empty<UnityEngine.Object>() : Array.ConvertAll(objects, obj => (UnityEngine.Object)obj);
            }

            GameObject[] sortedObjects = (GameObject[])objects.Clone();
            Dictionary<int, int> sceneOrderLookup = BuildSceneOrderLookup();

            Array.Sort(sortedObjects,
                (left, right) => CompareGameObjectsByHierarchyOrder(left, right, sceneOrderLookup));
            return Array.ConvertAll(sortedObjects, obj => (UnityEngine.Object)obj);
        }

        private static UnityEngine.Object[] SortObjectsByProjectOrder(UnityEngine.Object[] objects)
        {
            UnityEngine.Object[] sortedObjects = (UnityEngine.Object[])objects.Clone();
            Array.Sort(sortedObjects, CompareProjectObjectsByWindowOrder);
            return sortedObjects;
        }

        private static int CompareGameObjectsByHierarchyOrder(GameObject left, GameObject right,
            Dictionary<int, int> sceneOrderLookup)
        {
            if (ReferenceEquals(left, right))
            {
                return 0;
            }

            if (left == null)
            {
                return 1;
            }

            if (right == null)
            {
                return -1;
            }

            int leftSceneOrder = GetSceneOrder(left.scene, sceneOrderLookup);
            int rightSceneOrder = GetSceneOrder(right.scene, sceneOrderLookup);
            int sceneComparison = leftSceneOrder.CompareTo(rightSceneOrder);
            if (sceneComparison != 0)
            {
                return sceneComparison;
            }

            List<int> leftPath = GetHierarchyPath(left.transform);
            List<int> rightPath = GetHierarchyPath(right.transform);
            int compareLength = Mathf.Min(leftPath.Count, rightPath.Count);
            for (int i = 0; i < compareLength; i++)
            {
                int pathComparison = leftPath[i].CompareTo(rightPath[i]);
                if (pathComparison != 0)
                {
                    return pathComparison;
                }
            }

            int depthComparison = leftPath.Count.CompareTo(rightPath.Count);
            if (depthComparison != 0)
            {
                return depthComparison;
            }

            return string.CompareOrdinal(left.name, right.name);
        }

        private static int CompareProjectObjectsByWindowOrder(UnityEngine.Object left, UnityEngine.Object right)
        {
            if (ReferenceEquals(left, right))
            {
                return 0;
            }

            if (left == null)
            {
                return 1;
            }

            if (right == null)
            {
                return -1;
            }

            string leftPath = AssetDatabase.GetAssetPath(left);
            string rightPath = AssetDatabase.GetAssetPath(right);
            int pathComparison = EditorUtility.NaturalCompare(leftPath, rightPath);
            if (pathComparison != 0)
            {
                return pathComparison;
            }

            return EditorUtility.NaturalCompare(GetObjectDisplayName(left), GetObjectDisplayName(right));
        }

        private static Dictionary<int, int> BuildSceneOrderLookup()
        {
            Dictionary<int, int> sceneOrderLookup = new Dictionary<int, int>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                sceneOrderLookup[scene.handle] = i;
            }

            return sceneOrderLookup;
        }

        private static int GetSceneOrder(Scene scene, Dictionary<int, int> sceneOrderLookup)
        {
            if (sceneOrderLookup.TryGetValue(scene.handle, out int sceneOrder))
            {
                return sceneOrder;
            }

            return int.MaxValue;
        }

        private static List<int> GetHierarchyPath(Transform targetTransform)
        {
            List<int> path = new List<int>();
            Transform current = targetTransform;
            while (current != null)
            {
                path.Add(current.GetSiblingIndex());
                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        private string BuildIndexedName(UnityEngine.Object targetObject, int index)
        {
            if (targetObject == null)
            {
                return "";
            }

            string numberText = FormatSequenceNumber(startIndex + index * incrementValue);
            string sequenceName;
            if (string.IsNullOrEmpty(baseName))
            {
                sequenceName = numberText;
            }
            else
            {
                sequenceName = sequencePosition == 0
                    ? $"{baseName}_{numberText}"
                    : $"{numberText}_{baseName}";
            }

            if (!useUIComponentNameAsPrefix || !(targetObject is GameObject gameObject))
            {
                return sequenceName;
            }

            string componentPrefix = GetUIComponentPrefix(gameObject);
            if (string.IsNullOrEmpty(componentPrefix))
            {
                return sequenceName;
            }

            return string.IsNullOrEmpty(sequenceName)
                ? componentPrefix
                : $"{componentPrefix}_{sequenceName}";
        }

        private static string GetUIComponentPrefix(GameObject targetObject)
        {
            if (targetObject == null)
            {
                return "";
            }

            UIBehaviour[] uiBehaviours = targetObject.GetComponents<UIBehaviour>();
            if (uiBehaviours == null || uiBehaviours.Length == 0)
            {
                return "";
            }

            string[] preferredTypeNames =
            {
                "Button",
                "Toggle",
                "Slider",
                "Scrollbar",
                "ScrollRect",
                "InputField",
                "TMP_InputField",
                "Dropdown",
                "TMP_Dropdown",
                "TextMeshProUGUI",
                "Text",
                "Image",
                "RawImage"
            };

            List<string> componentNames = new List<string>();
            HashSet<string> addedNames = new HashSet<string>();

            foreach (string preferredTypeName in preferredTypeNames)
            {
                foreach (UIBehaviour uiBehaviour in uiBehaviours)
                {
                    if (uiBehaviour == null)
                    {
                        continue;
                    }

                    if (uiBehaviour.GetType().Name == preferredTypeName)
                    {
                        if (addedNames.Add(preferredTypeName))
                        {
                            componentNames.Add(preferredTypeName);
                        }
                    }
                }
            }

            foreach (UIBehaviour uiBehaviour in uiBehaviours)
            {
                if (uiBehaviour == null)
                {
                    continue;
                }

                string typeName = uiBehaviour.GetType().Name;
                if (addedNames.Add(typeName))
                {
                    componentNames.Add(typeName);
                }
            }

            return componentNames.Count == 0 ? "" : string.Join("_", componentNames);
        }

        private bool RenameTargetObject(UnityEngine.Object targetObject, string newName, out string errorMessage)
        {
            errorMessage = "";

            if (targetObject == null)
            {
                errorMessage = "对象为空";
                return false;
            }

            if (currentSelectionSource == SelectionSource.Project)
            {
                string assetPath = AssetDatabase.GetAssetPath(targetObject);
                if (string.IsNullOrEmpty(assetPath))
                {
                    errorMessage = "找不到资源路径";
                    return false;
                }

                errorMessage = AssetDatabase.RenameAsset(assetPath, newName);
                return string.IsNullOrEmpty(errorMessage);
            }

            targetObject.name = newName;
            return true;
        }

        private string GetSelectionSourceLabel()
        {
            return currentSelectionSource == SelectionSource.Project ? "Project" : "Hierarchy";
        }

        private string GetTargetLabel()
        {
            return currentSelectionSource == SelectionSource.Project ? "资源" : "物体";
        }

        private static string GetObjectDisplayName(UnityEngine.Object targetObject)
        {
            return targetObject == null ? "" : targetObject.name;
        }

        private static void RegisterAssetRenameUndo(AssetRenameBatch assetRenameBatch)
        {
            if (assetRenameBatch == null || assetRenameBatch.records.Count == 0)
            {
                return;
            }

            AssetRenameUndoState undoState = AssetRenameUndoState.instance;
            Undo.RecordObject(undoState, "批量重命名资源");

            if (undoState.appliedBatchCount < AssetRenameHistory.Count)
            {
                AssetRenameHistory.RemoveRange(undoState.appliedBatchCount, AssetRenameHistory.Count - undoState.appliedBatchCount);
            }

            AssetRenameHistory.Add(assetRenameBatch);
            undoState.appliedBatchCount = AssetRenameHistory.Count;
            appliedAssetRenameBatchCount = undoState.appliedBatchCount;
            EditorUtility.SetDirty(undoState);
        }

        private static void OnUndoRedoPerformed()
        {
            AssetRenameUndoState undoState = AssetRenameUndoState.instance;
            int targetBatchCount = Mathf.Clamp(undoState.appliedBatchCount, 0, AssetRenameHistory.Count);
            if (targetBatchCount == appliedAssetRenameBatchCount)
            {
                return;
            }

            if (targetBatchCount < appliedAssetRenameBatchCount)
            {
                for (int i = appliedAssetRenameBatchCount - 1; i >= targetBatchCount; i--)
                {
                    ApplyAssetRenameBatch(AssetRenameHistory[i], true);
                }
            }
            else
            {
                for (int i = appliedAssetRenameBatchCount; i < targetBatchCount; i++)
                {
                    ApplyAssetRenameBatch(AssetRenameHistory[i], false);
                }
            }

            appliedAssetRenameBatchCount = targetBatchCount;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorApplication.RepaintProjectWindow();
        }

        private static void ApplyAssetRenameBatch(AssetRenameBatch batch, bool undo)
        {
            if (batch == null || batch.records == null || batch.records.Count == 0)
            {
                return;
            }

            if (undo)
            {
                for (int i = batch.records.Count - 1; i >= 0; i--)
                {
                    ApplyAssetRenameRecord(batch.records[i], true);
                }
            }
            else
            {
                for (int i = 0; i < batch.records.Count; i++)
                {
                    ApplyAssetRenameRecord(batch.records[i], false);
                }
            }
        }

        private static void ApplyAssetRenameRecord(AssetRenameRecord record, bool undo)
        {
            if (record == null || string.IsNullOrEmpty(record.assetGuid))
            {
                return;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(record.assetGuid);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            string targetName = undo ? record.oldName : record.newName;
            if (string.IsNullOrEmpty(targetName))
            {
                return;
            }

            string currentName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            if (currentName == targetName)
            {
                return;
            }

            _ = AssetDatabase.RenameAsset(assetPath, targetName);
        }

        private string FormatSequenceNumber(int number)
        {
            if (numberPadding <= 0)
            {
                return number.ToString();
            }

            return number.ToString("D" + numberPadding);
        }

        private string GetTrimmedName(string originalName)
        {
            if (string.IsNullOrEmpty(originalName))
            {
                return originalName;
            }

            int start = Mathf.Min(trimFrontCount, originalName.Length);
            int endExclusive = Mathf.Max(start, originalName.Length - trimBackCount);
            int length = endExclusive - start;

            if (length <= 0)
            {
                return "";
            }

            return originalName.Substring(start, length);
        }

        private string GetReplacedName(string originalName)
        {
            if (string.IsNullOrEmpty(originalName) || string.IsNullOrEmpty(replaceOld))
            {
                return originalName;
            }

            if (!useRegexReplace)
            {
                return originalName.Replace(replaceOld, replaceNew);
            }

            try
            {
                return Regex.Replace(originalName, replaceOld, replaceNew);
            }
            catch (System.ArgumentException)
            {
                return originalName;
            }
        }

        private string ApplyCaseMode(string originalName)
        {
            if (string.IsNullOrEmpty(originalName))
            {
                return originalName;
            }

            switch ((RenameCaseMode)caseMode)
            {
                case RenameCaseMode.Upper:
                    return originalName.ToUpper();
                case RenameCaseMode.Lower:
                    return originalName.ToLower();
                case RenameCaseMode.Title:
                    return ToTitleCasePreservingSeparators(originalName);
                default:
                    return originalName;
            }
        }

        private string ToTitleCasePreservingSeparators(string originalName)
        {
            char[] chars = originalName.ToLower().ToCharArray();
            bool capitalizeNext = true;

            for (int i = 0; i < chars.Length; i++)
            {
                if (char.IsLetterOrDigit(chars[i]))
                {
                    if (capitalizeNext && char.IsLetter(chars[i]))
                    {
                        chars[i] = char.ToUpper(chars[i]);
                    }

                    capitalizeNext = false;
                }
                else
                {
                    capitalizeNext = true;
                }
            }

            return new string(chars);
        }

        private string ValidateRegexPattern(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return "";
            }

            try
            {
                _ = new Regex(pattern);
                return "";
            }
            catch (System.ArgumentException exception)
            {
                return $"正则表达式无效: {exception.Message}";
            }
        }

        private void DrawPresetSection()
        {
            EditorGUILayout.LabelField("命名预设", EditorStyles.boldLabel);

            if (!hasLoadedPresets)
            {
                LoadPresets();
            }

            string[] presetNames = GetPresetNames();
            int safeIndex = Mathf.Clamp(selectedPresetIndex, 0, Mathf.Max(0, presetNames.Length - 1));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("已保存:", GUILayout.Width(80));
            int newIndex = EditorGUILayout.Popup(safeIndex, presetNames);
            if (newIndex != safeIndex)
            {
                selectedPresetIndex = newIndex;
                if (selectedPresetIndex > 0 && selectedPresetIndex - 1 < presets.Count)
                {
                    presetName = presets[selectedPresetIndex - 1].presetName;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("预设名:", GUILayout.Width(80));
            presetName = EditorGUILayout.TextField(presetName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存当前预设", GUILayout.Width(120)))
            {
                SaveCurrentPreset();
            }

            if (GUILayout.Button("加载所选预设", GUILayout.Width(120)))
            {
                LoadSelectedPreset();
            }

            if (GUILayout.Button("删除所选预设", GUILayout.Width(120)))
            {
                DeleteSelectedPreset();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void LoadPresets()
        {
            hasLoadedPresets = true;
            presets = new List<RenamePresetData>();
            string json = F8EditorPrefs.GetString(PresetStorageKey, "");
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            try
            {
                RenamePresetCollection collection = JsonUtility.FromJson<RenamePresetCollection>(json);
                if (collection != null && collection.presets != null)
                {
                    presets = collection.presets;
                }
            }
            catch
            {
                presets = new List<RenamePresetData>();
            }
        }

        private void SavePresets()
        {
            RenamePresetCollection collection = new RenamePresetCollection
            {
                presets = presets
            };

            F8EditorPrefs.SetString(PresetStorageKey, JsonUtility.ToJson(collection));
        }

        private string[] GetPresetNames()
        {
            List<string> names = new List<string> { "未选择" };
            foreach (RenamePresetData preset in presets)
            {
                names.Add(preset.presetName);
            }

            return names.ToArray();
        }

        private void SaveCurrentPreset()
        {
            string trimmedPresetName = presetName == null ? "" : presetName.Trim();
            if (string.IsNullOrEmpty(trimmedPresetName))
            {
                EditorUtility.DisplayDialog("提示", "请先输入预设名称", "确定");
                return;
            }

            presetName = trimmedPresetName;

            RenamePresetData data = CaptureCurrentPreset(trimmedPresetName);
            int existingIndex = presets.FindIndex(p => p.presetName == trimmedPresetName);
            if (existingIndex >= 0)
            {
                presets[existingIndex] = data;
                selectedPresetIndex = existingIndex + 1;
            }
            else
            {
                presets.Add(data);
                selectedPresetIndex = presets.Count;
            }

            SavePresets();
            EditorUtility.DisplayDialog("完成", $"已保存预设：{trimmedPresetName}", "确定");
        }

        private void LoadSelectedPreset()
        {
            if (selectedPresetIndex <= 0 || selectedPresetIndex - 1 >= presets.Count)
            {
                EditorUtility.DisplayDialog("提示", "请先选择一个预设", "确定");
                return;
            }

            ApplyPreset(presets[selectedPresetIndex - 1]);
            EditorUtility.DisplayDialog("完成", $"已加载预设：{presets[selectedPresetIndex - 1].presetName}", "确定");
        }

        private void DeleteSelectedPreset()
        {
            if (selectedPresetIndex <= 0 || selectedPresetIndex - 1 >= presets.Count)
            {
                EditorUtility.DisplayDialog("提示", "请先选择一个预设", "确定");
                return;
            }

            string deletingName = presets[selectedPresetIndex - 1].presetName;
            if (!EditorUtility.DisplayDialog("确认删除", $"确定删除预设“{deletingName}”吗？", "删除", "取消"))
            {
                return;
            }

            presets.RemoveAt(selectedPresetIndex - 1);
            selectedPresetIndex = 0;
            if (presetName == deletingName)
            {
                presetName = "";
            }

            SavePresets();
            EditorUtility.DisplayDialog("完成", $"已删除预设：{deletingName}", "确定");
            Repaint();
        }

        private RenamePresetData CaptureCurrentPreset(string currentPresetName)
        {
            return new RenamePresetData
            {
                presetName = currentPresetName,
                selectedTab = selectedTab,
                baseName = baseName,
                replaceOld = replaceOld,
                replaceNew = replaceNew,
                startIndex = startIndex,
                incrementValue = incrementValue,
                prefix = prefix,
                suffix = suffix,
                useUIComponentNameAsPrefix = useUIComponentNameAsPrefix,
                numberPadding = numberPadding,
                sequencePosition = sequencePosition,
                trimFrontCount = trimFrontCount,
                trimBackCount = trimBackCount,
                useRegexReplace = useRegexReplace,
                caseMode = caseMode
            };
        }

        private void ApplyPreset(RenamePresetData data)
        {
            if (data == null)
            {
                return;
            }

            presetName = data.presetName;
            selectedTab = Mathf.Clamp(data.selectedTab, 0, tabNames.Length - 1);
            baseName = data.baseName ?? "";
            replaceOld = data.replaceOld ?? "";
            replaceNew = data.replaceNew ?? "";
            startIndex = data.startIndex;
            incrementValue = data.incrementValue;
            prefix = data.prefix ?? "";
            suffix = data.suffix ?? "";
            useUIComponentNameAsPrefix = data.useUIComponentNameAsPrefix;
            numberPadding = Mathf.Max(0, data.numberPadding);
            sequencePosition = Mathf.Clamp(data.sequencePosition, 0, sequencePositionOptions.Length - 1);
            trimFrontCount = Mathf.Max(0, data.trimFrontCount);
            trimBackCount = Mathf.Max(0, data.trimBackCount);
            useRegexReplace = data.useRegexReplace;
            caseMode = Mathf.Clamp(data.caseMode, 0, caseModeOptions.Length - 1);
            regexValidationMessage = useRegexReplace ? ValidateRegexPattern(replaceOld) : "";
            Repaint();
        }

        private void OnEnable()
        {
            LoadPresets();
            appliedAssetRenameBatchCount = Mathf.Clamp(AssetRenameUndoState.instance.appliedBatchCount, 0, AssetRenameHistory.Count);
        }
    }
}
