using UnityEngine;

namespace F8Framework.Core
{
	public class AudioLocalizer : LocalizerBase
	{
		public string localizedTextID = "";
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
			if (injector == null)
			{
				return;
			}
			if (!localizedTextID.IsNullOrEmpty())
			{
				ChangeID(localizedTextID);
				return;
			}
			var index = Localization.Instance.CurrentLanguageIndex;
			injector.Inject(clips?[index], this);
		}
		
		public bool ChangeID(string textId)
		{
			if (string.IsNullOrEmpty(textId)) return false;

#if UNITY_EDITOR
			// for Timeline Preview
			if (!Application.isPlaying)
			{
				Localization.Instance.Load();
				Prepare();
			}
#endif

			if (!Localization.Instance.Has(textId))
			{
				if (Application.isPlaying) LogF8.LogError($"Text ID: {textId} 不可用。");
				return false;
			}

			this.localizedTextID = textId;
			var text = Localization.Instance.GetTextFromId(textId);
			injector.Inject(text, this);
			return true;
		}

		public void Clear()
		{
			localizedTextID = null;
			injector?.Inject("", this);
		}
	}
}
