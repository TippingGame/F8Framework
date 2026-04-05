using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 默认的输入管理器助手
    /// </summary>
    public sealed class DefaultInputHelper : IInputHelper
    {
        private bool _isEnableInputDevice = true;
        private readonly Dictionary<string, VirtualAxis> _virtualAxes = new Dictionary<string, VirtualAxis>(StringComparer.Ordinal);
        private readonly Dictionary<string, VirtualButton> _virtualButtons = new Dictionary<string, VirtualButton>(StringComparer.Ordinal);
        private readonly Dictionary<string, VirtualAxis> _stagedVirtualAxes = new Dictionary<string, VirtualAxis>(StringComparer.Ordinal);
        private readonly Dictionary<string, VirtualButton> _stagedVirtualButtons = new Dictionary<string, VirtualButton>(StringComparer.Ordinal);
        private bool _isSwitchingDevice;

        /// <summary>
        /// 所属的内置模块
        /// </summary>
        public IModule Module { get; set; }

        /// <summary>
        /// 输入设备
        /// </summary>
        public InputDeviceBase Device { get; private set; }

        /// <summary>
        /// 是否启用输入设备
        /// </summary>
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

        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Vector3 MousePosition { get; private set; }

        /// <summary>
        /// 任意键按住
        /// </summary>
        public bool AnyKey
        {
            get
            {
                return IsEnableInputDevice ? GetAnyKey() : false;
            }
        }

        /// <summary>
        /// 任意键按下
        /// </summary>
        public bool AnyKeyDown
        {
            get
            {
                return IsEnableInputDevice ? GetAnyKeyDown() : false;
            }
        }

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            Device?.OnStartUp();
        }

        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            if (IsEnableInputDevice)
            {
                Device?.OnRun();
            }
        }

        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            Device?.OnShutdown();
        }

        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {
            ResetAll();
        }

        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {
        }

        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualAxis(string name)
        {
            return !string.IsNullOrEmpty(name) && _virtualAxes.ContainsKey(name);
        }

        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistVirtualButton(string name)
        {
            return !string.IsNullOrEmpty(name) && _virtualButtons.ContainsKey(name);
        }

        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void RegisterVirtualAxis(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (_virtualAxes.ContainsKey(name))
            {
                return;
            }

            if (_isSwitchingDevice && _stagedVirtualAxes.TryGetValue(name, out var axis))
            {
                _stagedVirtualAxes.Remove(name);
                _virtualAxes.Add(name, axis);
                return;
            }

            _virtualAxes.Add(name, new VirtualAxis());
        }

        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void RegisterVirtualButton(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (_virtualButtons.ContainsKey(name))
            {
                return;
            }

            if (_isSwitchingDevice && _stagedVirtualButtons.TryGetValue(name, out var button))
            {
                _stagedVirtualButtons.Remove(name);
                _virtualButtons.Add(name, button);
                return;
            }

            _virtualButtons.Add(name, new VirtualButton(name));
        }

        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void UnRegisterVirtualAxis(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (_isSwitchingDevice)
            {
                if (_virtualAxes.TryGetValue(name, out var axis))
                {
                    _virtualAxes.Remove(name);
                    _stagedVirtualAxes[name] = axis;
                }

                return;
            }

            _virtualAxes.Remove(name);
            _stagedVirtualAxes.Remove(name);
        }

        internal void BeginDeviceSwitch()
        {
            _isSwitchingDevice = true;
            _stagedVirtualAxes.Clear();
            _stagedVirtualButtons.Clear();
        }

        internal void EndDeviceSwitch()
        {
            _isSwitchingDevice = false;
            _stagedVirtualAxes.Clear();
            _stagedVirtualButtons.Clear();
        }

        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void UnRegisterVirtualButton(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (_isSwitchingDevice)
            {
                if (_virtualButtons.TryGetValue(name, out var button))
                {
                    _virtualButtons.Remove(name);
                    _stagedVirtualButtons[name] = button;
                }

                return;
            }

            _virtualButtons.Remove(name);
            _stagedVirtualButtons.Remove(name);
        }

        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        public void SetVirtualMousePosition(Vector3 value)
        {
            MousePosition = value;
        }

        /// <summary>
        /// 开始按钮按
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonStart(string name)
        {
            GetOrCreateVirtualButton(name)?.Began();
        }

        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonDown(string name)
        {
            GetOrCreateVirtualButton(name)?.Pressed();
        }

        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        public void SetButtonUp(string name)
        {
            GetOrCreateVirtualButton(name)?.Released();
        }

        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisPositive(string name)
        {
            GetOrCreateVirtualAxis(name)?.SetValue(1f);
        }

        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisNegative(string name)
        {
            GetOrCreateVirtualAxis(name)?.SetValue(-1f);
        }

        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        public void SetAxisZero(string name)
        {
            GetOrCreateVirtualAxis(name)?.SetValue(0f);
        }

        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">轴线值</param>
        public void SetAxis(string name, float value)
        {
            GetOrCreateVirtualAxis(name)?.SetValue(value);
        }

        public void AddButtonStarted(string name, Action<string> started)
        {
            var button = GetOrCreateVirtualButton(name);
            if (button != null)
            {
                button.Started += started;
            }
        }

        public void AddButtonPerformed(string name, Action<string> performed)
        {
            var button = GetOrCreateVirtualButton(name);
            if (button != null)
            {
                button.Performed += performed;
            }
        }

        public void AddButtonCanceled(string name, Action<string> canceled)
        {
            var button = GetOrCreateVirtualButton(name);
            if (button != null)
            {
                button.Canceled += canceled;
            }
        }

        public void AddAxisValueChanged(string name, Action<float> valueChanged)
        {
            var axis = GetOrCreateVirtualAxis(name);
            if (axis != null)
            {
                axis.ValueChanged += valueChanged;
            }
        }

        public void RemoveButtonStarted(string name, Action<string> started)
        {
            if (IsExistVirtualButton(name))
            {
                _virtualButtons[name].Started -= started;
            }
        }

        public void RemoveButtonPerformed(string name, Action<string> performed)
        {
            if (IsExistVirtualButton(name))
            {
                _virtualButtons[name].Performed -= performed;
            }
        }

        public void RemoveButtonCanceled(string name, Action<string> canceled)
        {
            if (IsExistVirtualButton(name))
            {
                _virtualButtons[name].Canceled -= canceled;
            }
        }

        public void RemoveAxisValueChanged(string name, Action<float> valueChanged)
        {
            if (IsExistVirtualAxis(name))
            {
                _virtualAxes[name].ValueChanged -= valueChanged;
            }
        }

        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按住</returns>
        public bool GetButton(string name)
        {
            return GetOrCreateVirtualButton(name)?.GetButton == true;
        }

        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name)
        {
            return GetOrCreateVirtualButton(name)?.GetButtonDown == true;
        }

        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        public bool GetButtonUp(string name)
        {
            return GetOrCreateVirtualButton(name)?.GetButtonUp == true;
        }

        /// <summary>
        /// 获取轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="raw">是否获取整数值</param>
        /// <returns>轴线值</returns>
        public float GetAxis(string name, bool raw)
        {
            var axis = GetOrCreateVirtualAxis(name);
            if (axis == null)
            {
                return 0f;
            }

            return raw ? axis.GetValueRaw : axis.GetValue;
        }

        /// <summary>
        /// 键盘按键按住
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode)
        {
            return IsEnableInputDevice ? GetLegacyKey(keyCode) : false;
        }

        /// <summary>
        /// 键盘按键按下
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode)
        {
            return IsEnableInputDevice ? GetLegacyKeyDown(keyCode) : false;
        }

        /// <summary>
        /// 键盘按键抬起
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode)
        {
            return IsEnableInputDevice ? GetLegacyKeyUp(keyCode) : false;
        }

        /// <summary>
        /// 键盘组合按键按住（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按住</returns>
        public bool GetKey(KeyCode keyCode1, KeyCode keyCode2)
        {
            return IsEnableInputDevice ? (GetLegacyKey(keyCode1) && GetLegacyKey(keyCode2)) : false;
        }

        /// <summary>
        /// 键盘组合按键按下（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按下</returns>
        public bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2)
        {
            return IsEnableInputDevice && GetLegacyKey(keyCode1) && GetLegacyKeyDown(keyCode2);
        }

        /// <summary>
        /// 键盘组合按键抬起（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否抬起</returns>
        public bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2)
        {
            return IsEnableInputDevice && GetLegacyKey(keyCode1) && GetLegacyKeyUp(keyCode2);
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
            return IsEnableInputDevice ? (GetLegacyKey(keyCode1) && GetLegacyKey(keyCode2) && GetLegacyKey(keyCode3)) : false;
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
            return IsEnableInputDevice && GetLegacyKey(keyCode1) && GetLegacyKey(keyCode2) && GetLegacyKeyDown(keyCode3);
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
            return IsEnableInputDevice && GetLegacyKey(keyCode1) && GetLegacyKey(keyCode2) && GetLegacyKeyUp(keyCode3);
        }

        /// <summary>
        /// 加载输入设备
        /// </summary>
        /// <param name="deviceType">输入设备类型</param>
        public void LoadDevice(InputDeviceBase deviceType)
        {
            Device = deviceType;
        }

        /// <summary>
        /// 清除所有输入状态
        /// </summary>
        public void ResetAll()
        {
            foreach (var item in _virtualAxes)
            {
                item.Value.SetValue(0f);
            }

            foreach (var item in _virtualButtons)
            {
                item.Value.Released();
            }

            MousePosition = Vector3.zero;
        }

        /// <summary>
        /// 清除所有输入事件
        /// </summary>
        public void ClearAllAction()
        {
            foreach (var item in _virtualAxes)
            {
                item.Value.ClearAction();
            }

            foreach (var item in _virtualButtons)
            {
                item.Value.ClearAction();
            }
        }

        private VirtualAxis GetOrCreateVirtualAxis(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!_virtualAxes.TryGetValue(name, out var axis))
            {
                axis = new VirtualAxis();
                _virtualAxes.Add(name, axis);
            }

            return axis;
        }

        private VirtualButton GetOrCreateVirtualButton(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!_virtualButtons.TryGetValue(name, out var button))
            {
                button = new VirtualButton(name);
                _virtualButtons.Add(name, button);
            }

            return button;
        }

        private static bool GetAnyKey()
        {
            return Input.anyKey;
        }

        private static bool GetAnyKeyDown()
        {
            return Input.anyKeyDown;
        }

        private static bool GetLegacyKey(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }

        private static bool GetLegacyKeyDown(KeyCode keyCode)
        {
            return Input.GetKeyDown(keyCode);
        }

        private static bool GetLegacyKeyUp(KeyCode keyCode)
        {
            return Input.GetKeyUp(keyCode);
        }

        private sealed class VirtualButton
        {
            private readonly string _name;
            private long _pressedFrame = -5;
            private long _releasedFrame = -5;
            private bool _pressed;

            public event Action<string> Started;
            public event Action<string> Performed;
            public event Action<string> Canceled;

            public VirtualButton(string name)
            {
                _name = name;
            }

            public bool GetButton => _pressed;

            public bool GetButtonDown => _pressedFrame == InputManager.Instance.FrameCount;

            public bool GetButtonUp => _releasedFrame == InputManager.Instance.FrameCount;

            public void Began()
            {
                _pressedFrame = -5;
                _releasedFrame = -5;
                _pressed = false;
                Started?.Invoke(_name);
            }

            public void Pressed()
            {
                if (_pressed)
                {
                    return;
                }

                _pressed = true;
                _pressedFrame = InputManager.Instance.FrameCount + 1;
                Performed?.Invoke(_name);
            }

            public void Released()
            {
                if (!_pressed)
                {
                    return;
                }

                _pressed = false;
                _releasedFrame = InputManager.Instance.FrameCount + 1;
                Canceled?.Invoke(_name);
            }

            public void ClearAction()
            {
                Started = null;
                Performed = null;
                Canceled = null;
            }
        }

        private sealed class VirtualAxis
        {
            private float _value;

            public event Action<float> ValueChanged;

            public float GetValue => _value;

            public float GetValueRaw
            {
                get
                {
                    if (_value < 0f)
                    {
                        return -1f;
                    }

                    if (_value > 0f)
                    {
                        return 1f;
                    }

                    return 0f;
                }
            }

            public void SetValue(float value)
            {
                if (!Mathf.Approximately(_value, value))
                {
                    _value = value;
                    ValueChanged?.Invoke(_value);
                }
            }

            public void ClearAction()
            {
                ValueChanged = null;
            }
        }
    }
}
