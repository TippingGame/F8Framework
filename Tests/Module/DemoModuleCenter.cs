using F8Framework.Core;
using UnityEngine;

public class DemoModuleCenter : MonoBehaviour
{
    void Start()
    {
        // 获取所有模块，并调用进入游戏
        foreach (var center in ModuleCenter.GetSubCenter())
        {
            center.Value.OnEnterGame();
        }
        
        // 获取指定模块
        ModuleCenter demoCenter = ModuleCenter.GetCenterByType(typeof(DemoCenter));
        
        // 使用模块
        DemoCenter.Instance.OnEnterGame();
    }
}

// 继承ModuleCenter的模块中心
public class DemoCenter : ModuleCenter
{
    public static DemoCenter Instance => GetInstance<DemoCenter>();
    
    protected override void Init()
    {
        // 初始化Center
    }
        
    public override void OnEnterGame()
    {
        // 进入游戏
    }

    public override void OnQuitGame()
    {
        // 退出游戏
    }
}
