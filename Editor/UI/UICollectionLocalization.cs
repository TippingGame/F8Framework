using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
// using TMPro;

namespace F8Framework.Core.Editor
{
    public class UICollectionLocalization : UnityEditor.Editor
    {
        static string CSV_Path = "Assets/Asset/Localization收集的中文.csv";
        static List<string> textInstanceIDs = new List<string>();

        [MenuItem("Assets/（F8UI界面管理功能）/（收集UI所有的中文放入本地化表）", false, -5)]
        public static void CollectionLocalization()
        {
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            textInstanceIDs.Clear();
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string absolutePath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));

                if (File.Exists(absolutePath))
                {
                    GetAllPrefabsText(assetPath);
                    // LogF8.Log("文件" + assetPath);
                }
                else if (Directory.Exists(absolutePath))
                {
                    // LogF8.Log("文件夹" + assetPath);

                    // 获取所有文件
                    string[] assetPaths = Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories)
                        .Where(path =>
                            !path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) &&
                            !path.EndsWith(".DS_Store", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    foreach (string _assetPath in assetPaths)
                    {
                        GetAllPrefabsText(ABBuildTool.GetAssetPath(_assetPath));
                        LogF8.Log("文件" + _assetPath);
                    }
                }
            }

            LogF8.Log("生成SCV完成，数据有：" + textInstanceIDs.Count);
            
            AssetDatabase.Refresh();

            LogF8.Log("收集中文完成！（可自定义其他操作：如搜索TMP文字，添加本地化组件。）" + CSV_Path);
        }

        static void GetAllPrefabsText(string assetPath)
        {
            int index = 0;
            GameObject go = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            if (go != null)
            {
                // 创建 Prefab 实例
                GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(go);
                
                // 获取Prefab及其子物体孙物体.......的所有Text组件
                Text[] texts = prefabInstance.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i].text != "" && CheckString(texts[i].text))
                    {
                        textInstanceIDs.Add(textInstanceIDs.Count.ToString().PadLeft(4, '0') + "：" + texts[i].text);
                        
                        // 添加本地化组件
                        // if (texts[i].gameObject != null)
                        // {
                        //     if (!texts[i].gameObject.GetComponent<TextLocalizer>())
                        //     {
                        //         TextLocalizer tl = texts[i].gameObject.AddComponent<TextLocalizer>();
                        //         tl.textId = textInstanceIDs.Count.ToString().PadLeft(4, '0');
                        //     }
                        //    
                        // }

                        index++;
                    }
                }
                
                // // TMP组件
                // TMPro.TMP_Text[] tmps = prefabInstance.GetComponentsInChildren<TMPro.TMP_Text>(true);
                // for (int i = 0; i < tmps.Length; i++)
                // {
                //     if (tmps[i].text != "" && CheckString(tmps[i].text))
                //     {
                //         textInstanceIDs.Add(textInstanceIDs.Count.ToString().PadLeft(4, '0') + "：" + tmps[i].text);
                //         
                //         // 添加本地化组件
                //         // if (texts[i].gameObject != null)
                //         // {
                //         //     if (!texts[i].gameObject.GetComponent<TextLocalizer>())
                //         //     {
                //         //         TextLocalizer tl = texts[i].gameObject.AddComponent<TextLocalizer>();
                //         //         tl.textId = textInstanceIDs.Count.ToString().PadLeft(4, '0');
                //         //     }
                //         //    
                //         // }
                //
                //         index++;
                //     }
                // }
                //
                // // 保存Prefab的修改
                // if (index > 0)
                // {
                //     // 标记 Prefab 为脏以确保更改被保存
                //     EditorUtility.SetDirty(prefabInstance);
                //     // 保存 Prefab 更改
                //     PrefabUtility.SaveAsPrefabAsset(prefabInstance, assetPath);
                //     // 销毁实例以清理内存
                //     GameObject.DestroyImmediate(prefabInstance);
                // }
            }

            WriteCsv(textInstanceIDs, CSV_Path);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static bool IsChinese(char c)
        {
            return c >= 0x4E00 && c <= 0x9FA5;
        }

        static bool CheckString(string str)
        {
            char[] ch = str.ToCharArray();
            if (str != null)
            {
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

        static void WriteCsv(List<string> strs, string path)
        {
            FileTools.CheckFileAndCreateDirWhenNeeded(path);
            // UTF-8方式保存
            using (StreamWriter stream = new StreamWriter(path, false, Encoding.UTF8))
            {
                for (int i = 0; i < strs.Count; i++)
                {
                    if (i == 0 && path == CSV_Path)
                    {
                        stream.WriteLine("id：简体中文");
                    }

                    if (strs[i] != null && strs[i] != "")
                        stream.WriteLine(strs[i]);
                }
            }
        }
    }
}