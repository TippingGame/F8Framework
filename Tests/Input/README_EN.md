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

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
/*------------------------------Input Management Methods------------------------------*/
void Start()
{
    // Switch input device (doesn't clear callbacks, enables hot-switching)
    FF8.Input.SwitchDevice(new StandardInputDevice());

    // Enable/disable input
    FF8.Input.IsEnableInputDevice = false;
    
    // Set button callbacks:
    // Started - when button begins pressing
    // Performed - when button is pressed  
    // Canceled - when button is released
    FF8.Input.AddButtonStarted(InputButtonType.MouseLeft, MouseLeft);
    FF8.Input.AddButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
    FF8.Input.AddButtonCanceled(InputButtonType.MouseLeft, MouseLeft);
    
    FF8.Input.AddAxisValueChanged(InputAxisType.MouseX, MouseX);
    
    // Remove button callbacks
    FF8.Input.RemoveButtonStarted(InputButtonType.MouseLeft, MouseLeft);
    FF8.Input.RemoveButtonPerformed(InputButtonType.MouseLeft, MouseLeft);
    FF8.Input.RemoveButtonCanceled(InputButtonType.MouseLeft, MouseLeft);

    FF8.Input.RemoveAxisValueChanged(InputAxisType.MouseX, MouseX);
    
    // Clear all input callbacks
    FF8.Input.ClearAllAction();
    
    // Reset all input states
    FF8.Input.ResetAll();
}

// Mouse left button callback
void MouseLeft(string name)
{
    
}

// Mouse X-axis movement
void MouseX(float value)
{

}


/*------------------------------Key Listening Usage------------------------------*/
void Update()
{
    // Any key pressed
    if (FF8.Input.AnyKeyDown)
    {
        
    }
    
    // Key combination pressed
    if (FF8.Input.GetKeyDown(KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.M))
    {
        
    }
    
    // Mouse right button held
    if (FF8.Input.GetButton(InputButtonType.MouseRight))
    {
        
    }
    
    // Mouse left button double click
    if (FF8.Input.GetButtonDown(InputButtonType.MouseLeftDoubleClick))
    {
        
    }
    
    LogF8.Log("Mouse wheel: " + FF8.Input.GetAxis(InputAxisType.MouseScrollWheel));
    LogF8.Log("Horizontal axis: " + FF8.Input.GetAxis(InputAxisType.Horizontal));
    LogF8.Log("Vertical axis: " + FF8.Input.GetAxis(InputAxisType.Vertical));
}
```


