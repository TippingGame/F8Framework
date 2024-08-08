using System;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 输入管理器的助手接口
    /// </summary>
    public interface IInputHelper
    {
        /// <summary>
        /// 输入设备
        /// </summary>
        InputDeviceBase Device { get; }
        
        /// <summary>
        /// 是否启用输入设备
        /// </summary>
        bool IsEnableInputDevice { get; set; }
        
        /// <summary>
        /// 鼠标位置
        /// </summary>
        Vector3 MousePosition { get; }
        
        /// <summary>
        /// 任意键按住
        /// </summary>
        bool AnyKey { get; }
        
        /// <summary>
        /// 任意键按下
        /// </summary>
        bool AnyKeyDown { get; }

        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        bool IsExistVirtualAxis(string name);
        
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        bool IsExistVirtualButton(string name);
        
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        void RegisterVirtualAxis(string name);
        
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        void RegisterVirtualButton(string name);
        
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        void UnRegisterVirtualAxis(string name);
        
        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        void UnRegisterVirtualButton(string name);

        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        void SetVirtualMousePosition(Vector3 value);
        
        /// <summary>
        /// 开始按按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonStart(string name);
        
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonDown(string name);
        
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        void SetButtonUp(string name);
        
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisPositive(string name);
        
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisNegative(string name);
        
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        void SetAxisZero(string name);
        
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">轴线值</param>
        void SetAxis(string name, float value);

        /// <summary>
        /// 开始按按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="started">回调</param>
        void AddButtonStarted(string name, Action<string> started);
        
        /// <summary>
        /// 按下按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="performed">回调</param>
        void AddButtonPerformed(string name, Action<string> performed);
        
        /// <summary>
        /// 结束按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="canceled">回调</param>
        void AddButtonCanceled(string name, Action<string> canceled);

        /// <summary>
        /// Axis值改变Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="valueChanged">回调</param>
        void AddAxisValueChanged(string name, Action<float> valueChanged);
        
        /// <summary>
        /// 移除开始按按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="started">回调</param>
        void RemoveButtonStarted(string name, Action<string> started);
        
        /// <summary>
        /// 移除按下按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="performed">回调</param>
        void RemoveButtonPerformed(string name, Action<string> performed);
        
        /// <summary>
        /// 移除结束按钮Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="canceled">回调</param>
        void RemoveButtonCanceled(string name, Action<string> canceled);
        
        /// <summary>
        /// 移除Axis值改变Action
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <param name="valueChanged">回调</param>
        void RemoveAxisValueChanged(string name, Action<float> valueChanged);

        /// <summary>
        /// 清除所有输入事件
        /// </summary>
        void ClearAllAction();
        
        /// <summary>
        /// 按钮按住
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按住</returns>
        bool GetButton(string name);
        
        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        bool GetButtonDown(string name);
        
        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        bool GetButtonUp(string name);
        
        /// <summary>
        /// 获取轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="raw">是否获取整数值</param>
        /// <returns>轴线值</returns>
        float GetAxis(string name, bool raw);

        /// <summary>
        /// 键盘按键按住
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按住</returns>
        bool GetKey(KeyCode keyCode);
        
        /// <summary>
        /// 键盘按键按下
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否按下</returns>
        bool GetKeyDown(KeyCode keyCode);
        
        /// <summary>
        /// 键盘按键抬起
        /// </summary>
        /// <param name="keyCode">按键代码</param>
        /// <returns>是否抬起</returns>
        bool GetKeyUp(KeyCode keyCode);
        
        /// <summary>
        /// 键盘组合按键按住（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按住</returns>
        bool GetKey(KeyCode keyCode1, KeyCode keyCode2);
        
        /// <summary>
        /// 键盘组合按键按下（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否按下</returns>
        bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2);
        
        /// <summary>
        /// 键盘组合按键抬起（两个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <returns>是否抬起</returns>
        bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2);
        
        /// <summary>
        /// 键盘组合按键按住（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否按住</returns>
        bool GetKey(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3);
        
        /// <summary>
        /// 键盘组合按键按下（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否按下</returns>
        bool GetKeyDown(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3);
        
        /// <summary>
        /// 键盘组合按键抬起（三个组合键）
        /// </summary>
        /// <param name="keyCode1">按键1代码</param>
        /// <param name="keyCode2">按键2代码</param>
        /// <param name="keyCode3">按键3代码</param>
        /// <returns>是否抬起</returns>
        bool GetKeyUp(KeyCode keyCode1, KeyCode keyCode2, KeyCode keyCode3);

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit();

        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate();

        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate();

        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause();

        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume();
        
        /// <summary>
        /// 加载输入设备
        /// </summary>
        /// <param name="deviceType">输入设备类型</param>
        void LoadDevice(InputDeviceBase deviceType);
        
        /// <summary>
        /// 清除所有输入状态
        /// </summary>
        void ResetAll();
    }
}