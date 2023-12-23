using UnityEngine;

namespace F8Framework.Core
{
    public class AudioMusic
    {
        // 背景音乐播放完成回调
        public System.Action OnComplete { get; set; }
        private float _progress = 0;
        private string _url = null;
        private bool _isPlay = false;
        public int Priority { get; set; }
        public AudioSource MusicSource { get; set; }

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
        public bool Load(string url, System.Action callback = null)
        {
            AudioClip audioClip = Resources.Load<AudioClip>(url);

            if (audioClip == null)
            {
                LogF8.LogError("Failed to load AudioClip at path: " + url);
                return false;
            }

            if (MusicSource.isPlaying)
            {
                _isPlay = false;
                MusicSource.Stop();
                OnComplete?.Invoke();
            }

            MusicSource.clip = audioClip;

            OnComplete = callback;

            MusicSource.Play();

            _url = url;
            return true;
        }

        public void Tick()
        {
            if (MusicSource.time > 0)
            {
                _isPlay = true;
            }
            if (_isPlay && !MusicSource.isPlaying)
            {
                _isPlay = false;
                OnComplete?.Invoke();
            }
        }

        // 释放当前背景音乐资源
        public void Release()
        {
            if (!string.IsNullOrEmpty(_url))
            {
                Resources.UnloadAsset(MusicSource.clip);
                _url = null;
            }
        }
    }
}