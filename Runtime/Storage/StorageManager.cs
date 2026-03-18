using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using UnityEngine;

namespace F8Framework.Core
{
    public class StorageManager : ModuleSingleton<StorageManager>, IModule
    {
        public enum Location
        {
            File,
            PlayerPrefs,
            Resources
        }

        public enum Directory
        {
            PersistentDataPath,
            DataPath
        }

        public enum CompressionType
        {
            None,
            Gzip
        }

        [Serializable]
        public class Settings
        {
            public Location location = Location.PlayerPrefs;
            public Directory directory = Directory.PersistentDataPath;
            public string defaultFilePath = "StorageData.json";
            public CompressionType compressionType = CompressionType.None;
            public Util.OptimizedAES encryption;
        }

        [Serializable]
        private class StorageFileEntry
        {
            public string key;
            public string value;
        }

        [Serializable]
        private class StorageFileData
        {
            public List<StorageFileEntry> entries = new List<StorageFileEntry>();
        }

        private const byte PayloadFlagCompressed = 1 << 0;
        private const byte PayloadFlagEncrypted = 1 << 1;
        private static readonly byte[] PayloadMagic = { (byte)'F', (byte)'8', (byte)'S', (byte)'M', 1 };
        private static readonly byte[] FilePayloadMagic = { (byte)'F', (byte)'8', (byte)'S', (byte)'F', 1 };

        private readonly Dictionary<string, string> _fileStorage = new Dictionary<string, string>();
        private readonly Settings _settings = new Settings();

        private string _id = "";
        private bool _isFileStorageLoaded;
        private bool _isFileStorageDirty;

        public Settings Config => _settings;
        private Util.OptimizedAES Encryption => _settings.encryption;

        public string DefaultFilePath
        {
            get => _settings.defaultFilePath;
            set
            {
                _settings.defaultFilePath = value;
                ResetFileStorageCache();
            }
        }

        public string DefaultFileFullPath => GetResolvedPath(_settings.defaultFilePath);

        public void Configure(Settings settings)
        {
            if (settings == null)
            {
                return;
            }

            _settings.location = settings.location;
            _settings.directory = settings.directory;
            _settings.defaultFilePath = settings.defaultFilePath;
            _settings.compressionType = settings.compressionType;
            _settings.encryption = settings.encryption;
            ResetFileStorageCache();
        }

        public void SetLocation(Location location)
        {
            _settings.location = location;
            ResetFileStorageCache();
        }

        public void SetDirectory(Directory directory)
        {
            _settings.directory = directory;
            ResetFileStorageCache();
        }

        public void SetDefaultFilePath(string filePath)
        {
            DefaultFilePath = filePath;
        }

        public void SetCompression(CompressionType compressionType)
        {
            _settings.compressionType = compressionType;
        }
        
        public void SetUser(string id)
        {
            _id = id;
        }

        public void SetEncrypt(Util.OptimizedAES optimizedAES)
        {
            _settings.encryption = optimizedAES;
        }
        
        private string GetKeywords(string key, bool user = false)
        {
            string keywords = user ? $"{key}_{_id}" : key;
            if (Encryption != null)
            {
                return Util.Encryption.MD5Encrypt16(keywords);
            }
            return keywords;
        }

        private bool UseLegacyPlayerPrefsStorage => _settings.location == Location.PlayerPrefs
                                                    && Encryption == null;

        private bool UseFileLevelTransforms => _settings.location == Location.File;
        private bool UsePlainTextValueStorage => Encryption == null
                                                 && (UseFileLevelTransforms || _settings.compressionType == CompressionType.None);

        private string GetResolvedPath(string filePath)
        {
            string path = string.IsNullOrEmpty(filePath) ? _settings.defaultFilePath : filePath;
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            if (Path.IsPathRooted(path))
            {
                return path;
            }

            switch (_settings.location)
            {
                case Location.File:
                    string rootPath = _settings.directory == Directory.DataPath
                        ? Application.dataPath
                        : Application.persistentDataPath;
                    return Path.Combine(rootPath, path);
                case Location.Resources:
                    return Path.ChangeExtension(path.Replace('\\', '/'), null);
                default:
                    return path;
            }
        }

