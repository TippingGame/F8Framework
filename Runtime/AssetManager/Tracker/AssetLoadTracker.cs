using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    public sealed class AssetLoadTracker
    {
        private readonly Dictionary<string, int> _refs = new();
        private bool _released;
        private bool _releasedUnloadAllLoadedObjects;

        public T Load<T>(string assetName, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where T : Object
        {
            T asset = AssetManager.Instance.Load<T>(assetName, subAssetName, mode);
            if (asset != null)
                Retain(assetName, mode);

            return asset;
        }

        public Object Load(string assetName, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            Object asset = AssetManager.Instance.Load(assetName, subAssetName, mode);
            if (asset != null)
                Retain(assetName, mode);

            return asset;
        }

        public Object Load(string assetName, Type assetType, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            Object asset = AssetManager.Instance.Load(assetName, assetType, subAssetName, mode);
            if (asset != null)
                Retain(assetName, mode);

            return asset;
        }

        public Dictionary<string, TObject> LoadAll<TObject>(string assetName,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where TObject : Object
        {
            Dictionary<string, TObject> assets = AssetManager.Instance.LoadAll<TObject>(assetName, mode);
            if (assets != null)
                Retain(assetName, mode);

            return assets;
        }

        public Dictionary<string, Object> LoadAll(string assetName,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            Dictionary<string, Object> assets = AssetManager.Instance.LoadAll(assetName, mode);
            if (assets != null)
                Retain(assetName, mode);

            return assets;
        }

        public Dictionary<string, TObject> LoadSub<TObject>(string assetName, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where TObject : Object
        {
            Dictionary<string, TObject> assets = AssetManager.Instance.LoadSub<TObject>(assetName, subAssetName, mode);
            if (assets != null)
                Retain(assetName, mode);

            return assets;
        }

        public Dictionary<string, Object> LoadSub(string assetName, string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            Dictionary<string, Object> assets = AssetManager.Instance.LoadSub(assetName, subAssetName, mode);
            if (assets != null)
                Retain(assetName, mode);

            return assets;
        }

        public BaseDirLoader LoadDir(string assetName,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            BaseDirLoader loader = AssetManager.Instance.LoadDir(assetName, mode);
            RetainDirectory(assetName, mode);
            return loader;
        }

        public BaseLoader LoadAsync<T>(string assetName, OnAssetObject<T> callback = null,
            string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where T : Object
        {
            return AssetManager.Instance.LoadAsync<T>(assetName, asset =>
            {
                TrackAsyncResult(assetName, mode, asset, () => callback?.Invoke(asset));
            }, subAssetName, mode);
        }

        public BaseLoader LoadAsync(string assetName, OnAssetObject<Object> callback = null,
            string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetManager.Instance.LoadAsync(assetName, asset =>
            {
                TrackAsyncResult(assetName, mode, asset, () => callback?.Invoke(asset));
            }, subAssetName, mode);
        }

        public BaseLoader LoadAsync(string assetName, Type assetType, OnAssetObject<Object> callback = null,
            string subAssetName = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetManager.Instance.LoadAsync(assetName, assetType, asset =>
            {
                TrackAsyncResult(assetName, mode, asset, () => callback?.Invoke(asset));
            }, subAssetName, mode);
        }

        public BaseLoader LoadAllAsync<TObject>(string assetName, OnAllAssetObject<TObject> callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where TObject : Object
        {
            return AssetManager.Instance.LoadAllAsync<TObject>(assetName, assets =>
            {
                TrackAsyncResult(assetName, mode, assets, () => callback?.Invoke(assets));
            }, mode);
        }

        public BaseLoader LoadAllAsync(string assetName, OnAllAssetObject<Object> callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetManager.Instance.LoadAllAsync(assetName, assets =>
            {
                TrackAsyncResult(assetName, mode, assets, () => callback?.Invoke(assets));
            }, mode);
        }

        public BaseLoader LoadSubAsync<TObject>(string assetName, string subAssetName = null,
            OnAllAssetObject<TObject> callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
            where TObject : Object
        {
            return AssetManager.Instance.LoadSubAsync<TObject>(assetName, subAssetName, assets =>
            {
                TrackAsyncResult(assetName, mode, assets, () => callback?.Invoke(assets));
            }, mode);
        }

        public BaseLoader LoadSubAsync(string assetName, string subAssetName = null,
            OnAllAssetObject<Object> callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetManager.Instance.LoadSubAsync(assetName, subAssetName, assets =>
            {
                TrackAsyncResult(assetName, mode, assets, () => callback?.Invoke(assets));
            }, mode);
        }

        public BaseDirLoader LoadDirAsync(string assetName, Action callback = null,
            AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            return AssetManager.Instance.LoadDirAsync(assetName, () =>
            {
                if (_released)
                {
                    ReleaseDirectory(assetName, mode, _releasedUnloadAllLoadedObjects);
                    return;
                }

                RetainDirectory(assetName, mode);
                callback?.Invoke();
            }, mode);
        }

        public void Release(string assetName, bool unloadAllLoadedObjects = false)
        {
            if (string.IsNullOrEmpty(assetName) || !_refs.TryGetValue(assetName, out int count) || count <= 0)
                return;

            if (ModuleCenter.Contains<AssetManager>())
            {
                AssetManager.Instance.Unload(assetName, unloadAllLoadedObjects);
            }

            if (count <= 1)
            {
                _refs.Remove(assetName);
            }
            else
            {
                _refs[assetName] = count - 1;
            }
        }

        public void ReleaseAll(bool unloadAllLoadedObjects = false)
        {
            _released = true;
            _releasedUnloadAllLoadedObjects = unloadAllLoadedObjects;

            if (ModuleCenter.Contains<AssetManager>())
            {
                var assetManager = AssetManager.Instance;
                foreach (var pair in _refs)
                {
                    for (int i = 0; i < pair.Value; i++)
                        assetManager.Unload(pair.Key, unloadAllLoadedObjects);
                }
            }

            _refs.Clear();
        }

        private void Retain(string assetName, AssetManager.AssetAccessMode mode)
        {
            if (string.IsNullOrEmpty(assetName))
                return;

            _released = false;
            _releasedUnloadAllLoadedObjects = false;
            _refs[assetName] = _refs.TryGetValue(assetName, out int count) ? count + 1 : 1;
        }

        private void RetainDirectory(string assetName, AssetManager.AssetAccessMode mode)
        {
            if (!ModuleCenter.Contains<AssetManager>())
                return;

            var assetManager = AssetManager.Instance;
            if (assetManager == null)
                return;

            AssetManager.AssetInfo info = assetManager.GetAssetInfo(assetName + AssetManager.DirSuffix, mode);
            if (!assetManager.IsLegal(ref info) || info.AssetPath == null)
                return;

            foreach (string subAssetName in info.AssetPath)
            {
                if (!string.IsNullOrEmpty(subAssetName))
                    Retain(subAssetName, mode);
            }
        }

        private void ReleaseDirectory(string assetName, AssetManager.AssetAccessMode mode,
            bool unloadAllLoadedObjects)
        {
            if (!ModuleCenter.Contains<AssetManager>())
                return;

            var assetManager = AssetManager.Instance;
            if (assetManager == null)
                return;

            AssetManager.AssetInfo info = assetManager.GetAssetInfo(assetName + AssetManager.DirSuffix, mode);
            if (!assetManager.IsLegal(ref info) || info.AssetPath == null)
                return;

            foreach (string subAssetName in info.AssetPath)
            {
                if (!string.IsNullOrEmpty(subAssetName))
                    assetManager.Unload(subAssetName, unloadAllLoadedObjects);
            }
        }

        private void TrackAsyncResult(string assetName, AssetManager.AssetAccessMode mode, Object asset, Action callback)
        {
            if (asset == null)
            {
                callback?.Invoke();
                return;
            }

            if (_released)
            {
                if (ModuleCenter.Contains<AssetManager>())
                {
                    AssetManager.Instance.Unload(assetName, _releasedUnloadAllLoadedObjects);
                }
                return;
            }

            Retain(assetName, mode);
            callback?.Invoke();
        }

        private void TrackAsyncResult<TObject>(string assetName, AssetManager.AssetAccessMode mode,
            Dictionary<string, TObject> assets, Action callback) where TObject : Object
        {
            if (assets == null)
            {
                callback?.Invoke();
                return;
            }

            if (_released)
            {
                if (ModuleCenter.Contains<AssetManager>())
                {
                    AssetManager.Instance.Unload(assetName, _releasedUnloadAllLoadedObjects);
                }
                return;
            }

            Retain(assetName, mode);
            callback?.Invoke();
        }
    }
}

