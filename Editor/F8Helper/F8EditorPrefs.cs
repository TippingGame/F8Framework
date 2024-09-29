using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class F8EditorPrefs
    {
        // 获取 EditorPrefs 中的字符串
        public static string GetString(string key, string value = default)
        {
            return EditorPrefs.GetString(Application.dataPath.GetHashCode() + key, value);
        }

        // 设置 EditorPrefs 中的字符串
        public static void SetString(string key, string value)
        {
            EditorPrefs.SetString(Application.dataPath.GetHashCode() + key, value);
        }

        // 获取 EditorPrefs 中的布尔值
        public static bool GetBool(string key, bool defaultValue = default)
        {
            return EditorPrefs.GetBool(Application.dataPath.GetHashCode() + key, defaultValue);
        }

        // 设置 EditorPrefs 中的布尔值
        public static void SetBool(string key, bool value)
        {
            EditorPrefs.SetBool(Application.dataPath.GetHashCode() + key, value);
        }

        // 获取 EditorPrefs 中的整数值
        public static int GetInt(string key, int defaultValue = default)
        {
            return EditorPrefs.GetInt(Application.dataPath.GetHashCode() + key, defaultValue);
        }

        // 设置 EditorPrefs 中的整数值
        public static void SetInt(string key, int value)
        {
            EditorPrefs.SetInt(Application.dataPath.GetHashCode() + key, value);
        }

        // 获取 EditorPrefs 中的浮点值
        public static float GetFloat(string key, float defaultValue = default)
        {
            return EditorPrefs.GetFloat(Application.dataPath.GetHashCode() + key, defaultValue);
        }

        // 设置 EditorPrefs 中的浮点值
        public static void SetFloat(string key, float value)
        {
            EditorPrefs.SetFloat(Application.dataPath.GetHashCode() + key, value);
        }
    }
}
