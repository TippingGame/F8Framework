---
name: f8-features-sdkmanager-workflow
description: Use when implementing or troubleshooting SDKManager feature workflows — native platform integration, login, pay, ads, and platform-specific setup in F8Framework.
---

# SDKManager Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about SDK integration for Android, iOS, WebGL, or mini-games.
- The user asks about login, payment, ads, or native platform interaction.
- Setting up Jenkins CI/CD for automated builds.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/SDKManager/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/SDKManager
- Android plugins: Assets/F8Framework/Runtime/SDKManager/Plugins_Android
- iOS plugins: Assets/F8Framework/Plugins/iOS/SDKManager
- Test docs: Assets/F8Framework/Tests/SDKManager

## Key classes and interfaces

| Class | Role |
|-------|------|
| `SDKManager` | Core module. Access via `FF8.SDK`. |

## API quick reference

```csharp
FF8.SDK.SDKStart("platformId", "channelId");         // Initialize SDK
FF8.SDK.SDKLogin();                                   // Login
FF8.SDK.SDKLogout();                                  // Logout
FF8.SDK.SDKSwitchAccount();                           // Switch account
FF8.SDK.SDKLoadVideoAd("adId", "adType");             // Load video ad
FF8.SDK.SDKShowVideoAd("adId", "adType");             // Show video ad
FF8.SDK.SDKPay("serverNum", "serverName", "playerId",
    "playerName", "amount", "extra", "orderId",
    "productName", "productContent", "playerLevel",
    "sign", "guid");                                   // Payment
FF8.SDK.SDKUpdateRole("scenes", "serverId", "serverName",
    "roleId", "roleName", "roleLevel", "roleCTime",
    "rolePower", "guid");                              // Update user info
FF8.SDK.SDKExitGame();                                // Exit game
FF8.SDK.SDKToast("Native Toast");                      // Native toast
```

## Platform setup

### Android
1. Choose correct Gradle version directory based on Unity version.
3. Add `.xml` and `.aar` extensions respectively.
4. Rebuild when switching Unity versions.
5. Android Gradle version support: 6.1.1 (2021/2022), 7.6 (2023), 8.1+ (Unity 6).

### iOS
- Modify `F8SDKInterfaceUnity.h` and `F8SDKInterfaceUnity.mm` for SDK integration.

### WebGL
- Cannot use sync AB loading. Use Resources for sync or async AB.
- Configure in F5: enable MD5 AB names, force remote loading, disable cache on WebGL.

### WeChat Mini Games
- Remove `LitJson.dll` from WX-WASM-SDK-V2.
2. Add F8Framework `LitJson` reference to WX assemblies.
3. Configure CDN address and Bundle Path Identifier (default `StreamingAssets`, change to `AssetBundles`).
4. Enable "Force Remote Loading" and "Disable Cache" in F5 build tool for mini-games.

### Jenkins CI
1. Install Java SDK + Jenkins.
2. Install Unity3d plugin in Jenkins.
3. Configure Unity path in Jenkins Tools.
4. Copy `config.xml` to Jenkins job directory.
5. Configure build parameters matching F5 build tool settings.

## Workflow

1. Call `SDKStart()` to initialize the platform SDK.
2. Implement login flow with `SDKLogin()`.
3. Handle payment via `SDKPay()` with server verification.
4. Show ads via `SDKLoadVideoAd()` + `SDKShowVideoAd()`.
5. Update user info with `SDKUpdateRole()` for analytics.
6. For platform-specific deployment, follow per-platform setup above.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Android build fails | Wrong Gradle/AAR version | Match Gradle version to Unity version |
| iOS callback not received | Missing .mm implementation | Check F8SDKInterfaceUnity.mm |
| WebGL AB load fails | Sync loading not supported | Use async loading or Resources |
| WeChat LitJson conflict | Duplicate LitJson.dll | Remove from WX-WASM-SDK-V2 |

## Cross-module dependencies

- **Build**: SDK setup affects build configuration.
- **AssetManager**: WebGL/mini-game asset loading has special requirements.

## Output checklist

- Platform SDK initialized.
- Native integration files configured.
- Platform-specific build tested.
- Validation status and remaining risks.
