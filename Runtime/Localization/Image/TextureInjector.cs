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
				AssetManager.Instance.LoadAsync<Texture2D>(textIDValue, (asset) =>
				{
					renderer.material.SetTexture(propertyName, asset);
				});
			}
		}
	}
}
