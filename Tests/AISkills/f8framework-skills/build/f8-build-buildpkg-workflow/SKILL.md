---
name: f8-build-buildpkg-workflow
description: Use when executing builds, generating update packages, or verifying build artifacts with F5 build tool in F8Framework.
---

# Build Package Workflow



## Use this skill when

- The task is about packaging the game for release (Win, Android, iOS, Mac, Linux, WebGL).
- The user asks about hot update package generation, AB encryption, or CDN deployment.
- Build verification, Jenkins CI setup, or build failure debugging.

## Path resolution

1. Editor code: Assets/F8Framework/Editor/Build

## Sources of truth

- Editor module: Assets/F8Framework/Editor/Build
- Jenkins config: Assets/F8Framework/Editor/Build/Jenkins/config.xml
- Test docs: Assets/F8Framework/Tests/HotUpdateManager/README.md and SDKManager/README.md

## F5 Build Tool — configuration options

| Setting | Description |
|---------|-------------|
| **Target Platform** | Win / Android / iOS / Mac / Linux / WebGL |
| **Output Path** | Build output directory |
| **Version Number** | Game version string |
| **Remote Asset URL** | CDN address for remote assets |
| **Enable Hot Update** | Toggle hot update support |
| **Build Type** | Full build / Sub-package / Empty package |
| **Enable Encryption** | AB encryption toggle |
| **Config Export Format** | Binary (default) or JSON |
| **Config Export Directory** | Output for config data |
| **Full Resource Path** | Enable/disable full-path asset loading |
| **MD5 AB Names** | Add MD5 hash to AB names (for CDN cache busting) |
| **Force Remote Loading** | Force all assets to load from remote |
| **Disable WebGL Cache** | Disable browser caching on WebGL |

## Build workflow

### Standard build
1. Press **F8** to regenerate all indices and configs.
2. Press **F5** to open build tool.
3. Configure platform, version, and output path.
4. Click Build button.
5. If build fails, try Unity's built-in Build once first, then retry.

### Hot update build
1. Configure Remote Asset URL in F5.
2. Enable Hot Update.
3. Build full package first.
4. For subsequent updates, generate update packages only.
5. Upload generated files to CDN server.
6. Place files in CDN with platform subdirectory structure.

### Sub-package (DLC) build
1. Name DLC folders: `Package_ + identifier` in AssetBundles.
2. Configure in F5 build tool.
3. Build sub-packages separately.
4. Runtime loading via `FF8.HotUpdate.CheckPackageUpdate()`.

### WebGL / Mini-game builds
1. In F5, enable: MD5 AB names ✓, Force remote loading ✓, Disable WebGL cache ✓.
2. Configure CDN address for AB files.
3. Upload `StreamingAssets/AssetBundles` to CDN after build.
4. For WeChat Mini Games: configure CDN in MiniGameConfig, set Bundle Path Identifier to `AssetBundles`.

### Jenkins CI
1. Install Java SDK + Jenkins.
2. Install Unity3d plugin in Jenkins Plugins.
3. Add Unity version in Jenkins Tools.
4. Copy `config.xml` from `Editor/Build/Jenkins/` to Jenkins job directory.
5. Restart Jenkins service.
6. Configure build parameters matching F5 settings.
7. Trigger build with configured parameters.

## Pre-build checklist

1. Run F8 to regenerate all assets and configs.
2. Verify no compilation errors.
3. Check platform-specific settings:
   - Android: Correct Gradle version and AAR files in Plugins/Android
   - iOS: F8SDKInterfaceUnity.h/.mm configured
   - WebGL: Sync loading avoided for AB
4. Verify remote URL if hot update enabled.
5. Clear sandbox directory if testing locally.

## Post-build verification

1. Check build output directory for all expected files.
2. Verify AB files are in `StreamingAssets/AssetBundles/<Platform>/`.
3. For hot update: verify version files and asset manifests.
4. Test loading on target platform.
5. Upload to CDN if remote loading enabled.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Build fails immediately | Missing first build | Do Unity's built-in Build first, then retry F5 |
| AB not found on device | Platform mismatch | Build AB for target platform |
| Hot update check fails | Wrong remote URL | Verify CDN URL in F5 settings |
| WebGL load fails | Sync AB not supported | Enable Force Remote, use async loading |
| Jenkins build fails | Wrong JDK/Gradle/SDK path | Match paths to Unity's bundled tools |
| Android Gradle error | Wrong Gradle version | Match Gradle to Unity version per docs |

## Cross-module dependencies

- **AssetManager**: Must press F8 to generate indices before build.
- **ExcelTool**: Config tables must be exported before build.
- **HotUpdateManager**: Hot update version management for CDN deployment.
- **SDKManager**: Platform-specific plugins affect build configuration.

## Output checklist

- Build completed without errors.
- AB files generated for target platform.
- Hot update packages (if applicable) ready for CDN.
- Config data exported in correct format.
- Platform-specific requirements met.
