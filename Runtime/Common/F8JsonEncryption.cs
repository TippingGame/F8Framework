using System;
using UnityEngine;

namespace F8Framework.Core
{
    public static class F8JsonEncryption
    {
        private const string ENCRYPTED_PREFIX = "F8ENC:";
        
        private static string AssetManifestEncryptKey =>
            F8GamePrefs.GetString(nameof(F8GameConfig.AssetManifestEncryptKey), "");

        public static string EncryptJsonIfNeeded(string json)
        {
            return EncryptJsonIfNeeded(json, AssetManifestEncryptKey);
        }

        public static string EncryptJsonIfNeeded(string json, string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(json))
            {
                return json;
            }

            return ENCRYPTED_PREFIX + Util.Encryption.AES_Encrypt(json, new Util.OptimizedAES(key));
        }

        public static string DecryptJsonIfNeeded(string text)
        {
            return DecryptJsonIfNeeded(text, AssetManifestEncryptKey);
        }

        public static string DecryptJsonIfNeeded(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string trimmedText = text.TrimStart('\uFEFF').Trim();
            if (IsJson(trimmedText))
            {
                return text;
            }

            bool hasPrefix = trimmedText.StartsWith(ENCRYPTED_PREFIX, StringComparison.Ordinal);
            if (hasPrefix)
            {
                trimmedText = trimmedText.Substring(ENCRYPTED_PREFIX.Length);
            }

            if (string.IsNullOrEmpty(key))
            {
                if (hasPrefix)
                {
                    throw new InvalidOperationException($"资源清单Json已加密，请配置{nameof(F8GameConfig.AssetManifestEncryptKey)}。");
                }

                return text;
            }

            try
            {
                return Util.Encryption.AES_Decrypt(trimmedText, new Util.OptimizedAES(key));
            }
            catch (Exception e)
            {
                LogF8.LogError($"资源清单Json解密失败，请检查{nameof(F8GameConfig.AssetManifestEncryptKey)}配置。\n{e}");
                throw;
            }
        }

        public static string ReadJsonFromFile(string path)
        {
            return ReadJsonFromFile(path, AssetManifestEncryptKey);
        }

        public static string ReadJsonFromFile(string path, string key)
        {
            return DecryptJsonIfNeeded(FileTools.SafeReadAllText(path), key);
        }

        public static string ReadJsonFromTextAsset(TextAsset textAsset)
        {
            return ReadJsonFromTextAsset(textAsset, AssetManifestEncryptKey);
        }

        public static string ReadJsonFromTextAsset(TextAsset textAsset, string key)
        {
            return textAsset == null ? null : DecryptJsonIfNeeded(textAsset.text, key);
        }

        public static bool WriteJsonToFile(string path, string json)
        {
            return WriteJsonToFile(path, json, AssetManifestEncryptKey);
        }

        public static bool WriteJsonToFile(string path, string json, string key)
        {
            return FileTools.SafeWriteAllText(path, EncryptJsonIfNeeded(json, key));
        }

        private static bool IsJson(string text)
        {
            return !string.IsNullOrEmpty(text) && (text[0] == '{' || text[0] == '[');
        }
    }
}
