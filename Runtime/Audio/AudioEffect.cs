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
        private readonly List<PlayingAudioEffect> _playingAudioEffects = new List<PlayingAudioEffect>();
        // 定义音量和音高的范围
        private float minVolume = 0.6f;
        private float maxVolume = 1.0f;
        private float minPitch = 0.8f;
        private float maxPitch = 1.2f;
        private GameObject OneShotAudio;
        private int _playVersion;
        private float _volumeScale = 1f;
        private AssetLoadTracker _assetLoadTracker;
        private AssetLoadTracker AssetLoadTracker => _assetLoadTracker ??= new AssetLoadTracker();
        private TimerTracker _timerTracker;
        private TimerTracker TimerTracker => _timerTracker ??= new TimerTracker();
        
        private class PlayingAudioEffect
        {
            public string Url;
            public GameObject GameObject;
            public AudioSource AudioSource;
            public int TimerId;
            public float BaseVolume;
            public Action Callback;
            public bool IsPausedByManager;
            public bool IsFromPool;
        }
        
        public AudioEffect(Transform transform)
        {
            OneShotAudio = new GameObject("AudioEffect3D", typeof(AudioSource));
            OneShotAudio.transform.SetParent(transform);
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
            int playVersion = _playVersion;
            
            if (_effects != null && _effects.TryGetValue(url, out AudioClip audioClip) && audioClip)
            {
                PlayClipAtPoint(url, audioClip, position, volume, spatialBlend, callback, audioEffectMixerGroup, isRandom);
            }
            else
            {
                AssetLoadTracker.LoadAsync<AudioClip>(url, (asset) =>
                {
                    if (playVersion != _playVersion)
                    {
                        DecreaseEffectCount(url);
                        return;
                    }
                    
                    if (asset == null || asset.Equals(null))
                    {
                        DecreaseEffectCount(url);
                        return;
                    }

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
            if (clip == null || clip.Equals(null))
            {
                DecreaseEffectCount(url);
                return;
            }

            bool isFromPool = GameObjectPool.Instance != null;
            GameObject gameObject = GameObjectPool.Instance?.Spawn(OneShotAudio);
            if (gameObject == null || gameObject.Equals(null))
            {
                isFromPool = false;
                gameObject = Object.Instantiate(OneShotAudio);
            }

            if (gameObject == null || gameObject.Equals(null))
            {
                DecreaseEffectCount(url);
                return;
            }

            gameObject.transform.position = position;
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null || audioSource.Equals(null))
            {
                ReleaseGameObject(gameObject, isFromPool);
                DecreaseEffectCount(url);
                return;
            }

            audioSource.clip = clip;
            audioSource.spatialBlend = spatialBlend;
            float baseVolume = isRandom ? Random.Range(minVolume, maxVolume) : volume;
            audioSource.volume = baseVolume * _volumeScale;
            audioSource.pitch = isRandom ? Random.Range(minPitch, maxPitch) : 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            if (audioEffectMixerGroup)
                audioSource.outputAudioMixerGroup = audioEffectMixerGroup;
            audioSource.Play();
            PlayingAudioEffect playingAudioEffect = new PlayingAudioEffect
            {
                Url = url,
                GameObject = gameObject,
                AudioSource = audioSource,
                BaseVolume = baseVolume,
                Callback = callback,
                IsFromPool = isFromPool
            };
            _playingAudioEffects.Add(playingAudioEffect);
            
            float time = clip.length / Mathf.Max(Mathf.Abs(audioSource.pitch), 0.01f);
            playingAudioEffect.TimerId = TimerTracker.AddTimer(this, 1f, time, 1, null, () =>
            {
                FinishPlayingAudioEffect(playingAudioEffect, true);
            }, true);
        }
        
        public void SetVolume(float volume)
        {
            _volumeScale = volume;
            for (int i = _playingAudioEffects.Count - 1; i >= 0; i--)
            {
                PlayingAudioEffect playingAudioEffect = _playingAudioEffects[i];
                if (playingAudioEffect.AudioSource == null || playingAudioEffect.AudioSource.Equals(null))
                {
                    RemoveInvalidPlayingAudioEffect(i);
                    continue;
                }
                
                playingAudioEffect.AudioSource.volume = playingAudioEffect.BaseVolume * _volumeScale;
            }
        }
        
        public void Pause()
        {
            for (int i = _playingAudioEffects.Count - 1; i >= 0; i--)
            {
                PlayingAudioEffect playingAudioEffect = _playingAudioEffects[i];
                if (playingAudioEffect.AudioSource == null || playingAudioEffect.AudioSource.Equals(null))
                {
                    RemoveInvalidPlayingAudioEffect(i);
                    continue;
                }
                
                if (playingAudioEffect.AudioSource.isPlaying)
                {
                    playingAudioEffect.AudioSource.Pause();
                    playingAudioEffect.IsPausedByManager = true;
                    _timerTracker?.Pause(playingAudioEffect.TimerId);
                }
            }
        }
        
        public void Resume()
        {
            for (int i = _playingAudioEffects.Count - 1; i >= 0; i--)
            {
                PlayingAudioEffect playingAudioEffect = _playingAudioEffects[i];
                if (playingAudioEffect.AudioSource == null || playingAudioEffect.AudioSource.Equals(null))
                {
                    RemoveInvalidPlayingAudioEffect(i);
                    continue;
                }
                
                if (playingAudioEffect.IsPausedByManager && !playingAudioEffect.AudioSource.isPlaying && playingAudioEffect.AudioSource.clip != null)
                {
                    playingAudioEffect.AudioSource.Play();
                    playingAudioEffect.IsPausedByManager = false;
                    _timerTracker?.Resume(playingAudioEffect.TimerId);
                }
            }
        }
        
        public void Stop()
        {
            _playVersion++;
            for (int i = _playingAudioEffects.Count - 1; i >= 0; i--)
            {
                FinishPlayingAudioEffect(_playingAudioEffects[i], false);
            }
            ClearTimerTracker();
        }
        
        // 释放所有音效资源
        public void UnloadAll(bool unloadAllLoadedObjects = true)
        {
            Stop();
            ClearAssetLoadTracker(unloadAllLoadedObjects);
            _effects.Clear();
            _effectsNum.Clear();
        }
        
        private void FinishPlayingAudioEffect(PlayingAudioEffect playingAudioEffect, bool invokeCallback)
        {
            if (playingAudioEffect == null)
            {
                return;
            }
            
            RemoveTimer(playingAudioEffect.TimerId);
            if (playingAudioEffect.AudioSource != null && !playingAudioEffect.AudioSource.Equals(null))
            {
                playingAudioEffect.AudioSource.Stop();
            }
            if (playingAudioEffect.GameObject != null && !playingAudioEffect.GameObject.Equals(null))
            {
                ReleaseGameObject(playingAudioEffect.GameObject, playingAudioEffect.IsFromPool);
            }
            
            DecreaseEffectCount(playingAudioEffect.Url);
            _playingAudioEffects.Remove(playingAudioEffect);
            
            if (invokeCallback)
            {
                playingAudioEffect.Callback?.Invoke();
            }
        }

        private void RemoveInvalidPlayingAudioEffect(int index)
        {
            PlayingAudioEffect playingAudioEffect = _playingAudioEffects[index];
            RemoveTimer(playingAudioEffect.TimerId);
            DecreaseEffectCount(playingAudioEffect.Url);
            _playingAudioEffects.RemoveAt(index);
        }

        private void ReleaseGameObject(GameObject gameObject, bool isFromPool)
        {
            if (gameObject == null || gameObject.Equals(null))
            {
                return;
            }

            if (isFromPool && GameObjectPool.Instance != null)
            {
                GameObjectPool.Instance.Despawn(gameObject);
            }
            else
            {
                Object.Destroy(gameObject);
            }
        }
        
        private void DecreaseEffectCount(string url)
        {
            if (_effectsNum.TryGetValue(url, out int num))
            {
                num--;
                if (num <= 0)
                {
                    _effectsNum.Remove(url);
                }
                else
                {
                    _effectsNum[url] = num;
                }
            }
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
