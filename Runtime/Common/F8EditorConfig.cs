#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class F8EditorConfig : ScriptableObject
    {
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
#endif