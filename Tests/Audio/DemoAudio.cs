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
            //assetName资产名
            //callback播放完成回调
            //loop是否循环
            //priority优先级，高的覆盖低的
            //fadeDuration淡入持续时间
            FF8.Audio.PlayMusic("assetName", CallBack, loop: true, priority: 1, fadeDuration: 3f); // 背景音乐
            /*----------人声----------*/
            FF8.Audio.PlayVoice("assetName", CallBack, true, 1, 3f); // 角色语音
            /*----------特效声----------*/
            FF8.Audio.PlayUISound("assetName", CallBack, true, 1, 3f); // ui音效
            FF8.Audio.PlayBtnClick("assetName", CallBack, false, 2, 3f); // 按钮音效
            FF8.Audio.PlayAudioEffect("assetName", CallBack, false, 2, 3f); // 音效特效
            
            /*----------一次性3D音效----------*/
            //assetName资产名
            //isRandom是否随机音量音高
            //audioPosition音频播放位置
            //volume音量
            //spatialBlend2d到3d的比例
            //maxNum最大同时播放个数
            //callback播放完成回调
            FF8.Audio.PlayAudioEffect3D("assetName", isRandom: true, transform.position, volume: 1f, spatialBlend: 1f, maxNum: 5, CallBack);
            
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

            FF8.Audio.UnloadAll(true); // 卸载所有音频和音效。（true:完全卸载，包括正在使用的）

            void CallBack()
            {

            }
        }
    }
}
