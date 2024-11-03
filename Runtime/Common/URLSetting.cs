using System.IO;
using UnityEngine;

namespace F8Framework.Core
{
    public class URLSetting
    {
            public const string AssetBundlesName = "AssetBundles";  // AB包名
            public const string AssetBundlesPath = "Assets/AssetBundles/"; // 打包AB包根路径
            public const string AssetBundlesPathLower = "assets/assetbundles/"; // 打包AB包根路径小写
            public const string ResourcesPath = "Resources/"; // Resources根路径
            
            public static string GetAssetBundlesFolder()
            {
                    return Application.dataPath + "/" + AssetBundlesName;
            }
            
            public static string GetAssetBundlesOutPath()
            {
                    return Application.dataPath + "/StreamingAssets/" + AssetBundlesName + "/" + GetPlatformName();
            }
            
            public static string GetPlatformName()
            {
 
#if UNITY_STANDALONE_WIN
                string strReturenPlatformName = "Windows";
#elif UNITY_STANDALONE_OSX
                string strReturenPlatformName = "macOS";
#elif UNITY_STANDALONE_LINUX
                string strReturenPlatformName = "Linux";
#elif UNITY_IPHONE || UNITY_IOS
                string strReturenPlatformName = "iOS";
#elif UNITY_ANDROID
                string strReturenPlatformName = "Android";
#elif UNITY_WEBGL
                string strReturenPlatformName = "WebGL";
#else
                string strReturenPlatformName = "Unknown";
#endif
                    return strReturenPlatformName;
            }
            
        //读取资源的路径
#if UNITY_EDITOR || UNITY_STANDALONE
        public static string STREAMINGASSETS_URL = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
        public static string STREAMINGASSETS_URL = "jar:file://" + Application.dataPath + "!/assets/";
        // public static string STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#elif UNITY_IPHONE || UNITY_IOS
        public static string STREAMINGASSETS_URL = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_WEBGL
        public static string STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#else
        public static string STREAMINGASSETS_URL = "file://" + Application.streamingAssetsPath + "/";
#endif

        //CS_IO读取资源的路径
#if UNITY_EDITOR || UNITY_STANDALONE
        public static string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
        public static string CS_STREAMINGASSETS_URL = "null";
#elif UNITY_IPHONE || UNITY_IOS
        public static string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#elif UNITY_WEBGL
        public static string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#else
        public static string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#endif
            
        //上报错误地址
        public static string REPORT_ERROR_URL = "";
    }
}