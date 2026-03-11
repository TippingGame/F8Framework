---
name: f8-features-audio-workflow
description: Use when implementing or troubleshooting Audio feature workflows — BGM, voice, SFX, 3D audio, volume control, and AudioMixer in F8Framework.
---

# Audio Feature Workflow

> **⚠️ IMPORTANT**: Before using any F8Framework features, you **MUST** formally initialize the framework. Ensure `ModuleCenter.Initialize(this);` and all required modules (e.g. `FF8.Message = ModuleCenter.CreateModule<MessageManager>();`) are instantiated in the launch sequence (e.g. `GameLauncher.cs` or `GameManager.cs`)!


## Use this skill when

- The task is about sound playback, volume, or audio management.
- The user asks about BGM, voice, SFX, or 3D audio effects.
- The user needs global pause/resume or audio switch persistence.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. If F8Framework is installed as a package, use Packages/F8Framework.
3. For usage docs, read: Assets/F8Framework/Tests/Audio/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Audio
- Test docs: Assets/F8Framework/Tests/Audio

## Key classes and interfaces

| Class | Role |
|-------|------|
| `AudioManager` | Core module. Access via `FF8.Audio`. |
| `AudioMusic` | Background music player. |
| `AudioVoice` | Voice/dialogue player. |
| `AudioEffect` | SFX and UI sound player. |

## API quick reference

### Background music
```csharp
FF8.Audio.PlayMusic("assetName", callback, loop: true, priority: 1, fadeDuration: 3f);
float progress = FF8.Audio.ProgressMusic;
FF8.Audio.SetProgressMusic = 0.5f;
FF8.Audio.VolumeMusic = 0.5f;       // Auto-saves to PlayerPrefs
FF8.Audio.SwitchMusic = false;       // Auto-saves to PlayerPrefs
FF8.Audio.SetMusicComplete(callback);
```

### Voice
```csharp
FF8.Audio.PlayVoice("assetName", callback, loop: true, priority: 1, fadeDuration: 3f);
float progressVoice = FF8.Audio.ProgressVoice;
FF8.Audio.SetProgressVoice = 0.5f;
FF8.Audio.VolumeVoice = 0.5f;
FF8.Audio.SwitchVoice = false;
FF8.Audio.SetVoiceComplete(callback);
```

### Sound effects
```csharp
FF8.Audio.PlayUISound("assetName", callback, loop: true, priority: 1, fadeDuration: 3f);
FF8.Audio.PlayBtnClick("assetName", callback);
FF8.Audio.PlayAudioEffect("assetName", callback);
FF8.Audio.VolumeAudioEffect = 0.5f;
FF8.Audio.SwitchAudioEffect = false;
FF8.Audio.SetUISoundComplete(callback);
FF8.Audio.SetBtnClickComplete(callback);
FF8.Audio.SetAudioEffectComplete(callback);
```

### 3D audio effects
```csharp
FF8.Audio.PlayAudioEffect3D("assetName",
    isRandom: true,
    transform.position,
    volume: 1f,
    spatialBlend: 1f,
    maxNum: 5,
    callback);
```

### Global control
```csharp
FF8.Audio.PauseAll();     // Pause all (except AudioEffect)
FF8.Audio.ResumeAll();    // Resume all (except AudioEffect)
FF8.Audio.StopAll();      // Stop all (except AudioEffect)
FF8.Audio.UnloadAll(true); // Unload all audio (true = force including in-use)
```

### Optional AudioMixer
```csharp
FF8.Audio.SetAudioMixer(FF8.Asset.Load<AudioMixer>("F8AudioMixer"));
```

### Individual channel control
```csharp
FF8.Audio.AudioMusic.Pause();
FF8.Audio.AudioMusicVoice.Resume();
FF8.Audio.AudioMusicBtnClick.Stop();
FF8.Audio.AudioMusicUISound.UnloadAll();
```

## Workflow

1. Load audio clips via AssetManager (place in AssetBundles or Resources).
2. Choose category: Music (BGM), Voice, or AudioEffect (SFX/UI).
3. Use priority parameter for Music/Voice to handle overlapping tracks.
4. Volume and switch settings auto-persist to PlayerPrefs.
5. Use fadeDuration for smooth transitions between tracks.
6. For 3D effects, set `spatialBlend: 1f` and provide world position.
7. Optionally set up F8AudioMixer for advanced mixing control.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Audio clip not found | Asset not in loadable directory | Ensure clip is under AssetBundles or Resources, press F8 |
| No sound on mobile | Volume or switch set to off | Check `SwitchMusic`/`SwitchAudioEffect` values |
| Memory leak from audio | Not unloading unused clips | Call `UnloadAll()` when transitioning scenes |
| PauseAll doesn't affect SFX | By design, AudioEffect excluded | Control AudioEffect separately via channel |

## Cross-module dependencies

- **AssetManager**: All audio clips loaded via `FF8.Asset`.
- **Storage**: Volume/switch settings internally stored via PlayerPrefs.

## Output checklist

- Audio category selected (Music/Voice/AudioEffect).
- Volume persistence configured.
- Files changed and why.
- Validation status and remaining risks.
