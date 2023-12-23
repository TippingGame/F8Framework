using System;
using UnityEngine;

namespace F8Framework.Core
{
    public class AudioManager : SingletonMono<AudioManager>
    {
        /*----------背景音乐----------*/
        private AudioMusic _audioMusic;
        private float _volumeMusic = 1f;
        private bool _switchMusic = true;
        
        /*----------人声----------*/
        private AudioMusic _audioMusicVoice;
        private float _volumeVoice = 1f;
        private bool _switchVoice = true;
        
        /*----------特效声----------*/
        private AudioMusic _audioMusicBtnClick;
        
        private AudioMusic _audioMusicUISound;
       
        private AudioEffect _audioMusicAudioEffect;
        
        private float _volumeAudioEffect = 1f;
        private bool _switchAudioEffect = true;

        protected override void Init()
        {
            GameObject gameObjectMusic = new GameObject("Music", typeof(AudioSource));
            gameObjectMusic.transform.SetParent(this.transform);
            _audioMusic = new AudioMusic();
            _audioMusic.MusicSource = gameObjectMusic.GetComponent<AudioSource>();
            _audioMusic.MusicSource.playOnAwake = false;
            _audioMusic.MusicSource.loop = false;

            GameObject gameObjectVoice = new GameObject("Voice", typeof(AudioSource));
            gameObjectVoice.transform.SetParent(this.transform);
            _audioMusicVoice = new AudioMusic();
            _audioMusicVoice.MusicSource = gameObjectVoice.GetComponent<AudioSource>();
            _audioMusicVoice.MusicSource.playOnAwake = false;
            _audioMusicVoice.MusicSource.loop = false;

            GameObject gameObjectBtnClick = new GameObject("BtnClick", typeof(AudioSource));
            gameObjectBtnClick.transform.SetParent(this.transform);
            _audioMusicBtnClick = new AudioMusic();
            _audioMusicBtnClick.MusicSource = gameObjectBtnClick.GetComponent<AudioSource>();
            _audioMusicBtnClick.MusicSource.playOnAwake = false;
            _audioMusicBtnClick.MusicSource.loop = false;
            
            GameObject gameObjectUISound = new GameObject("UISound", typeof(AudioSource));
            gameObjectUISound.transform.SetParent(this.transform);
            _audioMusicUISound = new AudioMusic();
            _audioMusicUISound.MusicSource = gameObjectUISound.GetComponent<AudioSource>();
            _audioMusicUISound.MusicSource.playOnAwake = false;
            _audioMusicUISound.MusicSource.loop = false;
            
            _audioMusicAudioEffect = new AudioEffect();

            _volumeMusic = PlayerPrefsManager.Instance.GetFloat("_volumeMusic", 1f);
            _switchMusic = PlayerPrefsManager.Instance.GetBool("_switchMusic", true);
            
            _volumeVoice = PlayerPrefsManager.Instance.GetFloat("_volumeVoice", 1f);
            _switchVoice = PlayerPrefsManager.Instance.GetBool("_switchVoice", true);
            
            _volumeAudioEffect = PlayerPrefsManager.Instance.GetFloat("_volumeAudioEffect", 1f);
            _switchAudioEffect = PlayerPrefsManager.Instance.GetBool("_switchAudioEffect", true);
        }

        private void Update()
        {
            _audioMusic.Tick();
            _audioMusicVoice.Tick();
            _audioMusicBtnClick.Tick();
            _audioMusicUISound.Tick();
        }
        /*----------背景音乐----------*/
        
        // 设置背景音乐播放完成回调
        public void SetMusicComplete(Action callback)
        {
            _audioMusic.OnComplete = callback;
        }

        // 播放背景音乐
        public AudioMusic PlayMusic(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchMusic)
            {
                return null;
            }
            if (priority < _audioMusic.Priority)
            {
                return null;
            }
            if (_audioMusic.Load(assetName, callback))
            {
                _audioMusic.MusicSource.loop = loop;
                _audioMusic.Priority = priority;
                return _audioMusic;
            }
            return null;
        }
        
        // 设置背景乐播放进度
        public float SetProgressMusic
        {
            set => _audioMusic.Progress = value;
        }
        
        // 获取背景音乐播放进度
        public float ProgressMusic => _audioMusic.Progress;
        
        // 获取和设置背景音乐音量
        public float VolumeMusic
        {
            get => _volumeMusic;
            set
            {
                _volumeMusic = value;
                PlayerPrefsManager.Instance.SetFloat("_volumeMusic", value);
                _audioMusic.MusicSource.volume = value;
            }
        }
        
        // 重置背景音乐音量
        public void ResetMusic()
        {
            VolumeMusic = _volumeMusic;
        }
        
        // 设置和获取背景音乐开关值
        public bool SwitchMusic
        {
            get => _switchMusic;
            set
            {
                _switchMusic = value;
                PlayerPrefsManager.Instance.SetBool("_switchMusic", value);
                if (!value)
                {
                    _audioMusic.MusicSource.Stop();
                }
            }
        }
        
        /*----------人声----------*/
        
