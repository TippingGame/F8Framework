#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

namespace F8Framework.Core
{
    /// <summary>
    /// Input System 进阶能力层。
    /// 负责 Control Scheme、PlayerInput/PlayerInputManager、多玩家输入路由与设备切换事件；
    /// 基础 Action/Binding 查询与读值能力保留在 InputSystemHelper.cs。
    /// </summary>
    public sealed partial class InputSystemHelper
    {
        private const string InternalPlayerPrefabName = "[F8]LocalPlayerTemplate";

        private string _currentControlScheme = string.Empty;
        private Action<string> _controlSchemeChanged;
        private Action<PlayerInput> _playerControlsChanged;
        private Action<PlayerInput> _playerJoined;
        private Action<PlayerInput> _playerLeft;
        private readonly List<PlayerInput> _managedPlayers = new List<PlayerInput>();
        private readonly Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>> _playerActionStartedCallbacks =
            new Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>>();
        private readonly Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>> _playerActionPerformedCallbacks =
            new Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>>();
        private readonly Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>> _playerActionCanceledCallbacks =
            new Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>>();
        private readonly Dictionary<int, Dictionary<string, Dictionary<Type, Dictionary<Delegate, Action<InputAction.CallbackContext>>>>> _playerActionValueCallbacks =
            new Dictionary<int, Dictionary<string, Dictionary<Type, Dictionary<Delegate, Action<InputAction.CallbackContext>>>>>();
        private GameObject _runtimeHost;
        private GameObject _internalPlayerPrefab;
        private PlayerInputManagerConfig _playerInputManagerConfig;

        public string CurrentControlScheme
        {
            get
            {
                var globalPlayer = _globalPlayerInput;
                if (globalPlayer != null)
                {
                    return globalPlayer.currentControlScheme ?? string.Empty;
                }

                return _currentControlScheme ?? string.Empty;
            }
        }

        public string DefaultControlScheme
        {
            get
            {
                var globalPlayer = _globalPlayerInput;
                if (globalPlayer != null)
                {
                    return globalPlayer.defaultControlScheme ?? string.Empty;
                }

                return string.Empty;
            }
        }

        public string CurrentActionMapName
        {
            get
            {
                var globalPlayer = _globalPlayerInput;
                if (globalPlayer != null)
                {
                    return globalPlayer.currentActionMap != null ? globalPlayer.currentActionMap.name : string.Empty;
                }

                return GetFirstEnabledActionMapName();
            }
        }

        public PlayerInputManager CurrentPlayerInputManager { get; private set; }

        public bool HasPlayerInputManager
        {
            get
            {
                return CurrentPlayerInputManager != null;
            }
        }

        public bool JoiningEnabled
        {
            get
            {
                return CurrentPlayerInputManager != null && CurrentPlayerInputManager.joiningEnabled;
            }
        }


