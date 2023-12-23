using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class AudioEffect
    {
        private Dictionary<string, AudioClip> _effects = new Dictionary<string, AudioClip>();

        public bool Load(string url, Vector3 position, float volume = 1f, Action callback = null)
        {
            if (_effects != null && _effects.ContainsKey(url))
            {
                AudioSource.PlayClipAtPoint(_effects[url], position, volume);
                callback?.Invoke();
            }
            else
            {
                AudioClip audioClip = Resources.Load<AudioClip>(url);
                
                if (audioClip == null)
                {
                    LogF8.LogError("Failed to load AudioClip at path: " + url);
                    return false;
                }
                
                if (_effects != null && !_effects.ContainsKey(url))
                {
                    _effects.Add(url, audioClip);
                    AudioSource.PlayClipAtPoint(audioClip, position, volume);
                    callback?.Invoke();
                }
            }
            return true;
        }

        public void Release()
        {
            foreach (var item in _effects)
            {
                Resources.UnloadAsset(item.Value);
            }
            _effects.Clear();
        }
    }
}