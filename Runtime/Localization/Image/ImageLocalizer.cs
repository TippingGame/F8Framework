using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
	public class ImageLocalizer : LocalizerBase
	{
		public string localizedTextID = "";
		public string propertyName = "_MainTex";
		public Texture2D[] texture2Ds;
		public Sprite[] sprites;
		public Texture[] textures;
		
		protected override void Prepare()
		{
			var component = ComponentFinder.Find<Image, RawImage, SpriteRenderer, Renderer>(this);
			if (component == null) return;
			
			if (component is Image image)
			{
				injector = new UIImageInjector(image, sprites);
			}
			else if (component is RawImage rawImage)
			{
				injector = new RawImageInjector(rawImage, textures);
			}
			else if (component is SpriteRenderer spriteRenderer)
			{
				injector = new SpriteRendererInjector(spriteRenderer, sprites);
			}
			else if (component is Renderer renderer)
			{
				injector = new TextureInjector(renderer, propertyName, texture2Ds);
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
			injector.Inject(index, this);
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
