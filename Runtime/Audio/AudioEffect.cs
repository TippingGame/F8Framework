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

        // 定义音量和音高的范围
        private float minVolume = 0.6f;
        private float maxVolume = 1.0f;
        private float minPitch = 0.8f;
        private float maxPitch = 1.2f;
        
        public void Load(string url, Vector3 position, float volume = 1f, Action callback = null, AudioMixerGroup audioEffectMixerGroup = null, bool isRandom = false)
        {
            if (_effects != null && _effects.ContainsKey(url))
            {
                PlayClipAtPoint(_effects[url], position, volume, audioEffectMixerGroup, isRandom);
                callback?.Invoke();
            }
            else
            {
                AssetManager.Instance.LoadAsync<AudioClip>(url, (audioClip) =>
                {
                    AssetManager.Instance.Unload(url, false);
                    
                    if (_effects != null && !_effects.ContainsKey(url))
                    {
                        _effects.Add(url, audioClip);
                        PlayClipAtPoint(audioClip, position, volume, audioEffectMixerGroup, isRandom);
                        callback?.Invoke();
                    }
                });
            }
        }
        
        /// <summary>
        /// 播放3D音效
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        /// <param name="volume"></param>
        /// <param name="audioEffectMixerGroup"></param>
        /// <param name="isRandom"></param>
        public void PlayClipAtPoint(AudioClip clip, Vector3 position, [DefaultValue("1.0F")] float volume, AudioMixerGroup audioEffectMixerGroup = null, bool isRandom = false)
        {
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            AudioSource audioSource = (AudioSource) gameObject.AddComponent(typeof (AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = isRandom ? Random.Range(minVolume, maxVolume) : volume;
            audioSource.pitch = isRandom ? Random.Range(minPitch, maxPitch) : 1f;
            if (audioEffectMixerGroup)
                audioSource.outputAudioMixerGroup = audioEffectMixerGroup;
            audioSource.Play();
            Object.Destroy((Object) gameObject, clip.length * ((double) Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
        }
        
        public void Release()
        {
            foreach (var item in _effects)
            {
                AssetManager.Instance.Unload(item.Key, true);
            }
            _effects.Clear();
        }
    }
}