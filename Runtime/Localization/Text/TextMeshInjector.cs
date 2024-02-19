using UnityEngine;

namespace F8Framework.Core
{
	public class TextMeshInjector : IInjector
	{
		readonly TextMesh textMesh;

		public TextMeshInjector(TextMesh textMesh)
		{
			this.textMesh = textMesh;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			textMesh.text = localizedData as string;
		}
	}
}
