# F8 AudioManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 AudioManager组件，播放，暂停，停止，进度控制，音量控制/保存，全局暂停/恢复。可选AudioMixer  
Audio分为三大类：  
1.背景音乐  
2.人声  
3.特效声  

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 代码使用方法
```C#
        void Start()
        {
            /*----------背景音乐----------*/
            FF8.Audio.PlayMusic("assetName", CallBack, loop: true, priority: 1); // 背景音乐
            /*----------人声----------*/
            FF8.Audio.PlayVoice("assetName", CallBack, loop: true, priority: 1); // 角色语音
            /*----------特效声----------*/
            FF8.Audio.PlayUISound("assetName", CallBack, loop: true, priority: 1); // ui音效
            FF8.Audio.PlayBtnClick("assetName", CallBack, loop: false, priority: 2); // 按钮音效
            FF8.Audio.PlayAudioEffect("assetName", CallBack, loop: false, priority: 2); // 音效特效
            FF8.Audio.PlayAudioEffect3D("assetName", isRandom: true, transform.position, volume: 1f, CallBack); // 3D音效

            /*----------其他功能----------*/
            
            //可选，设置混音组F8AudioMixer，需手动放到可加载目录
            FF8.Audio.SetAudioMixer(FF8.Asset.Load<AudioMixer>("F8AudioMixer"));
            
            float progress = FF8.Audio.ProgressMusic; // 获取进度
            FF8.Audio.SetProgressMusic = 0.5f; // 设置进度
            FF8.Audio.VolumeMusic = 0.5f; // 设置音量，自动保存至PlayerPrefs
            FF8.Audio.SwitchMusic = false; // 设置开关，自动保存至PlayerPrefs
            FF8.Audio.SetVoiceComplete(CallBack); // 设置完成回调

            /*----------全局控制----------*/
            FF8.Audio.PauseAll(); // 暂停所有，不包括AudioEffect
            FF8.Audio.ResumeAll(); // 恢复所有，不包括AudioEffect
            FF8.Audio.StopAll(); // 停止所有，不包括AudioEffect

            void CallBack()
            {

            }
        }
```


