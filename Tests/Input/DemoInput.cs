using System;
using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoInput : MonoBehaviour
    {
        void Start()
        {
            // 切换输入设备（不会清理回调，方便热切换输入设备）
            FF8.Input.SwitchDevice(new StandardInputDevice());

            // 启用或暂停输入
            FF8.Input.IsEnableInputDevice = false;
            
            // 设置按钮回调，Started开始按按钮，Performed按下按钮，Canceled结束按钮
            FF8.Input.AddButtonStarted(InputButtonType.MouseLeft, MouseLeft);
            FF8.Input.AddButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
            FF8.Input.AddButtonCanceled(InputButtonType.MouseLeft, MouseLeft);
            
            FF8.Input.AddAxisValueChanged(InputAxisType.MouseX, MouseX);
            
            // 移除按钮回调
            FF8.Input.RemoveButtonStarted(InputButtonType.MouseLeft, MouseLeft);
            FF8.Input.RemoveButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
            FF8.Input.RemoveButtonCanceled(InputButtonType.MouseLeft, MouseLeft);

            FF8.Input.RemoveAxisValueChanged(InputAxisType.MouseX, MouseX);
            
            // 移除所有输入回调
            FF8.Input.ClearAllAction();
            
            // 移除所有输入状态
            FF8.Input.ResetAll();
        }

        // 鼠标左键回调
        void MouseLeft(string name)
        {
            
        }
        
        // 鼠标X轴移动
        void MouseX(float value)
        {
        
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
