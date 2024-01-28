using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

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
        UIManager.Instance.Initialize(configs);
        // 打开UI，可选参数：data，new UICallbacks()
        UIManager.Instance.Open(1, data, new UICallbacks());
        UIManager.Instance.OpenAsync(1);
        // 打开提示类Notify
        UIManager.Instance.ShowNotify(1, "tip");
        // UI是否存在
        UIManager.Instance.Has(1);
        // 关闭UI，可选参数：isDestroy
        UIManager.Instance.Close(1, true);
        // 关闭所有UI（除了Notify类），可选参数：isDestroy
        UIManager.Instance.Clear(true);
    }
}
// 提供UI界面基类，BaseView
public class UIMain : BaseView
{
    protected override void OnAwake()
    {
        // Awake
    }
        
    protected override void OnAdded(object[] args, int uiId)
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
    
    protected override void OnBeforeRemove(object[] args, int uiId){
        // 删除之前
    }
    
    protected override void OnRemoved(object[] args, int uiId)
    {
        // 删除
    }
}
