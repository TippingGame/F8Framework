using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public enum LayerType
    {
        Game,
        UI,
        PopUp,
        Dialog,// 模态弹窗（只显示最老的窗口，关闭后自动显示下一个新窗口）
        Notify,// 非模态弹窗（老窗口和新窗口共存，新的显示在前，自己管理窗口关闭）
        Guide
    }

    public struct UIConfig
    {
        public LayerType Layer;
        public string AssetName;
        
        public UIConfig(LayerType layer, string assetName)
        {
            Layer = layer;
            AssetName = assetName;
        }
    }

    public class UIManager : ModuleSingletonMono<UIManager>, IModule
    {
        private LayerGame _layerGame;
        private LayerUI _layerUI;
        private LayerPopUp _layerPopUp;
        private LayerDialog _layerDialog;
        private LayerNotify _layerNotify;
        private LayerGuide _layerGuide;

        private Dictionary<int, UIConfig> _configs = new Dictionary<int, UIConfig>();
        private List<string> _currentUuids = new List<string>();
        
        public void Initialize(Dictionary<int, UIConfig> configs)
        {
            _configs = configs;
            
            GameObject gameGo = new GameObject("LayerGame");
            GameObject uiGo = new GameObject("LayerUI");
            GameObject popupGo = new GameObject("LayerPopUp");
            GameObject dialogGo = new GameObject("LayerDialog");
            GameObject notifyGo = new GameObject("LayerNotify");
            GameObject guideGo = new GameObject("LayerGuide");

            gameGo.SetParent(transform);
            uiGo.SetParent(transform);
            popupGo.SetParent(transform);
            dialogGo.SetParent(transform);
            notifyGo.SetParent(transform);
            guideGo.SetParent(transform);
            
            _layerGame = gameGo.AddComponent<LayerGame>();
            _layerUI = uiGo.AddComponent<LayerUI>();
            _layerPopUp = popupGo.AddComponent<LayerPopUp>();
            _layerDialog = dialogGo.AddComponent<LayerDialog>();
            _layerNotify = notifyGo.AddComponent<LayerNotify>();
            _layerGuide = guideGo.AddComponent<LayerGuide>();
            
            _layerGame.Init(100);
            _layerUI.Init(200);
            _layerPopUp.Init(300);
            _layerDialog.Init(400);
            _layerNotify.Init(500);
            _layerGuide.Init(600);
        }

        public void OnInit(object createParam)
        {
            
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            Destroy(gameObject);
        }
        
        public void ShowNotify(int uiId, string content)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                LogF8.LogView($"打开 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return;
            }
            _layerNotify.Show(uiId, config, content);
        }

        public List<string> GetCurrentUuids()
        {
            return _currentUuids;
        }

        public void Open(int uiId, object[] uiArgs = null, UICallbacks callbacks = null)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                LogF8.LogView($"打开 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return;
            }
            
            switch (config.Layer)
            {
                case LayerType.Game:
                    _layerGame.Add(uiId, config, uiArgs, callbacks);
                    break;
                case LayerType.UI:
                    _layerUI.Add(uiId, config, uiArgs, callbacks);
                    break;
                case LayerType.PopUp:
                    _layerPopUp.Add(uiId, config, uiArgs, callbacks);
                    break;
                case LayerType.Dialog:
                    _layerDialog.Add(uiId, config, uiArgs, callbacks);
                    break;
                case LayerType.Notify:
                    LogF8.LogView($"请使用ShowNotify 打开 ID 为 {uiId} 的 UI");
                    break;
                case LayerType.Guide:
                    _layerGuide.Add(uiId, config, uiArgs, callbacks);
                    break;
            }
        }

        public void OpenAsync(int uiId, object[] uiArgs = null, UICallbacks callbacks = null, Action openCallback = null)
        {
           
        }

        public bool Has(int uiId)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                LogF8.LogView($"检查 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return false;
            }

            bool result = false;

            switch (config.Layer)
            {
                case LayerType.Game:
                    result = _layerGame.Has(config.AssetName);
                    break;
                case LayerType.UI:
                    result = _layerUI.Has(config.AssetName);
                    break;
                case LayerType.PopUp:
                    result = _layerPopUp.Has(config.AssetName);
                    break;
                case LayerType.Dialog:
                    result = _layerDialog.Has(config.AssetName);
                    break;
                case LayerType.Notify:
                    result = _layerNotify.Has(config.AssetName);
                    break;
                case LayerType.Guide:
                    result = _layerGuide.Has(config.AssetName);
                    break;
            }

            return result;
        }

        public void Close(int uiId, bool isDestroy = false)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                LogF8.LogView($"移除 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return;
            }

            switch (config.Layer)
            {
                case LayerType.Game:
                    _layerGame.Close(config.AssetName, isDestroy);
                    break;
                case LayerType.UI:
                    _layerUI.Close(config.AssetName, isDestroy);
                    break;
                case LayerType.PopUp:
                    _layerPopUp.Close(config.AssetName, isDestroy);
                    break;
                case LayerType.Dialog:
                    _layerDialog.Close(uiId, config.AssetName, isDestroy);
                    break;
                case LayerType.Notify:
                    LogF8.LogView($"Notify 不能移除 ID 为 {uiId} 的 UI");
                    break;
                case LayerType.Guide:
                    _layerGuide.Close(config.AssetName, isDestroy);
                    break;
            }
            for (int i = _currentUuids.Count - 1; i >= 0; i--)
            {
                if (_currentUuids[i] == config.AssetName)
                {
                    _currentUuids.RemoveAt(i);
                    break; // 退出循环
                }
            }
        }

        public void Clear(bool isDestroy = true)
        {
            _layerGame.Clear(isDestroy);
            _layerUI.Clear(isDestroy);
            _layerPopUp.Clear(isDestroy);
            _layerDialog.Clear(isDestroy);
            _layerGuide.Clear(isDestroy);
            _currentUuids.Clear();
        }
    }
}
