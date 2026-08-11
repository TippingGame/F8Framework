---
name: f8-build-buildpkg-workflow
description: Use when executing or maintaining F8/F5 editor build pipelines, generating Player or update packages, configuring Jenkins, recovering builds across script reloads, or verifying build artifacts in F8Framework.
---

# Build Package Workflow



## Use this skill when

- The task is about packaging the game for release (Win, Android, iOS, Mac, Linux, WebGL).
- The user asks about hot update package generation, AB encryption, or CDN deployment.
- Build verification, Jenkins CI setup, or build failure debugging.
- The task involves `F8BuildPipeline`, `F8EditorPipeline`, optional build contributors, or recovery after script compilation.

## Path resolution

1. Build UI and steps: Assets/F8Framework/Editor/Build
2. Shared pipeline: Assets/F8Framework/Editor/Pipeline

## Sources of truth

- Editor module: Assets/F8Framework/Editor/Build
- Build composition: Assets/F8Framework/Editor/Pipeline/F8BuildPipeline.cs
- Persistent execution: Assets/F8Framework/Editor/Pipeline/F8EditorPipeline.cs
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
| **Pre-build Excel Generation** | Enabled by default; add/remove Excel generation steps without coupling Core Editor to ExcelTool |
| **Config Export Format** | Binary (default) or JSON |
| **Config Export Directory** | Output for config data |
| **Full Resource Path** | Enable/disable full-path asset loading |
| **MD5 AB Names** | Add MD5 hash to AB names (for CDN cache busting) |
| **Force Remote Loading** | Force all assets to load from remote |
| **Disable WebGL Cache** | Disable browser caching on WebGL |

## Build workflow

### Standard build
1. Press **F5** to open the build tool.
2. Configure platform, version, output path, and optional extension settings.
3. Enable or disable **Pre-build Excel Generation** intentionally.
4. Click **Build Game**, **Build Update Package**, or **Build and Run**.
5. Let the composed pipeline execute in order:
   - Write the game version when building a Player.
   - Discover optional contributors; generate and serialize Excel data only when enabled.
   - Generate/copy hot-update DLLs.
   - Build AssetBundles.
   - Build the Player or update package.
   - Write the asset version when required.
6. Do not run F8 first merely to satisfy prerequisites; F5 requests already include the required core and enabled extension steps.

### F8Run
1. Press **F8** to start `F8BuildRequest.CreateF8Run()`.
2. Run enabled contributors, hot-update DLL generation, and AssetBundle building.
3. Do not build a Player or update package from F8Run; use F5 for those outputs.

### Pipeline lifecycle
1. Persist pipeline step IDs under `Library/F8EditorPipeline/state.json` before execution.
2. Return `RequestScriptReload` when a step changes scripts; let the pipeline request compilation and resume after Domain Reload.
3. Use **开发工具 → 流水线 → 重试失败任务** or **取消当前任务** for recovery.
4. Treat either `state.json` or `state.json.tmp` as pending. If state JSON is damaged, use **取消当前任务**; cancellation does not require successful deserialization.
5. Cancel an old pending pipeline before removing an optional module whose step IDs are already persisted.
6. From IMGUI buttons, call `F8BuildPipeline.StartDeferred(...)` so the current layout event finishes before refresh/compilation. Use layout scopes around extension UI.
7. Treat a non-successful `BuildReport` or null AssetBundle manifest as a thrown step failure. Do not only log an error and let the pipeline advance; restore temporarily moved package assets in `finally`.

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
7. Use `UseExcelDataTool- false` when CI should omit Excel steps; configure Excel path/format/output arguments only when the module participates.
8. Trigger `JenkinsStart`; use `JenkinsResume` in the follow-up Unity process when compilation requires a restart.

## Pre-build checklist

1. Verify no failed or unexpected pending editor pipeline remains.
2. Verify no compilation errors.
3. Confirm which optional contributors, especially ExcelTool, should participate.
4. Check platform-specific settings:
   - Android: Correct Gradle version and AAR files in Plugins/Android
   - iOS: F8SDKInterfaceUnity.h/.mm configured
   - WebGL: Sync loading avoided for AB
5. Verify remote URL if hot update enabled.
6. Clear sandbox directory if testing locally.

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
| `EndLayoutGroup` after clicking Build | Pipeline started synchronously inside `OnGUI` and refresh reset the IMGUI layout stack | Start with `F8BuildPipeline.StartDeferred(...)` from GUI buttons |
| Pipeline reports a missing step handler | An optional module was removed while its step IDs remained in pending state | Cancel the old pipeline and start a new build |
| Pipeline remains failed after fixing code | Failed state is persisted for recovery | Use **重试失败任务** or cancel and start again |
| Pending pipeline name shows invalid state | `state.json`/`.tmp` is truncated or damaged | Use **取消当前任务** to remove both files, then start a new build |
| Player/AssetBundle build returns failure | Unity returned a failed/cancelled report or no manifest | Fix the reported build error and retry the persisted step; later version-writing steps must not run |
| AB not found on device | Platform mismatch | Build AB for target platform |
| Hot update check fails | Wrong remote URL | Verify CDN URL in F5 settings |
| WebGL load fails | Sync AB not supported | Enable Force Remote, use async loading |
| Jenkins build fails | Wrong JDK/Gradle/SDK path | Match paths to Unity's bundled tools |
| Android Gradle error | Wrong Gradle version | Match Gradle to Unity version per docs |

## Cross-module dependencies

- **AssetManager**: AssetBundle generation is a core pipeline step; no separate F8 prerequisite is required for F5 builds.
- **ExcelTool**: Optional contributor. When enabled it generates code/data before hot-update DLL and AssetBundle steps; when disabled or absent, core builds continue.
- **HotUpdateManager**: Hot update version management for CDN deployment.
- **SDKManager**: Platform-specific plugins affect build configuration.

## Output checklist

- Build completed without errors.
- Enabled contributors completed, and the persistent pipeline state was cleared.
- AB files generated for target platform.
- Hot update packages (if applicable) ready for CDN.
- Config data exported in correct format.
- Platform-specific requirements met.
