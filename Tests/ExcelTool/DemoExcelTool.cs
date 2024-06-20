using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F8Framework.Core;
using F8Framework.F8ExcelDataClass;
using F8Framework.Launcher;

namespace F8Framework.Tests
{
    public class DemoExcelTool : MonoBehaviour
    {
        // 方式一：读取二进制或者json
        IEnumerator Start()
        {
            // 同步加载全部配置
            FF8.Config.LoadAll();
        
            foreach (var item in FF8.Config.LoadAllAsync()) // 异步加载全部配置
            {
                yield return item;
            }
        
            // 单个表单个数据
            LogF8.Log(FF8.Config.GetSheet1ByID(2).name);
        
            // 单个表全部数据
            foreach (var item in FF8.Config.GetSheet1())
            {
                LogF8.Log(item.Key);
                LogF8.Log(item.Value.name);
            }
        
            // 指定名字加载
            Sheet1 sheet1 = new Sheet1();
            sheet1 = FF8.Config.Load<Sheet1>("Sheet1");
            LogF8.Log(sheet1.Dict[2].name);
        }

        // // 方式二：运行时读取Excel
        // IEnumerator Start()
        // {
        //     // 由于安卓资源都在包内，需要先复制到可读写文件夹1
        //     string assetPath = URLSetting.STREAMINGASSETS_URL + "config";
        //     string[] paths = null;
        //     WWW www = new WWW(assetPath + "/fileindex.txt");
        //     yield return www;
        //     if (www.error != null)
        //     {
        //         LogF8.Log(www.error);
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
        //     // 读取Excel文件
        //     ReadExcel.Instance.LoadAllExcelData();
        //     LogF8.Log(FF8.Config.GetSheet1ByID(1).name);
        //     LogF8.Log(FF8.Config.GetSheet1());
        // }
        //
        // // 由于安卓资源都在包内，需要先复制到可读写文件夹2
        // IEnumerator CopyAssets(string paths)
        // {
        //     string assetPath = URLSetting.STREAMINGASSETS_URL + "config";
        //     string sdCardPath = Application.persistentDataPath + "/config";
        //     WWW www = new WWW(assetPath + "/" + paths);
        //     yield return www;
        //     if(www.error != null)
        //     {
        //         LogF8.Log(www.error);
        //         yield return null;
        //     }
        //     else
        //     {
        //         FileTools.SafeWriteAllBytes(sdCardPath + "/" + paths, www.bytes);
        //     }
        // }
    }
}