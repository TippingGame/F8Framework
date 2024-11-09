using UnityEngine;
using UnityEngine.UI;
#if LOCALIZER_TMP
using TMPro;
#endif

namespace F8Framework.Core
{
	public class FontInjector : IInjector
	{
		readonly Text text;
		readonly TextMesh textMesh;
#if LOCALIZER_TMP
		readonly TMP_Text TMP_text;
#endif

		public FontInjector(Text text)
		{
			this.text = text;
		}

		public FontInjector(TextMesh textMesh)
		{
			this.textMesh = textMesh;
		}
		
#if LOCALIZER_TMP
		public FontInjector(TMP_Text TMP_text)
		{
			this.TMP_text = TMP_text;
		}
#endif

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			if (localizedData is Font font)
			{
				if (text)
				{
					text.font = font;
				}
				else if (textMesh)
				{
					text.font = font;
				}
			}
#if LOCALIZER_TMP
			else if (localizedData is TMP_FontAsset TMP_fonts)
			{
				if (TMP_text)
				{
					TMP_text.font = TMP_fonts;
				}
			}
#endif
			else if (localizedData is string textIDValue)
			{
				AssetManager.Instance.LoadAsync(textIDValue, (asset) =>
				{
					if (asset is Font loadFont)
					{
						if (text)
						{
							text.font = loadFont;
						}
						else if (textMesh)
						{
							text.font = loadFont;
						}
						return;
					}
#if LOCALIZER_TMP
					if (asset is TMP_FontAsset loadTMP_fontAsset)
					{
						if (TMP_text)
						{
							TMP_text.font = loadTMP_fontAsset;
						}
					}
#endif
				});
			}
		}
	}
}
