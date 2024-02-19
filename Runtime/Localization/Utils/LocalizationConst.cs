namespace F8Framework.Core
{
	public static class LocalizationConst
	{
		public static readonly string LocalizationSettingsKey = "LocalizationSettings";
		public static readonly string CurrentLanguageKey = "CurrentLanguage";
		public static string SheetName => "LocalizedStrings.tsv";

		public static string SheetPath => UnityEngine.Application.streamingAssetsPath + "/localization/" + SheetName;
	}
}
