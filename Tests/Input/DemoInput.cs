using System;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoInput : MonoBehaviour
    {
        void Start()
        {
            // 切换输入设备
            FF8.Input.SwitchDevice(new StandardInputDevice());

            // 启用或暂停输入
            FF8.Input.IsEnableInputDevice = false;
        }

        void Update()
        {
            if (FF8.Input.AnyKeyDown)
            {
                LogF8.Log("任意键按下");
            }

            if (FF8.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M))
            {
                LogF8.Log("按下组合键Control+Alt+M");
            }

            if (FF8.Input.GetButtonDown(InputButtonType.MouseLeft))
            {
                LogF8.Log("鼠标左键按下");
            }

            if (FF8.Input.GetButton(InputButtonType.MouseRight))
            {
                LogF8.Log("鼠标右键按住");
            }

            if (FF8.Input.GetButtonDown(InputButtonType.MouseLeftDoubleClick))
            {
                LogF8.Log("鼠标左键双击");
            }

            LogF8.Log("滚轮：" + FF8.Input.GetAxis(InputAxisType.MouseScrollWheel));
            LogF8.Log("水平轴线值：" + FF8.Input.GetAxis(InputAxisType.Horizontal));
            LogF8.Log("垂直轴线值：" + FF8.Input.GetAxis(InputAxisType.Vertical));
        }
    }
}
