using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 移动输入设备
    /// </summary>
    public class MobileInputDevice : InputDeviceBase
    {
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
            if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
            {
                SetButtonStart(InputButtonType.MouseLeft);
                SetButtonDown(InputButtonType.MouseLeft);
            }
            else if (UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                SetButtonUp(InputButtonType.MouseLeft);
            }

            if (UnityEngine.Input.touchCount == 2 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began &&
                UnityEngine.Input.GetTouch(1).phase == TouchPhase.Began)
            {
                SetButtonStart(InputButtonType.MouseRight);
                SetButtonDown(InputButtonType.MouseRight);
            }
            else
            {
                SetButtonUp(InputButtonType.MouseRight);
            }

            if (UnityEngine.Input.touchCount == 3 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began &&
                UnityEngine.Input.GetTouch(1).phase == TouchPhase.Began &&
                UnityEngine.Input.GetTouch(2).phase == TouchPhase.Began)
            {
                SetButtonStart(InputButtonType.MouseMiddle);
                SetButtonDown(InputButtonType.MouseMiddle);
            }
            else
            {
                SetButtonUp(InputButtonType.MouseMiddle);
            }

            SetAxis(InputAxisType.MouseX, UnityEngine.Input.GetAxis("Horizontal"));
            SetAxis(InputAxisType.MouseY, UnityEngine.Input.GetAxis("Vertical"));
            SetAxis(InputAxisType.MouseScrollWheel, 0);
            SetAxis(InputAxisType.Horizontal, UnityEngine.Input.GetAxis("Horizontal"));
            SetAxis(InputAxisType.Vertical, UnityEngine.Input.GetAxis("Vertical"));
            SetAxis(InputAxisType.HorizontalRaw, UnityEngine.Input.GetAxisRaw("Horizontal"));
            SetAxis(InputAxisType.VerticalRaw, UnityEngine.Input.GetAxisRaw("Vertical"));
            if (UnityEngine.Input.touchCount == 1)
            {
                SetVirtualMousePosition(UnityEngine.Input.GetTouch(0).position);
            }
            else
            {
                SetVirtualMousePosition(Vector3.zero);
            }
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