# F8 Input

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 InputManager Component**  
Cross-platform Input System with Hot-Switching Capability
* Unified Codebase - Single implementation works across all platforms
* Custom Input Devices - Define and manage custom control schemes
* Runtime Device Switching - Seamlessly change input methods without restarting
* Platform Adaptation - Auto-adapts to different hardware configurations
* Rebinding
* Local Multiplayer Input

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Instructions for use
1. Use Unity `KeyCode`
   * Example: `FF8.Input.GetKeyDown(KeyCode.M)`
   * Key combinations are also supported, for example: `FF8.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M)`
2. Use custom virtual button strings
   * Custom virtual buttons are mainly for custom input devices, such as `InputButtonType` and `InputAxisType` defined in [InputDeviceBase.cs](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Input/Device/InputDeviceBase.cs)
   * For a virtual-button-based device, refer to [MobileInputDevice](https://github.com/TippingGame/F8Framework/blob/main/Runtime/Input/Device/MobileInputDevice.cs)
   * A typical custom device flow is:
     1. Inherit from `InputDeviceBase`
     2. Register virtual buttons with `RegisterVirtualButton`
     3. Call `SetButtonStart`, `SetButtonDown`, and `SetButtonUp` as needed
     4. Unregister virtual buttons with `UnRegisterVirtualButton`
#### **Important**: If you use the advanced Input System API, you do not need custom virtual button strings. Instead, right-click in the Project window, create an `Input Actions` asset, and edit your ActionMaps, Actions, and Bindings there
3. Use the advanced Input System API
   * Enable `Input System Package (New)` or `Both` in `Active Input Handling`
   * Use `FF8.Input.UseInputSystem(new InputSystemHelper(), inputActionAsset)` to bind an `InputActionAsset`
   * This keeps the unified F8 input entry point while adding `ActionMap`, `Binding`, rebinding, `ControlScheme`, device switching, and local multiplayer support

### Code Examples
The examples below are synchronized with [DemoInput.cs](./DemoInput.cs).

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
        /*------------------------------Basic API------------------------------*/

        // Switch input device without clearing callbacks, Use SwitchControlScheme after switching to Input System
        FF8.Input.SwitchDevice(new StandardInputDevice());

        // Enable or pause input
        FF8.Input.IsEnableInputDevice = false;
        
        // Register button callbacks
        FF8.Input.AddButtonStarted(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.AddButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.AddButtonCanceled(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.AddAxisValueChanged(InputAxisType.MouseX, MouseX);
        
        // Remove callbacks
        FF8.Input.RemoveButtonStarted(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveButtonCanceled(InputButtonType.MouseLeft, MouseLeft);
        FF8.Input.RemoveAxisValueChanged(InputAxisType.MouseX, MouseX);
        
        // Clear callback registrations and reset input state
        FF8.Input.ClearAllAction();
        FF8.Input.ResetAll();

#if ENABLE_INPUT_SYSTEM
        /*--------------------------Advanced Input System API--------------------------*/

        // Create an Input Actions asset from the Project window first, and enable New or Both in Active Input Handling
        FF8.Input.UseInputSystem(new InputSystemHelper(), FF8.Asset.Load<InputActionAsset>("InputSystem_Actions"));

        // ActionMap switching and Action callbacks
        FF8.Input.SwitchActionMap("Player", disableOthers: true);
        FF8.Input.AddButtonStarted("Player/Jump", ActionName);
        FF8.Input.AddButtonPerformed("Player/Jump", ActionName);
        FF8.Input.AddButtonCanceled("Player/Jump", ActionName);
        FF8.Input.AddValueChanged<Vector2>("Move", Move);

        // Action and Binding inspection
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

        // Rebinding
        var bindingIndex = FF8.Input.FindBindingIndex("Player/Jump", "<Keyboard>/space");
        FF8.Input.StartRebind(
            "Player/Jump",
            bindingIndex,
            (actionName, displayString) =>
            {
                LogF8.Log($"{actionName} rebind completed, new key: {displayString}");
            },
            actionName =>
            {
                LogF8.Log($"{actionName} rebind canceled");
            });

        bool isRebinding = FF8.Input.IsRebinding;
        FF8.Input.CancelRebind();
        FF8.Input.RemoveBindingOverride("Player/Jump", 0);
        FF8.Input.RemoveAllBindingOverrides();
        FF8.Input.LoadBindingOverridesFromJson(FF8.Storage.GetString("SaveBindingOverridesAsJson"));
        FF8.Storage.SetString("SaveBindingOverridesAsJson", FF8.Input.SaveBindingOverridesAsJson());

        // ControlScheme and device events
        string currentControlScheme = FF8.Input.CurrentControlScheme;
        FF8.Input.GetControlSchemes();
        FF8.Input.AddControlSchemeChanged(ControlSchemeChanged);
        FF8.Input.AddLastUsedDeviceChanged(LastUsedDeviceChanged);
        FF8.Input.AddDeviceChanged(DeviceChanged);
        FF8.Input.SwitchControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        FF8.Input.ClearControlSchemeBindingMask();

        // Local multiplayer configuration and player events
        FF8.Input.SetPlayerInputManager(new PlayerInputManagerConfig
        {
            // PlayerPrefab must contain the PlayerInput component. Note that the Input Action within the component cannot be the same as the one used by UseInputSystem.
            PlayerPrefab = FF8.Asset.Load<GameObject>("PlayerInput"),
            JoinBehavior = PlayerJoinBehavior.JoinPlayersManually,
            NotificationBehavior = PlayerNotifications.InvokeCSharpEvents,
            MaxPlayerCount = 4
        });

        FF8.Input.AddPlayerJoined(PlayerJoined);
        FF8.Input.AddPlayerLeft(PlayerLeft);
        FF8.Input.AddPlayerControlsChanged(PlayerControlsChanged);

        // Enable joining.
        FF8.Input.DisableJoining();
        FF8.Input.EnableJoining();

        // Manually join the local player.
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
        // Add all available gamepads.
        foreach (var gamepad in Gamepad.all)
        {
            FF8.Input.JoinPlayer(-1, -1, "Gamepad", gamepad);
        }

        // Get all players.
        foreach (var player in FF8.Input.GetAllPlayers())
        {
            var jumpAction = FF8.Input.FindPlayerAction(player.playerIndex, "Player/Jump");
            var playerIndex = player.playerIndex;
            jumpAction.started += _ =>
            {
                LogF8.Log($"P{playerIndex} Jump");
            };
        }

        // After enabling local multiplayer, the API can distinguish player indices.
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
        /*------------------------------Polling Examples------------------------------*/

        if (FF8.Input.AnyKeyDown)
        {
            LogF8.Log("Any key pressed");
        }

        if (FF8.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M))
        {
            LogF8.Log("Pressed Control+Alt+M");
        }

        if (FF8.Input.GetButtonDown(InputButtonType.MouseLeft))
        {
            LogF8.Log("Mouse left button down");
        }

        if (FF8.Input.GetButton(InputButtonType.MouseRight))
        {
            LogF8.Log("Mouse right button held");
        }

        LogF8.Log("Mouse wheel: " + FF8.Input.GetAxis(InputAxisType.MouseScrollWheel));
        LogF8.Log("Horizontal axis: " + FF8.Input.GetAxis(InputAxisType.Horizontal));
        LogF8.Log("Vertical axis: " + FF8.Input.GetAxis(InputAxisType.Vertical));

#if ENABLE_INPUT_SYSTEM
        if (FF8.Input.HasInputActionAsset)
        {
            LogF8.Log("Current ActionMap: " + FF8.Input.CurrentActionMapName);
            LogF8.Log("Current ControlScheme: " + FF8.Input.CurrentControlScheme);
            LogF8.Log("Move: " + FF8.Input.ReadValue<Vector2>("Move"));

            if (FF8.Input.WasPressedThisFrame("Player/Jump"))
            {
                LogF8.Log("Jump pressed this frame");
            }

            if (FF8.Input.WasReleasedThisFrame("Player/Jump"))
            {
                LogF8.Log("Jump released this frame");
            }

            if (FF8.Input.PlayerCount > 0 && FF8.Input.IsPressed(0, "Player/Jump"))
            {
                LogF8.Log("Player0 is holding Jump");
            }
        }
#endif
    }

    void OnDestroy()
    {
        /*------------------------------Cleanup------------------------------*/

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
