#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    public class AssetDatabaseManager : Singleton<AssetDatabaseManager>
    {
        private Dictionary<string, EditorLoader> editorLoaders = new Dictionary<string, EditorLoader>();
        
        public Dictionary<string, EditorLoader> GetEditorLoaders()
        {
            return editorLoaders;
        }

        private void EditorLoaderPool(string assetPath, out EditorLoader loader)
        {
            if (editorLoaders.ContainsKey(assetPath))
            {
                loader = editorLoaders[assetPath];
            }
            else
            {
                loader = new EditorLoader(null, new Dictionary<string, Object>());
                editorLoaders.Add(assetPath, loader);
            }
        }

        public T EditorLoadAsset<T>(string assetPath, string subAssetName, out EditorLoader loader) where T : Object
        {
            EditorLoaderPool(assetPath, out loader);
            
            if (subAssetName.IsNullOrEmpty())
            {
                T o = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                loader.Asset = o;
                loader.AssetDatabaseLoadSuccess();
                if (loader.Asset is SceneAsset)
                {
                    var loadSceneParams = new UnityEngine.SceneManagement.LoadSceneParameters(UnityEngine.SceneManagement.LoadSceneMode.Single, UnityEngine.SceneManagement.LocalPhysicsMode.None);
                    UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(assetPath, loadSceneParams);
                    LogF8.LogAsset("编辑器模式下自动加载场景");
                }
                return o;
            }
            else
            {
                var objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var obj in objs)
                {
                    loader.AllAsset.TryAdd(obj.name, obj);
                    if (obj.name == subAssetName)
                    {
                        loader.Asset = obj;
                    }
                }
                loader.AssetDatabaseLoadSuccess();
                return loader.Asset == null ? null : loader.Asset as T;
            }
        }

        public Object EditorLoadAsset(string assetPath, out EditorLoader loader, System.Type assetType = null, string subAssetName = null)
        {
            EditorLoaderPool(assetPath, out loader);
            
            if (subAssetName.IsNullOrEmpty())
            {
                if (assetType == null)
                {
                    Object o = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                    loader.Asset = o;
                    loader.AssetDatabaseLoadSuccess();
                    if (loader.Asset is SceneAsset)
                    {
                        var loadSceneParams = new UnityEngine.SceneManagement.LoadSceneParameters(UnityEngine.SceneManagement.LoadSceneMode.Single, UnityEngine.SceneManagement.LocalPhysicsMode.None);
                        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(assetPath, loadSceneParams);
                        LogF8.LogAsset("编辑器模式下自动加载场景");
                    }
                    return o;
                }

                Object o2 = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                loader.Asset = o2;
                return o2;
            }
            else
            {
                var objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var obj in objs)
                {
                    loader.AllAsset.TryAdd(obj.name, obj);
                    if (obj.name == subAssetName)
                    {
                        loader.Asset = obj;
                    }
                }
                loader.AssetDatabaseLoadSuccess();
                return loader.Asset == null ? null : loader.Asset;
            }
        }

        public Dictionary<string, Object> EditorLoadAllAsset(string assetPath, out EditorLoader loader)
        {
            EditorLoaderPool(assetPath, out loader);
            
            var objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (var obj in objs)
            {
                loader.AllAsset.TryAdd(obj.name, obj);
            }
            loader.AssetDatabaseLoadSuccess();
            return loader.AllAsset;
        }

        public void Unload(string assetPath)
        {
            if (editorLoaders.TryGetValue(assetPath, out EditorLoader loader))
            {
                loader.isLoadSuccess = false;
                loader.Asset = null;
                loader.AllAsset = null;
            }
        }
    }
}
#endif