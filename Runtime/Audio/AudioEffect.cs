using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public class AudioEffect
    {
        private Dictionary<string, AudioClip> _effects = new Dictionary<string, AudioClip>();

        public void Load(string url, Vector3 position, float volume = 1f, Action callback = null)
        {
            if (_effects != null && _effects.ContainsKey(url))
            {
                AudioSource.PlayClipAtPoint(_effects[url], position, volume);
                callback?.Invoke();
            }
            else
            {
                AssetManager.Instance.LoadAsync<AudioClip>(url, (audioClip) =>
                {
                    if (_effects != null && !_effects.ContainsKey(url))
                    {
                        _effects.Add(url, audioClip);
                        AudioSource.PlayClipAtPoint(audioClip, position, volume);
                        callback?.Invoke();
                    }
                });
            }
        }

        public void Release()
        {
            foreach (var item in _effects)
            {
                AssetManager.Instance.Unload(item.Key);
            }
            _effects.Clear();
        }
    }
}