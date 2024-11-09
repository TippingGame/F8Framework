using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
	public class RawImageInjector : IInjector
	{
		readonly RawImage rawImage;
		readonly Texture[] textures;

		public RawImageInjector(RawImage rawImage, Texture[] textures)
		{
			this.rawImage = rawImage;
			this.textures = textures;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			if (localizedData is int index)
			{
				rawImage.texture = textures?[index];
			}
			else if (localizedData is string textIDValue)
			{
				AssetManager.Instance.LoadAsync(textIDValue, (asset) =>
				{
					if (asset is Sprite sprite)
					{
						Texture texture = sprite.texture;
						rawImage.texture = texture;
						LogF8.LogAsset("本地化图片类型错误，已自动转换：" + asset);
						return;
					}
					rawImage.texture = asset as Texture;
				});
			}
		}
	}
}
