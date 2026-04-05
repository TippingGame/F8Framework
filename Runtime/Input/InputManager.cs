using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace F8Framework.Core
{
    /// <summary>
    /// 输入管理器
    /// </summary>
    [UpdateRefresh]
    public sealed class InputManager : ModuleSingletonMono<InputManager>, IModule
    {
        private IInputHelper _helper;
        private bool _isFocusListenerRegistered;
        private bool _isTerminated;
        internal long FrameCount = 0;
#if ENABLE_INPUT_SYSTEM
        private InputSystemHelper CurrentInputSystemHelper => _helper as InputSystemHelper;

        #region Input System

        /// <summary>
        /// 是否已配置 InputActionAsset
        /// </summary>
        public bool HasInputActionAsset
        {
            get
            {
                return CurrentInputSystemHelper != null && CurrentInputSystemHelper.HasInputActionAsset;
            }
        }

        /// <summary>
        /// 设置 InputActionAsset 资源
        /// </summary>
        /// <param name="inputActionAsset">输入资源</param>
        public void SetInputActionAsset(InputActionAsset inputActionAsset)
        {
            CurrentInputSystemHelper?.SetInputActionAsset(inputActionAsset);
        }

        /// <summary>
        /// 是否存在 ActionMap
        /// </summary>
        /// <param name="mapName">ActionMap 名称</param>
        /// <returns>是否存在</returns>
        public bool HasActionMap(string mapName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.HasActionMap(mapName);
        }

        /// <summary>
        /// 指定玩家是否存在 ActionMap
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="mapName">ActionMap 名称</param>
        /// <returns>是否存在</returns>
        public bool HasActionMap(int playerIndex, string mapName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.HasActionMap(playerIndex, mapName);
        }

        /// <summary>
        /// 是否存在 Action
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <returns>是否存在</returns>
        public bool HasAction(string actionName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.HasAction(actionName);
        }

        /// <summary>
        /// 指定玩家是否存在 Action
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="actionName">Action 名称</param>
        /// <returns>是否存在</returns>
        public bool HasAction(int playerIndex, string actionName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.HasAction(playerIndex, actionName);
        }

        /// <summary>
        /// 是否存在 Control Scheme
        /// </summary>
        /// <param name="controlSchemeName">Control Scheme 名称</param>
        /// <returns>是否存在</returns>
        public bool HasControlScheme(string controlSchemeName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.HasControlScheme(controlSchemeName);
        }

        /// <summary>
        /// 获取全部 Control Scheme 名称
        /// </summary>
        /// <returns>Control Scheme 名称数组</returns>
        public string[] GetControlSchemes(int? playerIndex = null)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetControlSchemes(playerIndex) : Array.Empty<string>();
        }

        /// <summary>
        /// 查找 ActionMap
        /// </summary>
        /// <param name="mapName">ActionMap 名称</param>
        /// <returns>ActionMap</returns>
        public InputActionMap FindActionMap(string mapName)
        {
            return CurrentInputSystemHelper?.FindActionMap(mapName);
        }

        /// <summary>
        /// 查找指定玩家的 ActionMap
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="mapName">ActionMap 名称</param>
        /// <returns>ActionMap</returns>
        public InputActionMap FindActionMap(int playerIndex, string mapName)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.FindActionMap(playerIndex, mapName) : null;
        }

        /// <summary>
        /// 查找 Action
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <returns>Action</returns>
        public InputAction FindAction(string actionName)
        {
            return CurrentInputSystemHelper?.FindAction(actionName);
        }

        /// <summary>
        /// 查找指定玩家的 Action
        /// </summary>
        public InputAction FindAction(int playerIndex, string actionName)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.FindAction(playerIndex, actionName) : null;
        }

        /// <summary>
        /// 启用 ActionMap
        /// </summary>
        /// <param name="mapName">ActionMap 名称</param>
        public void EnableActionMap(string mapName)
        {
            CurrentInputSystemHelper?.EnableActionMap(mapName);
        }

        /// <summary>
        /// 启用指定玩家的 ActionMap
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="mapName">ActionMap 名称</param>
        public void EnableActionMap(int playerIndex, string mapName)
        {
            CurrentInputSystemHelper?.EnableActionMap(playerIndex, mapName);
        }

        /// <summary>
        /// 关闭 ActionMap
        /// </summary>
        /// <param name="mapName">ActionMap 名称</param>
        public void DisableActionMap(string mapName)
        {
            CurrentInputSystemHelper?.DisableActionMap(mapName);
        }

        /// <summary>
        /// 关闭指定玩家的 ActionMap
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="mapName">ActionMap 名称</param>
        public void DisableActionMap(int playerIndex, string mapName)
        {
            CurrentInputSystemHelper?.DisableActionMap(playerIndex, mapName);
        }

        /// <summary>
        /// ActionMap 是否启用
        /// </summary>
        /// <param name="mapName">ActionMap 名称</param>
        /// <returns>是否启用</returns>
        public bool IsActionMapEnabled(string mapName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.IsActionMapEnabled(mapName);
        }

        /// <summary>
        /// 指定玩家的 ActionMap 是否启用
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="mapName">ActionMap 名称</param>
        /// <returns>是否启用</returns>
        public bool IsActionMapEnabled(int playerIndex, string mapName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.IsActionMapEnabled(playerIndex, mapName);
        }

        /// <summary>
        /// 切换到指定 ActionMap
        /// </summary>
        /// <param name="mapName">ActionMap 名称</param>
        /// <param name="disableOthers">是否关闭其他 ActionMap</param>
        /// <returns>是否切换成功</returns>
        public bool SwitchActionMap(string mapName, bool disableOthers = true)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.SwitchActionMap(mapName, disableOthers);
        }

        /// <summary>
        /// 切换指定玩家到指定 ActionMap
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <param name="mapName">ActionMap 名称</param>
        /// <param name="disableOthers">是否关闭其他 ActionMap</param>
        /// <returns>是否切换成功</returns>
        public bool SwitchActionMap(int playerIndex, string mapName, bool disableOthers = true)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.SwitchActionMap(playerIndex, mapName, disableOthers);
        }

        /// <summary>
        /// 获取当前启用的 ActionMap 名称列表
        /// </summary>
        /// <returns>启用中的 ActionMap 名称数组</returns>
        public string[] GetEnabledActionMaps()
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetEnabledActionMaps() : Array.Empty<string>();
        }

        /// <summary>
        /// 获取指定玩家当前启用的 ActionMap 名称列表
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        /// <returns>启用中的 ActionMap 名称数组</returns>
        public string[] GetEnabledActionMaps(int playerIndex)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetEnabledActionMaps(playerIndex) : Array.Empty<string>();
        }

        /// <summary>
        /// 启用全部 ActionMap
        /// </summary>
        public void EnableAllActionMaps()
        {
            CurrentInputSystemHelper?.EnableAllActionMaps();
        }

        /// <summary>
        /// 启用指定玩家的全部 ActionMap
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        public void EnableAllActionMaps(int playerIndex)
        {
            CurrentInputSystemHelper?.EnableAllActionMaps(playerIndex);
        }

        /// <summary>
        /// 关闭全部 ActionMap
        /// </summary>
        public void DisableAllActionMaps()
        {
            CurrentInputSystemHelper?.DisableAllActionMaps();
        }

        /// <summary>
        /// 关闭指定玩家的全部 ActionMap
        /// </summary>
        /// <param name="playerIndex">玩家索引</param>
        public void DisableAllActionMaps(int playerIndex)
        {
            CurrentInputSystemHelper?.DisableAllActionMaps(playerIndex);
        }

        /// <summary>
        /// 当前 Control Scheme 名称
        /// </summary>
        public string CurrentControlScheme
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.CurrentControlScheme : string.Empty;
            }
        }

        /// <summary>
        /// 默认 Control Scheme 名称
        /// </summary>
        public string DefaultControlScheme
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.DefaultControlScheme : string.Empty;
            }
        }

        /// <summary>
        /// 切换 Control Scheme
        /// </summary>
        /// <param name="controlSchemeName">Control Scheme 名称</param>
        /// <param name="devices">绑定设备</param>
        /// <returns>是否切换成功</returns>
        public bool SwitchControlScheme(string controlSchemeName, params InputDevice[] devices)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.SwitchControlScheme(controlSchemeName, devices);
        }

        /// <summary>
        /// 切换指定玩家的 Control Scheme
        /// </summary>
        public bool SwitchControlScheme(int playerIndex, string controlSchemeName, params InputDevice[] devices)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.SwitchControlScheme(playerIndex, controlSchemeName, devices);
        }

        /// <summary>
        /// 清除 Control Scheme 的 BindingMask
        /// </summary>
        public void ClearControlSchemeBindingMask()
        {
            CurrentInputSystemHelper?.ClearControlSchemeBindingMask();
        }

        /// <summary>
        /// 清除指定玩家 Control Scheme 的 BindingMask
        /// </summary>
        public void ClearControlSchemeBindingMask(int playerIndex)
        {
            CurrentInputSystemHelper?.ClearControlSchemeBindingMask(playerIndex);
        }

        /// <summary>
        /// 添加 Control Scheme 变化回调
        /// </summary>
        /// <param name="changed">回调</param>
        public void AddControlSchemeChanged(Action<string> changed)
        {
            CurrentInputSystemHelper?.AddControlSchemeChanged(changed);
        }

        /// <summary>
        /// 移除 Control Scheme 变化回调
        /// </summary>
        /// <param name="changed">回调</param>
        public void RemoveControlSchemeChanged(Action<string> changed)
        {
            CurrentInputSystemHelper?.RemoveControlSchemeChanged(changed);
        }

        /// <summary>
        /// 读取 Action 值
        /// </summary>
        public T ReadValue<T>(string actionName, T defaultValue = default) where T : struct
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.ReadValue(actionName, defaultValue) : defaultValue;
        }

        /// <summary>
        /// 读取指定玩家的 Action 值
        /// </summary>
        public T ReadValue<T>(int playerIndex, string actionName, T defaultValue = default) where T : struct
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.ReadValue(playerIndex, actionName, defaultValue) : defaultValue;
        }

        /// <summary>
        /// Action 是否按住
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <returns>是否按住</returns>
        public bool IsPressed(string actionName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.IsPressed(actionName);
        }

        /// <summary>
        /// 指定玩家的 Action 是否按住
        /// </summary>
        public bool IsPressed(int playerIndex, string actionName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.IsPressed(playerIndex, actionName);
        }

        /// <summary>
        /// Action 是否在本帧按下
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <returns>是否按下</returns>
        public bool WasPressedThisFrame(string actionName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.WasPressedThisFrame(actionName);
        }

        /// <summary>
        /// 指定玩家的 Action 是否在本帧按下
        /// </summary>
        public bool WasPressedThisFrame(int playerIndex, string actionName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.WasPressedThisFrame(playerIndex, actionName);
        }

        /// <summary>
        /// Action 是否在本帧抬起
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <returns>是否抬起</returns>
        public bool WasReleasedThisFrame(string actionName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.WasReleasedThisFrame(actionName);
        }

        /// <summary>
        /// 指定玩家的 Action 是否在本帧抬起
        /// </summary>
        public bool WasReleasedThisFrame(int playerIndex, string actionName)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.WasReleasedThisFrame(playerIndex, actionName);
        }

        /// <summary>
        /// 获取 Action 的全部 Binding
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <returns>Binding 数组</returns>
        public InputBinding[] GetBindings(string actionName)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetBindings(actionName) : Array.Empty<InputBinding>();
        }

        /// <summary>
        /// 获取 Action 的 Binding 数量
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <returns>Binding 数量</returns>
        public int GetBindingCount(string actionName)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetBindingCount(actionName) : 0;
        }

        /// <summary>
        /// 通过 Binding 标识查找索引
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <param name="bindingIdOrPath">Binding Id、Path、Name 或显示文本</param>
        /// <returns>Binding 索引，未找到返回 -1</returns>
        public int FindBindingIndex(string actionName, string bindingIdOrPath)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.FindBindingIndex(actionName, bindingIdOrPath) : -1;
        }

        /// <summary>
        /// 获取 Binding 显示文本
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <param name="bindingIndex">Binding 索引</param>
        /// <returns>显示文本</returns>
        public string GetBindingDisplayString(string actionName, int bindingIndex)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetBindingDisplayString(actionName, bindingIndex) : string.Empty;
        }

        /// <summary>
        /// 获取 Binding 原始路径
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <param name="bindingIndex">Binding 索引</param>
        /// <returns>Binding 路径</returns>
        public string GetBindingPath(string actionName, int bindingIndex)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetBindingPath(actionName, bindingIndex) : string.Empty;
        }

        /// <summary>
        /// 是否正在进行重绑定
        /// </summary>
        public bool IsRebinding
        {
            get
            {
                return CurrentInputSystemHelper != null && CurrentInputSystemHelper.IsRebinding;
            }
        }

        /// <summary>
        /// 开始重绑定
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <param name="bindingIndex">Binding 索引，-1 时自动选择</param>
        /// <param name="completed">完成回调，参数为 Action 名称和新的显示文本</param>
        /// <param name="canceled">取消回调，参数为 Action 名称</param>
        /// <returns>是否成功开始</returns>
        public bool StartRebind(string actionName, int bindingIndex = -1, Action<string, string> completed = null, Action<string> canceled = null)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.StartRebind(actionName, bindingIndex, completed, canceled);
        }

        /// <summary>
        /// 取消当前重绑定
        /// </summary>
        public void CancelRebind()
        {
            CurrentInputSystemHelper?.CancelRebind();
        }

        /// <summary>
        /// 移除指定 Binding 的 Override
        /// </summary>
        /// <param name="actionName">Action 名称</param>
        /// <param name="bindingIndex">Binding 索引</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveBindingOverride(string actionName, int bindingIndex)
        {
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.RemoveBindingOverride(actionName, bindingIndex);
        }

        /// <summary>
        /// 移除全部 Binding Override
        /// </summary>
        public void RemoveAllBindingOverrides()
        {
            CurrentInputSystemHelper?.RemoveAllBindingOverrides();
        }

        /// <summary>
        /// 导出 Binding Override Json
        /// </summary>
        /// <returns>Json 字符串</returns>
        public string SaveBindingOverridesAsJson()
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.SaveBindingOverridesAsJson() : string.Empty;
        }

        /// <summary>
        /// 加载 Binding Override Json
        /// </summary>
        /// <param name="overridesJson">Json 字符串</param>
        /// <param name="removeExisting">是否先移除现有 Override</param>
        public void LoadBindingOverridesFromJson(string overridesJson, bool removeExisting = true)
        {
            CurrentInputSystemHelper?.LoadBindingOverridesFromJson(overridesJson, removeExisting);
        }

        /// <summary>
        /// 当前 PlayerInput 的 ActionMap 名称
        /// </summary>
        public string CurrentActionMapName
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.CurrentActionMapName : string.Empty;
            }
        }

        /// <summary>
        /// 获取当前或指定玩家的 Control Scheme 名称
        /// </summary>
        public string GetControlScheme(int? playerIndex = null)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetControlScheme(playerIndex) : string.Empty;
        }

        /// <summary>
        /// 获取当前或指定玩家默认 Control Scheme 名称
        /// </summary>
        public string GetDefaultControlScheme(int? playerIndex = null)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetDefaultControlScheme(playerIndex) : string.Empty;
        }

        /// <summary>
        /// 获取当前或指定玩家当前 ActionMap 名称
        /// </summary>
        public string GetCurrentActionMapName(int? playerIndex = null)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetCurrentActionMapName(playerIndex) : string.Empty;
        }

        /// <summary>
        /// 添加 PlayerInput 控制变化回调
        /// </summary>
        public void AddPlayerControlsChanged(Action<PlayerInput> changed)
        {
            CurrentInputSystemHelper?.AddPlayerControlsChanged(changed);
        }

        /// <summary>
        /// 移除 PlayerInput 控制变化回调
        /// </summary>
        public void RemovePlayerControlsChanged(Action<PlayerInput> changed)
        {
            CurrentInputSystemHelper?.RemovePlayerControlsChanged(changed);
        }

        /// <summary>
        /// 当前 PlayerInputManager
        /// </summary>
        public PlayerInputManager CurrentPlayerInputManager
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.CurrentPlayerInputManager : null;
            }
        }

        /// <summary>
        /// 是否已绑定 PlayerInputManager
        /// </summary>
        public bool HasPlayerInputManager
        {
            get
            {
                return CurrentInputSystemHelper != null && CurrentInputSystemHelper.HasPlayerInputManager;
            }
        }

        /// <summary>
        /// 设置 PlayerInputManager
        /// </summary>
        public void SetPlayerInputManager(PlayerInputManagerConfig config)
        {
            var helper = CurrentInputSystemHelper;
            if (helper == null)
            {
                LogF8.LogError("请先调用 UseInputSystem 初始化 InputActionAsset，再配置 PlayerInputManager。");
                return;
            }

            if (config != null)
            {
                helper.SetRuntimeHost(GetOrCreateInputSystemHostObject());
            }

            helper.SetPlayerInputManager(config);
        }

        /// <summary>
        /// 是否允许加入玩家
        /// </summary>
        public bool JoiningEnabled
        {
            get
            {
                return CurrentInputSystemHelper != null && CurrentInputSystemHelper.JoiningEnabled;
            }
        }

        /// <summary>
        /// 当前玩家数量
        /// </summary>
        public int PlayerCount
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.PlayerCount : 0;
            }
        }

        /// <summary>
        /// 获取全部玩家
        /// </summary>
        public PlayerInput[] GetAllPlayers()
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetAllPlayers() : Array.Empty<PlayerInput>();
        }

        /// <summary>
        /// 按索引获取玩家
        /// </summary>
        public PlayerInput GetPlayer(int playerIndex)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetPlayer(playerIndex) : null;
        }

        /// <summary>
        /// 获取当前或指定玩家设备
        /// </summary>
        public InputDevice[] GetDevices(int? playerIndex = null)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.GetDevices(playerIndex) : Array.Empty<InputDevice>();
        }

        /// <summary>
        /// 加入玩家
        /// </summary>
        public PlayerInput JoinPlayer(int playerIndex = -1, int splitScreenIndex = -1, string controlScheme = null, params InputDevice[] pairWithDevices)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.JoinPlayer(playerIndex, splitScreenIndex, controlScheme, pairWithDevices) : null;
        }

        /// <summary>
        /// 启用加入玩家
        /// </summary>
        public void EnableJoining()
        {
            CurrentInputSystemHelper?.EnableJoining();
        }

        /// <summary>
        /// 禁用加入玩家
        /// </summary>
        public void DisableJoining()
        {
            CurrentInputSystemHelper?.DisableJoining();
        }

        /// <summary>
        /// 添加玩家加入回调
        /// </summary>
        public void AddPlayerJoined(Action<PlayerInput> joined)
        {
            CurrentInputSystemHelper?.AddPlayerJoined(joined);
        }

        /// <summary>
        /// 移除玩家加入回调
        /// </summary>
        public void RemovePlayerJoined(Action<PlayerInput> joined)
        {
            CurrentInputSystemHelper?.RemovePlayerJoined(joined);
        }

        /// <summary>
        /// 添加玩家离开回调
        /// </summary>
        public void AddPlayerLeft(Action<PlayerInput> left)
        {
            CurrentInputSystemHelper?.AddPlayerLeft(left);
        }

        /// <summary>
        /// 移除玩家离开回调
        /// </summary>
        public void RemovePlayerLeft(Action<PlayerInput> left)
        {
            CurrentInputSystemHelper?.RemovePlayerLeft(left);
        }

        /// <summary>
        /// 查找指定玩家的 Action
        /// </summary>
        public InputAction FindPlayerAction(int playerIndex, string actionName)
        {
            return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.FindPlayerAction(playerIndex, actionName) : null;
        }

        /// <summary>
        /// 最近一次实际输入所使用的设备
        /// </summary>
        public InputDevice LastUsedDevice
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.LastUsedDevice : null;
            }
        }

        /// <summary>
        /// 最近一次实际输入设备名称
        /// </summary>
        public string LastUsedDeviceName
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.LastUsedDeviceName : string.Empty;
            }
        }

        /// <summary>
        /// 最近一次发生设备变化的设备
        /// </summary>
        public InputDevice LastDeviceChanged
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.LastDeviceChanged : null;
            }
        }

        /// <summary>
        /// 最近一次设备变化类型
        /// </summary>
        public InputDeviceChange LastDeviceChange
        {
            get
            {
                return CurrentInputSystemHelper != null ? CurrentInputSystemHelper.LastDeviceChange : default;
            }
        }

        /// <summary>
        /// 添加最近使用设备变化回调
        /// </summary>
        /// <param name="changed">回调</param>
        public void AddLastUsedDeviceChanged(Action<InputDevice> changed)
        {
            CurrentInputSystemHelper?.AddLastUsedDeviceChanged(changed);
        }

        /// <summary>
        /// 移除最近使用设备变化回调
        /// </summary>
        /// <param name="changed">回调</param>
        public void RemoveLastUsedDeviceChanged(Action<InputDevice> changed)
        {
            CurrentInputSystemHelper?.RemoveLastUsedDeviceChanged(changed);
        }

        /// <summary>
        /// 添加设备变化回调
        /// </summary>
        /// <param name="changed">回调</param>
        public void AddDeviceChanged(Action<InputDevice, InputDeviceChange> changed)
        {
            CurrentInputSystemHelper?.AddDeviceChanged(changed);
        }

        /// <summary>
        /// 移除设备变化回调
        /// </summary>
        /// <param name="changed">回调</param>
        public void RemoveDeviceChanged(Action<InputDevice, InputDeviceChange> changed)
        {
            CurrentInputSystemHelper?.RemoveDeviceChanged(changed);
        }

        /// <summary>
        /// 使用新版 Input System Helper
        /// </summary>
        public void UseInputSystem(InputSystemHelper helper, InputActionAsset inputActionAsset)
        {
            if (helper == null)
            {
                LogF8.LogError("InputSystemHelper 为空，无法初始化 InputManager。");
                return;
            }

            if (inputActionAsset == null)
            {
                LogF8.LogError("InputActionAsset 为空，无法初始化 InputSystemHelper。");
                return;
            }

            helper.SetInputActionAsset(inputActionAsset);
            InitializeHelper(helper, false);
        }

        #endregion
