using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace F8Framework.Core
{
    /// <summary>
    /// 标准输入设备
    /// </summary>
    public sealed class StandardInputDevice : InputDeviceBase
    {
        /// <summary>
        /// 鼠标左键双击时间间隔
        /// </summary>
        private readonly float _mouseLeftDoubleClickInterval = 0.3f;
        
        /// <summary>
        /// 鼠标左键单击计时器
        /// </summary>
        private float _mouseLeftClickTimer = 0;
        
        /// <summary>
        /// UpperLower轴线值
        /// </summary>
        private float _upperLowerValue = 0;
        
        /// <summary>
        /// 上一次选中的输入框
        /// </summary>
        private InputField _lastInput;
        
        /// <summary>
        /// 上一次选中的UI控件
        /// </summary>
        private GameObject _lastSelected;

        /// <summary>
        /// 当前选中的UI控件
        /// </summary>
        private GameObject CurrentSelected
        {
            get
            {
                if (EventSystem.current)
                {
                    return EventSystem.current.currentSelectedGameObject;
                }
                return null;
            }
        }
        
        /// <summary>
        /// 当前是否有任一输入框控件处于焦点中
        /// </summary>
        private bool IsAnyInputFocused
        {
            get
            {
                if (CurrentSelected == null)
                {
                    _lastSelected = null;
                    _lastInput = null;
                    return false;
                }
                else
                {
                    if (_lastSelected == CurrentSelected)
                    {
                        return _lastInput ? _lastInput.isFocused : false;
                    }

                    _lastSelected = CurrentSelected;
                    _lastInput = _lastSelected.GetComponent<InputField>();
                    return _lastInput ? _lastInput.isFocused : false;
                }
            }
        }

        public override void OnStartUp()
        {
            RegisterVirtualButton(InputButtonType.MouseLeft);
            RegisterVirtualButton(InputButtonType.MouseRight);
            RegisterVirtualButton(InputButtonType.MouseMiddle);
            RegisterVirtualButton(InputButtonType.MouseLeftDoubleClick);

            RegisterVirtualAxis(InputAxisType.MouseX);
            RegisterVirtualAxis(InputAxisType.MouseY);
            RegisterVirtualAxis(InputAxisType.MouseScrollWheel);
            RegisterVirtualAxis(InputAxisType.Horizontal);
            RegisterVirtualAxis(InputAxisType.Vertical);
            RegisterVirtualAxis(InputAxisType.UpperLower);
        }
        
        public override void OnRun()
        {
            //标准PC平台：鼠标和键盘做为输入设备
            if (Input.GetMouseButtonDown(0))
            {
                SetButtonStart(InputButtonType.MouseLeft);
                SetButtonDown(InputButtonType.MouseLeft);
            }

            if (Input.GetMouseButtonUp(0))
            {
                SetButtonUp(InputButtonType.MouseLeft);
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetButtonStart(InputButtonType.MouseRight);
                SetButtonDown(InputButtonType.MouseRight);
            }

            if (Input.GetMouseButtonUp(1))
            {
                SetButtonUp(InputButtonType.MouseRight);
            }

            if (Input.GetMouseButtonDown(2))
            {
                SetButtonStart(InputButtonType.MouseMiddle);
                SetButtonDown(InputButtonType.MouseMiddle);
            }

            if (Input.GetMouseButtonUp(2))
            {
                SetButtonUp(InputButtonType.MouseMiddle);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_mouseLeftClickTimer <= 0)
                {
                    _mouseLeftClickTimer = _mouseLeftDoubleClickInterval;
                    SetButtonStart(InputButtonType.MouseLeftDoubleClick);
                }
                else
                {
                    _mouseLeftClickTimer = 0;
                    SetButtonDown(InputButtonType.MouseLeftDoubleClick);
                    SetButtonUp(InputButtonType.MouseLeftDoubleClick);
                }
            }
            if (_mouseLeftClickTimer > 0)
            {
                _mouseLeftClickTimer -= Time.deltaTime;
            }

            SetAxis(InputAxisType.MouseX, Input.GetAxis("Mouse X"));
            SetAxis(InputAxisType.MouseY, Input.GetAxis("Mouse Y"));
            SetAxis(InputAxisType.MouseScrollWheel, Input.GetAxis("Mouse ScrollWheel"));

            if (IsAnyInputFocused)
            {
                SetAxis(InputAxisType.Horizontal, 0);
                SetAxis(InputAxisType.Vertical, 0);
                SetAxis(InputAxisType.UpperLower, 0);
            }
            else
            {
                SetAxis(InputAxisType.Horizontal, Input.GetAxis("Horizontal"));
                SetAxis(InputAxisType.Vertical, Input.GetAxis("Vertical"));
                SetAxis(InputAxisType.HorizontalRaw, Input.GetAxisRaw("Horizontal"));
                SetAxis(InputAxisType.VerticalRaw, Input.GetAxisRaw("Vertical"));

                if (Input.GetKey(KeyCode.Q)) _upperLowerValue -= Time.deltaTime;
                else if (Input.GetKey(KeyCode.E)) _upperLowerValue += Time.deltaTime;
                else _upperLowerValue = 0;
                SetAxis(InputAxisType.UpperLower, Mathf.Clamp(_upperLowerValue, -1, 1));
            }
            
            SetVirtualMousePosition(Input.mousePosition);
        }
        
        public override void OnShutdown()
        {
            UnRegisterVirtualButton(InputButtonType.MouseLeft);
            UnRegisterVirtualButton(InputButtonType.MouseRight);
            UnRegisterVirtualButton(InputButtonType.MouseMiddle);
            UnRegisterVirtualButton(InputButtonType.MouseLeftDoubleClick);

            UnRegisterVirtualAxis(InputAxisType.MouseX);
            UnRegisterVirtualAxis(InputAxisType.MouseY);
            UnRegisterVirtualAxis(InputAxisType.MouseScrollWheel);
            UnRegisterVirtualAxis(InputAxisType.Horizontal);
            UnRegisterVirtualAxis(InputAxisType.Vertical);
            UnRegisterVirtualAxis(InputAxisType.UpperLower);
        }
    }
}