using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

public class DemoAudio : MonoBehaviour
{
    // Start is called before the first frame update
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
}
