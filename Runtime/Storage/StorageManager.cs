using System;
using System.Security.Cryptography;
using UnityEngine;

namespace F8Framework.Core
{
    public class StorageManager : ModuleSingleton<StorageManager>, IModule
    {
        private string _id = "";
        private Util.OptimizedAES _optimizedAES;
        
        public void SetUser(string id)
        {
            _id = id;
        }

        public void SetEncrypt(Util.OptimizedAES optimizedAES)
        {
#if !UNITY_EDITOR
            _optimizedAES = optimizedAES;
#endif
        }
        
        private string GetKeywords(string key, bool user = false)
        {
            string keywords = user ? $"{key}_{_id}" : key;
            if (_optimizedAES != null)
            {
                return Util.Encryption.MD5Encrypt16(keywords);
            }
            return keywords;
        }

        private byte[] GetRawBytes(string key, bool user = false)
        {
            if (!PlayerPrefs.HasKey(GetKeywords(key, user)))
                return null;

            try
            {
                string base64String = PlayerPrefs.GetString(GetKeywords(key, user));
                byte[] compressedBytes = Convert.FromBase64String(base64String);
                return Util.Encryption.AES_Decrypt(compressedBytes, _optimizedAES);
            }
            catch (FormatException)
            {
                Debug.LogError($"Invalid Base64 format for key: {key}");
                return null;
            }
            catch (CryptographicException)
            {
                Debug.LogError($"Cryptographic error occurred while decrypting key: {key}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error occurred while getting raw bytes for key: {key}. Error: {ex.Message}");
                return null;
            }
        }

        private void SetRawBytes(string key, byte[] rawBytes, bool user = false)
        {
            byte[] encryptedBytes = Util.Encryption.AES_Encrypt(rawBytes, _optimizedAES);
            string base64String = Convert.ToBase64String(encryptedBytes);
            PlayerPrefs.SetString(GetKeywords(key, user), base64String);
        }
        
        public string GetString(string key, string defaultValue = "", bool user = false)
        {
            if (_optimizedAES != null)
            {
                byte[] rawBytes = GetRawBytes(key, user);
                if (rawBytes == null) return defaultValue;
                using StringPrefItem item = ReferencePool.Acquire<StringPrefItem>();
                item.ImportRawBytes(rawBytes);
                return item.Value;
            }
            else
            {
                return PlayerPrefs.GetString(GetKeywords(key, user), defaultValue);
            }
        }

        public void SetString(string key, string value, bool user = false)
        {
            if (_optimizedAES != null)
            {
                using StringPrefItem prefItem = ReferencePool.Acquire<StringPrefItem>();
                prefItem.Value = value;
                SetRawBytes(key, prefItem.ExportRawBytes(), user);
            }
            else
            {
                PlayerPrefs.SetString(GetKeywords(key, user), value);
            }
        }

        public int GetInt(string key, int defaultValue = 0, bool user = false)
        {
            if (_optimizedAES != null)
            {
                byte[] rawBytes = GetRawBytes(key, user);
                if (rawBytes == null) return defaultValue;
                using IntPrefItem item = ReferencePool.Acquire<IntPrefItem>();
                item.ImportRawBytes(rawBytes);
                return item.Value;
            }
            else
            {
                return PlayerPrefs.GetInt(GetKeywords(key, user), defaultValue);
            }
        }

        public void SetInt(string key, int value, bool user = false)
        {
            if (_optimizedAES != null)
            {
                using IntPrefItem prefItem = ReferencePool.Acquire<IntPrefItem>();
                prefItem.Value = value;
                SetRawBytes(key, prefItem.ExportRawBytes(), user);
            }
            else
            {
                PlayerPrefs.SetInt(GetKeywords(key, user), value);
            }
        }

        public float GetFloat(string key, float defaultValue = 0.0f, bool user = false)
        {
            if (_optimizedAES != null)
            {
                byte[] rawBytes = GetRawBytes(key, user);
                if (rawBytes == null) return defaultValue;
                using FloatPrefItem item = ReferencePool.Acquire<FloatPrefItem>();
                item.ImportRawBytes(rawBytes);
                return item.Value;
            }
            else
            {
                return PlayerPrefs.GetFloat(GetKeywords(key, user), defaultValue);
            }
        }

        public void SetFloat(string key, float value, bool user = false)
        {
            if (_optimizedAES != null)
            {
                using FloatPrefItem prefItem = ReferencePool.Acquire<FloatPrefItem>();
                prefItem.Value = value;
                SetRawBytes(key, prefItem.ExportRawBytes(), user);
            }
            else
            {
                PlayerPrefs.SetFloat(GetKeywords(key, user), value);
            }
        }

        public bool GetBool(string key, bool defaultValue = false, bool user = false)
        {
            if (_optimizedAES != null)
            {
                byte[] rawBytes = GetRawBytes(key, user);
                if (rawBytes == null) return defaultValue;
                using BoolPrefItem item = ReferencePool.Acquire<BoolPrefItem>();
                item.ImportRawBytes(rawBytes);
                return item.Value;
            }
            else
            {
                int intValue = PlayerPrefs.GetInt(GetKeywords(key, user), defaultValue ? 1 : 0);
                return intValue != 0;
            }
        }

        public void SetBool(string key, bool value, bool user = false)
        {
            if (_optimizedAES != null)
            {
                using BoolPrefItem prefItem = ReferencePool.Acquire<BoolPrefItem>();
                prefItem.Value = value;
                SetRawBytes(key, prefItem.ExportRawBytes(), user);
            }
            else
            {
                PlayerPrefs.SetInt(GetKeywords(key, user), value ? 1 : 0);
            }
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
                 string jsonString = GetString(key, "", user);
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
            if (obj == null)
            {
                LogF8.LogError("本地数据存入对象不能为空");
                return;
            }
            SetString(key, Util.LitJson.ToJson(obj), user);
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
