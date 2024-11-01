using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Internal;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace F8Framework.Core
{
    public class AudioEffect
    {
        private Dictionary<string, AudioClip> _effects = new Dictionary<string, AudioClip>();
        private Dictionary<string, int> _effectsNum = new Dictionary<string, int>();
        // 定义音量和音高的范围
        private float minVolume = 0.6f;
        private float maxVolume = 1.0f;
        private float minPitch = 0.8f;
        private float maxPitch = 1.2f;
        private GameObject OneShotAudio;
        
        public AudioEffect()
        {
            OneShotAudio = new GameObject("AudioEffect3D", typeof (AudioSource));
            Object.DontDestroyOnLoad(OneShotAudio);
        }
        
        public void Load(string url, Vector3 position, float volume = 1f, float spatialBlend = 1f,
            int maxNum = 5, Action callback = null, AudioMixerGroup audioEffectMixerGroup = null, bool isRandom = false)
        {
            if (!_effectsNum.TryGetValue(url, out int count))
            {
                _effectsNum[url] = 1;
            }
            else if (count >= maxNum)
            {
                return;
            }
            _effectsNum[url] = count + 1;
            
            if (_effects != null && _effects.TryGetValue(url, out AudioClip audioClip) && audioClip)
            {
                PlayClipAtPoint(url, audioClip, position, volume, spatialBlend, callback, audioEffectMixerGroup, isRandom);
            }
            else
            {
                AssetManager.Instance.LoadAsync<AudioClip>(url, (asset) =>
                {
                    _effects[url] = asset;
                    
                    PlayClipAtPoint(url, _effects[url], position, volume, spatialBlend, callback, audioEffectMixerGroup, isRandom);
                });
            }
        }
        
        /// <summary>
        /// 播放3D音效
        /// </summary>
        /// <param name="url"></param>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        /// <param name="volume"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="callback"></param>
        /// <param name="audioEffectMixerGroup"></param>
        /// <param name="isRandom"></param>
        public void PlayClipAtPoint(string url, AudioClip clip, Vector3 position, [DefaultValue("1.0F")] float volume, [DefaultValue("1.0F")] float spatialBlend,
            Action callback = null, AudioMixerGroup audioEffectMixerGroup = null, bool isRandom = false)
        {
            GameObject gameObject = GameObjectPool.Instance.Spawn(OneShotAudio);
            gameObject.transform.position = position;
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.spatialBlend = spatialBlend;
            audioSource.volume = isRandom ? Random.Range(minVolume, maxVolume) : volume;
            audioSource.pitch = isRandom ? Random.Range(minPitch, maxPitch) : 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            if (audioEffectMixerGroup)
                audioSource.outputAudioMixerGroup = audioEffectMixerGroup;
            audioSource.Play();
            
            float time = clip.length * ((double)Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale);
            TimerManager.Instance.AddTimer(this, 1f, time, 1, null, () =>
            {
                GameObjectPool.Instance.Despawn(gameObject);
                if (_effectsNum.TryGetValue(url, out int num))
                {
                    _effectsNum[url] = num - 1;
                }
                callback?.Invoke();
            });
        }
        
        // 释放所有音效资源
        public void UnloadAll(bool unloadAllLoadedObjects = true)
        {
            foreach (var item in _effects)
            {
                AssetManager.Instance.Unload(item.Key, unloadAllLoadedObjects);
            }
            _effects.Clear();
            _effectsNum.Clear();
        }
    }
}