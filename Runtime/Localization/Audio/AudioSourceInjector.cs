using UnityEngine;

namespace F8Framework.Core
{
	public class AudioSourceInjector : AssetInjectorBase
	{
		readonly AudioSource audio;

		public AudioSourceInjector(AudioSource audio)
		{
			this.audio = audio;
		}

		public override void Inject<T1, T2>(T1 localizedData, T2 localizer)
		{
			if (!audio)
			{
				Unload();
				return;
			}

			if (localizedData is null)
			{
				Unload();
				return;
			}

			var isPlaying = audio.isPlaying;
#if !UNITY_WEBGL
            var time = audio.time;
#endif
			if (isPlaying) audio.Stop();
			var playFromSamePosition = (localizer as AudioLocalizer)?.playFromSamePositionWhenInject;

			if (localizedData is AudioClip audioClip)
			{
				UseDirectAsset();
				Play(audioClip);
			}
			else if (localizedData is string textIDValue)
			{
				LoadLocalizedAsset<AudioClip>(textIDValue, Play);
			}
			
			void Play(AudioClip clip)
			{
				if (!audio || !clip)
				{
					Unload();
					return;
				}

				audio.clip = clip;
				if (isPlaying)
				{
					audio.Play();
#if !UNITY_WEBGL
					if (playFromSamePosition.HasValue && playFromSamePosition.Value)
					{
						audio.time = time;
					}
					else
					{
						audio.time = 0f;
					}
#endif
				}
			}
		}

		protected override void ClearTarget()
		{
			if (!audio)
			{
				return;
			}

			audio.Stop();
			audio.clip = null;
		}
	}
}
