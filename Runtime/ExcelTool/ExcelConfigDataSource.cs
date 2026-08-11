using System.Collections.Generic;
using System.IO;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.ExcelData
{
    /// <summary>
    /// Excel 运行时数据源。Core 只认识 IConfigDataSource，不依赖本程序集。
    /// </summary>
    public sealed class ExcelConfigDataSource : IConfigDataSource
    {
        public const string SourceId = "excel";

        public string Id => SourceId;
        public int Priority => 100;

        public bool IsAvailable
        {
            get
            {
                string inputPath = GetInputPath();
                if (string.IsNullOrEmpty(inputPath))
                {
                    return false;
                }

#if UNITY_EDITOR
                return Directory.Exists(inputPath);
#else
                // Android 等平台的 StreamingAssets 并不是普通目录，实际可用性由读取器判断。
                return true;
#endif
            }
        }

        public void LoadAll(IDictionary<string, object> destination)
        {
            ReadExcel.Instance.LoadAllExcelData(destination);
        }

        internal static string GetInputPath()
        {
#if UNITY_EDITOR
            return URLSetting.AddRootPath(F8EditorPrefs.GetString("ExcelPath", null)) ??
                   URLSetting.CS_STREAMINGASSETS_URL + "config";
#else
            return URLSetting.CS_STREAMINGASSETS_URL + "config";
#endif
        }
    }

    internal static class ExcelConfigDataSourceBootstrap
    {
        private static readonly ExcelConfigDataSource Source = new ExcelConfigDataSource();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterAtRuntime()
        {
            ConfigDataSourceRegistry.Register(Source);
        }
    }
}
