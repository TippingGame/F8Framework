#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace F8Framework.Core
{
    /// <summary>
    /// 仅适配新版 Input System 的输入助手
    /// </summary>
    public sealed partial class InputSystemHelper : IInputHelper
    {
        private const string GlobalPlayerInputName = "[F8]GlobalPlayerInput";
        private readonly Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>> _actionStartedCallbacks = new Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>> _actionPerformedCallbacks = new Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>> _actionCanceledCallbacks = new Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>>(StringComparer.Ordinal);
        private readonly Dictionary<string, Dictionary<Type, Dictionary<Delegate, Action<InputAction.CallbackContext>>>> _actionValueCallbacks =
            new Dictionary<string, Dictionary<Type, Dictionary<Delegate, Action<InputAction.CallbackContext>>>>(StringComparer.Ordinal);
        private readonly Dictionary<KeyCode, Key> _keyCodeMappings = new Dictionary<KeyCode, Key>
        {
            { KeyCode.Alpha0, Key.Digit0 },
            { KeyCode.Alpha1, Key.Digit1 },
            { KeyCode.Alpha2, Key.Digit2 },
            { KeyCode.Alpha3, Key.Digit3 },
            { KeyCode.Alpha4, Key.Digit4 },
            { KeyCode.Alpha5, Key.Digit5 },
            { KeyCode.Alpha6, Key.Digit6 },
            { KeyCode.Alpha7, Key.Digit7 },
            { KeyCode.Alpha8, Key.Digit8 },
            { KeyCode.Alpha9, Key.Digit9 },
            { KeyCode.Return, Key.Enter },
            { KeyCode.KeypadEnter, Key.NumpadEnter },
            { KeyCode.LeftControl, Key.LeftCtrl },
            { KeyCode.RightControl, Key.RightCtrl },
            { KeyCode.LeftCommand, Key.LeftMeta },
            { KeyCode.RightCommand, Key.RightMeta },
            { KeyCode.BackQuote, Key.Backquote },
            { KeyCode.Quote, Key.Quote }
        };
        private bool _isEnableInputDevice = true;
        private bool _isInitialized;
        private bool _areActionCallbacksRegistered;
        private bool _areInputSystemCallbacksRegistered;
        private InputActionAsset _configuredInputActionAsset;
        private GameObject _globalPlayerObject;
        private PlayerInput _globalPlayerInput;
        private readonly HashSet<int> _globalScopedDeviceIds = new HashSet<int>();
        private bool _isGlobalControlSchemeScoped;
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;
        private Action<InputDevice> _lastUsedDeviceChanged;
        private Action<InputDevice, InputDeviceChange> _deviceChanged;

        public InputDeviceBase Device { get; private set; }

        public InputActionAsset InputActionAsset
        {
            get
            {
                var globalActions = _globalPlayerInput != null ? _globalPlayerInput.actions : null;
                return globalActions != null ? globalActions : _configuredInputActionAsset;
            }
        }

        public InputDevice LastUsedDevice { get; private set; }

        public string LastUsedDeviceName
        {
            get
            {
                if (LastUsedDevice == null)
                {
                    return string.Empty;
                }

                return !string.IsNullOrEmpty(LastUsedDevice.displayName) ? LastUsedDevice.displayName : LastUsedDevice.name;
            }
        }

        public InputDevice LastDeviceChanged { get; private set; }

        public InputDeviceChange LastDeviceChange { get; private set; }

        public bool IsEnableInputDevice
        {
            get
            {
                return _isEnableInputDevice;
            }
            set
            {
                if (_isEnableInputDevice == value)
                {
                    return;
                }

                _isEnableInputDevice = value;
                ResetAll();
            }
        }

        public Vector3 MousePosition { get; private set; }

        public bool AnyKey
        {
            get
            {
                return IsEnableInputDevice && IsAnyButtonPressed(false);
            }
        }

        public bool AnyKeyDown
        {
            get
            {
                return IsEnableInputDevice && IsAnyButtonPressed(true);
            }
        }

        public bool HasInputActionAsset
        {
            get
            {
                return InputActionAsset != null;
            }
        }

        public bool IsRebinding
        {
            get
            {
                return _rebindOperation != null;
            }
        }

        public void SetInputActionAsset(InputActionAsset inputActionAsset)
        {
            if (ReferenceEquals(_configuredInputActionAsset, inputActionAsset) && (inputActionAsset == null || InputActionAsset != null))
            {
                return;
            }

            CancelRebind();

            var previousCurrentActionMapName = CurrentActionMapName;
            var previousEnabledActionMaps = GetEnabledActionMaps();
            var previousControlScheme = CurrentControlScheme;

            var oldAsset = InputActionAsset;
            if (oldAsset != null)
            {
                UnregisterActionCallbacksIfNeeded(oldAsset);
                oldAsset.Disable();
            }

            _configuredInputActionAsset = inputActionAsset;
            _currentControlScheme = previousControlScheme ?? string.Empty;
            RefreshGlobalPlayerInput(previousCurrentActionMapName, previousEnabledActionMaps);

            if (InputActionAsset != null)
            {
                RegisterActionCallbacksIfNeeded(InputActionAsset);
            }

            RefreshInputActionAssetConsumers();
        }

        public bool HasActionMap(string mapName)
        {
            return FindActionMap(mapName) != null;
        }

        public bool HasAction(string actionName)
        {
            return FindAction(actionName) != null;
        }

        public InputActionMap FindActionMap(string mapName)
        {
            var actionAsset = InputActionAsset;
            if (string.IsNullOrEmpty(mapName) || actionAsset == null)
            {
                return null;
            }

            return actionAsset.FindActionMap(mapName, false);
        }

        public InputActionMap FindActionMap(string mapName, bool throwIfNotFound)
        {
            var actionMap = FindActionMap(mapName);
            if (actionMap == null && throwIfNotFound)
            {
                throw new ArgumentException($"ActionMap 不存在: {mapName}", nameof(mapName));
            }

            return actionMap;
        }

        public InputAction FindAction(string actionName)
        {
            var actionAsset = InputActionAsset;
            if (string.IsNullOrEmpty(actionName) || actionAsset == null)
            {
                return null;
            }

            return actionAsset.FindAction(actionName, false);
        }

        public InputAction FindAction(string actionName, bool throwIfNotFound)
        {
            var action = FindAction(actionName);
            if (action == null && throwIfNotFound)
            {
                throw new ArgumentException($"Action 不存在: {actionName}", nameof(actionName));
            }

            return action;
        }

        public void Enable()
        {
            EnableAllActionMaps();
        }

        public void Disable()
        {
            DisableAllActionMaps();
        }

        public void Enable(string mapName)
        {
            EnableActionMap(mapName);
        }

        public void Disable(string mapName)
        {
            DisableActionMap(mapName);
        }

        public void EnableActionMap(string mapName)
        {
            FindActionMap(mapName)?.Enable();
        }

        public void DisableActionMap(string mapName)
        {
            FindActionMap(mapName)?.Disable();
        }

        public bool IsActionMapEnabled(string mapName)
        {
            return FindActionMap(mapName)?.enabled ?? false;
        }

        public bool SwitchActionMap(string mapName, bool disableOthers = true)
        {
            var targetMap = FindActionMap(mapName);
            if (targetMap == null)
            {
                return false;
            }

            if (disableOthers && InputActionAsset != null)
            {
                foreach (var map in InputActionAsset.actionMaps)
                {
                    if (!ReferenceEquals(map, targetMap))
                    {
                        map.Disable();
                    }
                }
            }

            targetMap.Enable();
            return true;
        }

        public string[] GetEnabledActionMaps()
        {
            if (InputActionAsset == null)
            {
                return Array.Empty<string>();
            }

            var actionMaps = InputActionAsset.actionMaps;
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

        public void EnableAllActionMaps()
        {
            InputActionAsset?.Enable();
        }

        public void DisableAllActionMaps()
        {
            InputActionAsset?.Disable();
        }

        public T ReadValue<T>(string actionName, T defaultValue = default) where T : struct
        {
            if (!IsEnableInputDevice)
            {
                return defaultValue;
            }

            return TryReadValueFromAction(actionName, out T value) ? value : defaultValue;
        }

        public bool IsPressed(string actionName)
        {
            return IsEnableInputDevice && TryGetAction(actionName, out var action) && action.IsPressed();
        }

        public bool WasPressedThisFrame(string actionName)
        {
            return IsEnableInputDevice && TryGetAction(actionName, out var action) && action.WasPressedThisFrame();
        }

        public bool WasReleasedThisFrame(string actionName)
        {
            return IsEnableInputDevice && TryGetAction(actionName, out var action) && action.WasReleasedThisFrame();
        }

        public void AddValueChanged<T>(string actionName, Action<T> valueChanged) where T : struct
        {
            AddActionValueCallback(actionName, valueChanged);
        }

        public void RemoveValueChanged<T>(string actionName, Action<T> valueChanged) where T : struct
        {
            RemoveActionValueCallback(actionName, valueChanged);
        }

        public InputBinding[] GetBindings(string actionName)
        {
            if (!TryGetAction(actionName, out var action))
            {
                return Array.Empty<InputBinding>();
            }

            var bindings = new InputBinding[action.bindings.Count];
            for (var i = 0; i < action.bindings.Count; i++)
            {
                bindings[i] = action.bindings[i];
            }

            return bindings;
        }

        public int GetBindingCount(string actionName)
        {
            return TryGetAction(actionName, out var action) ? action.bindings.Count : 0;
        }

        public int FindBindingIndex(string actionName, string bindingIdOrPath)
        {
            if (string.IsNullOrEmpty(bindingIdOrPath) || !TryGetAction(actionName, out var action))
            {
                return -1;
            }

            var hasGuid = Guid.TryParse(bindingIdOrPath, out var bindingId);
            for (var i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];
                if (hasGuid && binding.id == bindingId)
                {
                    return i;
                }

                if (string.Equals(binding.path, bindingIdOrPath, StringComparison.Ordinal) ||
                    string.Equals(binding.name, bindingIdOrPath, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            for (var i = 0; i < action.bindings.Count; i++)
            {
                if (string.Equals(GetBindingDisplayString(action, i), bindingIdOrPath, StringComparison.Ordinal))
                {
                    return i;
                }
            }

            return -1;
        }

        public string GetBindingDisplayString(string actionName, int bindingIndex)
        {
            return TryGetAction(actionName, out var action) ? GetBindingDisplayString(action, bindingIndex) : string.Empty;
        }

        public string GetBindingPath(string actionName, int bindingIndex)
        {
            return TryGetBinding(actionName, bindingIndex, out _, out var binding) ? binding.path ?? string.Empty : string.Empty;
        }

        public bool StartRebind(string actionName, int bindingIndex = -1, Action<string, string> completed = null, Action<string> canceled = null)
        {
            if (IsRebinding || !TryGetAction(actionName, out var action))
            {
                return false;
            }

            if (!TryResolveBindingIndex(action, bindingIndex, out var resolvedBindingIndex))
            {
                return false;
            }

            var wasEnabled = action.enabled;
            action.Disable();

            if (action.bindings[resolvedBindingIndex].isComposite)
            {
                StartCompositeRebind(action, actionName, resolvedBindingIndex, resolvedBindingIndex + 1, wasEnabled, completed, canceled);
                return true;
            }

            StartSingleRebind(action, actionName, resolvedBindingIndex, resolvedBindingIndex, wasEnabled, completed, canceled);
            return true;
        }

        public void CancelRebind()
        {
            if (_rebindOperation == null)
            {
                return;
            }

            _rebindOperation.Cancel();
        }

        public bool RemoveBindingOverride(string actionName, int bindingIndex)
        {
            if (!TryGetAction(actionName, out var action) || bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            {
                return false;
            }

            action.RemoveBindingOverride(bindingIndex);
            return true;
        }

        public void RemoveAllBindingOverrides()
        {
            InputActionAsset?.RemoveAllBindingOverrides();
        }

        public string SaveBindingOverridesAsJson()
        {
            return InputActionAsset != null ? InputActionAsset.SaveBindingOverridesAsJson() : string.Empty;
        }

        public void LoadBindingOverridesFromJson(string overridesJson, bool removeExisting = true)
        {
            if (InputActionAsset == null || string.IsNullOrEmpty(overridesJson))
            {
                return;
            }

            InputActionAsset.LoadBindingOverridesFromJson(overridesJson, removeExisting);
        }

        public void AddLastUsedDeviceChanged(Action<InputDevice> changed)
        {
            _lastUsedDeviceChanged += changed;
        }

        public void RemoveLastUsedDeviceChanged(Action<InputDevice> changed)
        {
            _lastUsedDeviceChanged -= changed;
        }

        public void AddDeviceChanged(Action<InputDevice, InputDeviceChange> changed)
        {
            _deviceChanged += changed;
        }

        public void RemoveDeviceChanged(Action<InputDevice, InputDeviceChange> changed)
        {
            _deviceChanged -= changed;
        }

        public void OnInit()
        {
            _isInitialized = true;
            RegisterInputSystemCallbacksIfNeeded();
            RefreshGlobalPlayerInput(CurrentActionMapName, GetEnabledActionMaps());
            RegisterActionCallbacksIfNeeded(InputActionAsset);
        }

        public void OnUpdate()
        {
            if (!IsEnableInputDevice)
            {
                return;
            }

            UpdatePointerPosition();
        }

        public void OnTerminate()
        {
            CancelRebind();
            UnregisterActionCallbacksIfNeeded(InputActionAsset);
            UnregisterInputSystemCallbacksIfNeeded();
            UnregisterPlayerInputManagerCallbacks();
            ClearManagedPlayers();
            DestroyGlobalPlayerInput();
            DestroyInternalPlayerPrefab();
            InputActionAsset?.Disable();
            CurrentPlayerInputManager = null;
            LastUsedDevice = null;
            LastDeviceChanged = null;
            LastDeviceChange = default;
            _lastUsedDeviceChanged = null;
            _deviceChanged = null;
            _controlSchemeChanged = null;
            _playerControlsChanged = null;
            _playerJoined = null;
            _playerLeft = null;
            _currentControlScheme = string.Empty;
            _runtimeHost = null;
            _playerInputManagerConfig = null;
            _configuredInputActionAsset = null;
            _isInitialized = false;
        }

        public void OnPause()
        {
            ResetAll();
        }

        public void OnResume()
        {
        }

        public bool IsExistVirtualAxis(string name)
        {
            return false;
        }

        public bool IsExistVirtualButton(string name)
        {
            return false;
        }

        public void RegisterVirtualAxis(string name)
        {
        }

        public void RegisterVirtualButton(string name)
        {
        }

        public void UnRegisterVirtualAxis(string name)
        {
        }

        public void UnRegisterVirtualButton(string name)
        {
        }

        public void SetVirtualMousePosition(Vector3 value)
        {
            MousePosition = value;
        }

        public void SetButtonStart(string name)
        {
        }

        public void SetButtonDown(string name)
        {
        }

        public void SetButtonUp(string name)
        {
        }

        public void SetAxisPositive(string name)
        {
        }

        public void SetAxisNegative(string name)
        {
        }

        public void SetAxisZero(string name)
        {
        }

        public void SetAxis(string name, float value)
        {
        }

        public void AddButtonStarted(string name, Action<string> started)
        {
            if (started == null)
            {
                return;
            }

            if (TryGetAction(name, out var action))
            {
                AddActionStringCallback(_actionStartedCallbacks, action, name, started, callback => action.started += callback);
            }
        }

        public void AddButtonPerformed(string name, Action<string> performed)
        {
            if (performed == null)
            {
                return;
            }

            if (TryGetAction(name, out var action))
            {
                AddActionStringCallback(_actionPerformedCallbacks, action, name, performed, callback => action.performed += callback);
            }
        }

        public void AddButtonCanceled(string name, Action<string> canceled)
        {
            if (canceled == null)
            {
                return;
            }

            if (TryGetAction(name, out var action))
            {
                AddActionStringCallback(_actionCanceledCallbacks, action, name, canceled, callback => action.canceled += callback);
            }
        }

        public void AddAxisValueChanged(string name, Action<float> valueChanged)
        {
        }

        public void RemoveButtonStarted(string name, Action<string> started)
        {
            if (started == null)
            {
                return;
            }

            if (_actionStartedCallbacks.TryGetValue(name, out var callbackMap) && callbackMap.ContainsKey(started))
            {
                if (TryGetAction(name, out var action))
                {
                    RemoveActionStringCallback(_actionStartedCallbacks, action, name, started, callback => action.started -= callback);
                }
                else
                {
                    callbackMap.Remove(started);
                    if (callbackMap.Count == 0)
                    {
                        _actionStartedCallbacks.Remove(name);
                    }
                }
                return;
            }
        }

        public void RemoveButtonPerformed(string name, Action<string> performed)
        {
            if (performed == null)
            {
                return;
            }

            if (_actionPerformedCallbacks.TryGetValue(name, out var callbackMap) && callbackMap.ContainsKey(performed))
            {
                if (TryGetAction(name, out var action))
                {
                    RemoveActionStringCallback(_actionPerformedCallbacks, action, name, performed, callback => action.performed -= callback);
                }
                else
                {
                    callbackMap.Remove(performed);
                    if (callbackMap.Count == 0)
                    {
                        _actionPerformedCallbacks.Remove(name);
                    }
                }
                return;
            }
        }

        public void RemoveButtonCanceled(string name, Action<string> canceled)
        {
            if (canceled == null)
            {
                return;
            }

            if (_actionCanceledCallbacks.TryGetValue(name, out var callbackMap) && callbackMap.ContainsKey(canceled))
            {
                if (TryGetAction(name, out var action))
                {
                    RemoveActionStringCallback(_actionCanceledCallbacks, action, name, canceled, callback => action.canceled -= callback);
                }
                else
                {
                    callbackMap.Remove(canceled);
                    if (callbackMap.Count == 0)
                    {
                        _actionCanceledCallbacks.Remove(name);
                    }
                }
                return;
            }
        }

        public void RemoveAxisValueChanged(string name, Action<float> valueChanged)
        {
        }

        public void ClearAllAction()
        {
            UnregisterActionCallbacksIfNeeded(InputActionAsset);
            for (var i = 0; i < _managedPlayers.Count; i++)
            {
                var player = _managedPlayers[i];
                if (player != null)
                {
                    UnregisterPlayerScopedCallbacks(player);
                }
            }

            _actionStartedCallbacks.Clear();
            _actionPerformedCallbacks.Clear();
            _actionCanceledCallbacks.Clear();
            _actionValueCallbacks.Clear();
            _playerActionStartedCallbacks.Clear();
            _playerActionPerformedCallbacks.Clear();
            _playerActionCanceledCallbacks.Clear();
            _playerActionValueCallbacks.Clear();
            
            RegisterActionCallbacksIfNeeded(InputActionAsset);
        }

        public bool GetButton(string name)
        {
            if (!IsEnableInputDevice)
            {
                return false;
            }

            return TryGetAction(name, out var action) && action.IsPressed();
        }

        public bool GetButtonDown(string name)
        {
            if (!IsEnableInputDevice)
            {
                return false;
            }

            return TryGetAction(name, out var action) && action.WasPressedThisFrame();
        }

        public bool GetButtonUp(string name)
        {
            if (!IsEnableInputDevice)
            {
                return false;
            }

            return TryGetAction(name, out var action) && action.WasReleasedThisFrame();
        }

        public float GetAxis(string name, bool raw)
        {
            return 0;
        }

        public void LoadDevice(InputDeviceBase deviceType)
        {
        }

        public void ResetAll()
        {
            CancelRebind();
            MousePosition = Vector3.zero;
        }

        public bool GetKey(KeyCode keyCode)
        {
            if (!IsEnableInputDevice)
            {
                return false;
            }

            return TryGetButtonControl(keyCode, out var button) && button.isPressed;
        }

        public bool GetKeyDown(KeyCode keyCode)
        {
            if (!IsEnableInputDevice)
            {
                return false;
            }

            return TryGetButtonControl(keyCode, out var button) && button.wasPressedThisFrame;
        }

        public bool GetKeyUp(KeyCode keyCode)
        {
            if (!IsEnableInputDevice)
            {
                return false;
            }

            return TryGetButtonControl(keyCode, out var button) && button.wasReleasedThisFrame;
        }

        public bool GetKey(KeyCode keyCode1, KeyCode keyCode2)
        {
            return GetKey(keyCode1) && GetKey(keyCode2);
        }

        public bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2)
        {
            return GetKey(keyCode1) && GetKeyDown(keyCode2);
        }

        public bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2)
        {
            return GetKey(keyCode1) && GetKeyUp(keyCode2);
        }

        public bool GetKey(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return GetKey(keyCode1) && GetKey(keyCode2) && GetKey(keyCode3);
        }

        public bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return GetKey(keyCode1) && GetKey(keyCode2) && GetKeyDown(keyCode3);
        }

        public bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3)
        {
            return GetKey(keyCode1) && GetKey(keyCode2) && GetKeyUp(keyCode3);
        }

        private void UpdatePointerPosition()
        {
            if (Mouse.current != null)
            {
                MousePosition = Mouse.current.position.ReadValue();
                return;
            }

            if (Pointer.current != null)
            {
                MousePosition = Pointer.current.position.ReadValue();
                return;
            }

            if (Touchscreen.current != null)
            {
                MousePosition = Touchscreen.current.primaryTouch.position.ReadValue();
                return;
            }

            MousePosition = Vector3.zero;
        }

        private void RefreshGlobalPlayerInput(string currentActionMapName, string[] enabledActionMaps)
        {
            if (_configuredInputActionAsset == null)
            {
                if (_globalPlayerInput != null)
                {
                    UnregisterGlobalPlayerInputCallbacks(_globalPlayerInput);
                    _globalPlayerInput.DeactivateInput();
                    _globalPlayerInput.actions = null;
                }

                return;
            }

            var playerInput = GetOrCreateGlobalPlayerInput();
            if (playerInput == null)
            {
                return;
            }

            ConfigureGlobalPlayerInput(playerInput, currentActionMapName, enabledActionMaps);
            if (InputActionAsset != null)
            {
                InputActionAsset.bindingMask = null;
            }
        }

        private PlayerInput GetOrCreateGlobalPlayerInput()
        {
            if (_runtimeHost == null)
            {
                return _globalPlayerInput;
            }

            if (_globalPlayerObject == null)
            {
                _globalPlayerObject = new GameObject(GlobalPlayerInputName);
                _globalPlayerObject.transform.SetParent(_runtimeHost.transform, false);
            }
            else if (!ReferenceEquals(_globalPlayerObject.transform.parent, _runtimeHost.transform))
            {
                _globalPlayerObject.transform.SetParent(_runtimeHost.transform, false);
            }

            if (_globalPlayerInput == null)
            {
                _globalPlayerInput = _globalPlayerObject.GetComponent<PlayerInput>();
                if (_globalPlayerInput == null)
                {
                    _globalPlayerInput = _globalPlayerObject.AddComponent<PlayerInput>();
                }
            }

            RegisterGlobalPlayerInputCallbacks(_globalPlayerInput);
            return _globalPlayerInput;
        }

        private void ConfigureGlobalPlayerInput(PlayerInput playerInput, string currentActionMapName, string[] enabledActionMaps)
        {
            if (playerInput == null)
            {
                return;
            }

            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            playerInput.neverAutoSwitchControlSchemes = true;
            playerInput.actions = _configuredInputActionAsset;
            playerInput.defaultActionMap = GetDefaultActionMapName(_configuredInputActionAsset);

            if (_configuredInputActionAsset == null)
            {
                playerInput.DeactivateInput();
                return;
            }

            if (!_isInitialized)
            {
                playerInput.DeactivateInput();
                playerInput.actions?.Disable();
                return;
            }

            playerInput.ActivateInput();
            PairAllDevicesToGlobalPlayerInput(playerInput);
            RestoreGlobalActionMaps(playerInput, currentActionMapName, enabledActionMaps);
            _currentControlScheme = playerInput.currentControlScheme ?? _currentControlScheme;
        }

        private void PairAllDevicesToGlobalPlayerInput(PlayerInput playerInput)
        {
            if (playerInput == null || !playerInput.user.valid)
            {
                return;
            }

            playerInput.user.UnpairDevices();
            playerInput.user.AssociateActionsWithUser(playerInput.actions);

            foreach (var device in InputSystem.devices)
            {
                if (device == null || !device.added)
                {
                    continue;
                }

                InputUser.PerformPairingWithDevice(device, playerInput.user);
            }

            if (playerInput.actions != null)
            {
                playerInput.actions.bindingMask = null;
            }

            _globalScopedDeviceIds.Clear();
            _isGlobalControlSchemeScoped = false;
        }

        private void PairDevicesToGlobalPlayerInput(PlayerInput playerInput, InputDevice[] devices, string controlSchemeName)
        {
            if (playerInput == null || !playerInput.user.valid)
            {
                return;
            }

            playerInput.user.UnpairDevices();
            playerInput.user.AssociateActionsWithUser(playerInput.actions);
            _globalScopedDeviceIds.Clear();

            if (devices != null)
            {
                for (var i = 0; i < devices.Length; i++)
                {
                    var device = devices[i];
                    if (device == null || !device.added)
                    {
                        continue;
                    }

                    InputUser.PerformPairingWithDevice(device, playerInput.user);
                    _globalScopedDeviceIds.Add(device.deviceId);
                }
            }

            if (playerInput.actions != null)
            {
                playerInput.actions.bindingMask = string.IsNullOrEmpty(controlSchemeName) ? null : InputBinding.MaskByGroup(controlSchemeName);
            }

            _isGlobalControlSchemeScoped = !string.IsNullOrEmpty(controlSchemeName);
        }

        private void ShareDevicesWithGlobalPlayerInput(PlayerInput playerInput)
        {
            if (playerInput == null || _globalPlayerInput == null || !_globalPlayerInput.user.valid)
            {
                return;
            }

            var devices = playerInput.devices;
            for (var i = 0; i < devices.Count; i++)
            {
                var device = devices[i];
                if (device == null)
                {
                    continue;
                }

                if (_isGlobalControlSchemeScoped && !_globalScopedDeviceIds.Contains(device.deviceId))
                {
                    continue;
                }

                if (IsDevicePairedToUser(_globalPlayerInput.user, device))
                {
                    continue;
                }

                InputUser.PerformPairingWithDevice(device, _globalPlayerInput.user);
            }
        }

        private static bool IsDevicePairedToUser(InputUser user, InputDevice device)
        {
            if (!user.valid || device == null)
            {
                return false;
            }

            var pairedDevices = user.pairedDevices;
            for (var i = 0; i < pairedDevices.Count; i++)
            {
                if (ReferenceEquals(pairedDevices[i], device))
                {
                    return true;
                }
            }

            return false;
        }

        private void RestoreGlobalActionMaps(PlayerInput playerInput, string currentActionMapName, string[] enabledActionMaps)
        {
            var actions = playerInput != null ? playerInput.actions : null;
            if (actions == null)
            {
                return;
            }

            actions.Disable();

            var preferredActionMapName = currentActionMapName;
            if (string.IsNullOrEmpty(preferredActionMapName) || actions.FindActionMap(preferredActionMapName, false) == null)
            {
                preferredActionMapName = string.Empty;
                if (enabledActionMaps != null)
                {
                    for (var i = 0; i < enabledActionMaps.Length; i++)
                    {
                        var actionMapName = enabledActionMaps[i];
                        if (!string.IsNullOrEmpty(actionMapName) && actions.FindActionMap(actionMapName, false) != null)
                        {
                            preferredActionMapName = actionMapName;
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(preferredActionMapName))
                {
                    preferredActionMapName = GetDefaultActionMapName(_configuredInputActionAsset);
                }
            }

            var restoredAny = false;
            if (!string.IsNullOrEmpty(preferredActionMapName))
            {
                playerInput.SwitchCurrentActionMap(preferredActionMapName);
                restoredAny = true;
            }

            if (enabledActionMaps != null)
            {
                for (var i = 0; i < enabledActionMaps.Length; i++)
                {
                    var actionMapName = enabledActionMaps[i];
                    if (string.IsNullOrEmpty(actionMapName) || string.Equals(actionMapName, preferredActionMapName, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    var actionMap = actions.FindActionMap(actionMapName, false);
                    if (actionMap == null || actionMap.enabled)
                    {
                        continue;
                    }

                    actionMap.Enable();
                    restoredAny = true;
                }
            }

            if (!restoredAny && actions.actionMaps.Count > 0)
            {
                playerInput.SwitchCurrentActionMap(actions.actionMaps[0].name);
            }
        }

        private void RegisterGlobalPlayerInputCallbacks(PlayerInput playerInput)
        {
            if (playerInput == null)
            {
                return;
            }

            playerInput.onControlsChanged -= OnGlobalPlayerInputControlsChanged;
            playerInput.onControlsChanged += OnGlobalPlayerInputControlsChanged;
        }

        private void UnregisterGlobalPlayerInputCallbacks(PlayerInput playerInput)
        {
            if (playerInput == null)
            {
                return;
            }

            playerInput.onControlsChanged -= OnGlobalPlayerInputControlsChanged;
        }

        private void DestroyGlobalPlayerInput()
        {
            if (_globalPlayerInput != null)
            {
                UnregisterGlobalPlayerInputCallbacks(_globalPlayerInput);
                _globalPlayerInput = null;
            }

            DestroyHelperObject(_globalPlayerObject);
            _globalPlayerObject = null;
        }

        private void DestroyInternalPlayerPrefab()
        {
            DestroyHelperObject(_internalPlayerPrefab);
            _internalPlayerPrefab = null;
        }

        private static void DestroyHelperObject(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(target);
                return;
            }

            UnityEngine.Object.DestroyImmediate(target);
        }

        private bool TryGetAction(string actionName, out InputAction action)
        {
            action = FindAction(actionName);
            return action != null;
        }

        private bool TryGetBinding(string actionName, int bindingIndex, out InputAction action, out InputBinding binding)
        {
            binding = default;
            if (!TryGetAction(actionName, out action) || bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            {
                return false;
            }

            binding = action.bindings[bindingIndex];
            return true;
        }

        private static bool TryResolveBindingIndex(InputAction action, int bindingIndex, out int resolvedBindingIndex)
        {
            resolvedBindingIndex = -1;
            if (action == null || action.bindings.Count == 0)
            {
                return false;
            }

            if (bindingIndex >= 0)
            {
                if (bindingIndex >= action.bindings.Count)
                {
                    return false;
                }

                resolvedBindingIndex = bindingIndex;
                return true;
            }

            for (var i = 0; i < action.bindings.Count; i++)
            {
                if (!action.bindings[i].isPartOfComposite)
                {
                    resolvedBindingIndex = i;
                    return true;
                }
            }

            return false;
        }

        private void StartSingleRebind(InputAction action, string actionName, int displayBindingIndex, int bindingIndex, bool wasEnabled, Action<string, string> completed, Action<string> canceled)
        {
            _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("<Keyboard>/escape")
                .OnCancel(operation =>
                {
                    CompleteRebind(operation, action, wasEnabled);
                    canceled?.Invoke(actionName);
                })
                .OnComplete(operation =>
                {
                    CompleteRebind(operation, action, wasEnabled);
                    completed?.Invoke(actionName, GetBindingDisplayString(action, displayBindingIndex));
                });

            _rebindOperation.Start();
        }

        private void StartCompositeRebind(InputAction action, string actionName, int displayBindingIndex, int bindingIndex, bool wasEnabled, Action<string, string> completed, Action<string> canceled)
        {
            if (bindingIndex >= action.bindings.Count || !action.bindings[bindingIndex].isPartOfComposite)
            {
                if (wasEnabled)
                {
                    action.Enable();
                }
                completed?.Invoke(actionName, GetBindingDisplayString(action, displayBindingIndex));
                return;
            }

            _rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("<Keyboard>/escape")
                .OnCancel(operation =>
                {
                    CompleteRebind(operation, action, wasEnabled);
                    canceled?.Invoke(actionName);
                })
                .OnComplete(operation =>
                {
                    CompleteRebind(operation, action, false);
                    StartCompositeRebind(action, actionName, displayBindingIndex, bindingIndex + 1, wasEnabled, completed, canceled);
                });

            _rebindOperation.Start();
        }

        private void CompleteRebind(InputActionRebindingExtensions.RebindingOperation operation, InputAction action, bool enableAction)
        {
            operation.Dispose();
            if (ReferenceEquals(_rebindOperation, operation))
            {
                _rebindOperation = null;
            }

            if (enableAction)
            {
                action.Enable();
            }
        }

        private bool TryReadValueFromAction<T>(string actionName, out T value) where T : struct
        {
            value = default;
            return TryGetAction(actionName, out var action) && TryReadValueFromAction(action, out value);
        }

        private bool TryReadValueFromAction<T>(InputAction action, out T value) where T : struct
        {
            value = default;
            if (action == null)
            {
                return false;
            }

            value = action.ReadValue<T>();
            return true;
        }

        private static bool TryReadValueFromContext<T>(InputAction.CallbackContext context, out T value) where T : struct
        {
            value = default;
            if (context.action == null)
            {
                return false;
            }

            if (context.valueType != typeof(T))
            {
                return false;
            }

            value = context.ReadValue<T>();
            return true;
        }

        private bool IsAnyButtonPressed(bool downThisFrame)
        {
            foreach (var device in InputSystem.devices)
            {
                foreach (var control in device.allControls)
                {
                    if (!(control is ButtonControl button))
                    {
                        continue;
                    }

                    if (downThisFrame)
                    {
                        if (button.wasPressedThisFrame)
                        {
                            return true;
                        }
                    }
                    else if (button.isPressed)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetBindingDisplayString(InputAction action, int bindingIndex)
        {
            if (action == null || bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            {
                return string.Empty;
            }

            var binding = action.bindings[bindingIndex];
            if (!binding.isComposite)
            {
                return action.GetBindingDisplayString(bindingIndex);
            }

            StringBuilder builder = null;
            for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
            {
                var displayString = action.GetBindingDisplayString(i);
                if (!string.IsNullOrEmpty(displayString))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(displayString);
                    }
                    else
                    {
                        builder.Append('/').Append(displayString);
                    }
                }
            }

            return builder != null ? builder.ToString() : action.GetBindingDisplayString(bindingIndex);
        }

        private bool TryGetButtonControl(KeyCode keyCode, out ButtonControl button)
        {
            button = null;

            var mouse = Mouse.current;
            switch (keyCode)
            {
                case KeyCode.Mouse0:
                    button = mouse?.leftButton;
                    return button != null;
                case KeyCode.Mouse1:
                    button = mouse?.rightButton;
                    return button != null;
                case KeyCode.Mouse2:
                    button = mouse?.middleButton;
                    return button != null;
                case KeyCode.Mouse3:
                    button = mouse?.forwardButton;
                    return button != null;
                case KeyCode.Mouse4:
                    button = mouse?.backButton;
                    return button != null;
            }

            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return false;
            }

            if (!_keyCodeMappings.TryGetValue(keyCode, out var key))
            {
                if (!Enum.TryParse(keyCode.ToString(), true, out key))
                {
                    return false;
                }

                _keyCodeMappings[keyCode] = key;
            }

            button = keyboard[key];
            return button != null;
        }

        private void RegisterActionCallbacksIfNeeded(InputActionAsset asset)
        {
            if (asset == null || _areActionCallbacksRegistered)
            {
                return;
            }

            RegisterActionCallbacks(asset);
            _areActionCallbacksRegistered = true;
        }

        private void UnregisterActionCallbacksIfNeeded(InputActionAsset asset)
        {
            if (asset == null || !_areActionCallbacksRegistered)
            {
                return;
            }

            UnregisterActionCallbacks(asset);
            _areActionCallbacksRegistered = false;
        }

        private void RegisterActionCallbacks(InputActionAsset asset)
        {
            if (asset == null)
            {
                return;
            }

            RegisterStringCallbackMap(asset, _actionStartedCallbacks, (action, callback) => action.started += callback);
            RegisterStringCallbackMap(asset, _actionPerformedCallbacks, (action, callback) => action.performed += callback);
            RegisterStringCallbackMap(asset, _actionCanceledCallbacks, (action, callback) => action.canceled += callback);
            RegisterValueCallbackMap(asset);
        }

        private void UnregisterActionCallbacks(InputActionAsset asset)
        {
            if (asset == null)
            {
                return;
            }

            RegisterStringCallbackMap(asset, _actionStartedCallbacks, (action, callback) => action.started -= callback);
            RegisterStringCallbackMap(asset, _actionPerformedCallbacks, (action, callback) => action.performed -= callback);
            RegisterStringCallbackMap(asset, _actionCanceledCallbacks, (action, callback) => action.canceled -= callback);
            UnregisterValueCallbackMap(asset);
        }

        private void RegisterInputSystemCallbacksIfNeeded()
        {
            if (_areInputSystemCallbacksRegistered)
            {
                return;
            }

            InputSystem.onDeviceChange += OnInputDeviceChange;
            InputSystem.onEvent += OnInputSystemEvent;
            _areInputSystemCallbacksRegistered = true;
        }

        private void UnregisterInputSystemCallbacksIfNeeded()
        {
            if (!_areInputSystemCallbacksRegistered)
            {
                return;
            }

            InputSystem.onDeviceChange -= OnInputDeviceChange;
            InputSystem.onEvent -= OnInputSystemEvent;
            _areInputSystemCallbacksRegistered = false;
        }

        private void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
        {
            LastDeviceChanged = device;
            LastDeviceChange = change;
            _deviceChanged?.Invoke(device, change);

            if (_globalPlayerInput != null &&
                _globalPlayerInput.user.valid &&
                (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected || change == InputDeviceChange.Enabled))
            {
                if (!_isGlobalControlSchemeScoped || _globalScopedDeviceIds.Contains(device.deviceId))
                {
                    InputUser.PerformPairingWithDevice(device, _globalPlayerInput.user);
                }
            }

            if ((change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled) &&
                ReferenceEquals(LastUsedDevice, device))
            {
                SetLastUsedDevice(null);
            }
        }

        private void OnInputSystemEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (!IsEnableInputDevice || device == null)
            {
                return;
            }

            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            {
                return;
            }

            if (ReferenceEquals(LastUsedDevice, device))
            {
                return;
            }

            SetLastUsedDevice(device);
        }

        private void SetLastUsedDevice(InputDevice device)
        {
            if (ReferenceEquals(LastUsedDevice, device))
            {
                return;
            }

            LastUsedDevice = device;
            _lastUsedDeviceChanged?.Invoke(device);
        }

        private void RegisterStringCallbackMap(InputActionAsset asset, Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>> callbackSource, Action<InputAction, Action<InputAction.CallbackContext>> binder)
        {
            foreach (var pair in callbackSource)
            {
                if (!TryGetAction(asset, pair.Key, out var action))
                {
                    continue;
                }

                foreach (var callback in pair.Value.Values)
                {
                    binder(action, callback);
                }
            }
        }

        private void RegisterValueCallbackMap(InputActionAsset asset)
        {
            foreach (var actionPair in _actionValueCallbacks)
            {
                if (!TryGetAction(asset, actionPair.Key, out var action))
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

        private void UnregisterValueCallbackMap(InputActionAsset asset)
        {
            foreach (var actionPair in _actionValueCallbacks)
            {
                if (!TryGetAction(asset, actionPair.Key, out var action))
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

        private void AddActionValueCallback<T>(string actionName, Action<T> valueChanged) where T : struct
        {
            if (string.IsNullOrEmpty(actionName) || valueChanged == null)
            {
                return;
            }

            if (!_actionValueCallbacks.TryGetValue(actionName, out var typeMap))
            {
                typeMap = new Dictionary<Type, Dictionary<Delegate, Action<InputAction.CallbackContext>>>();
                _actionValueCallbacks.Add(actionName, typeMap);
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
            if (TryGetAction(actionName, out var action))
            {
                action.performed += wrappedCallback;
                action.canceled += wrappedCallback;
            }
        }

        private void RemoveActionValueCallback<T>(string actionName, Action<T> valueChanged) where T : struct
        {
            if (string.IsNullOrEmpty(actionName) || valueChanged == null || !_actionValueCallbacks.TryGetValue(actionName, out var typeMap))
            {
                return;
            }

            var valueType = typeof(T);
            if (!typeMap.TryGetValue(valueType, out var callbackMap) || !callbackMap.TryGetValue(valueChanged, out var wrappedCallback))
            {
                return;
            }

            if (TryGetAction(actionName, out var action))
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
                _actionValueCallbacks.Remove(actionName);
            }
        }

        private void AddActionStringCallback(Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>> callbackSource, InputAction action, string actionName, Action<string> callback, Action<Action<InputAction.CallbackContext>> binder)
        {
            if (!callbackSource.TryGetValue(actionName, out var callbackMap))
            {
                callbackMap = new Dictionary<Action<string>, Action<InputAction.CallbackContext>>();
                callbackSource.Add(actionName, callbackMap);
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
            binder(wrappedCallback);
        }

        private void RemoveActionStringCallback(Dictionary<string, Dictionary<Action<string>, Action<InputAction.CallbackContext>>> callbackSource, InputAction action, string actionName, Action<string> callback, Action<Action<InputAction.CallbackContext>> unbinder)
        {
            if (!callbackSource.TryGetValue(actionName, out var callbackMap) || !callbackMap.TryGetValue(callback, out var wrappedCallback))
            {
                return;
            }

            unbinder(wrappedCallback);
            callbackMap.Remove(callback);
            if (callbackMap.Count == 0)
            {
                callbackSource.Remove(actionName);
            }
        }

        private bool TryGetAction(InputActionAsset asset, string actionName, out InputAction action)
        {
            action = null;
            if (asset == null || string.IsNullOrEmpty(actionName))
            {
                return false;
            }

            action = asset.FindAction(actionName, false);
            return action != null;
        }

    }
}
#endif