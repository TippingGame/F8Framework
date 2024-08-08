# F8 Input

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 Input输入管理组件。
1. 使用同一套代码，通过自定义输入设备，适配多平台，可热切换输入设备。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 代码使用方法
```C#
/*------------------------------输入管理方法------------------------------*/

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
        
        void MouseLeft(string name)
        {
            
        }
        
        void MouseX(float value)
        {
        
        }

/*------------------------------按键监听使用------------------------------*/
        
        // 任意键按下
        if (FF8.Input.AnyKeyDown)
        {
            
        }
        
        // 按下组合键
        if (FF8.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M))
        {
            
        }
        
        // 鼠标右键按住
        if (FF8.Input.GetButton(InputButtonType.MouseRight))
        {
            
        }
        
        // 鼠标左键双击
        if (FF8.Input.GetButtonDown(InputButtonType.MouseLeftDoubleClick))
        {
            
        }
        
        LogF8.Log("滚轮：" + FF8.Input.GetAxis(InputAxisType.MouseScrollWheel));
        LogF8.Log("水平轴线值：" + FF8.Input.GetAxis(InputAxisType.Horizontal));
        LogF8.Log("垂直轴线值：" + FF8.Input.GetAxis(InputAxisType.Vertical));
```


