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
            // 指定名字加载
            Sheet1 sheet1 = FF8.Config.Load<Sheet1>("Sheet1");
            LogF8.Log(sheet1.Dict[2].name);
            
            // 同步加载全部配置
            FF8.Config.LoadAll();

            // 异步加载全部配置
            // yield return FF8.Config.LoadAllAsyncIEnumerator();
            // 也可以这样
            foreach (var item in FF8.Config.LoadAllAsync())
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
            
            // 运行时读取Excel的方式（如没有需求请谨慎使用）
            ReadExcel.Instance.LoadAllExcelData();
        }
    }
}