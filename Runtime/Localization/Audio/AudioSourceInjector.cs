using UnityEngine;

namespace F8Framework.Core
{
	public class AudioSourceInjector : IInjector
	{
		readonly string localizedTextID;
		readonly AudioSource audio;

		public AudioSourceInjector(AudioSource audio, string localizedTextID)
		{
			this.localizedTextID = localizedTextID;
			this.audio = audio;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			var isPlaying = audio.isPlaying;
			var time = audio.time;
			if (isPlaying) audio.Stop();
			var playFromSamePosition = (localizer as AudioLocalizer)?.playFromSamePositionWhenInject;

			if (localizedTextID.IsNullOrEmpty())
			{
				Play(localizedData as AudioClip);
			}
			else
			{
				string textIDValue = Localization.Instance.GetTextFromId(localizedTextID);
				AssetManager.Instance.LoadAsync<AudioClip>(textIDValue, (asset) =>
				{
					Play(asset);
				});
			}
			
			void Play(AudioClip audioClip)
			{
				audio.clip = audioClip;
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
