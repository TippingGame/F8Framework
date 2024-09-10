using System.Collections.Generic;
using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class DemoUIManager : MonoBehaviour
    {
        private Dictionary<int, UIConfig> configs = new Dictionary<int, UIConfig>
        {
            { 1, new UIConfig(LayerType.UI, "UIMain") }
        };

        private object[] data = new object[] { 123123, "asdasd" };

        void Start()
        {
            /*----------UI管理功能----------*/

            // 初始化
            FF8.UI.Initialize(configs);
            // 设置UI Canvas属性
            FF8.UI.SetCanvas(null, 1, "Default", RenderMode.ScreenSpaceCamera, false, Camera.main);
            // 设置UI CanvasScaler属性
            FF8.UI.SetCanvasScaler(null, CanvasScaler.ScaleMode.ConstantPixelSize, 1f, 100f);
            FF8.UI.SetCanvasScaler(LayerType.UI, CanvasScaler.ScaleMode.ScaleWithScreenSize, new Vector2(1920, 1080), CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, 0f, 100f);
            FF8.UI.SetCanvasScaler(LayerType.UI, CanvasScaler.ScaleMode.ConstantPhysicalSize, CanvasScaler.Unit.Points, 96f, 100f, 100f);
            // 打开UI，可选参数：data，new UICallbacks()
            FF8.UI.Open(1, data, new UICallbacks(
                (parameters, id) => // onAdded
                {
                    
                }, (parameters, id) => // OnRemoved
                {
                    
                }, () => // OnBeforeRemove
                {
                    
                }));
            // 打开提示类Notify
            FF8.UI.ShowNotify(1, "tip");
            // UI是否存在
            FF8.UI.Has(1);
            // 关闭UI，可选参数：isDestroy
            FF8.UI.Close(1, true);
            // 关闭所有UI（除了Notify类），可选参数：isDestroy
            FF8.UI.Clear(true);
        }
    }

    // 提供UI界面基类，BaseView
    public class UIMain : BaseView
    {
        protected override void OnAwake()
        {
            // Awake
        }

        protected override void OnAdded(int uiId, object[] args = null)
        {
            // 参数传入
        }

        protected override void OnStart()
        {
            // Start
            object[] args = Args;
            int uiId = UIid;
        }

        protected override void OnViewTweenInit()
        {
            transform.localScale = Vector3.one * 0.7f;
        }

        protected override void OnPlayViewTween()
        {
            // 打开界面动画，可自行添加关闭界面动画
            transform.ScaleTween(Vector3.one, 0.1f).SetEase(Ease.Linear).SetOnComplete(OnViewOpen);
        }

        protected override void OnViewOpen()
        {
            // 打开界面动画完成后
        }

        protected override void OnBeforeRemove()
        {
            // 删除之前
        }

        protected override void OnRemoved()
        {
            // 删除
        }

        // 自动获取组件（自动生成，不能删除）

        // 自动获取组件（自动生成，不能删除）
    }
}
