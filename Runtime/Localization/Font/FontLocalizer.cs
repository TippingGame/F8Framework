using UnityEngine;
using UnityEngine.UI;
#if LOCALIZER_TMP
using TMPro;
#endif

namespace F8Framework.Core
{
	public class FontLocalizer : LocalizerBase
	{
		public string localizedTextID = "";
		public Font[] fonts;
#if LOCALIZER_TMP
		public TMP_FontAsset[] TMP_fontAsset;
#endif
		private bool hasTMP_Text = false;
		
		protected override void Prepare()
		{
#if LOCALIZER_TMP
			var component = ComponentFinder.Find<TextMesh, Text, TMP_Text>(this);
#else
			var component = ComponentFinder.Find<TextMesh, Text>(this);
#endif
			if (component == null) return;

			if (component is TextMesh textMesh)
			{
				injector = new FontInjector(textMesh);
			}
			else if (component is Text text)
			{
				injector = new FontInjector(text);
			}
#if LOCALIZER_TMP
			else if (component is TMP_Text tmp)
			{
				injector = new FontInjector(tmp);
				hasTMP_Text = true;
			}
#endif
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
			if (!hasTMP_Text)
			{
				injector.Inject(fonts?[index], this);
			}
#if LOCALIZER_TMP
			else
			{
				injector.Inject(TMP_fontAsset?[index], this);
			}
#endif
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
