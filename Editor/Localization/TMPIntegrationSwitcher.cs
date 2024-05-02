using UnityEditor;

namespace F8Framework.Core.Editor
{
	public static class TMPIntegrationSwitcher
	{
		const string Define = "LOCALIZER_TMP";

		// 确保集成状态
		public static void EnsureIntegrationState()
		{
			if (Enabled && !LocalizationEditorSettings.current.enableTMP) Disable();
			if (!Enabled && LocalizationEditorSettings.current.enableTMP) Enable();
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
