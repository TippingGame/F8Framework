using F8Framework.Core;
using UnityEngine;

public class DemoCustomModule : MonoBehaviour
{
    void Start()
    {
        // 获取所有模块，并调用进入游戏
        foreach (var center in CustomModule.GetCustomModule())
        {
            center.Value.OnEnterGame();
        }
        
        // 获取指定模块
        CustomModule demo = CustomModule.GetCustomModuleByType(typeof(CustomModuleClass));
        
        // 使用模块
        CustomModuleClass.Instance.OnEnterGame();
    }
}

// 继承CustomModule的自定义模块
public class CustomModuleClass : CustomModule
{
    public static CustomModuleClass Instance => GetInstance<CustomModuleClass>();
    
    protected override void Init()
    {
        // 初始化CustomModule
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
