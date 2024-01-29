# F8AudioManager

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8AudioManager组件，播放，暂停，停止，进度控制，音量控制/保存，全局暂停/恢复  
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
        AudioManager.Instance.PlayMusic("assetName", CallBack, loop:true, priority:1); //背景音乐
        /*----------人声----------*/
        AudioManager.Instance.PlayVoice("assetName", CallBack, loop:true, priority:1); //角色语音
        /*----------特效声----------*/
        AudioManager.Instance.PlayUISound("assetName", CallBack, loop:true, priority:1); //ui音效
        AudioManager.Instance.PlayBtnClick("assetName", CallBack, loop:false, priority:2); //按钮音效
        AudioManager.Instance.PlayAudioEffect("assetName", transform.position, volume:1f, CallBack); //场景音效
    
        /*----------功能示例----------*/
        float progress = AudioManager.Instance.ProgressMusic; //获取进度
        AudioManager.Instance.SetProgressMusic = 0.5f; //设置进度
        AudioManager.Instance.VolumeMusic = 0.5f; //设置音量，自动保存至PlayerPrefs
        AudioManager.Instance.SwitchMusic = false; //设置开关，自动保存至PlayerPrefs
        AudioManager.Instance.SetVoiceComplete(CallBack); //设置完成回调
            
        /*----------全局控制----------*/
        AudioManager.Instance.PauseAll(); //暂停所有，不包括AudioEffect
        AudioManager.Instance.ResumeAll(); //恢复所有，不包括AudioEffect
        AudioManager.Instance.StopAll(); //停止所有，不包括AudioEffect
    }

    void CallBack()
    {
        
    }
```


