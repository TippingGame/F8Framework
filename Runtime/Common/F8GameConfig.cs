using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class F8GameConfig : ScriptableObject
    {
        // 打包后AB名加上MD5
        public const bool AppendHashToAssetBundleName = false;
        // 强制更改资产加载模式为远程
        public const bool ForceRemoteAssetBundle = false;
        // 禁用Unity缓存系统在WebGL平台
        public const bool DisableUnityCacheOnWebGL = false;
        
        // AB加密设置
        public const byte AssetBundleOffset = 0;
        public const byte AssetBundleXorKey = 0;
        
        [System.Serializable]
        public class ConfigEntry
        {
            public string key;
            public string stringValue;
            public int intValue;
            public float floatValue;
            public bool boolValue;
            public ValueType valueType;

            public enum ValueType
            {
                None = 0,
                String,
                Int,
                Float,
                Bool
            }
        }

        public List<ConfigEntry> entries = new List<ConfigEntry>();
        
        public ConfigEntry GetOrCreateEntry(string key)
        {
            var entry = entries.Find(e => e.key == key);
            if (entry == null)
            {
                entry = new ConfigEntry { key = key };
                entries.Add(entry);
            }

            return entry;
        }
    }
}