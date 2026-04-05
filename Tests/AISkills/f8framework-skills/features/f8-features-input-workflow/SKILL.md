---
name: f8-features-input-workflow
description: Use when implementing or troubleshooting Input feature workflows — multi-platform input, virtual buttons, device switching, and key/axis listening in F8Framework.
---

# Input Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.Input = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());`.


## Use this skill when

- The task is about input management, virtual buttons, or device switching.
- The user asks about multi-platform input handling or key mapping.
- Troubleshooting input not responding or device switch issues.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Input/README.md and Assets/F8Framework/Tests/Input/README_EN.md
3. For the latest usage sample, read: Assets/F8Framework/Tests/Input/DemoInput.cs

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Input
- Test docs: Assets/F8Framework/Tests/Input
- Runtime usage sample: Assets/F8Framework/Tests/Input/DemoInput.cs

## Key classes and interfaces

| Class | Role |
|-------|------|
| `InputManager` | Core module. Access via `FF8.Input`. |
| `InputDeviceBase` | Abstract base for input devices. Defines `InputButtonType` and `InputAxisType`. |
| `StandardInputDevice` | Default PC keyboard/mouse device. |
| `MobileInputDevice` | Mobile touch/virtual button device. |

## API quick reference

### Device management
```csharp
FF8.Input.SwitchDevice(new StandardInputDevice()); // Hot-switch device (keeps callbacks)
FF8.Input.IsEnableInputDevice = false;              // Enable/disable input
```

### Button callbacks
```csharp
// Register callbacks: Started (press begin), Performed (held), Canceled (released)
FF8.Input.AddButtonStarted(InputButtonType.MouseLeft, OnButton);
FF8.Input.AddButtonPerformed(InputButtonType.MouseLeft, OnButton);
FF8.Input.AddButtonCanceled(InputButtonType.MouseLeft, OnButton);
FF8.Input.AddAxisValueChanged(InputAxisType.MouseX, OnAxis);

// Remove callbacks
FF8.Input.RemoveButtonStarted(InputButtonType.MouseLeft, OnButton);
FF8.Input.RemoveButtonPerformed(InputButtonType.MouseLeft, OnButton);
FF8.Input.RemoveButtonCanceled(InputButtonType.MouseLeft, OnButton);
FF8.Input.RemoveAxisValueChanged(InputAxisType.MouseX, OnAxis);

// Clear all
FF8.Input.ClearAllAction();
FF8.Input.ResetAll();

void OnButton(string name) { }
void OnAxis(float value) { }
```

### Key polling (Update)
```csharp
// Unity KeyCode
if (FF8.Input.GetKeyDown(KeyCode.M)) { }
// Combo keys
if (FF8.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M)) { }
// Virtual buttons
if (FF8.Input.GetButton(InputButtonType.MouseRight)) { }
if (FF8.Input.GetButtonDown(InputButtonType.MouseLeftDoubleClick)) { }
// Any key
if (FF8.Input.AnyKeyDown) { }
// Axis values
float scroll = FF8.Input.GetAxis(InputAxisType.MouseScrollWheel);
float h = FF8.Input.GetAxis(InputAxisType.Horizontal);
float v = FF8.Input.GetAxis(InputAxisType.Vertical);
```

### Virtual buttons vs Input Actions

- Use custom virtual button strings when implementing a custom `InputDeviceBase`.
- Register them with `RegisterVirtualButton`, drive them with `SetButtonStart`, `SetButtonDown`, `SetButtonUp`, and unregister with `UnRegisterVirtualButton`.
- If the task uses the advanced Unity Input System API, do not add custom virtual button strings by default.
- Instead, create an `Input Actions` asset from the Project window, then edit ActionMaps, Actions, and Bindings there.

### Advanced Input System API
```csharp
// Requires Active Input Handling = Input System Package (New) or Both
FF8.Input.UseInputSystem(new InputSystemHelper(), inputActionAsset);

// ActionMap / Action callbacks
FF8.Input.SwitchActionMap("Player", disableOthers: true);
FF8.Input.AddButtonStarted("Player/Jump", OnAction);
FF8.Input.AddButtonPerformed("Player/Jump", OnAction);
FF8.Input.AddButtonCanceled("Player/Jump", OnAction);
FF8.Input.AddValueChanged<Vector2>("Move", OnMove);

// Action / Binding inspection
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
int bindingIndex = FF8.Input.FindBindingIndex("Player/Jump", "<Keyboard>/space");
FF8.Input.StartRebind("Player/Jump", bindingIndex, OnRebindCompleted, OnRebindCanceled);
bool isRebinding = FF8.Input.IsRebinding;
FF8.Input.CancelRebind();
FF8.Input.RemoveBindingOverride("Player/Jump", 0);
FF8.Input.RemoveAllBindingOverrides();
FF8.Input.LoadBindingOverridesFromJson(savedJson);
string savedJson = FF8.Input.SaveBindingOverridesAsJson();

// ControlScheme / devices
string currentControlScheme = FF8.Input.CurrentControlScheme;
FF8.Input.GetControlSchemes();
FF8.Input.AddControlSchemeChanged(OnSchemeChanged);
FF8.Input.AddLastUsedDeviceChanged(OnLastUsedDeviceChanged);
FF8.Input.AddDeviceChanged(OnDeviceChanged);
FF8.Input.SwitchControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
FF8.Input.ClearControlSchemeBindingMask();

// Runtime state query
if (FF8.Input.HasInputActionAsset)
{
    string currentActionMap = FF8.Input.CurrentActionMapName;
    Vector2 move = FF8.Input.ReadValue<Vector2>("Move");
}
```

