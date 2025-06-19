using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class AudioMusic
    {
        private Dictionary<string, AudioClip> _audios = new Dictionary<string, AudioClip>();
        // 背景音乐播放完成回调
        public System.Action OnComplete;
        private float _progress = 0;
        public int Priority;
        public AudioSource MusicSource;
        public int AudioTween;
        private int _timerId = 0;

        // 获取音乐播放进度
        public float Progress
        {
            get
            {
#if !UNITY_WEBGL
                if (MusicSource.clip && MusicSource.clip.length > 0)
                {
                    _progress = MusicSource.time / MusicSource.clip.length;
                }
#endif
                return _progress;
            }
            set
            {
                _progress = value;
#if !UNITY_WEBGL
                MusicSource.time = value * MusicSource.clip.length;
#endif
            }
        }

        // 加载音乐并播放
        public void Load(string url, System.Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            MusicSource.loop = loop;
            Priority = priority;
            
            if (_audios != null && _audios.TryGetValue(url, out AudioClip audioClip) && audioClip)
            {
                PlayClip(audioClip, callback, fadeDuration);
            }
            else
            {
                AssetManager.Instance.LoadAsync<AudioClip>(url, (asset) =>
                {
                    _audios[url] = asset;
                    
                    PlayClip(_audios[url], callback, fadeDuration);
                });
            }
        }

        private void PlayClip(AudioClip audioClip, System.Action callback = null, float fadeDuration = 0f)
        {
            if (MusicSource.isPlaying)
            {
                StopCurrentPlayback();
            }

            MusicSource.clip = audioClip;

            OnComplete = callback;

            if (audioClip != null)
            {
                MusicSource.Play();
                
                _timerId = TimerManager.Instance.AddTimer(this, 1f, audioClip.length, 1, null,
                    () =>
                    {
                        OnComplete?.Invoke();
                    }
                );
            }
            
            if (fadeDuration > 0f)
            {
                float tempVolume = MusicSource.volume;
                MusicSource.volume = 0f;
                AudioTween = Tween.Instance.ValueTween(0f, tempVolume, fadeDuration)
                    .SetOnUpdateFloat((float v) => { MusicSource.volume = v; }).ID;
            }
        }
        
        private void StopCurrentPlayback()
        {
            TimerManager.Instance.RemoveTimer(_timerId);
            Tween.Instance.CancelTween(AudioTween);
            MusicSource.Stop();
            OnComplete?.Invoke();
        }
        
        public void Pause()
        {
            if (MusicSource.isPlaying)
            {
                MusicSource.Pause();
                TimerManager.Instance.Pause(_timerId);
                Tween.Instance.SetIsPause(AudioTween, true);
            }
        }

        public void Resume()
        {
            if (!MusicSource.isPlaying && MusicSource.clip != null)
            {
                MusicSource.Play();
                TimerManager.Instance.Resume(_timerId);
                Tween.Instance.SetIsPause(AudioTween, false);
            }
        }
        
        public void Stop()
        {
            if (MusicSource.isPlaying)
            {
                StopCurrentPlayback();
            }
        }
        
        // 释放所有音乐资源
        public void UnloadAll(bool unloadAllLoadedObjects = true)
        {
            StopCurrentPlayback();
            foreach (var item in _audios)
            {
                AssetManager.Instance.Unload(item.Key, unloadAllLoadedObjects);
            }
            _audios.Clear();
        }
    }
}