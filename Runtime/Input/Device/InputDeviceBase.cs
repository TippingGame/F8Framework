using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 输入设备基类
    /// </summary>
    public abstract class InputDeviceBase
    {
        /// <summary>
        /// 是否存在虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>是否存在</returns>
        protected bool IsExistVirtualAxis(string name)
        {
            return InputManager.Instance.IsExistVirtualAxis(name);
        }
        
        /// <summary>
        /// 是否存在虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否存在</returns>
        protected bool IsExistVirtualButton(string name)
        {
            return InputManager.Instance.IsExistVirtualButton(name);
        }
        
        /// <summary>
        /// 注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void RegisterVirtualAxis(string name)
        {
            InputManager.Instance.RegisterVirtualAxis(name);
        }
        
        /// <summary>
        /// 注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void RegisterVirtualButton(string name)
        {
            InputManager.Instance.RegisterVirtualButton(name);
        }
        
        /// <summary>
        /// 取消注册虚拟轴线
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void UnRegisterVirtualAxis(string name)
        {
            InputManager.Instance.UnRegisterVirtualAxis(name);
        }
        
        /// <summary>
        /// 取消注册虚拟按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void UnRegisterVirtualButton(string name)
        {
            InputManager.Instance.UnRegisterVirtualButton(name);
        }

        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <param name="z">z值</param>
        protected void SetVirtualMousePosition(float x, float y, float z)
        {
            InputManager.Instance.SetVirtualMousePosition(new Vector3(x, y, z));
        }
        
        /// <summary>
        /// 设置虚拟鼠标位置
        /// </summary>
        /// <param name="value">鼠标位置</param>
        protected void SetVirtualMousePosition(Vector3 value)
        {
            InputManager.Instance.SetVirtualMousePosition(value);
        }
        
        /// <summary>
        /// 开始按按钮
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void SetButtonStart(string name)
        {
            InputManager.Instance.SetButtonStart(name);
        }
        
        /// <summary>
        /// 设置按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void SetButtonDown(string name)
        {
            InputManager.Instance.SetButtonDown(name);
        }
        
        /// <summary>
        /// 设置按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        protected void SetButtonUp(string name)
        {
            InputManager.Instance.SetButtonUp(name);
        }
        
        /// <summary>
        /// 设置轴线值为正方向1
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void SetAxisPositive(string name)
        {
            InputManager.Instance.SetAxisPositive(name);
        }
        
        /// <summary>
        /// 设置轴线值为负方向-1
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void SetAxisNegative(string name)
        {
            InputManager.Instance.SetAxisNegative(name);
        }
        
        /// <summary>
        /// 设置轴线值为0
        /// </summary>
        /// <param name="name">轴线名称</param>
        protected void SetAxisZero(string name)
        {
            InputManager.Instance.SetAxisZero(name);
        }
        
        /// <summary>
        /// 设置轴线值
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <param name="value">值</param>
        protected void SetAxis(string name, float value)
        {
            InputManager.Instance.SetAxis(name, value);
        }

        /// <summary>
        /// 设备启动
        /// </summary>
        public abstract void OnStartUp();
        
        /// <summary>
        /// 设备运作
        /// </summary>
        public abstract void OnRun();
        
        /// <summary>
        /// 设备关闭
        /// </summary>
        public abstract void OnShutdown();
    }
    
    /// <summary>
    /// 输入按键类型
    /// </summary>
    public static class InputButtonType
    {
        /// <summary>
        /// 鼠标左键
        /// </summary>
        public static string MouseLeft = "MouseLeft";
        /// <summary>
        /// 鼠标右键
        /// </summary>
        public static string MouseRight = "MouseRight";
        /// <summary>
        /// 鼠标中键
        /// </summary>
        public static string MouseMiddle = "MouseMiddle";
        /// <summary>
        /// 鼠标左键双击
        /// </summary>
        public static string MouseLeftDoubleClick = "MouseLeftDoubleClick";
    }

    /// <summary>
    /// 输入轴线类型
    /// </summary>
    public static class InputAxisType
    {
        /// <summary>
        /// 鼠标X轴移动
        /// </summary>
        public static string MouseX = "MouseX";
        /// <summary>
        /// 鼠标Y轴移动
        /// </summary>
        public static string MouseY = "MouseY";
        /// <summary>
        /// 鼠标滚轮滚动
        /// </summary>
        public static string MouseScrollWheel = "MouseScrollWheel";
        /// <summary>
        /// 键盘水平输入
        /// </summary>
        public static string Horizontal = "Horizontal";
        public static string HorizontalRaw = "HorizontalRaw";
        /// <summary>
        /// 键盘垂直输入
        /// </summary>
        public static string Vertical = "Vertical";
        public static string VerticalRaw = "VerticalRaw";
        /// <summary>
        /// 键盘上下输入
        /// </summary>
        public static string UpperLower = "UpperLower";
    }
}