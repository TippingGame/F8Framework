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
				AssetManager.Instance.LoadAsync<Texture>(textIDValue, (asset) =>
				{
					rawImage.texture = asset;
				});
			}
		}
	}
}
