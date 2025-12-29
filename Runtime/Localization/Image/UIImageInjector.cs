using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
	public class UIImageInjector : IInjector
	{
		readonly Image image;
		readonly Sprite[] sprites;

		public UIImageInjector(Image image, Sprite[] sprites)
		{
			this.image = image;
			this.sprites = sprites;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			if (localizedData is int index)
			{
				image.sprite = sprites?[index];
			}
			else if (localizedData is string textIDValue)
			{
				AssetManager.Instance.LoadAsync<Sprite>(textIDValue, (asset) =>
				{
					image.sprite = asset;
				});
			}
		}
	}
}
