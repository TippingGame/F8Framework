using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    // 红点示例代码
    public class DemoUIRedDot : MonoBehaviour
    {
        // 手动添加
        public const string UIMain = "UIMain";
        public const string UIMain_Intensify = "UIMain_Intensify";
        public const string UIMain_AutoIntensify = "UIMain_AutoIntensify";
        public const string UIMain_AutoIntensify2 = "UIMain_AutoIntensify2";
        public const string UIMain_AutoIntensify3 = "UIMain_AutoIntensify3";
        
        // 初始化
        public void Init()
        {
            // 手动添加
            UIRedDot.Instance.AddRedDotCfg(UIMain);
            UIRedDot.Instance.AddRedDotCfg(UIMain_Intensify, UIMain);
            UIRedDot.Instance.AddRedDotCfg(UIMain_AutoIntensify, UIMain);
            UIRedDot.Instance.AddRedDotCfg(UIMain_AutoIntensify2, UIMain_AutoIntensify);
            UIRedDot.Instance.AddRedDotCfg(UIMain_AutoIntensify3, UIMain_AutoIntensify2);

            UIRedDot.Instance.Init();
        }

        private void Start()
        {
            // 改变布尔状态
            UIRedDot.Instance.Change(DemoUIRedDot.UIMain_AutoIntensify2, true);
            LogF8.Log(UIRedDot.Instance.GetState(DemoUIRedDot.UIMain_AutoIntensify3));
            
            // 改变数量状态
            UIRedDot.Instance.Change(DemoUIRedDot.UIMain_AutoIntensify2, 15);
            LogF8.Log(UIRedDot.Instance.GetCount(DemoUIRedDot.UIMain_AutoIntensify3));
            
            // 改变文本状态
            UIRedDot.Instance.Change(DemoUIRedDot.UIMain_AutoIntensify2, "空闲");
            LogF8.Log(UIRedDot.Instance.GetTextState(DemoUIRedDot.UIMain_AutoIntensify3));
            
            // 绑定，解绑GameObject
            UIRedDot.Instance.Binding(DemoUIRedDot.UIMain_AutoIntensify2, this.gameObject);
            UIRedDot.Instance.UnBinding(DemoUIRedDot.UIMain_AutoIntensify2);
            UIRedDot.Instance.UnBinding(DemoUIRedDot.UIMain_AutoIntensify2, this.gameObject);
            
            // 清空所有红点状态
            UIRedDot.Instance.RemoveAllRed();
        }
    }
}
