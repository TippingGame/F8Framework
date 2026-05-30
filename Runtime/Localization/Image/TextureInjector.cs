using UnityEngine;

namespace F8Framework.Core
{
	public class TextureInjector : AssetInjectorBase
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

		public override void Inject<T1, T2>(T1 localizedData, T2 localizer)
		{
			if (!renderer)
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
				renderer.material.SetTexture(propertyName, GetTexture(index));
			}
			else if (localizedData is string textIDValue)
			{
				LoadLocalizedAsset<Texture2D>(textIDValue, (asset) =>
				{
					if (renderer)
					{
						renderer.material.SetTexture(propertyName, asset);
						return;
					}

					Unload();
				});
			}
		}

		private Texture2D GetTexture(int index)
		{
			return texture2Ds != null && index >= 0 && index < texture2Ds.Length ? texture2Ds[index] : null;
		}

		protected override void ClearTarget()
		{
			if (renderer)
			{
				renderer.material.SetTexture(propertyName, null);
			}
		}
	}
}