        public int PlayerCount
        {
            get
            {
                return _managedPlayers.Count;
            }
        }
        public bool HasControlScheme(string controlSchemeName)
        {
            if (string.IsNullOrEmpty(controlSchemeName) || InputActionAsset == null)
            {
                return false;
            }

            foreach (var controlScheme in InputActionAsset.controlSchemes)
            {
                if (string.Equals(controlScheme.name, controlSchemeName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public string[] GetControlSchemes(int? playerIndex = null)
        {
            var actionAsset = ResolvePlayerActionAsset(playerIndex);
            if (actionAsset == null || actionAsset.controlSchemes.Count == 0)
            {
                return Array.Empty<string>();
            }

            var results = new string[actionAsset.controlSchemes.Count];
            for (var i = 0; i < actionAsset.controlSchemes.Count; i++)
            {
                results[i] = actionAsset.controlSchemes[i].name;
            }

            return results;
        }

        public bool SwitchControlScheme(string controlSchemeName, params InputDevice[] devices)
        {
            if (!HasControlScheme(controlSchemeName))
            {
                return false;
            }

            var globalPlayer = _globalPlayerInput;
            if (globalPlayer != null)
            {
                try
                {
                    var resolvedDevices = devices != null && devices.Length > 0 ? devices : CopyDevices(globalPlayer.devices);
                    globalPlayer.SwitchCurrentControlScheme(controlSchemeName, resolvedDevices ?? Array.Empty<InputDevice>());
                    PairDevicesToGlobalPlayerInput(globalPlayer, resolvedDevices, controlSchemeName);

                    _currentControlScheme = globalPlayer.currentControlScheme ?? controlSchemeName;
                    NotifyControlSchemeChanged(_currentControlScheme);
                    return true;
                }
                catch (Exception exception)
                {
                    LogF8.LogError($"切换 Control Scheme 失败: {controlSchemeName}，{exception.Message}");
                    return false;
                }
            }

            if (InputActionAsset == null)
            {
                return false;
            }

            InputActionAsset.bindingMask = InputBinding.MaskByGroup(controlSchemeName);
            _currentControlScheme = controlSchemeName;
            NotifyControlSchemeChanged(_currentControlScheme);
            return true;
        }

        public void ClearControlSchemeBindingMask()
        {
            if (_globalPlayerInput != null)
            {
                PairAllDevicesToGlobalPlayerInput(_globalPlayerInput);
            }
            else if (InputActionAsset != null)
            {
                InputActionAsset.bindingMask = null;
            }

            _currentControlScheme = _globalPlayerInput != null ? _globalPlayerInput.currentControlScheme ?? string.Empty : string.Empty;
            NotifyControlSchemeChanged(CurrentControlScheme);
        }

        public void AddControlSchemeChanged(Action<string> changed)
        {
            _controlSchemeChanged += changed;
        }

        public void RemoveControlSchemeChanged(Action<string> changed)
        {
            _controlSchemeChanged -= changed;
        }

        public void AddPlayerControlsChanged(Action<PlayerInput> changed)
        {
            _playerControlsChanged += changed;
        }

        public void RemovePlayerControlsChanged(Action<PlayerInput> changed)
        {
            _playerControlsChanged -= changed;
        }

        public void SetPlayerInputManager(PlayerInputManagerConfig config)
        {
            _playerInputManagerConfig = config;

            if (config == null)
            {
                DisablePlayerInputManager();
                return;
            }

            if (_configuredInputActionAsset == null)
            {
                LogF8.LogError("请先设置 InputActionAsset，再配置 PlayerInputManager。");
                return;
            }

            if (_runtimeHost == null)
            {
                LogF8.LogError("InputSystemHelper 未配置宿主物体，无法创建 PlayerInputManager。");
                return;
            }

            var manager = _runtimeHost.GetComponent<PlayerInputManager>();
            if (manager == null)
            {
                manager = _runtimeHost.AddComponent<PlayerInputManager>();
            }

            if (!ReferenceEquals(CurrentPlayerInputManager, manager))
            {
                UnregisterPlayerInputManagerCallbacks();
                CurrentPlayerInputManager = manager;
                RegisterPlayerInputManagerCallbacks();
            }

            ConfigurePlayerInputManager(manager, config);
        }

        public PlayerInput[] GetAllPlayers()
        {
            return _managedPlayers.Count == 0 ? Array.Empty<PlayerInput>() : _managedPlayers.ToArray();
        }

        public PlayerInput GetPlayer(int playerIndex)
        {
            if (playerIndex < 0)
            {
                return null;
            }

            for (var i = 0; i < _managedPlayers.Count; i++)
            {
                var player = _managedPlayers[i];
                if (player != null && player.playerIndex == playerIndex)
                {
                    return player;
                }
            }

            return null;
        }

        public InputDevice[] GetDevices(int? playerIndex = null)
        {
            var player = ResolvePlayerInput(playerIndex);
            return player != null ? CopyDevices(player.devices) : Array.Empty<InputDevice>();
        }

        public string GetControlScheme(int? playerIndex = null)
        {
            var player = ResolvePlayerInput(playerIndex);
            return player != null ? player.currentControlScheme ?? string.Empty : CurrentControlScheme;
        }

        public string GetDefaultControlScheme(int? playerIndex = null)
        {
            var player = ResolvePlayerInput(playerIndex);
            return player != null ? player.defaultControlScheme ?? string.Empty : DefaultControlScheme;
        }

        public string GetCurrentActionMapName(int? playerIndex = null)
        {
            var player = ResolvePlayerInput(playerIndex);
            return player != null && player.currentActionMap != null ? player.currentActionMap.name : CurrentActionMapName;
        }

        public PlayerInput JoinPlayer(int playerIndex = -1, int splitScreenIndex = -1, string controlScheme = null, params InputDevice[] pairWithDevices)
        {
            if (CurrentPlayerInputManager == null)
            {
                return null;
            }

            var player = pairWithDevices != null && pairWithDevices.Length > 0
                ? CurrentPlayerInputManager.JoinPlayer(playerIndex, splitScreenIndex, controlScheme, pairWithDevices)
                : CurrentPlayerInputManager.JoinPlayer(playerIndex, splitScreenIndex, controlScheme, (InputDevice)null);

            RegisterManagedPlayer(player);
            return player;
        }

        private PlayerInput ResolvePlayerInput(int? playerIndex)
        {
            if (playerIndex.HasValue)
            {
                return GetPlayer(playerIndex.Value);
            }

            return _globalPlayerInput;
        }

        private InputActionAsset ResolvePlayerActionAsset(int? playerIndex)
        {
            var player = ResolvePlayerInput(playerIndex);
            return player != null && player.actions != null ? player.actions : InputActionAsset;
        }


        public void EnableJoining()
        {
            CurrentPlayerInputManager?.EnableJoining();
        }

        public void DisableJoining()
        {
            CurrentPlayerInputManager?.DisableJoining();
        }

        public void AddPlayerJoined(Action<PlayerInput> joined)
        {
            _playerJoined += joined;
        }

        public void RemovePlayerJoined(Action<PlayerInput> joined)
        {
            _playerJoined -= joined;
        }

        public void AddPlayerLeft(Action<PlayerInput> left)
        {
            _playerLeft += left;
        }

        public void RemovePlayerLeft(Action<PlayerInput> left)
        {
            _playerLeft -= left;
        }

        public InputAction FindAction(int playerIndex, string actionName)
        {
            return FindPlayerAction(playerIndex, actionName);
        }

        public InputActionMap FindActionMap(int playerIndex, string mapName)
        {
            var player = GetPlayer(playerIndex);
            return player?.actions?.FindActionMap(mapName, false);
        }

        public bool HasActionMap(int playerIndex, string mapName)
        {
            return FindActionMap(playerIndex, mapName) != null;
        }

        public bool HasAction(int playerIndex, string actionName)
        {
            return FindPlayerAction(playerIndex, actionName) != null;
        }

        public InputAction FindPlayerAction(int playerIndex, string actionName)
        {
            var player = GetPlayer(playerIndex);
            return TryGetAction(player?.actions, actionName, out var action) ? action : null;
        }

        public void EnableActionMap(int playerIndex, string mapName)
        {
            FindActionMap(playerIndex, mapName)?.Enable();
        }

        public void DisableActionMap(int playerIndex, string mapName)
        {
            FindActionMap(playerIndex, mapName)?.Disable();
        }

        public bool IsActionMapEnabled(int playerIndex, string mapName)
        {
            return FindActionMap(playerIndex, mapName)?.enabled ?? false;
        }

        public bool SwitchActionMap(int playerIndex, string mapName, bool disableOthers = true)
        {
            var player = GetPlayer(playerIndex);
            if (player == null || player.actions == null)
            {
                return false;
            }

            var targetMap = player.actions.FindActionMap(mapName, false);
            if (targetMap == null)
            {
                return false;
            }

            if (disableOthers)
            {
                player.SwitchCurrentActionMap(mapName);
                return true;
            }

            targetMap.Enable();
            return true;
        }

        public string[] GetEnabledActionMaps(int playerIndex)
        {
            var player = GetPlayer(playerIndex);
            var actionAsset = player?.actions;
            if (actionAsset == null)
            {
                return Array.Empty<string>();
            }

            var actionMaps = actionAsset.actionMaps;
            var enabledMaps = new List<string>(actionMaps.Count);
            foreach (var map in actionMaps)
            {
                if (map.enabled)
                {
                    enabledMaps.Add(map.name);
                }
            }

            return enabledMaps.ToArray();
        }

        public void EnableAllActionMaps(int playerIndex)
        {
            GetPlayer(playerIndex)?.actions?.Enable();
        }

        public void DisableAllActionMaps(int playerIndex)
        {
            GetPlayer(playerIndex)?.actions?.Disable();
        }

        public void AddButtonStarted(int playerIndex, string actionName, Action<string> started)
        {
            AddPlayerActionStringCallback(_playerActionStartedCallbacks, playerIndex, actionName, started, (action, callback) => action.started += callback);
        }

        public void RemoveButtonStarted(int playerIndex, string actionName, Action<string> started)
        {
            RemovePlayerActionStringCallback(_playerActionStartedCallbacks, playerIndex, actionName, started, (action, callback) => action.started -= callback);
        }

        public void AddButtonPerformed(int playerIndex, string actionName, Action<string> performed)
        {
            AddPlayerActionStringCallback(_playerActionPerformedCallbacks, playerIndex, actionName, performed, (action, callback) => action.performed += callback);
        }

        public void RemoveButtonPerformed(int playerIndex, string actionName, Action<string> performed)
        {
            RemovePlayerActionStringCallback(_playerActionPerformedCallbacks, playerIndex, actionName, performed, (action, callback) => action.performed -= callback);
        }

        public void AddButtonCanceled(int playerIndex, string actionName, Action<string> canceled)
        {
            AddPlayerActionStringCallback(_playerActionCanceledCallbacks, playerIndex, actionName, canceled, (action, callback) => action.canceled += callback);
        }

        public void RemoveButtonCanceled(int playerIndex, string actionName, Action<string> canceled)
        {
            RemovePlayerActionStringCallback(_playerActionCanceledCallbacks, playerIndex, actionName, canceled, (action, callback) => action.canceled -= callback);
        }

        public void AddValueChanged<T>(int playerIndex, string actionName, Action<T> valueChanged) where T : struct
        {
            AddPlayerActionValueCallback(playerIndex, actionName, valueChanged);
        }

        public void RemoveValueChanged<T>(int playerIndex, string actionName, Action<T> valueChanged) where T : struct
        {
            RemovePlayerActionValueCallback(playerIndex, actionName, valueChanged);
        }

        public T ReadValue<T>(int playerIndex, string actionName, T defaultValue = default) where T : struct
        {
            if (!IsEnableInputDevice)
            {
                return defaultValue;
            }

            return TryReadValueFromAction(FindPlayerAction(playerIndex, actionName), out T value) ? value : defaultValue;
        }

        public bool IsPressed(int playerIndex, string actionName)
        {
            return IsEnableInputDevice && FindPlayerAction(playerIndex, actionName)?.IsPressed() == true;
        }

        public bool WasPressedThisFrame(int playerIndex, string actionName)
        {
            return IsEnableInputDevice && FindPlayerAction(playerIndex, actionName)?.WasPressedThisFrame() == true;
        }

        public bool WasReleasedThisFrame(int playerIndex, string actionName)
        {
            return IsEnableInputDevice && FindPlayerAction(playerIndex, actionName)?.WasReleasedThisFrame() == true;
        }

        public bool GetButton(int playerIndex, string actionName)
        {
            return IsPressed(playerIndex, actionName);
        }

        public bool GetButtonDown(int playerIndex, string actionName)
        {
            return WasPressedThisFrame(playerIndex, actionName);
        }

        public bool GetButtonUp(int playerIndex, string actionName)
        {
            return WasReleasedThisFrame(playerIndex, actionName);
        }

        public bool SwitchControlScheme(int playerIndex, string controlSchemeName, params InputDevice[] devices)
        {
            if (!HasControlScheme(controlSchemeName))
            {
                return false;
            }

            var player = GetPlayer(playerIndex);
            if (player == null)
            {
                LogF8.LogWarning($"切换玩家 Control Scheme 失败: 未找到 playerIndex={playerIndex} 的 PlayerInput。请使用 JoinPlayer 返回的 player.playerIndex。");
                return false;
            }

            try
            {
                var resolvedDevices = devices != null && devices.Length > 0 ? devices : CopyDevices(player.devices);
                player.SwitchCurrentControlScheme(controlSchemeName, resolvedDevices ?? Array.Empty<InputDevice>());
                if (player.user.valid)
                {
                    player.user.UnpairDevices();
                    player.user.AssociateActionsWithUser(player.actions);

                    if (resolvedDevices != null)
                    {
                        for (var i = 0; i < resolvedDevices.Length; i++)
                        {
                            var device = resolvedDevices[i];
                            if (device == null || !device.added)
                            {
                                continue;
                            }

                            InputUser.PerformPairingWithDevice(device, player.user);
                        }
                    }
                }

                if (player.actions != null)
                {
                    player.actions.bindingMask = InputBinding.MaskByGroup(controlSchemeName);
                }

                return true;
            }
            catch (Exception exception)
            {
                LogF8.LogError($"切换玩家 Control Scheme 失败: playerIndex={playerIndex}，controlScheme={controlSchemeName}，{exception.Message}");
                return false;
            }
        }

        public void ClearControlSchemeBindingMask(int playerIndex)
        {
            var player = GetPlayer(playerIndex);
            if (player == null || player.actions == null)
            {
                return;
            }

            player.actions.bindingMask = null;
        }

        internal void SetRuntimeHost(GameObject runtimeHost)
        {
            _runtimeHost = runtimeHost;

            if (_globalPlayerObject != null)
            {
                _globalPlayerObject.transform.SetParent(runtimeHost != null ? runtimeHost.transform : null, false);
            }

            if (_internalPlayerPrefab != null)
            {
                _internalPlayerPrefab.transform.SetParent(runtimeHost != null ? runtimeHost.transform : null, false);
            }

            if (_runtimeHost != null && _configuredInputActionAsset != null && _globalPlayerInput == null)
            {
                var previousAsset = InputActionAsset;
                if (previousAsset != null)
                {
                    UnregisterActionCallbacksIfNeeded(previousAsset);
                }

                RefreshGlobalPlayerInput(CurrentActionMapName, GetEnabledActionMaps());

                if (InputActionAsset != null)
                {
                    RegisterActionCallbacksIfNeeded(InputActionAsset);
                }
            }
        }

        private void AddPlayerActionStringCallback(Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>> callbackSource, int playerIndex, string actionName, Action<string> callback, Action<InputAction, Action<InputAction.CallbackContext>> binder)
        {
            if (playerIndex < 0 || string.IsNullOrEmpty(actionName) || callback == null)
            {
                return;
            }

            var actionMap = GetOrCreatePlayerStringActionMap(callbackSource, playerIndex);
            if (!actionMap.TryGetValue(actionName, out var callbackMap))
            {
                callbackMap = new Dictionary<Action<string>, Action<InputAction.CallbackContext>>();
                actionMap.Add(actionName, callbackMap);
            }

            if (callbackMap.ContainsKey(callback))
            {
                return;
            }

            Action<InputAction.CallbackContext> wrappedCallback = _ =>
            {
                if (!IsEnableInputDevice)
                {
                    return;
                }

                callback.Invoke(actionName);
            };
            callbackMap.Add(callback, wrappedCallback);

            if (TryGetPlayerAction(playerIndex, actionName, out var action))
            {
                binder(action, wrappedCallback);
            }
        }

        private void RemovePlayerActionStringCallback(Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>> callbackSource, int playerIndex, string actionName, Action<string> callback, Action<InputAction, Action<InputAction.CallbackContext>> unbinder)
        {
            if (playerIndex < 0 || string.IsNullOrEmpty(actionName) || callback == null || !callbackSource.TryGetValue(playerIndex, out var actionMap) || !actionMap.TryGetValue(actionName, out var callbackMap) || !callbackMap.TryGetValue(callback, out var wrappedCallback))
            {
                return;
            }

            if (TryGetPlayerAction(playerIndex, actionName, out var action))
            {
                unbinder(action, wrappedCallback);
            }

            callbackMap.Remove(callback);
            if (callbackMap.Count == 0)
            {
                actionMap.Remove(actionName);
            }

            if (actionMap.Count == 0)
            {
                callbackSource.Remove(playerIndex);
            }
        }

        private void AddPlayerActionValueCallback<T>(int playerIndex, string actionName, Action<T> valueChanged) where T : struct
        {
            if (playerIndex < 0 || string.IsNullOrEmpty(actionName) || valueChanged == null)
            {
                return;
            }

            var actionMap = GetOrCreatePlayerValueActionMap(playerIndex);
            if (!actionMap.TryGetValue(actionName, out var typeMap))
            {
                typeMap = new Dictionary<Type, Dictionary<Delegate, Action<InputAction.CallbackContext>>>();
                actionMap.Add(actionName, typeMap);
            }

            var valueType = typeof(T);
            if (!typeMap.TryGetValue(valueType, out var callbackMap))
            {
                callbackMap = new Dictionary<Delegate, Action<InputAction.CallbackContext>>();
                typeMap.Add(valueType, callbackMap);
            }

            if (callbackMap.ContainsKey(valueChanged))
            {
                return;
            }

            Action<InputAction.CallbackContext> wrappedCallback = context =>
            {
                if (!IsEnableInputDevice)
                {
                    return;
                }

                if (TryReadValueFromContext(context, out T value))
                {
                    valueChanged.Invoke(value);
                }
            };

            callbackMap.Add(valueChanged, wrappedCallback);
            if (TryGetPlayerAction(playerIndex, actionName, out var action))
            {
                action.performed += wrappedCallback;
                action.canceled += wrappedCallback;
            }
        }

        private void RemovePlayerActionValueCallback<T>(int playerIndex, string actionName, Action<T> valueChanged) where T : struct
        {
            if (playerIndex < 0 || string.IsNullOrEmpty(actionName) || valueChanged == null || !_playerActionValueCallbacks.TryGetValue(playerIndex, out var actionMap) || !actionMap.TryGetValue(actionName, out var typeMap))
            {
                return;
            }

            var valueType = typeof(T);
            if (!typeMap.TryGetValue(valueType, out var callbackMap) || !callbackMap.TryGetValue(valueChanged, out var wrappedCallback))
            {
                return;
            }

            if (TryGetPlayerAction(playerIndex, actionName, out var action))
            {
                action.performed -= wrappedCallback;
                action.canceled -= wrappedCallback;
            }

            callbackMap.Remove(valueChanged);
            if (callbackMap.Count == 0)
            {
                typeMap.Remove(valueType);
            }

            if (typeMap.Count == 0)
            {
                actionMap.Remove(actionName);
            }

            if (actionMap.Count == 0)
            {
                _playerActionValueCallbacks.Remove(playerIndex);
            }
        }

        private void ConfigurePlayerInput(PlayerInput playerInput, InputActionAsset sourceAsset, PlayerNotifications notificationBehavior, bool activateInput, bool enableAllActionMaps)
        {
            if (playerInput == null)
            {
                return;
            }

            playerInput.notificationBehavior = notificationBehavior;
            playerInput.actions = sourceAsset;
            playerInput.defaultActionMap = GetDefaultActionMapName(sourceAsset);

            if (sourceAsset == null)
            {
                playerInput.DeactivateInput();
                return;
            }

            if (activateInput)
            {
                playerInput.ActivateInput();
            }
            else
            {
                playerInput.DeactivateInput();
            }

            if (enableAllActionMaps)
            {
                playerInput.actions?.Enable();
            }
            else
            {
                playerInput.actions?.Disable();
            }
        }

        private void ConfigurePlayerInputManager(PlayerInputManager manager, PlayerInputManagerConfig config)
        {
            if (manager == null || config == null)
            {
                return;
            }

            manager.enabled = true;
            manager.playerPrefab = config.PlayerPrefab != null ? config.PlayerPrefab : GetOrCreateInternalPlayerPrefab(config);
            manager.joinBehavior = config.JoinBehavior;
            manager.notificationBehavior = config.NotificationBehavior;
            manager.splitScreen = config.SplitScreen;
            manager.joinAction = config.JoinAction != null ? new InputActionProperty(config.JoinAction) : default;
            SetPlayerInputManagerField(manager, "m_MaxPlayerCount", config.MaxPlayerCount);
            SetPlayerInputManagerField(manager, "m_MaintainAspectRatioInSplitScreen", config.MaintainAspectRatioInSplitScreen);
            SetPlayerInputManagerField(manager, "m_FixedNumberOfSplitScreens", config.FixedNumberOfSplitScreens);
        }

        private void RefreshInputActionAssetConsumers()
        {
            RefreshInternalPlayerPrefab();
            RefreshConfiguredPlayerPrefab();

            if (CurrentPlayerInputManager != null && _playerInputManagerConfig != null)
            {
                ConfigurePlayerInputManager(CurrentPlayerInputManager, _playerInputManagerConfig);
            }

            RefreshManagedPlayersForCurrentAsset();
            SyncCurrentControlScheme();
        }

        private void DisablePlayerInputManager()
        {
            DestroyManagedPlayers();
            UnregisterPlayerInputManagerCallbacks();

            if (CurrentPlayerInputManager != null)
            {
                CurrentPlayerInputManager.DisableJoining();
                CurrentPlayerInputManager.enabled = false;
            }

            CurrentPlayerInputManager = null;
        }

        private void DestroyManagedPlayers()
        {
            if (_managedPlayers.Count == 0)
            {
                return;
            }

            var players = _managedPlayers.ToArray();
            ClearManagedPlayers();

            for (var i = 0; i < players.Length; i++)
            {
                var player = players[i];
                if (player == null)
                {
                    continue;
                }

                player.DeactivateInput();
                if (player.user.valid)
                {
                    player.user.UnpairDevices();
                }

                DestroyHelperObject(player.gameObject);
            }
        }

        private void RefreshConfiguredPlayerPrefab()
        {
            var playerInput = GetConfiguredPlayerInputTemplate();
            if (playerInput == null)
            {
                return;
            }

            ConfigurePlayerInput(
                playerInput,
                _configuredInputActionAsset,
                _playerInputManagerConfig != null ? _playerInputManagerConfig.NotificationBehavior : playerInput.notificationBehavior,
                false,
                false);
        }

        private GameObject GetOrCreateInternalPlayerPrefab(PlayerInputManagerConfig config)
        {
            if (_internalPlayerPrefab == null)
            {
                _internalPlayerPrefab = new GameObject(InternalPlayerPrefabName);
                _internalPlayerPrefab.transform.SetParent(_runtimeHost != null ? _runtimeHost.transform : null, false);
                _internalPlayerPrefab.SetActive(false);
            }

            var playerInput = _internalPlayerPrefab.GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                playerInput = _internalPlayerPrefab.AddComponent<PlayerInput>();
            }

            ConfigurePlayerInput(playerInput, _configuredInputActionAsset, config != null ? config.NotificationBehavior : PlayerNotifications.InvokeCSharpEvents, false, false);
            return _internalPlayerPrefab;
        }

        private void RefreshInternalPlayerPrefab()
        {
            if (_internalPlayerPrefab == null)
            {
                return;
            }

            var playerInput = _internalPlayerPrefab.GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                playerInput = _internalPlayerPrefab.AddComponent<PlayerInput>();
            }

            ConfigurePlayerInput(playerInput, _configuredInputActionAsset, _playerInputManagerConfig != null ? _playerInputManagerConfig.NotificationBehavior : PlayerNotifications.InvokeCSharpEvents, false, false);
        }

        private void RefreshManagedPlayersForCurrentAsset()
        {
            for (var i = 0; i < _managedPlayers.Count; i++)
            {
                var player = _managedPlayers[i];
                if (player == null)
                {
                    continue;
                }

                var wasInputActive = player.inputIsActive;
                var currentActionMapName = player.currentActionMap != null ? player.currentActionMap.name : string.Empty;
                var notificationBehavior = player.notificationBehavior;

                UnregisterPlayerScopedCallbacks(player);
                ConfigurePlayerInput(player, _configuredInputActionAsset, notificationBehavior, wasInputActive, false);

                if (_configuredInputActionAsset != null)
                {
                    var targetMapName = currentActionMapName;
                    if (string.IsNullOrEmpty(targetMapName) || player.actions == null || player.actions.FindActionMap(targetMapName, false) == null)
                    {
                        targetMapName = GetDefaultActionMapName(_configuredInputActionAsset);
                    }

                    if (!string.IsNullOrEmpty(targetMapName))
                    {
                        player.SwitchCurrentActionMap(targetMapName);
                    }
                }

                RegisterPlayerScopedCallbacks(player);
                ShareDevicesWithGlobalPlayerInput(player);
            }
        }

        private PlayerInput GetConfiguredPlayerInputTemplate()
        {
            var prefab = _playerInputManagerConfig != null ? _playerInputManagerConfig.PlayerPrefab : null;
            if (prefab == null)
            {
                prefab = _internalPlayerPrefab;
            }

            return prefab != null ? prefab.GetComponent<PlayerInput>() : null;
        }


        private static InputDevice[] CopyDevices(ReadOnlyArray<InputDevice> devices)
        {
            if (devices.Count == 0)
            {
                return Array.Empty<InputDevice>();
            }

            var results = new InputDevice[devices.Count];
            for (var i = 0; i < devices.Count; i++)
            {
                results[i] = devices[i];
            }

            return results;
        }

        private string GetFirstEnabledActionMapName()
        {
            return GetFirstEnabledActionMapName(InputActionAsset);
        }

        private static string GetFirstEnabledActionMapName(InputActionAsset actionAsset)
        {
            if (actionAsset == null)
            {
                return string.Empty;
            }

            foreach (var map in actionAsset.actionMaps)
            {
                if (map.enabled)
                {
                    return map.name;
                }
            }

            return string.Empty;
        }

        private static string GetDefaultActionMapName(InputActionAsset sourceAsset)
        {
            return sourceAsset != null && sourceAsset.actionMaps.Count > 0 ? sourceAsset.actionMaps[0].name : string.Empty;
        }

        private void SyncCurrentControlScheme()
        {
            var previousControlScheme = _currentControlScheme;
            if (_globalPlayerInput != null)
            {
                _currentControlScheme = _globalPlayerInput.currentControlScheme ?? string.Empty;
            }
            else if (!string.IsNullOrEmpty(_currentControlScheme) && !HasControlScheme(_currentControlScheme))
            {
                _currentControlScheme = string.Empty;
            }

            if (!string.Equals(previousControlScheme, _currentControlScheme, StringComparison.Ordinal))
            {
                NotifyControlSchemeChanged(_currentControlScheme);
            }
        }

        private void RegisterPlayerInputManagerCallbacks()
        {
            if (CurrentPlayerInputManager == null)
            {
                return;
            }

            CurrentPlayerInputManager.onPlayerJoined += OnPlayerJoinedInternal;
            CurrentPlayerInputManager.onPlayerLeft += OnPlayerLeftInternal;
        }

        private void UnregisterPlayerInputManagerCallbacks()
        {
            if (CurrentPlayerInputManager == null)
            {
                return;
            }

            CurrentPlayerInputManager.onPlayerJoined -= OnPlayerJoinedInternal;
            CurrentPlayerInputManager.onPlayerLeft -= OnPlayerLeftInternal;
        }

        private void RegisterManagedPlayer(PlayerInput playerInput)
        {
            if (playerInput == null)
            {
                return;
            }

            var existingPlayer = GetPlayer(playerInput.playerIndex);
            if (existingPlayer != null && !ReferenceEquals(existingPlayer, playerInput))
            {
                UnregisterManagedPlayer(existingPlayer);
            }

            if (_managedPlayers.Contains(playerInput))
            {
                return;
            }

            playerInput.onControlsChanged += OnPlayerInputControlsChanged;
            _managedPlayers.Add(playerInput);
            RegisterPlayerScopedCallbacks(playerInput);
        }

        private void UnregisterManagedPlayer(PlayerInput playerInput)
        {
            if (playerInput == null)
            {
                return;
            }

            UnregisterPlayerScopedCallbacks(playerInput);
            playerInput.onControlsChanged -= OnPlayerInputControlsChanged;
            _managedPlayers.Remove(playerInput);
        }

        private void ClearManagedPlayers()
        {
            for (var i = 0; i < _managedPlayers.Count; i++)
            {
                var player = _managedPlayers[i];
                if (player != null)
                {
                    UnregisterPlayerScopedCallbacks(player);
                    player.onControlsChanged -= OnPlayerInputControlsChanged;
                }
            }

            _managedPlayers.Clear();
        }

        private void OnPlayerInputControlsChanged(PlayerInput playerInput)
        {
            _playerControlsChanged?.Invoke(playerInput);
        }

        private void OnGlobalPlayerInputControlsChanged(PlayerInput playerInput)
        {
            var currentControlScheme = playerInput != null ? playerInput.currentControlScheme ?? string.Empty : string.Empty;
            if (string.Equals(_currentControlScheme, currentControlScheme, StringComparison.Ordinal))
            {
                return;
            }

            _currentControlScheme = currentControlScheme;
            NotifyControlSchemeChanged(_currentControlScheme);
        }

        private void OnPlayerJoinedInternal(PlayerInput playerInput)
        {
            if (playerInput != null && _runtimeHost != null && !ReferenceEquals(playerInput.transform.parent, _runtimeHost.transform))
            {
                playerInput.transform.SetParent(_runtimeHost.transform, true);
            }

            RegisterManagedPlayer(playerInput);
            ShareDevicesWithGlobalPlayerInput(playerInput);
            _playerJoined?.Invoke(playerInput);
        }

        private void OnPlayerLeftInternal(PlayerInput playerInput)
        {
            UnregisterManagedPlayer(playerInput);
            _playerLeft?.Invoke(playerInput);
        }

        private void NotifyControlSchemeChanged(string controlSchemeName)
        {
            _controlSchemeChanged?.Invoke(controlSchemeName ?? string.Empty);
        }

        private bool TryGetPlayerAction(int playerIndex, string actionName, out InputAction action)
        {
            action = FindPlayerAction(playerIndex, actionName);
            return action != null;
        }

        private Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>> GetOrCreatePlayerStringActionMap(Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>> callbackSource, int playerIndex)
        {
            if (!callbackSource.TryGetValue(playerIndex, out var actionMap))
            {
                actionMap = new Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>(StringComparer.Ordinal);
                callbackSource.Add(playerIndex, actionMap);
            }

            return actionMap;
        }

        private Dictionary<string, Dictionary<Type, Dictionary<Delegate, Action<InputAction.CallbackContext>>>> GetOrCreatePlayerValueActionMap(int playerIndex)
        {
            if (!_playerActionValueCallbacks.TryGetValue(playerIndex, out var actionMap))
            {
                actionMap = new Dictionary<string, Dictionary<Type, Dictionary<Delegate, Action<InputAction.CallbackContext>>>>(StringComparer.Ordinal);
                _playerActionValueCallbacks.Add(playerIndex, actionMap);
            }

            return actionMap;
        }

        private void RegisterPlayerScopedCallbacks(PlayerInput playerInput)
        {
            if (playerInput == null || playerInput.actions == null)
            {
                return;
            }

            RegisterPlayerStringCallbackMap(playerInput, _playerActionStartedCallbacks, (action, callback) => action.started += callback);
            RegisterPlayerStringCallbackMap(playerInput, _playerActionPerformedCallbacks, (action, callback) => action.performed += callback);
            RegisterPlayerStringCallbackMap(playerInput, _playerActionCanceledCallbacks, (action, callback) => action.canceled += callback);
            RegisterPlayerValueCallbackMap(playerInput);
        }

        private void UnregisterPlayerScopedCallbacks(PlayerInput playerInput)
        {
            if (playerInput == null || playerInput.actions == null)
            {
                return;
            }

            RegisterPlayerStringCallbackMap(playerInput, _playerActionStartedCallbacks, (action, callback) => action.started -= callback);
            RegisterPlayerStringCallbackMap(playerInput, _playerActionPerformedCallbacks, (action, callback) => action.performed -= callback);
            RegisterPlayerStringCallbackMap(playerInput, _playerActionCanceledCallbacks, (action, callback) => action.canceled -= callback);
            UnregisterPlayerValueCallbackMap(playerInput);
        }

        private void RegisterPlayerStringCallbackMap(PlayerInput playerInput, Dictionary<int, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>> callbackSource, Action<InputAction, Action<InputAction.CallbackContext>> binder)
        {
            if (playerInput == null || !callbackSource.TryGetValue(playerInput.playerIndex, out var actionMap))
            {
                return;
            }

            foreach (var pair in actionMap)
            {
                if (!TryGetAction(playerInput.actions, pair.Key, out var action))
                {
                    continue;
                }

                foreach (var callback in pair.Value.Values)
                {
                    binder(action, callback);
                }
            }
        }

        private void RegisterPlayerValueCallbackMap(PlayerInput playerInput)
        {
            if (playerInput == null || !_playerActionValueCallbacks.TryGetValue(playerInput.playerIndex, out var actionMap))
            {
                return;
            }

            foreach (var actionPair in actionMap)
            {
                if (!TryGetAction(playerInput.actions, actionPair.Key, out var action))
                {
                    continue;
                }

                foreach (var typePair in actionPair.Value)
                {
                    foreach (var callback in typePair.Value.Values)
                    {
                        action.performed += callback;
                        action.canceled += callback;
                    }
                }
            }
        }

        private void UnregisterPlayerValueCallbackMap(PlayerInput playerInput)
        {
            if (playerInput == null || !_playerActionValueCallbacks.TryGetValue(playerInput.playerIndex, out var actionMap))
            {
                return;
            }

            foreach (var actionPair in actionMap)
            {
                if (!TryGetAction(playerInput.actions, actionPair.Key, out var action))
                {
                    continue;
                }

                foreach (var typePair in actionPair.Value)
                {
                    foreach (var callback in typePair.Value.Values)
                    {
                        action.performed -= callback;
                        action.canceled -= callback;
                    }
                }
            }
        }

        private static void SetPlayerInputManagerField(PlayerInputManager manager, string fieldName, object value)
        {
            if (manager == null || string.IsNullOrEmpty(fieldName))
            {
                return;
            }

            var field = typeof(PlayerInputManager).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                return;
            }

            field.SetValue(manager, value);
        }
    }
}
#endif