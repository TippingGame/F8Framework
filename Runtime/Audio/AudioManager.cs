using System;
using UnityEngine;
using UnityEngine.Audio;

namespace F8Framework.Core
{
    public class AudioManager : ModuleSingletonMono<AudioManager>, IModule
    {
        /*----------背景音乐----------*/
        public AudioMusic AudioMusic;
        private float _volumeMusic = 1f;
        private bool _switchMusic = true;
        
        /*----------人声----------*/
        public AudioMusic AudioMusicVoice;
        private float _volumeVoice = 1f;
        private bool _switchVoice = true;
        
        /*----------特效声----------*/
        public AudioMusic AudioMusicBtnClick;
        
        public AudioMusic AudioMusicUISound;
        
        public AudioMusic AudioMusicAudioEffect;

        /*----------一次性特效声----------*/
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
            AudioMusic = new AudioMusic();
            AudioMusic.MusicSource = gameObjectMusic.GetComponent<AudioSource>();
            AudioMusic.MusicSource.playOnAwake = false;
            AudioMusic.MusicSource.loop = false;

            GameObject gameObjectVoice = new GameObject("Voice", typeof(AudioSource));
            gameObjectVoice.transform.SetParent(_transform);
            AudioMusicVoice = new AudioMusic();
            AudioMusicVoice.MusicSource = gameObjectVoice.GetComponent<AudioSource>();
            AudioMusicVoice.MusicSource.playOnAwake = false;
            AudioMusicVoice.MusicSource.loop = false;

            GameObject gameObjectBtnClick = new GameObject("BtnClick", typeof(AudioSource));
            gameObjectBtnClick.transform.SetParent(_transform);
            AudioMusicBtnClick = new AudioMusic();
            AudioMusicBtnClick.MusicSource = gameObjectBtnClick.GetComponent<AudioSource>();
            AudioMusicBtnClick.MusicSource.playOnAwake = false;
            AudioMusicBtnClick.MusicSource.loop = false;
            
            GameObject gameObjectUISound = new GameObject("UISound", typeof(AudioSource));
            gameObjectUISound.transform.SetParent(_transform);
            AudioMusicUISound = new AudioMusic();
            AudioMusicUISound.MusicSource = gameObjectUISound.GetComponent<AudioSource>();
            AudioMusicUISound.MusicSource.playOnAwake = false;
            AudioMusicUISound.MusicSource.loop = false;
            
            GameObject gameObjectAudioEffect = new GameObject("AudioEffect", typeof(AudioSource));
            gameObjectAudioEffect.transform.SetParent(_transform);
            AudioMusicAudioEffect = new AudioMusic();
            AudioMusicAudioEffect.MusicSource = gameObjectAudioEffect.GetComponent<AudioSource>();
            AudioMusicAudioEffect.MusicSource.playOnAwake = false;
            AudioMusicAudioEffect.MusicSource.loop = false;
            
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
            AudioMusic.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Music")[0];
            AudioMusicVoice.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Voice")[0];
            AudioMusicBtnClick.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            AudioMusicUISound.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            AudioMusicAudioEffect.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioEffectMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioMixer = audioMixer;
        }
        
        public void OnUpdate()
        {
            
        }
        
        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            StopAll();
            Tween.Instance.CancelTween(AudioMusic.AudioTween);
            Tween.Instance.CancelTween(AudioMusicVoice.AudioTween);
            Tween.Instance.CancelTween(AudioMusicBtnClick.AudioTween);
            Tween.Instance.CancelTween(AudioMusicUISound.AudioTween);
            Tween.Instance.CancelTween(AudioMusicAudioEffect.AudioTween);
            
