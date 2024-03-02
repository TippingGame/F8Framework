namespace F8Framework.Core
{
    public static class FF8
    {
        //相当于重命名
        /* ------------------------核心模块------------------------ */
        
        // 全局消息
        private static MessageManager _message;
        // 输入管理
        private static InputManager _inputManager;
        // 本地存储
        private static StorageManager _storage;
        // 游戏时间管理
        private static TimerManager _timer;
        // 流程管理
        private static ProcedureManager _procedure;
        // 有限状态机
        private static FSMManager _fsm;
        // 游戏对象池
        private static GameObjectPool _gameObjectPool;
        // 游戏对象池全局设置
        private static F8PoolGlobal _poolGlobal;
        // 资产管理
        private static AssetManager _asset;
        // 读取配置表
        private static F8DataManager _config;
        // 音频管理
        private static AudioManager _audio;
        // 补间动画
        private static Tween _tween;
        // UI界面管理
        private static UIManager _ui;
        // 本地化
        private static Localization _localization;
        // SDK管理
        private static SDKManager _sdkManager;
        // 下载管理器
        private static DownloadManager _downloadManager;
        // 日志助手
        private static F8LogWriter _logWriter;
        
        public static MessageManager Message
        {
            get
            {
                if (_message == null)
                    _message = ModuleCenter.CreateModule<MessageManager>();
                return _message;
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
        }
        
        public static StorageManager Storage
        {
            get
            {
                if (_storage == null)
                    _storage = ModuleCenter.CreateModule<StorageManager>();
                return _storage;
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
        }

        public static ProcedureManager Procedure
        {
            get
            {
                if (_procedure == null)
                    _procedure = ModuleCenter.CreateModule<ProcedureManager>();
                return _procedure;
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
        }

        public static GameObjectPool GameObjectPool
        {
            get
            {
                if (_gameObjectPool == null)
                    _gameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
                return _gameObjectPool;
            }
        }
        
        public static F8PoolGlobal PoolGlobal
        {
            get
            {
                if (_poolGlobal == null)
                    _poolGlobal = ModuleCenter.CreateModule<F8PoolGlobal>();
                return _poolGlobal;
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
        }
        
        public static F8DataManager Config
        {
            get
            {
                if (_config == null)
                    _config = F8DataManager.Instance;
                return _config;
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
        }

        public static Tween Tween
        {
            get
            {
                if (_tween == null)
                    _tween = ModuleCenter.CreateModule<Tween>();
                return _tween;
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
        }

        public static Localization Local
        {
            get
            {
                if (_localization == null)
                    _localization = ModuleCenter.CreateModule<Localization>();
                return _localization;
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
        }
        
        public static DownloadManager Download
        {
            get
            {
                if (_downloadManager == null)
                    _downloadManager = ModuleCenter.CreateModule<DownloadManager>();
                return _downloadManager;
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
        }
        
        /* ------------------------可选模块------------------------ */
        
        

    }
}

