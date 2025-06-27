# F8 AudioManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 AudioManager Component**  
Supports playback control (play/pause/stop), progress control, volume adjustment/saving, global pause/resume. Optional AudioMixer integration.  
**Audio Classification**:  
* Background Music (BGM)
* Voice
* Sound Effects (SFX)

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
void Start()
{
    /*----------Background Music (BGM)----------*/
    // assetName: Asset name
    // callback: Playback completion callback
    // loop: Whether to loop
    // priority: Priority (higher values override lower)
    // fadeDuration: Fade-in duration (seconds)
    FF8.Audio.PlayMusic("assetName", CallBack, loop: true, priority: 1, fadeDuration: 3f); // BGM
    float progress = FF8.Audio.ProgressMusic; // Get playback progress (0-1)
    FF8.Audio.SetProgressMusic = 0.5f; // Set progress (0-1)
    FF8.Audio.VolumeMusic = 0.5f; // Set volume (0-1, auto-saved to PlayerPrefs)
    FF8.Audio.SwitchMusic = false; // Toggle on/off (auto-saved to PlayerPrefs)
    FF8.Audio.SetMusicComplete(CallBack); // Set completion callback
    
    /*----------Voice----------*/
    FF8.Audio.PlayVoice("assetName", CallBack, true, 1, 3f); // Character voice
    float progressVoice = FF8.Audio.ProgressVoice; // Get progress
    FF8.Audio.SetProgressVoice = 0.5f; // Set progress
    FF8.Audio.VolumeVoice = 0.5f; // Set volume (auto-saved to PlayerPrefs)
    FF8.Audio.SwitchVoice = false; // Toggle on/off (auto-saved to PlayerPrefs)
    FF8.Audio.SetVoiceComplete(CallBack); // Set completion callback
    
    /*----------Sound Effects (SFX)----------*/
    FF8.Audio.PlayUISound("assetName", CallBack, true, 1, 3f); // UI sound
    FF8.Audio.PlayBtnClick("assetName", CallBack, false, 2, 3f); // Button click sound
    FF8.Audio.PlayAudioEffect("assetName", CallBack, false, 2, 3f); // General SFX
    FF8.Audio.VolumeAudioEffect = 0.5f; // Set volume (auto-saved to PlayerPrefs)
    FF8.Audio.SwitchAudioEffect = false; // Toggle on/off (auto-saved to PlayerPrefs)
    FF8.Audio.SetUISoundComplete(CallBack); // Set completion callback
    FF8.Audio.SetBtnClickComplete(CallBack); // Set completion callback
    FF8.Audio.SetAudioEffectComplete(CallBack); // Set completion callback
    
    /*----------One-shot 3D Sound Effects----------*/
    // assetName: Asset name
    // isRandom: Randomize pitch/volume
    // audioPosition: World position
    // volume: Base volume (0-1)
    // spatialBlend: 2D-3D blend (0-1)
    // maxNum: Maximum concurrent instances
    // callback: Playback completion callback
    FF8.Audio.PlayAudioEffect3D("assetName", isRandom: true, transform.position, volume: 1f, spatialBlend: 1f, maxNum: 5, CallBack);
    FF8.Audio.VolumeAudioEffect = 0.5f; // Set volume (auto-saved to PlayerPrefs)
    FF8.Audio.SwitchAudioEffect = false; // Toggle on/off (auto-saved to PlayerPrefs)
    
    /*----------Additional Features----------*/
    
    // Optional: Set AudioMixer (must be manually placed in loadable directory)
    FF8.Audio.SetAudioMixer(FF8.Asset.Load<AudioMixer>("F8AudioMixer"));

    /*----------Global Controls----------*/
    FF8.Audio.PauseAll(); // Pause all (excluding AudioEffect)
    FF8.Audio.ResumeAll(); // Resume all (excluding AudioEffect)
    FF8.Audio.StopAll(); // Stop all (excluding AudioEffect)

    FF8.Audio.UnloadAll(true); // Unload all audio (true: force unload including in-use)

    /*----------Individual Controls----------*/
    FF8.Audio.AudioMusic.Pause();
    FF8.Audio.AudioMusicVoice.Resume();
    FF8.Audio.AudioMusicBtnClick.Stop();
    FF8.Audio.AudioMusicUISound.UnloadAll();
    FF8.Audio.AudioMusicAudioEffect.UnloadAll(true);
    
    void CallBack()
    {
        // Callback function
    }
}
```


