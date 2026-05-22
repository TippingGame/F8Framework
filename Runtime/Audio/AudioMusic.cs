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
        private AssetLoadTracker _assetLoadTracker;
        private AssetLoadTracker AssetLoadTracker => _assetLoadTracker ??= new AssetLoadTracker();
        private TimerTracker _timerTracker;
        private TimerTracker TimerTracker => _timerTracker ??= new TimerTracker();

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
                AssetLoadTracker.LoadAsync<AudioClip>(url, (asset) =>
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
            else
            {
                RemoveTimer(_timerId);
                _timerId = 0;
            }

            MusicSource.clip = audioClip;

            OnComplete = callback;

            if (audioClip != null)
            {
                MusicSource.Play();

                int timerId = 0;
                timerId = TimerTracker.AddTimer(this, 1f, audioClip.length, 1, null,
                    () =>
                    {
                        RemoveTimer(timerId);
                        if (_timerId == timerId)
                        {
                            _timerId = 0;
                        }
                        OnComplete?.Invoke();
                    }
                );
                _timerId = timerId;
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
            RemoveTimer(_timerId);
            _timerId = 0;
            Tween.Instance?.CancelTween(AudioTween);
            MusicSource.Stop();
            OnComplete?.Invoke();
        }
        
        public void Pause()
        {
            if (MusicSource.isPlaying)
            {
                MusicSource.Pause();
                _timerTracker?.Pause(_timerId);
                Tween.Instance?.SetIsPause(AudioTween, true);
            }
        }

        public void Resume()
        {
            if (!MusicSource.isPlaying && MusicSource.clip != null)
            {
                MusicSource.Play();
                _timerTracker?.Resume(_timerId);
                Tween.Instance?.SetIsPause(AudioTween, false);
            }
        }
        
        public void Stop()
        {
            if (MusicSource.isPlaying || _timerId != 0)
            {
                StopCurrentPlayback();
            }
        }
        
        // 释放所有音乐资源
        public void UnloadAll(bool unloadAllLoadedObjects = true)
        {
            StopCurrentPlayback();
            ClearTimerTracker();
            ClearAssetLoadTracker(unloadAllLoadedObjects);
            _audios.Clear();
        }

        private void RemoveTimer(int timerId)
        {
            _timerTracker?.RemoveTimer(timerId);
        }

        private void ClearTimerTracker()
        {
            if (_timerTracker == null)
                return;
            
            _timerTracker.Clear();
            _timerTracker = null;
        }

        private void ClearAssetLoadTracker(bool unloadAllLoadedObjects = false)
        {
            if (_assetLoadTracker == null)
                return;
            
            _assetLoadTracker.ReleaseAll(unloadAllLoadedObjects);
            _assetLoadTracker = null;
        }
    }
}