            Destroy(gameObject);
        }
        
        /*----------背景音乐----------*/
        
        // 设置背景音乐播放完成回调
        public void SetMusicComplete(Action callback)
        {
            AudioMusic.OnComplete = callback;
        }
        
        /// <summary>
        /// 播放背景音乐。
        /// </summary>
        /// <param name="assetName">资产名。</param>
        /// <param name="callback">播放完成回调。</param>
        /// <param name="loop">是否循环。</param>
        /// <param name="priority">优先级，高的覆盖低的。</param>
        /// <param name="fadeDuration">淡入持续时间。</param>
        public void PlayMusic(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchMusic)
            {
                return;
            }
            if (priority < AudioMusic.Priority)
            {
                return;
            }
            AudioMusic.Load(assetName, callback, loop, priority, fadeDuration);
        }
        
        // 设置背景乐播放进度
        public float SetProgressMusic
        {
            set => AudioMusic.Progress = value;
        }
        
        // 获取背景音乐播放进度
        public float ProgressMusic => AudioMusic.Progress;
        
        // 获取和设置背景音乐音量
        public float VolumeMusic
        {
            get => _volumeMusic;
            set
            {
                _volumeMusic = value;
                StorageManager.Instance.SetFloat(_volumeMusicKey, value);
                AudioMusic.MusicSource.volume = value;
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
                    AudioMusic.MusicSource.Stop();
                }
            }
        }
        
        /*----------人声----------*/
        
        // 设置人声播放完成回调
        public void SetVoiceComplete(Action callback)
        {
            AudioMusicVoice.OnComplete = callback;
        }

        // 播放人声
        public void PlayVoice(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchVoice)
            {
                return;
            }
            if (priority < AudioMusicVoice.Priority)
            {
                return;
            }
            AudioMusicVoice.Load(assetName, callback, loop, priority, fadeDuration);
        }
        
        // 设置人声播放进度
        public float SetProgressVoice
        {
            set => AudioMusicVoice.Progress = value;
        }
        
        // 获取人声播放进度
        public float ProgressVoice => AudioMusicVoice.Progress;
        
        // 获取和设置人声音量
        public float VolumeVoice
        {
            get => _volumeVoice;
            set
            {
                _volumeVoice = value;
                StorageManager.Instance.SetFloat(_volumeVoiceKey, value);
                AudioMusicVoice.MusicSource.volume = value;
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
                    AudioMusicVoice.MusicSource.Stop();
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
                AudioMusicBtnClick.MusicSource.volume = value;
                AudioMusicUISound.MusicSource.volume = value;
                AudioMusicAudioEffect.MusicSource.volume = value;
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
                    AudioMusicBtnClick.MusicSource.Stop();
                    AudioMusicUISound.MusicSource.Stop();
                    AudioMusicAudioEffect.MusicSource.Stop();
                }
            }
        }
        
        /*----------按钮音效特效----------*/
        
        // 设置按钮音效播放完成回调
        public void SetBtnClickComplete(Action callback)
        {
            AudioMusicBtnClick.OnComplete = callback;
        }

        // 播放按钮音效
        public void PlayBtnClick(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < AudioMusicBtnClick.Priority)
            {
                return;
            }
            AudioMusicBtnClick.Load(assetName, callback, loop, priority, fadeDuration);
        }
        
        /*----------UI音效特效----------*/
        
        // 设置UI音效播放完成回调
        public void SetUISoundComplete(Action callback)
        {
            AudioMusicUISound.OnComplete = callback;
        }

        // 播放UI音效
        public void PlayUISound(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < AudioMusicUISound.Priority)
            {
                return;
            }
            AudioMusicUISound.Load(assetName, callback, loop, priority, fadeDuration);
        }
                
        /*----------音效特效----------*/
        
        // 设置音效特效播放完成回调
        public void SetAudioEffectComplete(Action callback)
        {
            AudioMusicAudioEffect.OnComplete = callback;
        }

        // 播放音效特效
        public void PlayAudioEffect(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < AudioMusicAudioEffect.Priority)
            {
                return;
            }
            AudioMusicAudioEffect.Load(assetName, callback, loop, priority, fadeDuration);
        }
        
        /*----------一次性3D音效特效----------*/
        /// <summary>
        /// 播放一次性3D音效特效。
        /// </summary>
        /// <param name="assetName">资产名。</param>
        /// <param name="isRandom">是否随机音量音高。</param>
        /// <param name="audioPosition">音频播放位置。</param>
        /// <param name="volume">音量。</param>
        /// <param name="spatialBlend">2d到3d的比例。</param>
        /// <param name="maxNum">最大同时播放个数。</param>
        /// <param name="callback">播放完成回调。</param>
        public void PlayAudioEffect3D(string assetName, bool isRandom = false, Vector3? audioPosition = null, float volume = 1f, float spatialBlend = 1f,
            int maxNum = 5, Action callback = null)
        {
            if (!_switchAudioEffect)
            {
                return ;
            }
            Vector3 actualPosition = audioPosition.GetValueOrDefault(_transform.position);
            float actualVolume = volume * _volumeAudioEffect;
            _audioMusicAudioEffect3D.Load(assetName, actualPosition, actualVolume, spatialBlend, maxNum, callback, _audioEffectMixerGroup, isRandom);
        }
        
        /*----------全局控制----------*/
        public void ResumeAll()
        {
            AudioMusic.Resume();
            AudioMusicVoice.Resume();
            AudioMusicBtnClick.Resume();
            AudioMusicUISound.Resume();
            AudioMusicAudioEffect.Resume();
        }
        
        public void PauseAll() {
            AudioMusic.Pause();
            AudioMusicVoice.Pause();
            AudioMusicBtnClick.Pause();
            AudioMusicUISound.Pause();
            AudioMusicAudioEffect.Pause();
        }
        
        public void StopAll()
        {
            AudioMusic.Stop();
            AudioMusicVoice.Stop();
            AudioMusicBtnClick.Stop();
            AudioMusicUISound.Stop();
            AudioMusicAudioEffect.Stop();
        }
        
        public void UnloadAll(bool unloadAllLoadedObjects = true)
        {
            AudioMusic.UnloadAll(unloadAllLoadedObjects);
            AudioMusicVoice.UnloadAll(unloadAllLoadedObjects);
            AudioMusicBtnClick.UnloadAll(unloadAllLoadedObjects);
            AudioMusicUISound.UnloadAll(unloadAllLoadedObjects);
            AudioMusicAudioEffect.UnloadAll(unloadAllLoadedObjects);
            _audioMusicAudioEffect3D.UnloadAll(unloadAllLoadedObjects);
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