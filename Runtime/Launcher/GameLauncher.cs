using F8Framework.Core;
using UnityEngine;

public class GameLauncher : MonoBehaviour
{
    void Start()
    {
        // 初始化模块中心
        ModuleCenter.Initialize(this);

        // 按顺序创建模块
        FF8.Message.ToString();
        FF8.Storage.ToString();
        FF8.Timer.ToString();
        FF8.Procedure.ToString();
        FF8.FSM.ToString();
        FF8.Pool.ToString();
        FF8.Asset.ToString();
        FF8.Audio.ToString();
        FF8.Tween.ToString();
        FF8.UI.ToString();
        FF8.Localization.ToString();
        FF8.LogHelper.ToString();

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
    
    void FixedUpdate()
    {
        // 更新模块
        ModuleCenter.FixedUpdate();
    }
}
