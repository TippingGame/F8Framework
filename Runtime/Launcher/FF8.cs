namespace F8Framework.Core
{
    public static partial class FF8
    {
        //相当于重命名
        /* ------------------------核心模块------------------------ */

        // 全局消息
        public static MessageManager Message = ModuleCenter.CreateModule<MessageManager>();

        // 本地存储
        public static StorageManager Storage = ModuleCenter.CreateModule<StorageManager>();
        
        // 游戏时间管理
        public static TimerManager Timer = ModuleCenter.CreateModule<TimerManager>();
        
        // 流程管理
        public static ProcedureManager Procedure = ModuleCenter.CreateModule<ProcedureManager>();
        
        // 游戏对象池
        public static F8PoolGlobal Pool = ModuleCenter.CreateModule<F8PoolGlobal>();
        
        // 资产管理
        public static AssetManager Asset = ModuleCenter.CreateModule<AssetManager>();
        
        // 音频管理
        public static AudioManager Audio = ModuleCenter.CreateModule<AudioManager>();
        
        // 补间动画
        public static Tween Tween = ModuleCenter.CreateModule<Tween>();
        
        // UI界面管理
        public static UIManager UI = ModuleCenter.CreateModule<UIManager>();
        
        // 日志助手
        public static F8LogHelper LogHelper = ModuleCenter.CreateModule<F8LogHelper>();

        /* ------------------------可选模块------------------------ */
        
        // 读取配置表
        // public static F8Framework.F8DataManager.F8DataManager Config = ModuleCenter.CreateModule<F8Framework.F8DataManager.F8DataManager>();

    }
}

