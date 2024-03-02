using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoModuleCenter : MonoBehaviour
    {
        void Start()
        {
            //初始化模块中心
            ModuleCenter.Initialize(this);

            // 创建模块，（参数可选，优先级越小越早轮询）
            int priority = 100;
            ModuleCenter.CreateModule<TimerManager>(priority);

            // 通过ModuleCenter调用模块方法
            ModuleCenter.GetModule<TimerManager>().GetServerTime();

            // 通过获取实例调用模块方法
            TimerManager.Instance.GetServerTime();
        }
    }

    [UpdateRefresh]
    [LateUpdateRefresh]
    [FixedUpdateRefresh]
    public class DemoModuleCenterClass : ModuleSingleton<DemoModuleCenterClass>, IModule
    {
        public void OnInit(object createParam)
        {
            // 模块创建初始化
        }

        public void OnUpdate()
        {
            // 模块Update
        }

        public void OnLateUpdate()
        {
            // 模块LateUpdate
        }

        public void OnFixedUpdate()
        {
            // 模块FixedUpdate
        }

        public void OnTermination()
        {
            // 模块销毁
            base.Destroy();
        }
    }
}
