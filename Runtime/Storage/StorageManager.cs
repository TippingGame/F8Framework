using UnityEngine;

namespace F8Framework.Core
{
    public class StorageManager : ModuleSingleton<StorageManager>, IModule
    {
        private string _id = "";

        public void SetUser(string id)
        {
            _id = id;
        }

        private string GetKeywords(string key, bool user = false)
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

         /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="key">要获取游戏配置项的名称。</param>
        /// <returns>读取的对象。</returns>
         public T GetObject<T>(string key, bool user = false)
         {
             string keywords = GetKeywords(key, user);
             if (PlayerPrefs.HasKey(keywords))
             {
                 string jsonString = PlayerPrefs.GetString(keywords);
                 return Util.LitJson.ToObject<T>(jsonString);
             }

             return default(T);
         }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <typeparam name="T">要写入对象的类型。</typeparam>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public void SetObject<T>(string key, T obj, bool user = false)
        {
            PlayerPrefs.SetString(GetKeywords(key, user), Util.LitJson.ToJson(obj));
        }
        
        public void Remove(string key, bool user = false)
        {
            PlayerPrefs.DeleteKey(GetKeywords(key, user));
        }
        
        // 保存到硬盘
        public bool Save()
        {
            PlayerPrefs.Save();
            return true;
        }
        
        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }

        public void OnInit(object createParam)
        {
            
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            base.Destroy();
        }
    }
}
