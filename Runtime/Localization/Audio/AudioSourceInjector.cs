using UnityEngine;

namespace F8Framework.Core
{
	public class AudioSourceInjector : IInjector
	{
		readonly AudioSource audio;

		public AudioSourceInjector(AudioSource audio)
		{
			this.audio = audio;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			var isPlaying = audio.isPlaying;
			var time = audio.time;
			if (isPlaying) audio.Stop();
			var playFromSamePosition = (localizer as AudioLocalizer)?.playFromSamePositionWhenInject;

			if (localizedData is AudioClip audioClip)
			{
				Play(audioClip);
			}
			else if (localizedData is string textIDValue)
			{
				AssetManager.Instance.LoadAsync<AudioClip>(textIDValue, Play);
			}
			
			void Play(AudioClip clip)
			{
				audio.clip = clip;
				if (isPlaying)
				{
					audio.Play();
					if (playFromSamePosition.HasValue && playFromSamePosition.Value)
					{
						audio.time = time;
					}
					else
					{
						audio.time = 0f;
					}
				}
			}
		}
	}
}
