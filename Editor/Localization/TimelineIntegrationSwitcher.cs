using UnityEditor;

namespace F8Framework.Core.Editor
{
	public static class TimelineIntegrationSwitcher
	{
		const string Define = "LOCALIZER_TIMELINE";

		public static void EnsureIntegrationState()
		{
			if (Enabled && !LocalizationEditorSettings.current.enableTimeline) Disable();
			if (!Enabled && LocalizationEditorSettings.current.enableTimeline) Enable();
		}

		internal static void Enable()
		{
			if (Enabled) return;

			var symbols = $"{CurrentSymbols};{Define}";
			PlayerSettings.SetScriptingDefineSymbolsForGroup(Target, symbols);
		}

		internal static void Disable()
		{
			if (!Enabled) return;

			var symbols = CurrentSymbols.Replace(Define, "");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(Target, symbols);
		}

		static bool Enabled => CurrentSymbols.Contains(Define);
		static string CurrentSymbols => PlayerSettings.GetScriptingDefineSymbolsForGroup(Target);
		static BuildTargetGroup Target => EditorUserBuildSettings.selectedBuildTargetGroup;
	}
}
