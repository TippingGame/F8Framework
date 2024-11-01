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
        private bool _isPlay = false;
        public int Priority;
        public AudioSource MusicSource;
        public BaseTween AudioTween;

        // 获取音乐播放进度
        public float Progress
        {
            get
            {
                if (MusicSource.clip && MusicSource.clip.length > 0)
                {
                    _progress = MusicSource.time / MusicSource.clip.length;
                }
                return _progress;
            }
            set
            {
                _progress = value;
                MusicSource.time = value * MusicSource.clip.length;
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
                _isPlay = false;
                MusicSource.Stop();
                OnComplete?.Invoke();
            }

            MusicSource.clip = audioClip;

            OnComplete = callback;

            MusicSource.Play();

            if (fadeDuration > 0f)
            {
                float tempVolume = MusicSource.volume;
                MusicSource.volume = 0f;
                AudioTween = Tween.Instance.ValueTween(0f, tempVolume, fadeDuration)
                    .SetOnUpdateFloat((float v) => { MusicSource.volume = v; });
            }
        }
            
        public void Tick()
        {
            if (MusicSource.clip && MusicSource.time > 0)
            {
                _isPlay = true;
            }
            if (_isPlay && !MusicSource.isPlaying)
            {
                _isPlay = false;
                OnComplete?.Invoke();
            }
        }

        // 释放所有音乐资源
        public void UnloadAll(bool unloadAllLoadedObjects = true)
        {
            foreach (var item in _audios)
            {
                AssetManager.Instance.Unload(item.Key, unloadAllLoadedObjects);
            }
            _audios.Clear();
        }
    }
}