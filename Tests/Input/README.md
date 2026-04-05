# F8 Input

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
**Unity F8 Input 输入管理组件**  
* 使用同一套代码
* 通过自定义输入设备
* 适配多平台
* 可热切换输入设备
* 重绑定
* 本地多人输入

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 视频教程：[【Unity框架】（15）输入系统管理](https://www.bilibili.com/video/BV1kVXDYDEXj)

### 使用方法
1. 使用 Unity 定义的 `KeyCode`
   * 例如：`FF8.Input.GetKeyDown(KeyCode.M)`
   * 也支持组合键，例如：`FF8.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M)`
2. 使用自定义的虚拟按键字符串
   * 自定义虚拟按键主要用于自定义输入设备，例如 [InputDeviceBase.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Input/Device/InputDeviceBase.cs) 中的 `InputButtonType` 和 `InputAxisType`
   * 监听虚拟按键可参考 [MobileInputDevice](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Input/Device/MobileInputDevice.cs)
   * 自定义输入设备的一般流程：
     1. 继承 `InputDeviceBase`
     2. 注册虚拟按键 `RegisterVirtualButton`
     3. 按需调用 `SetButtonStart`、`SetButtonDown`、`SetButtonUp`
     4. 取消注册虚拟按键 `UnRegisterVirtualButton`
#### **注意**：如果使用 Input System 进阶 API，就不需要自定义虚拟按键字符串，而是在资产栏右键创建 `Input Actions` 文件并编辑 ActionMap、Action、Binding
3. 使用 Input System 进阶 API
   * 需要在 `Active Input Handling` 中开启 `Input System Package (New)` 或 `Both`
   * 使用 `FF8.Input.UseInputSystem(new InputSystemHelper(), inputActionAsset)` 接入 `InputActionAsset`
   * 可以继续使用 F8 的统一输入入口，同时获得 `ActionMap`、`Binding`、重绑定、`ControlScheme`、设备切换和本地多人支持

### 代码使用方法
下面示例与 [DemoInput.cs](./DemoInput.cs) 保持同步。

```csharp
using F8Framework.Core;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class DemoInput : MonoBehaviour
{
    void Start()
    {
        /*------------------------------基础 API------------------------------*/

        // 切换输入设备（不会清理回调，方便热切换输入设备），改用 Input System 后使用 SwitchControlScheme
        FF8.Input.SwitchDevice(new StandardInputDevice());

        // 启用或暂停输入
        FF8.Input.IsEnableInputDevice = false;
        
        // 设置按钮回调，Started 开始按按钮，Performed 按下按钮，Canceled 结束按钮
        FF8.Input.AddButtonStarted(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.AddButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.AddButtonCanceled(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.AddAxisValueChanged(InputAxisType.MouseX, MouseX);
        
        // 移除按钮回调
        FF8.Input.RemoveButtonStarted(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveButtonCanceled(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveAxisValueChanged(InputAxisType.MouseX, MouseX);
        
        // 移除所有输入回调和输入状态
        FF8.Input.ClearAllAction();
        FF8.Input.ResetAll();

#if ENABLE_INPUT_SYSTEM
        /*--------------------------Input System 进阶 API--------------------------*/

        // 需要先在资产栏右键创建 Input Actions 文件，并在 Active Input Handling 中开启 New 或 Both
        FF8.Input.UseInputSystem(new InputSystemHelper(), FF8.Asset.Load<InputActionAsset>("InputSystem_Actions"));

        // ActionMap 切换和 Action 事件
        FF8.Input.SwitchActionMap("Player", disableOthers: true);
        FF8.Input.AddButtonStarted("Player/Jump", ActionName);
        FF8.Input.AddButtonPerformed("Player/Jump", ActionName);
        FF8.Input.AddButtonCanceled("Player/Jump", ActionName);
        FF8.Input.AddValueChanged<Vector2>("Move", Move);

        // 获取 Action 和 Binding 信息
        FF8.Input.IsActionMapEnabled("UI");
        FF8.Input.GetEnabledActionMaps();
        FF8.Input.GetBindings("Player/Jump");
        FF8.Input.GetBindingCount("Player/Jump");
        FF8.Input.FindBindingIndex("Player/Jump", "<Keyboard>/space");
        FF8.Input.GetBindingDisplayString("Player/Jump", -1);
        FF8.Input.GetBindingPath("Player/Jump", 0);
        FF8.Input.HasActionMap("UI");
        FF8.Input.HasAction("UI/Click");
        FF8.Input.FindActionMap("UI");
        FF8.Input.FindAction("UI/Click");

        // 按键重绑定
        var bindingIndex = FF8.Input.FindBindingIndex("Player/Jump", "<Keyboard>/space");
        FF8.Input.StartRebind(
            "Player/Jump",
            bindingIndex,
            (actionName, displayString) =>
            {
                LogF8.Log($"{actionName} 重绑定完成，新按键: {displayString}");
            },
            actionName =>
            {
                LogF8.Log($"{actionName} 重绑定取消");
            });

        bool isRebinding = FF8.Input.IsRebinding;
        FF8.Input.CancelRebind();
        FF8.Input.RemoveBindingOverride("Player/Jump", 0);
        FF8.Input.RemoveAllBindingOverrides();
        FF8.Input.LoadBindingOverridesFromJson(FF8.Storage.GetString("SaveBindingOverridesAsJson"));
        FF8.Storage.SetString("SaveBindingOverridesAsJson", FF8.Input.SaveBindingOverridesAsJson());

        // ControlScheme 和设备事件
        string currentControlScheme = FF8.Input.CurrentControlScheme;
        FF8.Input.GetControlSchemes();
        FF8.Input.AddControlSchemeChanged(ControlSchemeChanged);
        FF8.Input.AddLastUsedDeviceChanged(LastUsedDeviceChanged);
        FF8.Input.AddDeviceChanged(DeviceChanged);
        FF8.Input.SwitchControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        FF8.Input.ClearControlSchemeBindingMask();

        // 本地多人配置和玩家事件
        FF8.Input.SetPlayerInputManager(new PlayerInputManagerConfig
        {
            // PlayerPrefab 必须含有 PlayerInput 组件，注意组件内 Input Action 不能与 UseInputSystem 使用的相同
            PlayerPrefab = FF8.Asset.Load<GameObject>("PlayerInput"),
            JoinBehavior = PlayerJoinBehavior.JoinPlayersManually,
            NotificationBehavior = PlayerNotifications.InvokeCSharpEvents,
            MaxPlayerCount = 4
        });

        FF8.Input.AddPlayerJoined(PlayerJoined);
        FF8.Input.AddPlayerLeft(PlayerLeft);
        FF8.Input.AddPlayerControlsChanged(PlayerControlsChanged);

        // 启用加入
        FF8.Input.DisableJoining();
        FF8.Input.EnableJoining();

        // 手动加入本地玩家
        if (FF8.Input.PlayerCount == 0 &&
            FF8.Input.HasControlScheme("Keyboard&Mouse") &&
            Keyboard.current != null &&
            Mouse.current != null)
        {
            FF8.Input.JoinPlayer(0, -1, "Keyboard&Mouse", Keyboard.current, Mouse.current);
        }

        if (FF8.Input.PlayerCount < 2 &&
            FF8.Input.HasControlScheme("Gamepad") &&
            Gamepad.current != null)
        {
            FF8.Input.JoinPlayer(1, -1, "Gamepad", Gamepad.current);
        }
        // 添加所有可用的 Gamepad
        foreach (var gamepad in Gamepad.all)
        {
            FF8.Input.JoinPlayer(-1, -1, "Gamepad", gamepad);
        }

        // 获取所有玩家
        foreach (var player in FF8.Input.GetAllPlayers())
        {
            var jumpAction = FF8.Input.FindPlayerAction(player.playerIndex, "Player/Jump");
            var playerIndex = player.playerIndex;
            jumpAction.started += _ =>
            {
                LogF8.Log($"P{playerIndex} Jump");
            };
        }

        // 启用本地多人后 API 可区分玩家 Index
        FF8.Input.AddButtonStarted(1, "Player/Jump", ActionName);
        FF8.Input.AddButtonPerformed(1, "Player/Jump", ActionName);
        FF8.Input.AddButtonCanceled(1, "Player/Jump", ActionName);
        FF8.Input.AddValueChanged<Vector2>(1, "Move", Move);
        FF8.Input.SwitchControlScheme(1, "Gamepad", Gamepad.current);
        FF8.Input.ClearControlSchemeBindingMask(1);
        FF8.Input.GetControlScheme(1);
        FF8.Input.GetCurrentActionMapName(1);
        FF8.Input.HasActionMap(1, "Player");
        FF8.Input.HasAction(1, "Player/Jump");
        FF8.Input.FindActionMap(1, "Player");
        FF8.Input.EnableActionMap(1, "Player");
        FF8.Input.DisableActionMap(1, "UI");
        FF8.Input.IsActionMapEnabled(1, "Player");
        FF8.Input.SwitchActionMap(1, "Player", disableOthers: true);
        FF8.Input.GetEnabledActionMaps(1);
        FF8.Input.EnableAllActionMaps(1);
        FF8.Input.DisableAllActionMaps(1);
        FF8.Input.GetButton(1, "Player/Jump");
        FF8.Input.GetButtonUp(1, "Player/Jump");
        FF8.Input.GetButtonDown(1, "Player/Jump");
        FF8.Input.WasPressedThisFrame(1, "Player/Jump");
        FF8.Input.WasReleasedThisFrame(1, "Player/Jump");
        FF8.Input.IsPressed(1, "Player/Jump");
#endif
    }

    void Update()
    {
        /*------------------------------查询示例------------------------------*/

        if (FF8.Input.AnyKeyDown)
        {
            LogF8.Log("任意键按下");
        }

        if (FF8.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M))
        {
            LogF8.Log("按下组合键 Control+Alt+M");
        }

        if (FF8.Input.GetButtonDown(InputButtonType.MouseLeft))
        {
            LogF8.Log("鼠标左键按下");
        }

        if (FF8.Input.GetButton(InputButtonType.MouseRight))
        {
            LogF8.Log("鼠标右键按住");
        }

        LogF8.Log("滚轮：" + FF8.Input.GetAxis(InputAxisType.MouseScrollWheel));
        LogF8.Log("水平轴线值：" + FF8.Input.GetAxis(InputAxisType.Horizontal));
        LogF8.Log("垂直轴线值：" + FF8.Input.GetAxis(InputAxisType.Vertical));

#if ENABLE_INPUT_SYSTEM
        if (FF8.Input.HasInputActionAsset)
        {
            LogF8.Log("当前 ActionMap：" + FF8.Input.CurrentActionMapName);
            LogF8.Log("当前 ControlScheme：" + FF8.Input.CurrentControlScheme);
            LogF8.Log("Move：" + FF8.Input.ReadValue<Vector2>("Move"));

            if (FF8.Input.WasPressedThisFrame("Player/Jump"))
            {
                LogF8.Log("Jump 本帧按下");
            }

            if (FF8.Input.WasReleasedThisFrame("Player/Jump"))
            {
                LogF8.Log("Jump 本帧抬起");
            }

            if (FF8.Input.PlayerCount > 0 && FF8.Input.IsPressed(0, "Player/Jump"))
            {
                LogF8.Log("Player0 正在按住 Jump");
            }
        }
#endif
    }

    void OnDestroy()
    {
        /*------------------------------销毁清理------------------------------*/

        FF8.Input.RemoveButtonStarted(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveButtonCanceled(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveAxisValueChanged(InputAxisType.MouseX, MouseX);

#if ENABLE_INPUT_SYSTEM
        FF8.Input.RemoveButtonStarted("Player/Jump", ActionName);
        FF8.Input.RemoveButtonPerformed("Player/Jump", ActionName);
        FF8.Input.RemoveButtonCanceled("Player/Jump", ActionName);
        FF8.Input.RemoveValueChanged<Vector2>("Move", Move);
        FF8.Input.CancelRebind();

        FF8.Input.RemoveControlSchemeChanged(ControlSchemeChanged);
        FF8.Input.RemoveLastUsedDeviceChanged(LastUsedDeviceChanged);
        FF8.Input.RemoveDeviceChanged(DeviceChanged);

        FF8.Input.RemovePlayerJoined(PlayerJoined);
        FF8.Input.RemovePlayerLeft(PlayerLeft);
        FF8.Input.RemovePlayerControlsChanged(PlayerControlsChanged);
        FF8.Input.SetPlayerInputManager(null);
#endif
    }

    void MouseLeft(string name) {}
    void MouseX(float value) {}

#if ENABLE_INPUT_SYSTEM
    void ActionName(string name) {}
    void Move(Vector2 value) {}
    void ControlSchemeChanged(string scheme) {}
    void LastUsedDeviceChanged(InputDevice device) {}
    void DeviceChanged(InputDevice device, InputDeviceChange change) {}
    void PlayerJoined(PlayerInput player) {}
    void PlayerLeft(PlayerInput player) {}
    void PlayerControlsChanged(PlayerInput player) {}
#endif
}
```
