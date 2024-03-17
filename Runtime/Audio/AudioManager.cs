using System;
using UnityEngine;
using UnityEngine.Audio;

namespace F8Framework.Core
{
    [UpdateRefresh]
    public class AudioManager : ModuleSingletonMono<AudioManager>, IModule
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
        
        private AudioMusic _audioMusicAudioEffect;
        
        private AudioEffect _audioMusicAudioEffect3D;
        private AudioMixerGroup _audioEffectMixerGroup;
        
        private float _volumeAudioEffect = 1f;
        private bool _switchAudioEffect = true;

        private Transform _transform;
        private AudioMixer _audioMixer;

        private const string _volumeMusicKey = "VolumeMusicKey";
        private const string _switchMusicKey = "SwitchMusicKey";
        private const string _volumeVoiceKey = "VolumeVoiceKey";
        private const string _switchVoiceKey = "SwitchVoiceKey";
        private const string _volumeAudioEffectKey = "VolumeAudioEffect";
        private const string _switchAudioEffectKey = "SwitchAudioEffect";
        public void OnInit(object createParam)
        {
            _transform = this.transform;
            GameObject gameObjectMusic = new GameObject("Music", typeof(AudioSource));
            gameObjectMusic.transform.SetParent(_transform);
            _audioMusic = new AudioMusic();
            _audioMusic.MusicSource = gameObjectMusic.GetComponent<AudioSource>();
            _audioMusic.MusicSource.playOnAwake = false;
            _audioMusic.MusicSource.loop = false;

            GameObject gameObjectVoice = new GameObject("Voice", typeof(AudioSource));
            gameObjectVoice.transform.SetParent(_transform);
            _audioMusicVoice = new AudioMusic();
            _audioMusicVoice.MusicSource = gameObjectVoice.GetComponent<AudioSource>();
            _audioMusicVoice.MusicSource.playOnAwake = false;
            _audioMusicVoice.MusicSource.loop = false;

            GameObject gameObjectBtnClick = new GameObject("BtnClick", typeof(AudioSource));
            gameObjectBtnClick.transform.SetParent(_transform);
            _audioMusicBtnClick = new AudioMusic();
            _audioMusicBtnClick.MusicSource = gameObjectBtnClick.GetComponent<AudioSource>();
            _audioMusicBtnClick.MusicSource.playOnAwake = false;
            _audioMusicBtnClick.MusicSource.loop = false;
            
            GameObject gameObjectUISound = new GameObject("UISound", typeof(AudioSource));
            gameObjectUISound.transform.SetParent(_transform);
            _audioMusicUISound = new AudioMusic();
            _audioMusicUISound.MusicSource = gameObjectUISound.GetComponent<AudioSource>();
            _audioMusicUISound.MusicSource.playOnAwake = false;
            _audioMusicUISound.MusicSource.loop = false;
            
            GameObject gameObjectAudioEffect = new GameObject("AudioEffect", typeof(AudioSource));
            gameObjectAudioEffect.transform.SetParent(_transform);
            _audioMusicAudioEffect = new AudioMusic();
            _audioMusicAudioEffect.MusicSource = gameObjectAudioEffect.GetComponent<AudioSource>();
            _audioMusicAudioEffect.MusicSource.playOnAwake = false;
            _audioMusicAudioEffect.MusicSource.loop = false;
            
            _audioMusicAudioEffect3D = new AudioEffect();

            _volumeMusic = StorageManager.Instance.GetFloat(_volumeMusicKey, 1f);
            _switchMusic = StorageManager.Instance.GetBool(_switchMusicKey, true);
            
            _volumeVoice = StorageManager.Instance.GetFloat(_volumeVoiceKey, 1f);
            _switchVoice = StorageManager.Instance.GetBool(_switchVoiceKey, true);
            
            _volumeAudioEffect = StorageManager.Instance.GetFloat(_volumeAudioEffectKey, 1f);
            _switchAudioEffect = StorageManager.Instance.GetBool(_switchAudioEffectKey, true);
        }

        /// <summary>
        /// 设置AudioMixer混音组
        /// </summary>
        /// <param name="audioMixer"></param>
        public void SetAudioMixer(AudioMixer audioMixer)
        {
            _audioMusic.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Music")[0];
            _audioMusicVoice.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Voice")[0];
            _audioMusicBtnClick.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioMusicUISound.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioMusicAudioEffect.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioEffectMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioMixer = audioMixer;
        }
        
