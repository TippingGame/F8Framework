using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;
using UnityEngine.Audio;

namespace F8Framework.Tests
{
    public class DemoAudio : MonoBehaviour
    {
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
            // 3D音效，随机音量音高，位置，音量，播放回调
            FF8.Audio.PlayAudioEffect3D("assetName", isRandom: true, transform.position, volume: 1f, CallBack);

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
    }
}
