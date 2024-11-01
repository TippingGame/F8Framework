using F8Framework.Core;
using F8Framework.F8ExcelDataClass;

namespace F8Framework.Launcher
{
    public static class FF8
    {
        //相当于重命名
        /* ------------------------核心模块------------------------ */
        
        // 全局消息
        private static MessageManager _message;
        // 输入管理-->使用了消息模块
        private static InputManager _inputManager;
        // 本地存储
        private static StorageManager _storage;
        // 游戏时间管理-->使用了消息模块
        private static TimerManager _timer;
        // 流程管理
        private static ProcedureManager _procedure;
        // 网络管理
        private static NetworkManager _networkManager;
        // 有限状态机
        private static FSMManager _fsm;
        // 游戏对象池
        private static GameObjectPool _gameObjectPool;
        // 资产管理
        private static AssetManager _asset;
        // 读取配置表-->使用了资产模块
        private static F8DataManager _config;
        // 音频管理-->使用了资产模块-->使用了游戏对象池模块-->使用了补间动画模块-->使用了时间模块
        private static AudioManager _audio;
        // 补间动画
        private static Tween _tween;
        // UI界面管理-->使用了资产模块
        private static UIManager _ui;
        // 本地化-->使用了配置模块-->使用了资产模块
        private static Localization _localization;
        // SDK管理-->使用了消息模块
        private static SDKManager _sdkManager;
        // 下载管理器
        private static DownloadManager _downloadManager;
        // 日志助手
        private static F8LogWriter _logWriter;
        
        
        /* ------------------------可选模块------------------------ */
        // 热更新版本管理-->使用了下载模块-->使用了资产模块
        private static HotUpdateManager _hotUpdateManager;
        
        
        public static MessageManager Message
        {
            get
            {
                if (_message == null)
                    _message = ModuleCenter.CreateModule<MessageManager>();
                return _message;
            }
            set
            {
                if (_message == null)
                    _message = value;
            }
        }

        public static InputManager Input
        {
            get
            {
                if (_inputManager == null)
                    _inputManager = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());
                return _inputManager;
            }
            set
            {
                if (_inputManager == null)
                    _inputManager = value;
            }
        }
        
        public static StorageManager Storage
        {
            get
            {
                if (_storage == null)
                    _storage = ModuleCenter.CreateModule<StorageManager>();
                return _storage;
            }
            set
            {
                if (_storage == null)
                    _storage = value;
            }
        }

        public static TimerManager Timer
        {
            get
            {
                if (_timer == null)
                    _timer = ModuleCenter.CreateModule<TimerManager>();
                return _timer;
            }
            set
            {
                if (_timer == null)
                    _timer = value;
            }
        }

        public static ProcedureManager Procedure
        {
            get
            {
                if (_procedure == null)
                    _procedure = ModuleCenter.CreateModule<ProcedureManager>();
                return _procedure;
            }
            set
            {
                if (_procedure == null)
                    _procedure = value;
            }
        }

        public static NetworkManager Network
        {
            get
            {
                if (_networkManager == null)
                    _networkManager = ModuleCenter.CreateModule<NetworkManager>();
                return _networkManager;
            }
            set
            {
                if (_networkManager == null)
                    _networkManager = value;
            }
        }
        
        public static FSMManager FSM
        {
            get
            {
                if (_fsm == null)
                    _fsm = ModuleCenter.CreateModule<FSMManager>();
                return _fsm;
            }
            set
            {
                if (_fsm == null)
                    _fsm = value;
            }
        }

        public static GameObjectPool GameObjectPool
        {
            get
            {
                if (_gameObjectPool == null)
                {
                    _gameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
                    ModuleCenter.CreateModule<F8PoolGlobal>();
                }
                    
                return _gameObjectPool;
            }
            set
            {
                if (_gameObjectPool == null)
                {
                    _gameObjectPool = value;
                    ModuleCenter.CreateModule<F8PoolGlobal>();
                }
            }
        }

        public static AssetManager Asset
        {
            get
            {
                if (_asset == null)
                    _asset = ModuleCenter.CreateModule<AssetManager>();
                return _asset;
            }
            set
            {
                if (_asset == null)
                    _asset = value;
            }
        }
        
        public static F8DataManager Config
        {
            get
            {
                if (_config == null)
                    _config = ModuleCenter.CreateModule<F8DataManager>();
                return _config;
            }
            set
            {
                if (_config == null)
                    _config = value;
            }
        }
        
        public static AudioManager Audio
        {
            get
            {
                if (_audio == null)
                    _audio = ModuleCenter.CreateModule<AudioManager>();
                return _audio;
            }
            set
            {
                if (_audio == null)
                    _audio = value;
            }
        }

        public static Tween Tween
        {
            get
            {
                if (_tween == null)
                    _tween = ModuleCenter.CreateModule<Tween>();
                return _tween;
            }
            set
            {
                if (_tween == null)
                    _tween = value;
            }
        }

        public static UIManager UI
        {
            get
            {
                if (_ui == null)
                    _ui = ModuleCenter.CreateModule<UIManager>();
                return _ui;
            }
            set
            {
                if (_ui == null)
                    _ui = value;
            }
        }

        public static Localization Local
        {
            get
            {
                if (_localization == null)
                    _localization = ModuleCenter.CreateModule<Localization>();
                return _localization;
            }
            set
            {
                if (_localization == null)
                    _localization = value;
            }
        }
        
        public static SDKManager SDK
        {
            get
            {
                if (_sdkManager == null)
                    _sdkManager = ModuleCenter.CreateModule<SDKManager>();
                return _sdkManager;
            }
            set
            {
                if (_sdkManager == null)
                    _sdkManager = value;
            }
        }
        
        public static DownloadManager Download
        {
            get
            {
                if (_downloadManager == null)
                    _downloadManager = ModuleCenter.CreateModule<DownloadManager>();
                return _downloadManager;
            }
            set
            {
                if (_downloadManager == null)
                    _downloadManager = value;
            }
        }
        
        public static F8LogWriter LogWriter
        {
            get
            {
                if (_logWriter == null)
                    _logWriter = ModuleCenter.CreateModule<F8LogWriter>();
                return _logWriter;
            }
            set
            {
                if (_logWriter == null)
                    _logWriter = value;
            }
        }
        
        public static HotUpdateManager HotUpdate
        {
            get
            {
                if (_hotUpdateManager == null)
                    _hotUpdateManager = ModuleCenter.CreateModule<HotUpdateManager>();
                return _hotUpdateManager;
            }
            set
            {
                if (_hotUpdateManager == null)
                    _hotUpdateManager = value;
            }
        }
    }
}

