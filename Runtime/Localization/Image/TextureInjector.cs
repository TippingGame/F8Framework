using UnityEngine;

namespace F8Framework.Core
{
	public class TextureInjector : IInjector
	{
		readonly Renderer renderer;
		readonly string propertyName;
		readonly Texture2D[] texture2Ds;

		public TextureInjector(Renderer renderer, string propertyName, Texture2D[] texture2Ds)
		{
			this.renderer = renderer;
			this.propertyName = propertyName;
			this.texture2Ds = texture2Ds;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			if (localizedData is int index)
			{
				renderer.material.SetTexture(propertyName, texture2Ds?[index]);
			}
			else if (localizedData is string textIDValue)
			{
				AssetManager.Instance.LoadAsync(textIDValue, (asset) =>
				{
					if (asset is Sprite sprite)
					{
						Texture2D texture = sprite.texture;
						renderer.material.SetTexture(propertyName, texture);
						LogF8.LogAsset("本地化图片类型错误，已自动转换：" + asset);
						return;
					}
					renderer.material.SetTexture(propertyName, asset as Texture2D);
				});
			}
		}
	}
}
