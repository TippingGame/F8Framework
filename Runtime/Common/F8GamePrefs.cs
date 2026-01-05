#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace F8Framework.Core
{
    public static class F8GamePrefs
    {
        private static F8GameConfig _config;
        
#if UNITY_EDITOR
        private const string GAMECONFIG_PATH = "Assets/F8Framework/Resources/F8GameConfig.asset";
        private static bool IsBuilding => BuildPipeline.isBuildingPlayer;
        // 编辑器模式下：加载配置（可读写）
        private static F8GameConfig LoadConfig()
        {
            if (IsBuilding)
            {
                if (!_config)
                {
                    _config = Resources.Load<F8GameConfig>(nameof(F8GameConfig));
                }
                return _config ?? ScriptableObject.CreateInstance<F8GameConfig>();
            }
            if (!_config)
            {
                _config = Resources.Load<F8GameConfig>(nameof(F8GameConfig));
                
                if (!_config)
                {
                    _config = AssetDatabase.LoadAssetAtPath<F8GameConfig>(GAMECONFIG_PATH);
                }
                
                if (!_config)
                {
                    _config = ScriptableObject.CreateInstance<F8GameConfig>();
                    
                    string resourcesPath = "Assets/F8Framework/Resources";
                    FileTools.CheckDirAndCreateWhenNeeded(resourcesPath);
                    
                    AssetDatabase.CreateAsset(_config, GAMECONFIG_PATH);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    LogF8.Log($"创建游戏配置文件: {GAMECONFIG_PATH}");
                }
            }
            return _config;
        }
        
        // 编辑器模式下：保存配置
        private static void SaveConfig()
        {
            if (IsBuilding) return;
            if (_config != null)
            {
                EditorUtility.SetDirty(_config);
                AssetDatabase.SaveAssetIfDirty(_config);
            }
        }
        
        // 编辑器模式下：设置方法
        public static void SetString(string key, string value)
        {
            if (IsBuilding) return;
            if (string.IsNullOrEmpty(key)) return;
            
            var config = LoadConfig();
            var entry = config.GetOrCreateEntry(key);
            
            if (entry.valueType != F8GameConfig.ConfigEntry.ValueType.String || entry.stringValue != value)
            {
                entry.stringValue = value;
                entry.valueType = F8GameConfig.ConfigEntry.ValueType.String;
                SaveConfig();
            }
        }
        
        public static void SetBool(string key, bool value)
        {
            if (IsBuilding) return;
            if (string.IsNullOrEmpty(key)) return;
            
            var config = LoadConfig();
            var entry = config.GetOrCreateEntry(key);
            
            if (entry.valueType != F8GameConfig.ConfigEntry.ValueType.Bool || entry.boolValue != value)
            {
                entry.boolValue = value;
                entry.valueType = F8GameConfig.ConfigEntry.ValueType.Bool;
                SaveConfig();
            }
        }
        
        public static void SetInt(string key, int value)
        {
            if (IsBuilding) return;
            if (string.IsNullOrEmpty(key)) return;
            
            var config = LoadConfig();
            var entry = config.GetOrCreateEntry(key);
            
            if (entry.valueType != F8GameConfig.ConfigEntry.ValueType.Int || entry.intValue != value)
            {
                entry.intValue = value;
                entry.valueType = F8GameConfig.ConfigEntry.ValueType.Int;
                SaveConfig();
            }
        }
        
        public static void SetFloat(string key, float value)
        {
            if (IsBuilding) return;
            if (string.IsNullOrEmpty(key)) return;
            
            var config = LoadConfig();
            var entry = config.GetOrCreateEntry(key);
            
            if (entry.valueType != F8GameConfig.ConfigEntry.ValueType.Float || entry.floatValue != value)
            {
                entry.floatValue = value;
                entry.valueType = F8GameConfig.ConfigEntry.ValueType.Float;
                SaveConfig();
            }
        }
#endif
        // 通用读取方法（编辑器和运行时都可调用）
        public static string GetString(string key, string defaultValue = "")
        {
            var config = GetConfig();
            var entry = config.entries.Find(e => e.key == key);
            
            if (entry != null && entry.valueType == F8GameConfig.ConfigEntry.ValueType.String)
            {
                return entry.stringValue;
            }
            return defaultValue;
        }
        
        public static bool GetBool(string key, bool defaultValue = false)
        {
            var config = GetConfig();
            var entry = config.entries.Find(e => e.key == key);
            
            if (entry != null && entry.valueType == F8GameConfig.ConfigEntry.ValueType.Bool)
            {
                return entry.boolValue;
            }
            return defaultValue;
        }
        
        public static int GetInt(string key, int defaultValue = 0)
        {
            var config = GetConfig();
            var entry = config.entries.Find(e => e.key == key);
            
            if (entry != null && entry.valueType == F8GameConfig.ConfigEntry.ValueType.Int)
            {
                return entry.intValue;
            }
            return defaultValue;
        }
        
        public static float GetFloat(string key, float defaultValue = 0f)
        {
            var config = GetConfig();
            var entry = config.entries.Find(e => e.key == key);
            
            if (entry != null && entry.valueType == F8GameConfig.ConfigEntry.ValueType.Float)
            {
                return entry.floatValue;
            }
            return defaultValue;
        }
        
        public static bool HasKey(string key)
        {
            var config = GetConfig();
            return config.entries.Exists(e => e.key == key);
        }
        
        private static F8GameConfig GetConfig()
        {
            if (!_config)
            {
#if UNITY_EDITOR
                _config = LoadConfig();
#else
                _config = Resources.Load<F8GameConfig>(nameof(F8GameConfig));
                
                if (!_config)
                {
                    LogF8.LogWarning("F8GameConfig 未在 Resources 中找到。创建默认配置。");
                    _config = ScriptableObject.CreateInstance<F8GameConfig>();
                }
#endif
            }
            return _config;
        }
    }
}
