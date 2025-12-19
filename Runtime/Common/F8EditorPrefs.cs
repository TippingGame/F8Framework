#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core
{
    public static class F8EditorPrefs
    {
        private static F8EditorConfig _config;
        private static readonly string _configPath = "Assets/F8Framework/Editor/F8EditorConfig.asset";
        private static bool IsBuilding => BuildPipeline.isBuildingPlayer;
        
        private static F8EditorConfig LoadConfig()
        {
            if (IsBuilding)
            {
                if (_config == null)
                {
                    _config = AssetDatabase.LoadAssetAtPath<F8EditorConfig>(_configPath);
                }
                return _config ?? ScriptableObject.CreateInstance<F8EditorConfig>();
            }
            if (_config == null)
            {
                _config = AssetDatabase.LoadAssetAtPath<F8EditorConfig>(_configPath);
                if (_config == null)
                {
                    _config = ScriptableObject.CreateInstance<F8EditorConfig>();
                    FileTools.CheckFileAndCreateDirWhenNeeded(_configPath);
                    AssetDatabase.CreateAsset(_config, _configPath);
                    AssetDatabase.SaveAssets();
                    
                    LogF8.Log($"创建编辑器配置文件: {_configPath}");
                }
            }
            return _config;
        }
        
        private static void SaveConfig()
        {
            if (IsBuilding) return;
            EditorUtility.SetDirty(_config);
            AssetDatabase.SaveAssets();
        }
        
        public static string GetString(string key, string defaultValue = default)
        {
            var entry = LoadConfig().GetOrCreateEntry(key);
            return entry.valueType == F8EditorConfig.ConfigEntry.ValueType.String ? 
                entry.stringValue : defaultValue;
        }

        public static void SetString(string key, string value)
        {
            if (IsBuilding) return;
            var entry = LoadConfig().GetOrCreateEntry(key);
            if (entry.valueType == F8EditorConfig.ConfigEntry.ValueType.String && entry.stringValue == value)
            {
                return;
            }
            entry.stringValue = value;
            entry.valueType = F8EditorConfig.ConfigEntry.ValueType.String;
            SaveConfig();
        }

        public static bool GetBool(string key, bool defaultValue = default)
        {
            var entry = LoadConfig().GetOrCreateEntry(key);
            return entry.valueType == F8EditorConfig.ConfigEntry.ValueType.Bool ? 
                entry.boolValue : defaultValue;
        }

        public static void SetBool(string key, bool value)
        {
            if (IsBuilding) return;
            var entry = LoadConfig().GetOrCreateEntry(key);
            if (entry.valueType == F8EditorConfig.ConfigEntry.ValueType.Bool && entry.boolValue == value)
            {
                return;
            }
            entry.boolValue = value;
            entry.valueType = F8EditorConfig.ConfigEntry.ValueType.Bool;
            SaveConfig();
        }

        public static int GetInt(string key, int defaultValue = default)
        {
            var entry = LoadConfig().GetOrCreateEntry(key);
            return entry.valueType == F8EditorConfig.ConfigEntry.ValueType.Int ? 
                entry.intValue : defaultValue;
        }

        public static void SetInt(string key, int value)
        {
            if (IsBuilding) return;
            var entry = LoadConfig().GetOrCreateEntry(key);
            if (entry.valueType == F8EditorConfig.ConfigEntry.ValueType.Int && entry.intValue == value)
            {
                return;
            }
            entry.intValue = value;
            entry.valueType = F8EditorConfig.ConfigEntry.ValueType.Int;
            SaveConfig();
        }

        public static float GetFloat(string key, float defaultValue = default)
        {
            var entry = LoadConfig().GetOrCreateEntry(key);
            return entry.valueType == F8EditorConfig.ConfigEntry.ValueType.Float ? 
                entry.floatValue : defaultValue;
        }

        public static void SetFloat(string key, float value)
        {
            if (IsBuilding) return;
            var entry = LoadConfig().GetOrCreateEntry(key);
            if (entry.valueType == F8EditorConfig.ConfigEntry.ValueType.Float && entry.floatValue == value)
            {
                return;
            }
            entry.floatValue = value;
            entry.valueType = F8EditorConfig.ConfigEntry.ValueType.Float;
            SaveConfig();
        }

        public static bool HasKey(string key)
        {
            return LoadConfig().entries.Exists(e => e.key == key);
        }
    }
}
#endif