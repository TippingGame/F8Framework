using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core.Editor
{
    public class UICollectionLocalization : UnityEditor.Editor
    {
        static string CSV_Path = "Assets/Asset/Localization收集的中文.csv";
        static List<string> textInstanceIDs = new List<string>();
        static Dictionary<string, string> textIdToText = new Dictionary<string, string>();

        [MenuItem("Assets/（F8UI界面管理功能）/（收集UI所有的中文放入本地化表，并添加组件）", false, 1025)]
        public static void CollectionLocalization()
        {
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            CSV_Path = "Assets/Asset/Localization收集的中文_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            textInstanceIDs.Clear();
            textIdToText.Clear();
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string absolutePath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));

                if (File.Exists(absolutePath))
                {
                    if (IsPrefabAsset(assetPath))
                    {
                        GetAllPrefabsText(assetPath);
                        LogF8.Log("文件:" + assetPath);
                    }
                }
                else if (Directory.Exists(absolutePath))
                {
                    // 获取所有文件
                    string[] assetPaths = Directory.GetFiles(absolutePath, "*.prefab", SearchOption.AllDirectories)
                        .Where(path =>
                            !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) &&
                            !path.EndsWith(".DS_Store", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    foreach (string _assetPath in assetPaths)
                    {
                        assetPath = FileTools.FormatToUnityPath(ABBuildTool.GetAssetPath(_assetPath));
                        if (IsPrefabAsset(assetPath))
                        {
                            GetAllPrefabsText(assetPath);
                            LogF8.Log("文件:" + assetPath);
                        }
                    }
                }
            }

            WriteCsv(textInstanceIDs, CSV_Path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            bool enableTMP = false;
#if LOCALIZER_TMP
            enableTMP = true;
#endif
            LogF8.Log($"收集中文数量：{textInstanceIDs.Count}，自动添加本地化组件，是否启用TMP组件：{enableTMP}，CSV路径：{CSV_Path}");
        }

        static void GetAllPrefabsText(string assetPath)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            if (go != null)
            {
                // 创建 Prefab 实例
                GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(go);
                bool prefabChanged = false;
                
                // 获取Prefab及其子物体孙物体.......的所有Text组件
                Text[] texts = prefabInstance.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i].text != "" && CheckString(texts[i].text))
                    {
                        string textId = CollectText(texts[i].text);
                        prefabChanged |= AddOrUpdateTextLocalizer(texts[i].gameObject, textId);
                    }
                }
                
#if LOCALIZER_TMP
                // TMP组件
                TMPro.TMP_Text[] tmps = prefabInstance.GetComponentsInChildren<TMPro.TMP_Text>(true);
                for (int i = 0; i < tmps.Length; i++)
                {
                    if (tmps[i].text != "" && CheckString(tmps[i].text))
                    {
                        string textId = CollectText(tmps[i].text);
                        prefabChanged |= AddOrUpdateTextLocalizer(tmps[i].gameObject, textId);
                    }
                }
#endif

                // 保存Prefab的修改
                if (prefabChanged)
                {
                    // 标记 Prefab 为脏以确保更改被保存
                    EditorUtility.SetDirty(prefabInstance);
                    // 保存 Prefab 更改
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, assetPath);
                }

                // 销毁实例以清理内存
                GameObject.DestroyImmediate(prefabInstance);
            }

        }

        static bool IsPrefabAsset(string assetPath)
        {
            return !string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase);
        }

        static bool IsChinese(char c)
        {
            return c >= 0x4E00 && c <= 0x9FFF;
        }

        static bool CheckString(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                char[] ch = str.ToCharArray();
                for (int i = 0; i < ch.Length; i++)
                {
                    if (IsChinese(ch[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static string CollectText(string text)
        {
            string baseTextId = Util.Encryption.MD5Encrypt16(text);
            string textId = baseTextId;
            int collisionIndex = 1;
            while (textIdToText.TryGetValue(textId, out string existingText) && existingText != text)
            {
                textId = baseTextId + "_" + collisionIndex;
                collisionIndex++;
            }

            if (!textIdToText.ContainsKey(textId))
            {
                textIdToText.Add(textId, text);
                textInstanceIDs.Add(textId + "," + EscapeCsv(text));
            }

            return textId;
        }

        static bool AddOrUpdateTextLocalizer(GameObject gameObject, string textId)
        {
            if (gameObject == null)
            {
                return false;
            }

            F8Framework.Core.TextLocalizer localizer = gameObject.GetComponent<F8Framework.Core.TextLocalizer>();
            if (localizer == null)
            {
                localizer = gameObject.AddComponent<F8Framework.Core.TextLocalizer>();
                localizer.textId = textId;
                EditorUtility.SetDirty(gameObject);
                return true;
            }

            if (localizer.textId == textId)
            {
                return false;
            }

            localizer.textId = textId;
            EditorUtility.SetDirty(localizer);
            return true;
        }

        static string EscapeCsv(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\r") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        static void WriteCsv(List<string> strs, string path)
        {
            FileTools.CheckFileAndCreateDirWhenNeeded(path);
            // UTF-8方式保存
            using (StreamWriter stream = new StreamWriter(path, false, Encoding.UTF8))
            {
                stream.WriteLine("TextID,ChineseSimplified");
                for (int i = 0; i < strs.Count; i++)
                {
                    if (strs[i] != null && strs[i] != "")
                        stream.WriteLine(strs[i]);
                }
            }
        }
    }
}
