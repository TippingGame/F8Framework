using UnityEngine;

namespace F8Framework.Core
{
    public abstract class ModuleSingletonMono<T> : MonoBehaviour where T : class, IModule
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    LogF8.LogError($"模块 {typeof(T)} 未创建。");
                return _instance;
            }
        }
        
        private void Awake()
        {
            //防止创建多余单例
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy()
        {
            _instance = null;
        }
    }
}