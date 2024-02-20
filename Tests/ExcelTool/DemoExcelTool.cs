using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using F8Framework.Core;
using F8Framework.ConfigData;

public class DemoExcelTool : MonoBehaviour
{
    public Text text;
    //方式一：
    // void Start()
    // {
    //     //读取二进制文件
    //     F8DataManager.Instance.LoadAll();
    //     text.text += F8DataManager.Instance.Gettable1ByID(1).category.Length;
    //     Debug.Log(F8DataManager.Instance.Gettable1ByID(1).category.Length);
    //     text.text += F8DataManager.Instance.Gettable1ByID(1).price3;
    //     Debug.Log(F8DataManager.Instance.Gettable1ByID(1).price3);
    //     text.text += F8DataManager.Instance.Gettable1ByID(1).name;
    //     Debug.Log(F8DataManager.Instance.Gettable1ByID(1).name);
    //     if (F8DataManager.Instance.Gettable1ByID(33) != null)
    //     {
    //         foreach (var VARIABLE in F8DataManager.Instance.Gettable1ByID(33).objfsaads)
    //         {
    //             foreach (var VARIABLE2 in VARIABLE)
    //             {
    //                 text.text += VARIABLE2;
    //                 Debug.Log(VARIABLE2);
    //                 Debug.Log(VARIABLE2.GetType());
    //             }
    //         }
    //     }
    // }
    //方式二：
    // IEnumerator Start()
    // {
    //     //由于安卓资源都在包内，需要先复制到可读写文件夹1
    //     string assetPath = URLSetting.STREAMINGASSETS_URL + "config";
    //     string[] paths = null;
    //     WWW www = new WWW(assetPath + "/fileindex.txt");
    //     yield return www;
    //     if (www.error != null)
    //     {
    //         Debug.Log(www.error);
    //         yield return null;
    //     }
    //     else
    //     {
    //         string ss = www.text;
    //         // 去除夹杂的空行
    //         string[] lines = ss.Split('\n');
    //         List<string> nonEmptyLines = new List<string>();
    //
    //         foreach (string line in lines)
    //         {
    //             string trimmedLine = line.Trim();
    //
    //             if (!string.IsNullOrEmpty(trimmedLine))
    //             {
    //                 nonEmptyLines.Add(trimmedLine);
    //             }
    //         }
    //
    //         paths = nonEmptyLines.ToArray();
    //     }
    //
    //     for (int i = 0; i < paths.Length; i++)
    //     {
    //         yield return CopyAssets(paths[i].Replace("\r", ""));
    //     }
    //     //读取Excel文件
    //     ReadExcel.Instance.LoadAllExcelData();
    //     text.text += F8DataManager.Instance.Gettable1ByID(115).name;
    //     Debug.Log(F8DataManager.Instance.Gettable1ByID(115).name);
    //     foreach (var VARIABLE in F8DataManager.Instance.Gettable1ByID(113).llliststr)
    //     {
    //         foreach (var VARIABLE2 in VARIABLE)
    //         {
    //             text.text += VARIABLE2;
    //             Debug.Log(VARIABLE2);
    //         }
    //     }
    // }
    // //由于安卓资源都在包内，需要先复制到可读写文件夹2
    // IEnumerator CopyAssets(string paths)
    // {
    //     string assetPath = URLSetting.STREAMINGASSETS_URL + "config";
    //     string sdCardPath = Application.persistentDataPath + "/config";
    //     WWW www = new WWW(assetPath + "/" + paths);
    //     yield return www;
    //     if(www.error != null)
    //     {
    //         Debug.Log(www.error);
    //         yield return null;
    //     }
    //     else
    //     {
    //         FileTools.SafeWriteAllBytes(sdCardPath + "/" + paths, www.bytes);
    //     }
    // }
}
