using UnityEngine;

namespace F8Framework.Core
{
    public class URLSetting
    {
        //读取资源的路径
#if UNITY_EDITOR || UNITY_STANDALONE
        public static string STREAMINGASSETS_URL = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
    public static string STREAMINGASSETS_URL = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE || UNITY_IOS
    public static string STREAMINGASSETS_URL = "file://" + Application.streamingAssetsPath + "/";
#endif

        //CS读取资源的路径
#if UNITY_EDITOR || UNITY_STANDALONE
        public static string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
    public static string CS_STREAMINGASSETS_URL = "null";
#elif UNITY_IPHONE || UNITY_IOS
    public static string CS_STREAMINGASSETS_URL = Application.streamingAssetsPath + "/";
#endif
    }
}