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
2. For usage docs, read: Assets/F8Framework/Tests/Input/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Input
- Test docs: Assets/F8Framework/Tests/Input

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
2. Register button/axis callbacks in Start or OnEnable.
3. Use polling in Update for frame-by-frame input queries.
4. Switch devices at runtime without losing callback registrations.
5. For mobile, implement custom `MobileInputDevice` with virtual joystick.
6. Disable input during UI menus with `IsEnableInputDevice = false`.

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
