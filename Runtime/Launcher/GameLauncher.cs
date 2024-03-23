using F8Framework.Core;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    void Start()
    {
        // 初始化模块中心
        ModuleCenter.Initialize(this);

        // 按顺序创建模块，可按需添加
        FF8.Message = ModuleCenter.CreateModule<MessageManager>();
        FF8.Input = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());
        FF8.Storage = ModuleCenter.CreateModule<StorageManager>();
        FF8.Timer = ModuleCenter.CreateModule<TimerManager>();
        FF8.Procedure = ModuleCenter.CreateModule<ProcedureManager>();
        FF8.Network = ModuleCenter.CreateModule<NetworkManager>();
        FF8.FSM = ModuleCenter.CreateModule<FSMManager>();
        FF8.GameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
        FF8.Asset = ModuleCenter.CreateModule<AssetManager>();
        FF8.Config = ModuleCenter.CreateModule<F8DataManager>();
        FF8.Audio = ModuleCenter.CreateModule<AudioManager>();
        FF8.Tween = ModuleCenter.CreateModule<Tween>();
        FF8.UI = ModuleCenter.CreateModule<UIManager>();
        FF8.Local = ModuleCenter.CreateModule<Localization>();
        FF8.SDK = ModuleCenter.CreateModule<SDKManager>();
        FF8.Download = ModuleCenter.CreateModule<DownloadManager>();
        FF8.LogWriter = ModuleCenter.CreateModule<F8LogWriter>();
        

        FF8.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();
        
        StartGame();
    }
    
    // 开始游戏
    public void StartGame()
    {
        
    }
    
    void Update()
    {
        // 更新模块
        ModuleCenter.Update();
    }
    
    void LateUpdate()
    {
        // 更新模块
        ModuleCenter.LateUpdate();
    }
    
    void FixedUpdate()
    {
        // 更新模块
        ModuleCenter.FixedUpdate();
    }
}