### Local multiplayer with Input System
```csharp
FF8.Input.SetPlayerInputManager(new PlayerInputManagerConfig
{
    PlayerPrefab = FF8.Asset.Load<GameObject>("PlayerInput"),
    JoinBehavior = PlayerJoinBehavior.JoinPlayersManually,
    NotificationBehavior = PlayerNotifications.InvokeCSharpEvents,
    MaxPlayerCount = 4
});

FF8.Input.AddPlayerJoined(OnPlayerJoined);
FF8.Input.AddPlayerLeft(OnPlayerLeft);
FF8.Input.AddPlayerControlsChanged(OnPlayerControlsChanged);

FF8.Input.DisableJoining();
FF8.Input.EnableJoining();

if (FF8.Input.PlayerCount == 0 && FF8.Input.HasControlScheme("Keyboard&Mouse"))
{
    FF8.Input.JoinPlayer(0, -1, "Keyboard&Mouse", Keyboard.current, Mouse.current);
}

if (FF8.Input.PlayerCount < 2 && FF8.Input.HasControlScheme("Gamepad"))
{
    FF8.Input.JoinPlayer(1, -1, "Gamepad", Gamepad.current);
}

foreach (var player in FF8.Input.GetAllPlayers())
{
    var jumpAction = FF8.Input.FindPlayerAction(player.playerIndex, "Player/Jump");
}

FF8.Input.AddButtonStarted(1, "Player/Jump", OnAction);
FF8.Input.AddButtonPerformed(1, "Player/Jump", OnAction);
FF8.Input.AddButtonCanceled(1, "Player/Jump", OnAction);
FF8.Input.AddValueChanged<Vector2>(1, "Move", OnMove);
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

// Cleanup in OnDestroy
FF8.Input.RemoveButtonPerformed("Player/Jump", OnAction);
FF8.Input.RemoveButtonCanceled("Player/Jump", OnAction);
FF8.Input.RemoveValueChanged<Vector2>("Move", OnMove);
FF8.Input.RemoveControlSchemeChanged(OnSchemeChanged);
FF8.Input.RemoveLastUsedDeviceChanged(OnLastUsedDeviceChanged);
FF8.Input.RemoveDeviceChanged(OnDeviceChanged);
FF8.Input.RemovePlayerJoined(OnPlayerJoined);
FF8.Input.RemovePlayerLeft(OnPlayerLeft);
FF8.Input.RemovePlayerControlsChanged(OnPlayerControlsChanged);
```

### Custom input device
```csharp
public class MyDevice : InputDeviceBase
{
    public override void OnInit()
    {
        RegisterVirtualButton("Jump");
    }
    public override void OnUpdate()
    {
        if (/* jump condition */) SetButtonDown("Jump");
    }
    public override void OnDestroy()
    {
        UnRegisterVirtualButton("Jump");
    }
}
// Set hotkeys or check source in FF8.Input.CurrentDevice
```

## Workflow

1. Choose or create an input device (Standard, Mobile, or custom).
2. If using a custom device, define and drive virtual buttons on `InputDeviceBase`.
3. If using Unity Input System advanced APIs, create an `Input Actions` asset instead of inventing virtual button strings.
4. Register button or action callbacks in `Start` or `OnEnable`.
5. Use polling in `Update` for frame-by-frame input queries.
6. Switch devices at runtime without losing callback registrations.
7. For mobile, implement a custom `MobileInputDevice` with virtual joystick logic.
8. Disable input during UI menus with `IsEnableInputDevice = false`.
9. Remove callbacks and player-input hooks in `OnDestroy`.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Input not responding | `IsEnableInputDevice` is false | Set to true |
| Callback fires twice | Registered both callback and polling | Choose one pattern |
| Custom device not working | Missing `RegisterVirtualButton` | Register all virtual buttons in `OnInit` |

## Cross-module dependencies

- None — Input is self-contained.

## Output checklist

- Input device selected and configured.
- Callbacks or polling pattern implemented.
- Files changed and why.
- Validation status and remaining risks.
