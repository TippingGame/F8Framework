#if LOCALIZER_TMP
using TMPro;

namespace F8Framework.Core
{
	public class TMPInjector : IInjector
	{
		readonly TMP_Text tmp;

		public TMPInjector(TMP_Text tmp)
		{
			this.tmp = tmp;
		}

		public void Inject<T1, T2>(T1 localizedData, T2 localizer) where T2 : LocalizerBase
		{
			tmp.text = localizedData as string;
		}
	}
}
#endif
