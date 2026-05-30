using UnityEngine;
using UnityEngine.UI;
#if LOCALIZER_TMP
using TMPro;
#endif

namespace F8Framework.Core
{
	public class FontInjector : AssetInjectorBase
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

		public override void Inject<T1, T2>(T1 localizedData, T2 localizer)
		{
			if (!HasTarget())
			{
				Unload();
				return;
			}

			if (localizedData is null)
			{
				Unload();
				return;
			}

			if (localizedData is Font font)
			{
				UseDirectAsset();
				if (text)
				{
					text.font = font;
				}
				else if (textMesh)
				{
					textMesh.font = font;
				}
			}
#if LOCALIZER_TMP
			else if (localizedData is TMP_FontAsset TMP_fonts)
			{
				UseDirectAsset();
				if (TMP_text)
				{
					TMP_text.font = TMP_fonts;
				}
			}
#endif
			else if (localizedData is string textIDValue)
			{
				LoadLocalizedAsset(textIDValue, (asset) =>
				{
					if (asset is Font loadFont)
					{
						if (text)
						{
							text.font = loadFont;
							return;
						}
						else if (textMesh)
						{
							textMesh.font = loadFont;
							return;
						}
					}
#if LOCALIZER_TMP
					if (asset is TMP_FontAsset loadTMP_fontAsset)
					{
						if (TMP_text)
						{
							TMP_text.font = loadTMP_fontAsset;
							return;
						}
					}
#endif
					Unload();
				});
			}
		}

		private bool HasTarget()
		{
			if (text || textMesh)
			{
				return true;
			}
#if LOCALIZER_TMP
			if (TMP_text)
			{
				return true;
			}
#endif
			return false;
		}

		protected override void ClearTarget()
		{
			if (text)
			{
				text.font = null;
			}
			else if (textMesh)
			{
				textMesh.font = null;
			}
#if LOCALIZER_TMP
			else if (TMP_text)
			{
				TMP_text.font = null;
			}
#endif
		}
	}
}
