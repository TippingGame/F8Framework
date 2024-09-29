using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
	public class LocalizationEditorSettings
	{
		public class SettingsDefinition
		{
			public string DefaultLanguage = Application.systemLanguage.ToString();
			public int maxSuggestion = 50;
			public bool enableTMP = false;

			public SettingsDefinition Clone() => (SettingsDefinition)MemberwiseClone();
		}

		public static SettingsDefinition current;

		public static void ResetAll()
		{
			current = new SettingsDefinition();
		}

		// 初始化设置
		public static void LoadEditorSettings()
		{
			var json = F8EditorPrefs.GetString(LocalizationConst.LocalizationSettingsKey, "");
			if (json == "")
			{
				current = new SettingsDefinition();
			}
			else
			{
				current = JsonUtility.FromJson<SettingsDefinition>(json);
			}
		}

		public static void SaveEditorSettings()
		{
			var definition = current;
			var json = JsonUtility.ToJson(definition);
			F8EditorPrefs.SetString(LocalizationConst.LocalizationSettingsKey, json);
		}
	}
}
