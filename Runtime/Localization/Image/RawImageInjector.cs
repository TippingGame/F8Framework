using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
	public class RawImageInjector : AssetInjectorBase
	{
		readonly RawImage rawImage;
		readonly Texture[] textures;

		public RawImageInjector(RawImage rawImage, Texture[] textures)
		{
			this.rawImage = rawImage;
			this.textures = textures;
		}

		public override void Inject<T1, T2>(T1 localizedData, T2 localizer)
		{
			if (!rawImage)
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
				rawImage.texture = GetTexture(index);
			}
			else if (localizedData is string textIDValue)
			{
				LoadLocalizedAsset<Texture>(textIDValue, (asset) =>
				{
					if (rawImage)
					{
						rawImage.texture = asset;
						return;
					}

					Unload();
				});
			}
		}

		private Texture GetTexture(int index)
		{
			return textures != null && index >= 0 && index < textures.Length ? textures[index] : null;
		}

		protected override void ClearTarget()
		{
			if (rawImage)
			{
				rawImage.texture = null;
			}
		}
	}
}
