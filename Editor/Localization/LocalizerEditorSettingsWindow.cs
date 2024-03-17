using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
	public class LocalizerEditorSettingsWindow : EditorWindow
	{
		public static int index = 0;
		public string[] options = Language.BuiltinLanguages.Select(lang => lang.ToString()).ToArray();
		
		[UnityEditor.MenuItem("开发工具/本地化工具 _F6", false, 100)]
		static void Open()
		{
			if (HasOpenInstances<LocalizerEditorSettingsWindow>())
			{
				GetWindow<LocalizerEditorSettingsWindow>("本地化工具 F6").Close();
			}
			else
			{
				LocalizationEditorSettings.LoadEditorSettings();

				for (int i = 0; i < Language.BuiltinLanguages.Length; i++)
				{
					if (Language.BuiltinLanguages[i].Name == LocalizationSettings.LoadLanguageSettings())
					{
						index = i;
					}
				}
				
				GetWindow<LocalizerEditorSettingsWindow>("本地化工具 F6");
			}
		}

		void OnGUI()
		{
			var prevSettings = LocalizationEditorSettings.current.Clone();
			EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
			for (int i = 0; i < Language.BuiltinLanguages.Length; i++)
			{
				if (Language.BuiltinLanguages[i].Name == LocalizationSettings.LoadLanguageSettings())
				{
					index = i;
				}
			}
			index = EditorGUILayout.Popup(index, options);
			if (options[index] != LocalizationSettings.LoadLanguageSettings())
			{
				Localization.Instance.CurrentLanguageName = options[index];
				LocalizationSettings.SaveLanguageSettings();
				if (Application.isPlaying)
				{
					Localization.Instance.InjectAll();
				}
			}

			DrawSettingsPanel(ref LocalizationEditorSettings.current);
			if (prevSettings.enableTMP != LocalizationEditorSettings.current.enableTMP)
			{
				if (LocalizationEditorSettings.current.enableTMP)
				{
					var enableTMP = AskToEnableTMP();
					LocalizationEditorSettings.current.enableTMP = enableTMP;
					if (enableTMP) TMPIntegrationSwitcher.Enable();
				}
				else
				{
					TMPIntegrationSwitcher.Disable();
				}
			}

			if (LocalizationEditorSettings.current != prevSettings) LocalizationEditorSettings.SaveEditorSettings();
		}

		static void DrawSettingsPanel(ref LocalizationEditorSettings.SettingsDefinition settings)
		{
			EditorGUILayout.Space();
			settings.DefaultLanguage = EditorGUILayout.TextField("当前语言（只读）", LocalizationSettings.LoadLanguageSettings());
			settings.maxSuggestion = EditorGUILayout.IntField("最大显示ID个数", settings.maxSuggestion);
			settings.enableTMP = EditorGUILayout.Toggle("项目使用 TextMesh Pro", settings.enableTMP);
			EditorGUILayout.Space();
			if (GUILayout.Button("Reset All")) ResetAllSettings();
		}

		static bool AskToEnableTMP()
		{
			const string message = "如果您的项目没有 TextMesh Pro，这将导致编译错误。是否继续开启？";
			return EditorUtility.DisplayDialog("启用 TextMesh Pro 集成", message, "确定", "取消");
		}

		static void ResetAllSettings()
		{
			LocalizationEditorSettings.ResetAll();
			TMPIntegrationSwitcher.Disable();
		}
	}
}
