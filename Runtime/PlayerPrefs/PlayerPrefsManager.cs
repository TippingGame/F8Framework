using UnityEngine;

namespace F8Framework.Core
{
    public class PlayerPrefsManager : Singleton<PlayerPrefsManager>
    {
        private string _id = "";

        public void SetUser(string id)
        {
            _id = id;
        }

        public string GetKeywords(string key, bool user = false)
        {
            return user ? $"{key}_{_id}" : key;
        }

        public void SetString(string key, string value, bool user = false)
        {
            PlayerPrefs.SetString(GetKeywords(key, user), value);
        }

        public void SetInt(string key, int value, bool user = false)
        {
            PlayerPrefs.SetInt(GetKeywords(key, user), value);
        }

        public void SetFloat(string key, float value, bool user = false)
        {
            PlayerPrefs.SetFloat(GetKeywords(key, user), value);
        }

        public void SetBool(string key, bool value, bool user = false)
        {
            int _value = value ? 1 : 0;
            PlayerPrefs.SetInt(GetKeywords(key, user), _value);
        }

        public string GetString(string key, string defaultValue = "", bool user = false)
        {
            return PlayerPrefs.GetString(GetKeywords(key, user), defaultValue);
        }

        public int GetInt(string key, int defaultValue = 0, bool user = false)
        {
            return PlayerPrefs.GetInt(GetKeywords(key, user), defaultValue);
        }

        public float GetFloat(string key, float defaultValue = 0.0f, bool user = false)
        {
            return PlayerPrefs.GetFloat(GetKeywords(key, user), defaultValue);
        }
        
        public bool GetBool(string key, bool defaultValue = false, bool user = false)
        {
            int _defaultValue = defaultValue ? 1 : 0;
            return PlayerPrefs.GetInt(GetKeywords(key, user), _defaultValue) == 1 ? true : false;
        }

        public void Remove(string key, bool user = false)
        {
            PlayerPrefs.DeleteKey(GetKeywords(key, user));
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
