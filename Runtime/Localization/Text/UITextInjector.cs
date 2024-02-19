using UnityEngine.UI;

namespace F8Framework.Core
{
	public class UITextInjector : IInjector
	{
		readonly Text uiText;

		public UITextInjector(Text uiText)
		{
			this.uiText = uiText;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			uiText.text = localizedData as string;
		}
	}
}
