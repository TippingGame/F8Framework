using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoStaticModule : MonoBehaviour
    {
        void Start()
        {
            // 获取所有静态模块，并调用进入游戏
            foreach (var center in StaticModule.GetStaticModule())
            {
                center.Value.OnEnterGame();
            }

            // 获取指定静态模块
            StaticModule demo = StaticModule.GetStaticModuleByType(typeof(StaticModuleClass));

            // 使用静态模块
            StaticModuleClass.Instance.OnEnterGame();
        }
    }

// 继承StaticModule的自定义静态模块
    public class StaticModuleClass : StaticModule
    {
        public static StaticModuleClass Instance => GetInstance<StaticModuleClass>();

        protected override void Init()
        {
            // 初始化StaticModule
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
}