        // 设置人声播放完成回调
        public void SetVoiceComplete(Action callback)
        {
            _audioMusicVoice.OnComplete = callback;
        }

        // 播放人声
        public AudioMusic PlayVoice(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchVoice)
            {
                return null;
            }
            if (priority < _audioMusicVoice.Priority)
            {
                return null;
            }
            if (_audioMusicVoice.Load(assetName, callback))
            {
                _audioMusicVoice.MusicSource.loop = loop;
                _audioMusicVoice.Priority = priority;
                return _audioMusicVoice;
            }
            return null;
        }
        
        // 设置人声播放进度
        public float SetProgressVoice
        {
            set => _audioMusicVoice.Progress = value;
        }
        
        // 获取人声播放进度
        public float ProgressVoice => _audioMusicVoice.Progress;
        
        // 获取和设置人声音量
        public float VolumeVoice
        {
            get => _volumeVoice;
            set
            {
                _volumeVoice = value;
                PlayerPrefsManager.Instance.SetFloat("_volumeVoice", value);
                _audioMusicVoice.MusicSource.volume = value;
            }
        }

        // 重置人声音量
        public void ResetVoice()
        {
            VolumeVoice = _volumeVoice;
        }
        
        // 设置和获取人声开关值
        public bool SwitchVoice
        {
            get => _switchVoice;
            set
            {
                _switchVoice = value;
                PlayerPrefsManager.Instance.SetBool("_switchVoice", value);
                if (!value)
                {
                    _audioMusicVoice.MusicSource.Stop();
                }
            }
        }
        
        /*----------音效特效控制----------*/
        
        // 获取和设置音效音量
        public float VolumeAudioEffect
        {
            get => _volumeAudioEffect;
            set
            {
                _volumeAudioEffect = value;
                PlayerPrefsManager.Instance.SetFloat("_volumeAudioEffect", value);
                _audioMusicBtnClick.MusicSource.volume = value;
                _audioMusicUISound.MusicSource.volume = value;
            }
        }

        // 重置音效音量
        public void ResetAudioEffect()
        {
            VolumeAudioEffect = _volumeAudioEffect;
        }
        
        // 设置和获取音效音量开关值
        public bool SwitchAudioEffect
        {
            get => _switchAudioEffect;
            set
            {
                _switchAudioEffect = value;
                PlayerPrefsManager.Instance.SetBool("_switchAudioEffect", value);
                if (!value)
                {
                    _audioMusicBtnClick.MusicSource.Stop();
                    _audioMusicUISound.MusicSource.Stop();
                }
            }
        }
        
        /*----------按钮音效特效----------*/
        
        // 设置按钮音效播放完成回调
        public void SetBtnClickComplete(Action callback)
        {
            _audioMusicBtnClick.OnComplete = callback;
        }

        // 播放按钮音效
        public AudioMusic PlayBtnClick(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchAudioEffect)
            {
                return null;
            }
            if (priority < _audioMusicBtnClick.Priority)
            {
                return null;
            }
            if (_audioMusicBtnClick.Load(assetName, callback))
            {
                _audioMusicBtnClick.MusicSource.loop = loop;
                _audioMusicBtnClick.Priority = priority;
                return _audioMusicBtnClick;
            }
            return null;
        }
        
        /*----------UI音效特效----------*/
        
        // 设置UI音效播放完成回调
        public void SetUISoundComplete(Action callback)
        {
            _audioMusicUISound.OnComplete = callback;
        }

        // 播放UI音效
        public AudioMusic PlayUISound(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchAudioEffect)
            {
                return null;
            }
            if (priority < _audioMusicUISound.Priority)
            {
                return null;
            }
            if (_audioMusicUISound.Load(assetName, callback))
            {
                _audioMusicUISound.MusicSource.loop = loop;
                _audioMusicUISound.Priority = priority;
                return _audioMusicUISound;
            }
            return null;
        }
        
        /*----------3D音效特效----------*/
        public void PlayAudioEffect(string assetName, Vector3? position = null, float volume = 1f, Action callback = null)
        {
            if (!_switchAudioEffect)
            {
                return ;
            }
            Vector3 actualPosition = position.GetValueOrDefault(this.transform.position);
            float actualVolume = volume * _volumeAudioEffect;
            _audioMusicAudioEffect.Load(assetName, actualPosition, actualVolume, callback);
        }
        
        /*----------全局控制----------*/
        public void ResumeAll()
        {
            _audioMusic.MusicSource.Play();
            _audioMusicVoice.MusicSource.Play();
            _audioMusicBtnClick.MusicSource.Play();
            _audioMusicUISound.MusicSource.Play();
        }
        
        public void PauseAll() {
            _audioMusic.MusicSource.Pause();
            _audioMusicVoice.MusicSource.Pause();
            _audioMusicBtnClick.MusicSource.Pause();
            _audioMusicUISound.MusicSource.Pause();
        }
        
        public void StopAll()
        {
            _audioMusic.MusicSource.Stop();
            _audioMusicVoice.MusicSource.Stop();
            _audioMusicBtnClick.MusicSource.Stop();
            _audioMusicUISound.MusicSource.Stop();
        }
    }
}