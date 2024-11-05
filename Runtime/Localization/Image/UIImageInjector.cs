using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
	public class UIImageInjector : IInjector
	{
		readonly string localizedTextID;
		readonly Image image;
		readonly Sprite[] sprites;

		public UIImageInjector(Image image, string localizedTextID, Sprite[] sprites)
		{
			this.localizedTextID = localizedTextID;
			this.image = image;
			this.sprites = sprites;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			if (localizedData is int index)
			{
				if (localizedTextID.IsNullOrEmpty())
				{
					image.sprite = sprites[index];
				}
				else
				{
					string textIDValue = Localization.Instance.GetTextFromId(localizedTextID);
					AssetManager.Instance.LoadAsync(textIDValue, (asset) =>
					{
						if (asset is Texture2D texture)
						{
							Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
							image.sprite = sprite;
							LogF8.LogAsset("本地化图片类型错误，已自动转换：" + asset);
							return;
						}
						image.sprite = asset as Sprite;
					});
				}
			}
		}
	}
}
