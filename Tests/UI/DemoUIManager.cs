using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class DemoUIManager : MonoBehaviour
    {
        // UI的定义，枚举
        public enum UIID
        {
            UIMain = 1, // 游戏主界面
        }
        private Dictionary<UIID, UIConfig> configs = new Dictionary<UIID, UIConfig>
        {
            // 兼容int和枚举作为Key
            { UIID.UIMain, new UIConfig(LayerType.UI, "UIMain") } // 手动添加UI配置
        };

        private object[] data = new object[] { 123123, "asdasd" };

        
        IEnumerator Start()
        {
            /*--------------------------UI管理功能--------------------------*/
            // 初始化（必须执行，兼容int和枚举作为Key的configs）
            FF8.UI.Initialize(configs);
        
            // 设置UI Canvas属性（如果不懂属性有什么用，可自建Canvas进行试验）
            // null代表设置所有Layer
            // sortOrder层级
            // sortingLayerName层级名称
            // RenderMode渲染模式
            // pixelPerfect像素模式
            // camera设置主相机
            FF8.UI.SetCanvas(null, 1, "Default", RenderMode.ScreenSpaceCamera, false, Camera.main);
        
            // 设置UI CanvasScaler属性（如果不懂属性有什么用，可自建Canvas进行试验）
            FF8.UI.SetCanvasScaler(null, CanvasScaler.ScaleMode.ConstantPixelSize, scaleFactor: 1f, referencePixelsPerUnit: 100f);
            FF8.UI.SetCanvasScaler(LayerType.UI, CanvasScaler.ScaleMode.ScaleWithScreenSize, referenceResolution: new Vector2(1920, 1080),
                CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, matchWidthOrHeight: 0f, referencePixelsPerUnit: 100f);
            FF8.UI.SetCanvasScaler(LayerType.UI, CanvasScaler.ScaleMode.ConstantPhysicalSize, CanvasScaler.Unit.Points,
                fallbackScreenDPI: 96f, defaultSpriteDPI: 100f, referencePixelsPerUnit: 100f);

            
            /*-------------------------------------同步加载-------------------------------------*/
            // 打开UI，兼容int和枚举，可选参数：data，new UICallbacks()
            FF8.UI.Open(UIID.UIMain, data, new UICallbacks(
                (parameters, id) => // onAdded
                {
                    
                }, (parameters, id) => // OnRemoved
                {
                    
                }, () => // OnBeforeRemove
                {
                    
                }));
            // 也可以这样，guid是唯一ID
            string guid = FF8.UI.Open(1);
            
            
            /*-------------------------------------异步加载-------------------------------------*/
            // async/await方式（无多线程，WebGL也可使用）
            // await FF8.UI.OpenAsync(UIID.UIMain);
            // 或者
            // UILoader load = FF8.UI.OpenAsync(UIID.UIMain);
            // await load;
            // string guid2 = load.Guid;
            
            // 协程方式
            yield return FF8.UI.OpenAsync(UIID.UIMain);
            // 或者
            UILoader load2 = FF8.UI.OpenAsync(UIID.UIMain);
            yield return load2;
            string guid2 = load2.Guid;
            
            /*-------------------------------------其他功能-------------------------------------*/
            // 打开提示类Notify
            FF8.UI.ShowNotify(UIID.UIMain, "tip");
            FF8.UI.ShowNotify(1, "tip");
            // 异步加载
            // await FF8.UI.ShowNotifyAsync(UIID.UIMain, "tip");
            // yield return FF8.UI.ShowNotifyAsync(UIID.UIMain, "tip");
            
            // UI是否存在
            FF8.UI.Has(UIID.UIMain);
            FF8.UI.Has(1);
            
            // 根据UIid获取UI物体列表
            FF8.UI.GetByUIid(UIID.UIMain);
            FF8.UI.GetByUIid(1);
            
            // 根据guid获取UI物体
            FF8.UI.GetByGuid(guid);
            
            // 关闭UI，可选参数：isDestroy
            FF8.UI.Close(UIID.UIMain, true);
            FF8.UI.Close(1, true);
            
            // 关闭所有UI（除了Notify类），可选参数：isDestroy
            FF8.UI.Clear(true);
        }
    }

    // 提供UI界面基类，BaseView
    public class UIMain : BaseView
    {
        // Awake
        protected override void OnAwake()
        {
        }
    
        // 参数传入，每次打开UI都会执行
        protected override void OnAdded(int uiId, object[] args = null)
        {
        }
    
        // Start
        protected override void OnStart()
        {
        }
    
        protected override void OnViewTweenInit()
        {
            //transform.localScale = Vector3.one * 0.7f;
        }
    
        // 自定义打开界面动画
        protected override void OnPlayViewTween()
        {
            //transform.ScaleTween(Vector3.one, 0.1f).SetEase(Ease.Linear).SetOnComplete(OnViewOpen);
        }
    
        // 打开界面动画完成后
        protected override void OnViewOpen()
        {
        }
    
        // 删除之前，每次UI关闭前调用
        protected override void OnBeforeRemove()
        {
        }
    
        // 删除，UI关闭后调用
        protected override void OnRemoved()
        {
        }
    
        // 自动获取组件（自动生成，不能删除）
    
        // 自动获取组件（自动生成，不能删除）
    }
}
