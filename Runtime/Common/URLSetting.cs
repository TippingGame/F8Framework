using System;
using System.IO;
using UnityEngine;

namespace F8Framework.Core
{
    public class URLSetting
    {
        public const string AssetBundlesName = "AssetBundles"; // AB包名
        public const string AssetBundlesPath = "Assets/AssetBundles/"; // 打包AB包根路径
        public const string AssetBundlesPathLower = "assets/assetbundles/"; // 打包AB包根路径小写
        public const string ResourcesPath = "Resources/"; // Resources根路径

        public static string GetAssetBundlesFolder()
        {
            return Application.dataPath + "/" + AssetBundlesName;
        }

        public static string GetAssetBundlesOutPath()
        {
            return FileTools.FormatToUnityPath(FileTools.TruncatePath(Application.dataPath, 1)) + "/Bundles/" + AssetBundlesName + "/" + GetPlatformName();
        }

        public static string GetAssetBundlesStreamPath()
        {
            return Application.dataPath + "/StreamingAssets/" + AssetBundlesName + "/" + GetPlatformName();
        }
        
        public static string GetTempExcelPath()
        {
            return FileTools.FormatToUnityPath(FileTools.TruncatePath(Application.dataPath, 1)) + "/temp_Excel";
        }
        
        public static string RemoveRootPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            
            string normalizedPath = FileTools.FormatToUnityPath(path);
            string rootPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(Application.dataPath, 1));
            
            if (!rootPath.EndsWith("/"))
                rootPath += "/";
            
            int index = normalizedPath.IndexOf(rootPath, StringComparison.Ordinal);
            
            if (index == 0)
            {
                string result = normalizedPath.Remove(0, rootPath.Length);
                
                if (result.StartsWith("/"))
                    result = result.Substring(1);
            
                return result;
            }
            
            return path;
        }
        
        public static string AddRootPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath) || Path.IsPathRooted(relativePath))
                return relativePath;
            
            string normalizedRelative = FileTools.FormatToUnityPath(relativePath);
            string rootPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(Application.dataPath, 1));
            
            normalizedRelative = normalizedRelative.TrimStart('/');
            
            rootPath = rootPath.TrimEnd('/') + "/";
            
            return rootPath + normalizedRelative;
        }

        public static string GetPlatformName()
        {
#if UNITY_SERVER && UNITY_STANDALONE_WIN
            return "WindowsServer";
#elif UNITY_SERVER && UNITY_STANDALONE_LINUX
            return "LinuxServer";
#elif UNITY_SERVER && UNITY_STANDALONE_OSX
            return "macOSServer";
#elif UNITY_STANDALONE_WIN
            return "Windows";
#elif UNITY_STANDALONE_OSX
            return "macOS";
#elif UNITY_STANDALONE_LINUX
            return "Linux";
#elif UNITY_IPHONE || UNITY_IOS
            return "iOS";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_WEBGL
            return "WebGL";
#elif UNITY_SERVER
            return "Server";
#elif UNITY_TVOS
            return "tvOS";
#elif UNITY_VISIONOS
            return "VisionOS";
#elif UNITY_WSA || UNITY_WSA_10_0
            return "WSAPlayer";
#elif UNITY_EMBEDDED_LINUX
            return "EmbeddedLinux";
#elif UNITY_QNX
            return "QNX";
#else
            return "Unknown";
#endif
        }

        //读取资源的路径
#if UNITY_EDITOR || UNITY_STANDALONE
        public static readonly string STREAMINGASSETS_URL = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
        public static readonly string STREAMINGASSETS_URL = "jar:file://" + Application.dataPath + "!/assets/";
        // public static string STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#elif UNITY_IPHONE || UNITY_IOS
        public static readonly string STREAMINGASSETS_URL = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_WEBGL
        public static readonly string STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#else
        public static readonly string STREAMINGASSETS_URL = "file://" + Application.streamingAssetsPath + "/";
#endif

        //CS_IO读取资源的路径
#if UNITY_EDITOR || UNITY_STANDALONE
        public static readonly string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
        public static readonly string CS_STREAMINGASSETS_URL = string.Empty;
#elif UNITY_IPHONE || UNITY_IOS
        public static readonly string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#elif UNITY_WEBGL
        public static readonly string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#else
        public static readonly string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#endif

        //上报错误地址
        public static string REPORT_ERROR_URL = "";
    }
}