        public void OnUpdate()
        {
            _audioMusic.Tick();
            _audioMusicVoice.Tick();
            _audioMusicBtnClick.Tick();
            _audioMusicUISound.Tick();
            _audioMusicAudioEffect.Tick();
        }
        
        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            Destroy(gameObject);
        }
        
        /*----------背景音乐----------*/
        
        // 设置背景音乐播放完成回调
        public void SetMusicComplete(Action callback)
        {
            _audioMusic.OnComplete = callback;
        }

        // 播放背景音乐
        public void PlayMusic(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchMusic)
            {
                return;
            }
            if (priority < _audioMusic.Priority)
            {
                return;
            }
            _audioMusic.Load(assetName, callback);
            _audioMusic.MusicSource.loop = loop;
            _audioMusic.Priority = priority;
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
                StorageManager.Instance.SetFloat(_volumeMusicKey, value);
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
                StorageManager.Instance.SetBool(_switchMusicKey, value);
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
        public void PlayVoice(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchVoice)
            {
                return;
            }
            if (priority < _audioMusicVoice.Priority)
            {
                return;
            }
            _audioMusicVoice.Load(assetName, callback);
            _audioMusicVoice.MusicSource.loop = loop;
            _audioMusicVoice.Priority = priority;
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
                StorageManager.Instance.SetFloat(_volumeVoiceKey, value);
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
                StorageManager.Instance.SetBool(_switchVoiceKey, value);
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
                StorageManager.Instance.SetFloat(_volumeAudioEffectKey, value);
                _audioMusicBtnClick.MusicSource.volume = value;
                _audioMusicUISound.MusicSource.volume = value;
                _audioMusicAudioEffect.MusicSource.volume = value;
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
                StorageManager.Instance.SetBool(_switchAudioEffectKey, value);
                if (!value)
                {
                    _audioMusicBtnClick.MusicSource.Stop();
                    _audioMusicUISound.MusicSource.Stop();
                    _audioMusicAudioEffect.MusicSource.Stop();
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
        public void PlayBtnClick(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < _audioMusicBtnClick.Priority)
            {
                return;
            }
            _audioMusicBtnClick.Load(assetName, callback);
            _audioMusicBtnClick.MusicSource.loop = loop;
            _audioMusicBtnClick.Priority = priority;
        }
        
        /*----------UI音效特效----------*/
        
        // 设置UI音效播放完成回调
        public void SetUISoundComplete(Action callback)
        {
            _audioMusicUISound.OnComplete = callback;
        }

        // 播放UI音效
        public void PlayUISound(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < _audioMusicUISound.Priority)
            {
                return;
            }
            _audioMusicUISound.Load(assetName, callback);
            _audioMusicUISound.MusicSource.loop = loop;
            _audioMusicUISound.Priority = priority;
        }
                
        /*----------音效特效----------*/
        
        // 设置音效特效播放完成回调
        public void SetAudioEffectComplete(Action callback)
        {
            _audioMusicAudioEffect.OnComplete = callback;
        }

        // 播放音效特效
        public void PlayAudioEffect(string assetName, Action callback = null, bool loop = false, int priority = 0)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < _audioMusicAudioEffect.Priority)
            {
                return;
            }
            _audioMusicAudioEffect.Load(assetName, callback);
            _audioMusicAudioEffect.MusicSource.loop = loop;
            _audioMusicAudioEffect.Priority = priority;
        }
        
        /*----------3D音效特效----------*/
        public void PlayAudioEffect3D(string assetName, bool isRandom = false, Vector3? audioListenerPosition = null, float volume = 1f, Action callback = null)
        {
            if (!_switchAudioEffect)
            {
                return ;
            }
            Vector3 actualPosition = audioListenerPosition.GetValueOrDefault(_transform.position);
            float actualVolume = volume * _volumeAudioEffect;
            _audioMusicAudioEffect3D.Load(assetName, actualPosition, actualVolume, callback, _audioEffectMixerGroup, isRandom);
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
        
        private float Remap01ToDB(float linearVolume)
        {
            // 如果音量为0或负数，将其调整为一个非常小的正数，以避免对数计算错误
            if (linearVolume <= 0.0f)
            {
                linearVolume = 0.0001f;
            }

            // 将线性音量值转换为分贝值
            float dbVolume = Mathf.Log10(linearVolume) * 20.0f;

            return dbVolume;
        }
    }
}