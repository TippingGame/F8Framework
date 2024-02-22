using UnityEngine;

namespace F8Framework.Core
{
	public class AudioLocalizer : LocalizerBase
	{
		public AudioClip[] clips;
		public bool playFromSamePositionWhenInject;

		protected override void Prepare()
		{
			var component = ComponentFinder.Find<AudioSource>(this);
			if (component == null) return;

			if (component is AudioSource audio)
			{
				injector = new AudioSourceInjector(audio);
			}
		}

		internal override void Localize()
		{
			var index = Localization.Instance.CurrentLanguageIndex;
			injector.Inject(clips[index], this);
		}
	}
}
