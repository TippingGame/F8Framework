using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
	public class UIImageInjector : AssetInjectorBase
	{
		readonly Image image;
		readonly Sprite[] sprites;

		public UIImageInjector(Image image, Sprite[] sprites)
		{
			this.image = image;
			this.sprites = sprites;
		}

		public override void Inject<T1, T2>(T1 localizedData, T2 localizer)
		{
			if (!image)
			{
				Unload();
				return;
			}

			if (localizedData is null)
			{
				Unload();
				return;
			}

			if (localizedData is int index)
			{
				UseDirectAsset();
				image.sprite = GetSprite(index);
			}
			else if (localizedData is string textIDValue)
			{
				LoadLocalizedAsset<Sprite>(textIDValue, (asset) =>
				{
					if (image)
					{
						image.sprite = asset;
						return;
					}

					Unload();
				});
			}
		}

		private Sprite GetSprite(int index)
		{
			return sprites != null && index >= 0 && index < sprites.Length ? sprites[index] : null;
		}

		protected override void ClearTarget()
		{
			if (image)
			{
				image.sprite = null;
			}
		}
	}
}
