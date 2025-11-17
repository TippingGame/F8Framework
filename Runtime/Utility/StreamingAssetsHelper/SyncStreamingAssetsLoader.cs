using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace F8Framework.Core
{
    public class SyncStreamingAssetsLoader : Singleton<SyncStreamingAssetsLoader>
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        private struct Entry
        {
            public Entry(long index, long size)
            {
                this.index = index;
                this.size = size;
            }

            public long index;
            public long size;
        }

        private Dictionary<string, Entry> _entries = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        private FileStream _fs = null;
        private ZipFile _zipFile = null;
        private bool _isInitialized = false;
#endif

        public SyncStreamingAssetsLoader()
        {
            Initialize();
        }

        private void Initialize()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            if (_isInitialized)
                return;

            _entries.Clear();

            try
            {
                _fs = File.OpenRead(Application.dataPath);
                _zipFile = new ZipFile(_fs);

                var e = _zipFile.GetEnumerator();
                while (e.MoveNext())
                {
                    ZipEntry zipEntry = e.Current as ZipEntry;

                    if (zipEntry.Name.StartsWith("assets/"))
                    {
                        _entries.Add(zipEntry.Name, new Entry(zipEntry.ZipFileIndex, zipEntry.Size));
                    }
                }
                
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                LogF8.LogError($"Failed to initialize SyncStreamingAssetsLoader: {ex.Message}");
                Close();
            }
#endif
        }

        public void Close()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            if (_zipFile != null)
            {
                _zipFile.Close();
                _zipFile = null;
            }

            if (_fs != null)
            {
                _fs.Close();
                _fs = null;
            }
            
            _entries.Clear();
            _isInitialized = false;
#endif
        }

        // 确保在操作前已初始化
        private void EnsureInitialized()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            if (!_isInitialized)
            {
                Initialize();
            }
#endif
        }

        // 读取所有内容
        public byte[] LoadBytes(string filePath)
        {
            EnsureInitialized();

            byte[] bytes = null;

#if !UNITY_EDITOR && UNITY_ANDROID
            string path = "assets/" + filePath;

            if (!_entries.TryGetValue(path, out Entry entry))
            {
                return null;
            }

            try
            {
                using (Stream s = _zipFile.GetInputStream(entry.index))
                {
                    bytes = new byte[entry.size];
                    s.Read(bytes, 0, (int)entry.size);
                }
            }
            catch (Exception ex)
            {
                LogF8.LogError($"Failed to load bytes from {filePath}: {ex.Message}");
                // 如果读取失败，可能是文件流已关闭，尝试重新初始化
                Close();
                Initialize();
                return null;
            }
#else
            string path = Path.Combine(Application.streamingAssetsPath, filePath);

            if (!File.Exists(path))
            {
                return null;
            }

            bytes = File.ReadAllBytes(path);
#endif

            return bytes;
        }

        // 读取文本
        public string LoadText(string filePath)
        {
            byte[] bytes = LoadBytes(filePath);
            if (bytes == null)
                return null;
            
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public bool FileExists(string filePath)
        {
            EnsureInitialized();

#if !UNITY_EDITOR && UNITY_ANDROID
            string path = "assets/" + filePath;
            return _entries.ContainsKey(path);
#else
            string path = Path.Combine(Application.streamingAssetsPath, filePath);
            return File.Exists(path);
#endif
        }

        // 读取JSON并反序列化
        public T LoadJson<T>(string filePath)
        {
            string json = LoadText(filePath);
            return Util.LitJson.ToObject<T>(json);
        }

        // 读取所有行
        public string[] ReadAllLines(string filePath)
        {
            string text = LoadText(filePath);
            return text?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }
    }
}