#endif

        /// <summary>
        /// 是否启用输入设备
        /// </summary>
        public bool IsEnableInputDevice
        {
            get
            {
                return _helper != null && _helper.IsEnableInputDevice;
            }
            set
            {
                if (_helper != null)
                {
                    _helper.IsEnableInputDevice = value;
                }
            }
        }
        
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition
        {
            get
            {
                return _helper != null ? _helper.MousePosition : Vector3.zero;
            }
        }
        
        /// <summary>
        /// 任意键按住
        /// </summary>
        public bool AnyKey
        {
            get
            {
                return _helper != null && _helper.AnyKey;
            }
        }
        
        /// <summary>
        /// 任意键按下
        /// </summary>
        public bool AnyKeyDown
        {
            get
            {
                return _helper != null && _helper.AnyKeyDown;
            }
        }

        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        internal bool IsExistVirtualAxis(string name)
        {
            return _helper.IsExistVirtualAxis(name);
        }
        
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        internal bool IsExistVirtualButton(string name)
        {
            return _helper.IsExistVirtualButton(name);
        }
        
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void RegisterVirtualAxis(string name)
        {
            _helper.RegisterVirtualAxis(name);
        }
        
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void RegisterVirtualButton(string name)
        {
            _helper.RegisterVirtualButton(name);
        }
        
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void UnRegisterVirtualAxis(string name)
        {
            _helper?.UnRegisterVirtualAxis(name);
        }
        
        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void UnRegisterVirtualButton(string name)
        {
            _helper?.UnRegisterVirtualButton(name);
        }

        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        internal void SetVirtualMousePosition(float x, float y, float z)
        {
            _helper.SetVirtualMousePosition(new Vector3(x, y, z));
        }
        
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        internal void SetVirtualMousePosition(Vector3 value)
        {
            _helper.SetVirtualMousePosition(value);
        }
        
        /// <summary>
        /// 开始按按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void SetButtonStart(string name)
        {
            _helper.SetButtonStart(name);
        }
        
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void SetButtonDown(string name)
        {
            _helper.SetButtonDown(name);
        }
        
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        internal void SetButtonUp(string name)
        {
            _helper.SetButtonUp(name);
        }
        
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void SetAxisPositive(string name)
        {
            _helper.SetAxisPositive(name);
        }
        
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void SetAxisNegative(string name)
        {
            _helper.SetAxisNegative(name);
        }
        
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        internal void SetAxisZero(string name)
        {
            _helper.SetAxisZero(name);
        }
        
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        internal void SetAxis(string name, float value)
        {
            _helper.SetAxis(name, value);
        }

        /// <summary>
        /// 开始按按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="started">回调</param>
        public void AddButtonStarted(string name, Action<string> started)
        {
            _helper.AddButtonStarted(name, started);
        }

        /// <summary>
        /// 指定玩家开始按按钮Action
        /// </summary>
        public void AddButtonStarted(int playerIndex, string name, Action<string> started)
        {
#if ENABLE_INPUT_SYSTEM
            CurrentInputSystemHelper?.AddButtonStarted(playerIndex, name, started);
#endif
        }

        /// <summary>
        /// 按下按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="performed">回调</param>
        public void AddButtonPerformed(string name, Action<string> performed)
        {
            _helper.AddButtonPerformed(name, performed);
        }

        /// <summary>
        /// 指定玩家按下按钮Action
        /// </summary>
        public void AddButtonPerformed(int playerIndex, string name, Action<string> performed)
        {
#if ENABLE_INPUT_SYSTEM
            CurrentInputSystemHelper?.AddButtonPerformed(playerIndex, name, performed);
#endif
        }
        
        /// <summary>
        /// 开始按按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="canceled">回调</param>
        public void AddButtonCanceled(string name, Action<string> canceled)
        {
            _helper.AddButtonCanceled(name, canceled);
        }

        /// <summary>
        /// 指定玩家按钮取消Action
        /// </summary>
        public void AddButtonCanceled(int playerIndex, string name, Action<string> canceled)
        {
#if ENABLE_INPUT_SYSTEM
            CurrentInputSystemHelper?.AddButtonCanceled(playerIndex, name, canceled);
#endif
        }
        
        /// <summary>
        /// 值改变Action
        /// </summary>
        public void AddValueChanged<T>(string name, Action<T> valueChanged) where T : struct
        {
#if ENABLE_INPUT_SYSTEM
            if (CurrentInputSystemHelper != null)
            {
                CurrentInputSystemHelper.AddValueChanged(name, valueChanged);
                return;
            }
#endif

            if (typeof(T) == typeof(float) && valueChanged is Action<float> floatChanged)
            {
                _helper.AddAxisValueChanged(name, floatChanged);
            }
        }

        /// <summary>
        /// Axis值改变Action
        /// </summary>
        public void AddAxisValueChanged(string name, Action<float> valueChanged)
        {
            AddValueChanged(name, valueChanged);
        }

        /// <summary>
        /// 指定玩家值改变Action
        /// </summary>
        public void AddValueChanged<T>(int playerIndex, string name, Action<T> valueChanged) where T : struct
        {
#if ENABLE_INPUT_SYSTEM
            CurrentInputSystemHelper?.AddValueChanged(playerIndex, name, valueChanged);
#endif
        }
        
        /// <summary>
        /// 移除开始按按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="started">回调</param>
        public void RemoveButtonStarted(string name, Action<string> started)
        {
            _helper.RemoveButtonStarted(name, started);
        }

        /// <summary>
        /// 移除指定玩家开始按按钮Action
        /// </summary>
        public void RemoveButtonStarted(int playerIndex, string name, Action<string> started)
        {
#if ENABLE_INPUT_SYSTEM
            CurrentInputSystemHelper?.RemoveButtonStarted(playerIndex, name, started);
#endif
        }

        /// <summary>
        /// 移除按下按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="performed">回调</param>
        public void RemoveButtonPerformed(string name, Action<string> performed)
        {
            _helper.RemoveButtonPerformed(name, performed);
        }

        /// <summary>
        /// 移除指定玩家按下按钮Action
        /// </summary>
        public void RemoveButtonPerformed(int playerIndex, string name, Action<string> performed)
        {
#if ENABLE_INPUT_SYSTEM
            CurrentInputSystemHelper?.RemoveButtonPerformed(playerIndex, name, performed);
#endif
        }
        
        /// <summary>
        /// 移除开始按按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="canceled">回调</param>
        public void RemoveButtonCanceled(string name, Action<string> canceled)
        {
            _helper.RemoveButtonCanceled(name, canceled);
        }

        /// <summary>
        /// 移除指定玩家按钮取消Action
        /// </summary>
        public void RemoveButtonCanceled(int playerIndex, string name, Action<string> canceled)
        {
#if ENABLE_INPUT_SYSTEM
            CurrentInputSystemHelper?.RemoveButtonCanceled(playerIndex, name, canceled);
#endif
        }
        
        /// <summary>
        /// 移除值改变Action
        /// </summary>
        public void RemoveValueChanged<T>(string name, Action<T> valueChanged) where T : struct
        {
#if ENABLE_INPUT_SYSTEM
            if (CurrentInputSystemHelper != null)
            {
                CurrentInputSystemHelper.RemoveValueChanged(name, valueChanged);
                return;
            }
#endif

            if (typeof(T) == typeof(float) && valueChanged is Action<float> floatChanged)
            {
                _helper.RemoveAxisValueChanged(name, floatChanged);
            }
        }

        /// <summary>
        /// 移除Axis值改变Action
        /// </summary>
        public void RemoveAxisValueChanged(string name, Action<float> valueChanged)
        {
            RemoveValueChanged(name, valueChanged);
        }

        /// <summary>
        /// 移除指定玩家值改变Action
        /// </summary>
        public void RemoveValueChanged<T>(int playerIndex, string name, Action<T> valueChanged) where T : struct
        {
#if ENABLE_INPUT_SYSTEM
            CurrentInputSystemHelper?.RemoveValueChanged(playerIndex, name, valueChanged);
#endif
        }
        
        /// <summary>
        /// 清除所有输入事件
        /// </summary>
        public void ClearAllAction()
        {
            _helper.ClearAllAction();
        }
        
        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按住</returns>
        public bool GetButton(string name)
        {
            return _helper.GetButton(name);
        }

        /// <summary>
        /// 指定玩家按钮按住
        /// </summary>
        public bool GetButton(int playerIndex, string name)
        {
#if ENABLE_INPUT_SYSTEM
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.GetButton(playerIndex, name);
#else
            return false;
#endif
        }
        
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name)
        {
            return _helper.GetButtonDown(name);
        }

        /// <summary>
        /// 指定玩家按钮按下
        /// </summary>
        public bool GetButtonDown(int playerIndex, string name)
        {
#if ENABLE_INPUT_SYSTEM
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.GetButtonDown(playerIndex, name);
#else
            return false;
#endif
        }
        
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        public bool GetButtonUp(string name)
        {
            return _helper.GetButtonUp(name);
        }

        /// <summary>
        /// 指定玩家按钮抬起
        /// </summary>
        public bool GetButtonUp(int playerIndex, string name)
        {
#if ENABLE_INPUT_SYSTEM
            return CurrentInputSystemHelper != null && CurrentInputSystemHelper.GetButtonUp(playerIndex, name);
#else
            return false;
#endif
        }
        
        /// <summary>
        /// 获取轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public float GetAxis(string name)
        {
            return _helper.GetAxis(name, false);
        }
        
        /// <summary>
        /// 获取轴线值（值为-1，0，1）
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public float GetAxisRaw(string name)
        {
            return _helper.GetAxis(name, true);
        }

        /// <summary>
        /// 键盘按键按住
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode)
        {
            return _helper.GetKey(keyCode);
        }
        
        /// <summary>
        /// 键盘按键按下
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode)
        {
            return _helper.GetKeyDown(keyCode);
        }
        
        /// <summary>
        /// 键盘按键抬起
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode)
        {
            return _helper.GetKeyUp(keyCode);
        }
        
        /// <summary>
        /// 键盘组合按键按住（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode1, KeyCode keyCode2)
        {
            return _helper.GetKey(keyCode1, keyCode2);
        }
        
        /// <summary>
        /// 键盘组合按键按下（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2)
        {
            return _helper.GetKeyDown(keyCode1, keyCode2);
        }
        
        /// <summary>
        /// 键盘组合按键抬起（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2)
        {
            return _helper.GetKeyUp(keyCode1, keyCode2);
        }
        
        /// <summary>
        /// 键盘组合按键按住（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return _helper.GetKey(keyCode1, keyCode2, keyCode3);
        }
        
        /// <summary>
        /// 键盘组合按键按下（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return _helper.GetKeyDown(keyCode1, keyCode2, keyCode3);
        }
        
        /// <summary>
        /// 键盘组合按键抬起（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return _helper.GetKeyUp(keyCode1, keyCode2, keyCode3);
        }

        public void OnInit(object createParam)
        {
            if (createParam is IInputHelper helper)
            {
#if ENABLE_INPUT_SYSTEM
                InitializeHelper(helper, !(helper is InputSystemHelper));
#else
                InitializeHelper(helper, true);
#endif
            }
            else
            {
                LogF8.LogError("初始化Input模块参数有误！");
            }
        }

        /// <summary>
        /// 设置输入设备类型
        /// </summary>
        public void SwitchDevice(InputDeviceBase deviceType)
        {
            if (_helper == null || deviceType == null)
            {
                return;
            }

            var previousDevice = _helper.Device;
            _helper.ResetAll();

            if (_helper is DefaultInputHelper defaultInputHelper)
            {
                defaultInputHelper.BeginDeviceSwitch();
            }

            try
            {
                previousDevice?.OnShutdown();
                _helper.LoadDevice(deviceType);
                _helper.OnInit();
            }
            finally
            {
                if (_helper is DefaultInputHelper switchableDefaultHelper)
                {
                    switchableDefaultHelper.EndDeviceSwitch();
                }
            }
        }
        
        public void ResetAll()
        {
            _helper?.ResetAll();
        }
        
        public void OnUpdate()
        {
            if (_helper == null)
            {
                return;
            }

            if (!_isFocusListenerRegistered)
            {
                RegisterFocusListeners();
            }

            _helper.OnUpdate();
            FrameCount++;
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            CleanupTerminationState();
            Destroy(gameObject);
        }

        private void OnApplicationQuit()
        {
            CleanupTerminationState();
        }

        private void OnDestroy()
        {
            CleanupTerminationState();
        }

        private void InitializeHelper(IInputHelper helper, bool loadDefaultDevice)
        {
            var previousHelper = _helper;
            previousHelper?.OnTerminate();

#if ENABLE_INPUT_SYSTEM
            if (previousHelper is InputSystemHelper previousInputSystemHelper)
            {
                previousInputSystemHelper.SetRuntimeHost(null);
            }
#endif

            _helper = helper;

#if ENABLE_INPUT_SYSTEM
            if (_helper is InputSystemHelper inputSystemHelper)
            {
                inputSystemHelper.SetRuntimeHost(GetOrCreateInputSystemHostObject());
            }
#endif

            if (loadDefaultDevice)
            {
                _helper.LoadDevice(new StandardInputDevice());
            }

            _helper.OnInit();
            RegisterFocusListeners();
        }

        private GameObject GetOrCreateInputSystemHostObject()
        {
            return gameObject;
        }

        private void RegisterFocusListeners()
        {
            if (_isFocusListenerRegistered)
            {
                return;
            }

            var messageManager = MessageManager.Instance;
            if (messageManager == null)
            {
                return;
            }

            messageManager.AddEventListener(MessageEvent.ApplicationFocus, ResetAll, this);
            messageManager.AddEventListener(MessageEvent.NotApplicationFocus, ResetAll, this);
            _isFocusListenerRegistered = true;
        }

        private void UnregisterFocusListeners()
        {
            if (!_isFocusListenerRegistered)
            {
                return;
            }

            MessageManager.Instance?.RemoveEventListener(MessageEvent.ApplicationFocus, ResetAll, this);
            MessageManager.Instance?.RemoveEventListener(MessageEvent.NotApplicationFocus, ResetAll, this);
            _isFocusListenerRegistered = false;
        }

        private void CleanupTerminationState()
        {
            if (_isTerminated)
            {
                return;
            }

            _isTerminated = true;
            UnregisterFocusListeners();

            var helper = _helper;
            _helper = null;

            if (helper != null)
            {
                helper.OnTerminate();
                helper.ClearAllAction();
                helper.ResetAll();

#if ENABLE_INPUT_SYSTEM
                if (helper is InputSystemHelper inputSystemHelper)
                {
                    inputSystemHelper.SetInputActionAsset(null);
                    inputSystemHelper.SetRuntimeHost(null);
                }
#endif
            }

            FrameCount = 0;
        }
    }
}