        private void ResetFileStorageCache()
        {
            _fileStorage.Clear();
            _isFileStorageLoaded = false;
            _isFileStorageDirty = false;
        }

        private void EnsureStorageLoaded()
        {
            if (_isFileStorageLoaded || _settings.location == Location.PlayerPrefs)
            {
                return;
            }

            _fileStorage.Clear();
            string json = string.Empty;

            try
            {
                if (_settings.location == Location.File)
                {
                    byte[] fileBytes = FileTools.SafeReadAllBytes(DefaultFileFullPath);
                    json = DecodeFileContent(fileBytes);
                }
                else if (_settings.location == Location.Resources)
                {
                    string resourcePath = GetResolvedPath(_settings.defaultFilePath);
                    if (!string.IsNullOrEmpty(resourcePath))
                    {
                        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
                        if (textAsset != null)
                        {
                            json = textAsset.text;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(json))
                {
                    StorageFileData data = Util.LitJson.ToObject<StorageFileData>(json);
                    if (data != null && data.entries != null)
                    {
                        for (int i = 0; i < data.entries.Count; i++)
                        {
                            StorageFileEntry entry = data.entries[i];
                            if (entry == null || string.IsNullOrEmpty(entry.key))
                            {
                                continue;
                            }

                            _fileStorage[entry.key] = entry.value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogF8.LogError($"从{_settings.location}加载存储数据失败：{ex.Message}");
            }

            _isFileStorageLoaded = true;
        }

        private StorageFileData CreateStorageFileData(Dictionary<string, string> storage)
        {
            StorageFileData data = new StorageFileData();
            foreach (KeyValuePair<string, string> pair in storage)
            {
                data.entries.Add(new StorageFileEntry
                {
                    key = pair.Key,
                    value = pair.Value
                });
            }

            return data;
        }

        private Dictionary<string, string> ParseStorageDictionary(string json)
        {
            Dictionary<string, string> storage = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(json))
            {
                return storage;
            }

            StorageFileData data = Util.LitJson.ToObject<StorageFileData>(json);
            if (data == null || data.entries == null)
            {
                return storage;
            }

            for (int i = 0; i < data.entries.Count; i++)
            {
                StorageFileEntry entry = data.entries[i];
                if (entry == null || string.IsNullOrEmpty(entry.key))
                {
                    continue;
                }

                storage[entry.key] = entry.value;
            }

            return storage;
        }

        private string ResolveFilePath(string filePath)
        {
            return GetResolvedPath(string.IsNullOrEmpty(filePath) ? _settings.defaultFilePath : filePath);
        }

        private Dictionary<string, string> LoadStorageFromFile(string filePath)
        {
            try
            {
                string fullPath = ResolveFilePath(filePath);
                byte[] fileBytes = FileTools.SafeReadAllBytes(fullPath);
                return ParseStorageDictionary(DecodeFileContent(fileBytes));
            }
            catch (Exception ex)
            {
                LogF8.LogError($"读取存储文件失败：{ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        private bool WriteStorageToFile(Dictionary<string, string> storage, string filePath)
        {
            try
            {
                string fullPath = ResolveFilePath(filePath);
                if (string.IsNullOrEmpty(fullPath))
                {
                    LogF8.LogError("存储文件路径不能为空。");
                    return false;
                }

                string directoryPath = Path.GetDirectoryName(fullPath);
                FileTools.CheckDirAndCreateWhenNeeded(directoryPath);

                string json = Util.LitJson.ToJson(CreateStorageFileData(storage));
                byte[] fileBytes = EncodeFileContent(json);
                FileTools.SafeWriteAllBytes(fullPath, fileBytes);
                return true;
            }
            catch (Exception ex)
            {
                LogF8.LogError($"写入存储文件失败：{ex.Message}");
                return false;
            }
        }

        private void DeleteStorageFile(string filePath)
        {
            string fullPath = ResolveFilePath(filePath);
            FileTools.SafeDeleteFile(fullPath);

            string metaPath = fullPath + ".meta";
            if (File.Exists(metaPath))
            {
                FileTools.SafeDeleteFile(metaPath);
            }
        }

        private bool FlushFileStorage()
        {
            if (_settings.location == Location.Resources)
            {
                LogF8.LogError("Resources 模式在运行时为只读，不能保存。");
                return false;
            }

            if (_settings.location != Location.File)
            {
                return true;
            }

            EnsureStorageLoaded();
            bool result = WriteStorageToFile(_fileStorage, _settings.defaultFilePath);
            if (result)
            {
                _isFileStorageDirty = false;
            }

            return result;
        }

        private bool HasStorageKey(string key, bool user = false)
        {
            string keywords = GetKeywords(key, user);
            if (_settings.location == Location.PlayerPrefs)
            {
                return PlayerPrefs.HasKey(keywords);
            }

            EnsureStorageLoaded();
            return _fileStorage.ContainsKey(keywords);
        }

        private string GetStoredString(string key, bool user = false)
        {
            string keywords = GetKeywords(key, user);
            if (_settings.location == Location.PlayerPrefs)
            {
                return PlayerPrefs.GetString(keywords, null);
            }

            EnsureStorageLoaded();
            return _fileStorage.TryGetValue(keywords, out string value) ? value : null;
        }

        private void SetStoredString(string key, string value, bool user = false)
        {
            if (_settings.location == Location.Resources)
            {
                LogF8.LogError("Resources 模式在运行时为只读，不能写入。");
                return;
            }

            string keywords = GetKeywords(key, user);
            if (_settings.location == Location.PlayerPrefs)
            {
                PlayerPrefs.SetString(keywords, value);
                return;
            }

            EnsureStorageLoaded();
            _fileStorage[keywords] = value;
            _isFileStorageDirty = true;
        }

        private void DeleteStoredKey(string key, bool user = false)
        {
            if (_settings.location == Location.Resources)
            {
                LogF8.LogError("Resources 模式在运行时为只读，不能修改。");
                return;
            }

            string keywords = GetKeywords(key, user);
            if (_settings.location == Location.PlayerPrefs)
            {
                PlayerPrefs.DeleteKey(keywords);
                return;
            }

            EnsureStorageLoaded();
            if (_fileStorage.Remove(keywords))
            {
                _isFileStorageDirty = true;
            }
        }

        private byte[] GetRawBytes(string key, bool user = false)
        {
            string storedValue = GetStoredString(key, user);
            if (string.IsNullOrEmpty(storedValue))
            {
                return null;
            }

            try
            {
                byte[] payloadBytes = Convert.FromBase64String(storedValue);
                return DecodePayload(payloadBytes, key);
            }
            catch (FormatException)
            {
                LogF8.LogError($"读取键 {key} 失败：Base64 格式无效。");
                return null;
            }
            catch (CryptographicException)
            {
                LogF8.LogError($"读取键 {key} 失败：解密错误。");
                return null;
            }
            catch (InvalidDataException ex)
            {
                LogF8.LogError($"读取键 {key} 失败：解压错误，{ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                LogF8.LogError($"读取键 {key} 原始数据失败：{ex.Message}");
                return null;
            }
        }

        private void SetRawBytes(string key, byte[] rawBytes, bool user = false)
        {
            byte[] payloadBytes = EncodePayload(rawBytes);
            string base64String = Convert.ToBase64String(payloadBytes);
            SetStoredString(key, base64String, user);
        }

        private byte[] EncodeFileContent(string json)
        {
            byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(json);
            byte flags = 0;

            if (_settings.compressionType == CompressionType.Gzip)
            {
                contentBytes = Compress(contentBytes);
                flags |= PayloadFlagCompressed;
            }

            if (flags == 0)
            {
                return contentBytes;
            }

            byte[] finalBytes = new byte[FilePayloadMagic.Length + 1 + contentBytes.Length];
            Buffer.BlockCopy(FilePayloadMagic, 0, finalBytes, 0, FilePayloadMagic.Length);
            finalBytes[FilePayloadMagic.Length] = flags;
            Buffer.BlockCopy(contentBytes, 0, finalBytes, FilePayloadMagic.Length + 1, contentBytes.Length);
            return finalBytes;
        }

        private string DecodeFileContent(byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                byte[] contentBytes = fileBytes;

                if (HasPayloadHeader(fileBytes, FilePayloadMagic))
                {
                    byte flags = fileBytes[FilePayloadMagic.Length];
                    contentBytes = new byte[fileBytes.Length - FilePayloadMagic.Length - 1];
                    Buffer.BlockCopy(fileBytes, FilePayloadMagic.Length + 1, contentBytes, 0, contentBytes.Length);

                    if ((flags & PayloadFlagCompressed) != 0)
                    {
                        contentBytes = Decompress(contentBytes);
                    }
                }

                return System.Text.Encoding.UTF8.GetString(contentBytes);
            }
            catch (Exception ex)
            {
                LogF8.LogError($"解析存储文件失败：{ex.Message}");
                return string.Empty;
            }
        }

        private byte[] EncodePayload(byte[] rawBytes)
        {
            byte flags = 0;
            byte[] payloadBytes = rawBytes;

            if (!UseFileLevelTransforms && _settings.compressionType == CompressionType.Gzip)
            {
                payloadBytes = Compress(payloadBytes);
                flags |= PayloadFlagCompressed;
            }

            if (Encryption != null)
            {
                payloadBytes = Util.Encryption.AES_Encrypt(payloadBytes, Encryption);
                flags |= PayloadFlagEncrypted;
            }

            if (flags == 0)
            {
                return payloadBytes;
            }

            byte[] finalBytes = new byte[PayloadMagic.Length + 1 + payloadBytes.Length];
            Buffer.BlockCopy(PayloadMagic, 0, finalBytes, 0, PayloadMagic.Length);
            finalBytes[PayloadMagic.Length] = flags;
            Buffer.BlockCopy(payloadBytes, 0, finalBytes, PayloadMagic.Length + 1, payloadBytes.Length);
            return finalBytes;
        }

        private byte[] DecodePayload(byte[] payloadBytes, string key)
        {
            if (UseFileLevelTransforms && !HasPayloadHeader(payloadBytes))
            {
                return payloadBytes;
            }

            if (HasPayloadHeader(payloadBytes))
            {
                byte flags = payloadBytes[PayloadMagic.Length];
                byte[] contentBytes = new byte[payloadBytes.Length - PayloadMagic.Length - 1];
                Buffer.BlockCopy(payloadBytes, PayloadMagic.Length + 1, contentBytes, 0, contentBytes.Length);

                if ((flags & PayloadFlagEncrypted) != 0)
                {
                    if (Encryption == null)
                    {
                        throw new CryptographicException($"键 {key} 缺少加密配置");
                    }

                    contentBytes = Util.Encryption.AES_Decrypt(contentBytes, Encryption);
                }

                if ((flags & PayloadFlagCompressed) != 0)
                {
                    contentBytes = Decompress(contentBytes);
                }

                return contentBytes;
            }

            if (Encryption != null)
            {
                payloadBytes = Util.Encryption.AES_Decrypt(payloadBytes, Encryption);
            }

            if (_settings.compressionType == CompressionType.Gzip)
            {
                payloadBytes = Decompress(payloadBytes);
            }

            return payloadBytes;
        }

        private static bool HasPayloadHeader(byte[] payloadBytes)
        {
            return HasPayloadHeader(payloadBytes, PayloadMagic);
        }

        private static bool HasPayloadHeader(byte[] payloadBytes, byte[] headerMagic)
        {
            if (payloadBytes == null || headerMagic == null || payloadBytes.Length <= headerMagic.Length)
            {
                return false;
            }

            for (int i = 0; i < headerMagic.Length; i++)
            {
                if (payloadBytes[i] != headerMagic[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static byte[] Compress(byte[] rawBytes)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(output, System.IO.Compression.CompressionMode.Compress, true))
                {
                    gzipStream.Write(rawBytes, 0, rawBytes.Length);
                }

                return output.ToArray();
            }
        }

        private static byte[] Decompress(byte[] compressedBytes)
        {
            using (MemoryStream input = new MemoryStream(compressedBytes))
            using (GZipStream gzipStream = new GZipStream(input, System.IO.Compression.CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream())
            {
                gzipStream.CopyTo(output);
                return output.ToArray();
            }
        }

        public string GetString(string key, string defaultValue = "", bool user = false)
        {
            if (UseLegacyPlayerPrefsStorage)
            {
                return PlayerPrefs.GetString(GetKeywords(key, user), defaultValue);
            }

            if (UsePlainTextValueStorage)
            {
                string storedValue = GetStoredString(key, user);
                return storedValue ?? defaultValue;
            }

            byte[] rawBytes = GetRawBytes(key, user);
            if (rawBytes == null)
            {
                return defaultValue;
            }

            using StringPrefItem item = ReferencePool.Acquire<StringPrefItem>();
            return item.ImportRawBytes(rawBytes) ? item.Value : defaultValue;
        }

        public void SetString(string key, string value, bool user = false)
        {
            if (UseLegacyPlayerPrefsStorage)
            {
                PlayerPrefs.SetString(GetKeywords(key, user), value);
                return;
            }

            if (UsePlainTextValueStorage)
            {
                SetStoredString(key, value ?? string.Empty, user);
                return;
            }

            using StringPrefItem prefItem = ReferencePool.Acquire<StringPrefItem>();
            prefItem.Value = value;
            SetRawBytes(key, prefItem.ExportRawBytes(), user);
        }

        public int GetInt(string key, int defaultValue = 0, bool user = false)
        {
            if (UseLegacyPlayerPrefsStorage)
            {
                return PlayerPrefs.GetInt(GetKeywords(key, user), defaultValue);
            }

            if (UsePlainTextValueStorage)
            {
                string storedValue = GetStoredString(key, user);
                if (string.IsNullOrEmpty(storedValue))
                {
                    return defaultValue;
                }

                return int.TryParse(storedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                    ? value
                    : defaultValue;
            }

            byte[] rawBytes = GetRawBytes(key, user);
            if (rawBytes == null)
            {
                return defaultValue;
            }

            using IntPrefItem item = ReferencePool.Acquire<IntPrefItem>();
            return item.ImportRawBytes(rawBytes) ? item.Value : defaultValue;
        }

        public void SetInt(string key, int value, bool user = false)
        {
            if (UseLegacyPlayerPrefsStorage)
            {
                PlayerPrefs.SetInt(GetKeywords(key, user), value);
                return;
            }

            if (UsePlainTextValueStorage)
            {
                SetStoredString(key, value.ToString(CultureInfo.InvariantCulture), user);
                return;
            }

            using IntPrefItem prefItem = ReferencePool.Acquire<IntPrefItem>();
            prefItem.Value = value;
            SetRawBytes(key, prefItem.ExportRawBytes(), user);
        }

        public float GetFloat(string key, float defaultValue = 0.0f, bool user = false)
        {
            if (UseLegacyPlayerPrefsStorage)
            {
                return PlayerPrefs.GetFloat(GetKeywords(key, user), defaultValue);
            }

            if (UsePlainTextValueStorage)
            {
                string storedValue = GetStoredString(key, user);
                if (string.IsNullOrEmpty(storedValue))
                {
                    return defaultValue;
                }

                return float.TryParse(storedValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float value)
                    ? value
                    : defaultValue;
            }

            byte[] rawBytes = GetRawBytes(key, user);
            if (rawBytes == null)
            {
                return defaultValue;
            }

            using FloatPrefItem item = ReferencePool.Acquire<FloatPrefItem>();
            return item.ImportRawBytes(rawBytes) ? item.Value : defaultValue;
        }

        public void SetFloat(string key, float value, bool user = false)
        {
            if (UseLegacyPlayerPrefsStorage)
            {
                PlayerPrefs.SetFloat(GetKeywords(key, user), value);
                return;
            }

            if (UsePlainTextValueStorage)
            {
                SetStoredString(key, value.ToString("R", CultureInfo.InvariantCulture), user);
                return;
            }

            using FloatPrefItem prefItem = ReferencePool.Acquire<FloatPrefItem>();
            prefItem.Value = value;
            SetRawBytes(key, prefItem.ExportRawBytes(), user);
        }

        public bool GetBool(string key, bool defaultValue = false, bool user = false)
        {
            if (UseLegacyPlayerPrefsStorage)
            {
                int intValue = PlayerPrefs.GetInt(GetKeywords(key, user), defaultValue ? 1 : 0);
                return intValue != 0;
            }

            if (UsePlainTextValueStorage)
            {
                string storedValue = GetStoredString(key, user);
                if (string.IsNullOrEmpty(storedValue))
                {
                    return defaultValue;
                }

                if (bool.TryParse(storedValue, out bool boolValue))
                {
                    return boolValue;
                }

                if (int.TryParse(storedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                {
                    return intValue != 0;
                }

                return defaultValue;
            }

            byte[] rawBytes = GetRawBytes(key, user);
            if (rawBytes == null)
            {
                return defaultValue;
            }

            using BoolPrefItem item = ReferencePool.Acquire<BoolPrefItem>();
            return item.ImportRawBytes(rawBytes) ? item.Value : defaultValue;
        }

        public void SetBool(string key, bool value, bool user = false)
        {
            if (UseLegacyPlayerPrefsStorage)
            {
                PlayerPrefs.SetInt(GetKeywords(key, user), value ? 1 : 0);
                return;
            }

            if (UsePlainTextValueStorage)
            {
                SetStoredString(key, value ? bool.TrueString : bool.FalseString, user);
                return;
            }

            using BoolPrefItem prefItem = ReferencePool.Acquire<BoolPrefItem>();
            prefItem.Value = value;
            SetRawBytes(key, prefItem.ExportRawBytes(), user);
        }

        public T Get<T>(string key, bool user = false)
        {
            return Get(key, default(T), user);
        }

        public T Get<T>(string key, T defaultValue, bool user = false)
        {
            Type type = typeof(T);
            object boxedDefaultValue = defaultValue;

            if (type == typeof(string))
            {
                return (T)(object)GetString(key, boxedDefaultValue == null ? string.Empty : (string)boxedDefaultValue, user);
            }

            if (type == typeof(int))
            {
                return (T)(object)GetInt(key, boxedDefaultValue == null ? default : (int)boxedDefaultValue, user);
            }

            if (type == typeof(float))
            {
                return (T)(object)GetFloat(key, boxedDefaultValue == null ? default : (float)boxedDefaultValue, user);
            }

            if (type == typeof(bool))
            {
                return (T)(object)GetBool(key, boxedDefaultValue != null && (bool)boxedDefaultValue, user);
            }

            return GetObject(key, defaultValue, user);
        }

        public void Set<T>(string key, T value, bool user = false)
        {
            Type type = typeof(T);
            object boxedValue = value;

            if (type == typeof(string))
            {
                SetString(key, boxedValue == null ? string.Empty : (string)boxedValue, user);
                return;
            }

            if (type == typeof(int))
            {
                SetInt(key, boxedValue == null ? default : (int)boxedValue, user);
                return;
            }

            if (type == typeof(float))
            {
                SetFloat(key, boxedValue == null ? default : (float)boxedValue, user);
                return;
            }

            if (type == typeof(bool))
            {
                SetBool(key, boxedValue != null && (bool)boxedValue, user);
                return;
            }

            SetObject(key, value, user);
        }

        public T[] GetArray<T>(string key, T[] defaultValue = null, bool user = false)
        {
            return Get(key, defaultValue, user);
        }

        public void SetArray<T>(string key, T[] value, bool user = false)
        {
            Set(key, value, user);
        }

        public List<T> GetList<T>(string key, List<T> defaultValue = null, bool user = false)
        {
            return Get(key, defaultValue, user);
        }

        public void SetList<T>(string key, List<T> value, bool user = false)
        {
            Set(key, value, user);
        }

        public Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> defaultValue = null, bool user = false)
        {
            return Get(key, defaultValue, user);
        }

        public void SetDictionary<TKey, TValue>(string key, Dictionary<TKey, TValue> value, bool user = false)
        {
            Set(key, value, user);
        }

        public Queue<T> GetQueue<T>(string key, Queue<T> defaultValue = null, bool user = false)
        {
            return Get(key, defaultValue, user);
        }

        public void SetQueue<T>(string key, Queue<T> value, bool user = false)
        {
            Set(key, value, user);
        }

        public HashSet<T> GetHashSet<T>(string key, HashSet<T> defaultValue = null, bool user = false)
        {
            return Get(key, defaultValue, user);
        }

        public void SetHashSet<T>(string key, HashSet<T> value, bool user = false)
        {
            Set(key, value, user);
        }

        public Stack<T> GetStack<T>(string key, Stack<T> defaultValue = null, bool user = false)
        {
            return Get(key, defaultValue, user);
        }

        public void SetStack<T>(string key, Stack<T> value, bool user = false)
        {
            Set(key, value, user);
        }

        public T[,] GetRectangularArray<T>(string key, T[,] defaultValue = null, bool user = false)
        {
            return Get(key, defaultValue, user);
        }

        public void SetRectangularArray<T>(string key, T[,] value, bool user = false)
        {
            Set(key, value, user);
        }

        public T[][] GetJaggedArray<T>(string key, T[][] defaultValue = null, bool user = false)
        {
            return Get(key, defaultValue, user);
        }

        public void SetJaggedArray<T>(string key, T[][] value, bool user = false)
        {
            Set(key, value, user);
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="key">要获取游戏配置项的名称。</param>
        /// <param name="user">是否用户独有数据</param>
        /// <returns>读取的对象。</returns>
        public T GetObject<T>(string key, bool user = false)
        {
            return GetObject(key, default(T), user);
        }

        public T GetObject<T>(string key, T defaultValue, bool user = false)
        {
            if (HasStorageKey(key, user))
            {
                string jsonString = GetString(key, "", user);
                try
                {
                    return Util.LitJson.ToObject<T>(jsonString);
                }
                catch (System.Exception ex)
                {
                    LogF8.LogError($"读取对象失败，键：{key}，原因：{ex.Message}");
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <typeparam name="T">要写入对象的类型。</typeparam>
        /// <param name="key">要写入游戏配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        /// <param name="user">是否用户独有数据</param>
        public void SetObject<T>(string key, T obj, bool user = false)
        {
            if (obj == null)
            {
                LogF8.LogError("本地数据存入对象不能为空");
                return;
            }
            try
            {
                string json = Util.LitJson.ToJson(obj);
                SetString(key, json, user);
            }
            catch (System.Exception ex)
            {
                LogF8.LogError($"写入对象失败，键：{key}，原因：{ex.Message}");
            }
        }
        
        public void Remove(string key, bool user = false, string filePath = null)
        {
            if (!string.IsNullOrEmpty(filePath) && _settings.location == Location.File)
            {
                Dictionary<string, string> storage = LoadStorageFromFile(filePath);
                storage.Remove(GetKeywords(key, user));
                WriteStorageToFile(storage, filePath);
                return;
            }

            DeleteStoredKey(key, user);
        }
        
        // 保存到硬盘
        public bool Save(string filePath = null)
        {
            if (_settings.location == Location.PlayerPrefs)
            {
                PlayerPrefs.Save();
                return true;
            }

            if (_settings.location == Location.Resources)
            {
                LogF8.LogError("Resources 模式在运行时为只读，不能保存。");
                return false;
            }

            if (!string.IsNullOrEmpty(filePath) && _settings.location == Location.File)
            {
                EnsureStorageLoaded();
                return WriteStorageToFile(_fileStorage, filePath);
            }

            if (!_isFileStorageDirty)
            {
                return true;
            }

            return FlushFileStorage();
        }
        
        public void Clear(string filePath = null)
        {
            if (_settings.location == Location.PlayerPrefs)
            {
                PlayerPrefs.DeleteAll();
                return;
            }

            if (_settings.location == Location.Resources)
            {
                LogF8.LogError("Resources 模式在运行时为只读，不能修改。");
                return;
            }

            if (!string.IsNullOrEmpty(filePath) && _settings.location == Location.File)
            {
                DeleteStorageFile(filePath);
                return;
            }

            _fileStorage.Clear();
            _isFileStorageLoaded = true;
            _isFileStorageDirty = false;

            if (_settings.location != Location.File)
            {
                return;
            }

            DeleteStorageFile(_settings.defaultFilePath);
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
            Save();
            base.Destroy();
        }
    }
}
