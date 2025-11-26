using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F8Framework.Core
{
    public class SceneLoader : BaseLoader
    {
        public override bool LoaderSuccess => _isDone;
        public override float Progress => _progress;

        private readonly bool _isEditor = false;
        private BaseLoader _assetLoader;
        private AsyncOperation _sceneOperation;
        private string _sceneName;
        private bool _isDone = false;
        private float _progress = 0f;
        private OnSceneObject _onAssetScene;
        public Scene SceneObject;

        public SceneLoader(bool isEditor = false)
        {
            _isEditor = isEditor;
        }
        
        public SceneLoader LoadAsync(string sceneName, LoadSceneParameters parameters, bool allowSceneActivation = true,
            OnSceneObject callback = null, AssetManager.AssetAccessMode mode = AssetManager.AssetAccessMode.UNKNOWN)
        {
            _sceneName = sceneName;
            _onAssetScene = callback;
            
            if (!_isEditor)
            {
                _assetLoader = AssetManager.Instance.LoadAsync(sceneName, null, null, mode);
            }
            
            Util.Unity.StartCoroutine(LoadSequence(parameters, allowSceneActivation));
            
            return this;
        }
        
        private IEnumerator LoadSequence(LoadSceneParameters parameters, bool allowSceneActivation)
        {
            if (_assetLoader != null)
            {
                while (!_assetLoader.LoaderSuccess)
                {
                    _progress = _assetLoader.Progress * 0.55f;
                    yield return null;
                }
            }
            
#if UNITY_EDITOR
            _sceneOperation = _isEditor ?
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(_sceneName, parameters) :
                SceneManager.LoadSceneAsync(_sceneName, parameters);
#else
            _sceneOperation = SceneManager.LoadSceneAsync(_sceneName, parameters);
#endif
            
            if (_sceneOperation != null)
            {
                _sceneOperation.allowSceneActivation = allowSceneActivation;
                SceneObject = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                
                while (!_sceneOperation.isDone)
                {
                    _progress = 0.55f + (_sceneOperation.progress * 0.5f);
                    yield return null;
                }
            }

            if (SceneObject.IsValid())
            {
                _progress = 1f;
                _isDone = true;
                _onAssetScene(SceneObject);
                OnComplete();
            }
            else
            {
                LogF8.LogError("加载的场景无效：" + _sceneName);
            }
        }
        
        // 手动激活场景（当allowSceneActivation为false时使用）
        public void AllowSceneActivation()
        {
            if (_sceneOperation != null && !_sceneOperation.allowSceneActivation)
            {
                _sceneOperation.allowSceneActivation = true;
                Util.Unity.StartCoroutine(WaitForActivation());
            }
        }

        private IEnumerator WaitForActivation()
        {
            while (_sceneOperation != null && !_sceneOperation.isDone)
            {
                _progress = _sceneOperation.progress;
                yield return null;
            }
            
            if (SceneObject.IsValid())
            {
                _progress = 1f;
                _isDone = true;
                _onAssetScene(SceneObject);
                OnComplete();
            }
            else
            {
                LogF8.LogError("加载的场景无效：" + _sceneName);
            }
        }
    }
}