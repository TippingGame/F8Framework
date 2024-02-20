using UnityEditor;
using UnityEngine;

namespace F8Framework.Core
{
	public static class LocalizationSettings
	{
		class Definition
		{
			public string currentLanguageName;
		}

		public static string LoadLanguageSettings()
		{
#if UNITY_EDITOR
			var json = EditorPrefs.GetString(LocalizationConst.CurrentLanguageKey, "");
			if (json == "")
			{
				// 根据系统语言设置
				Localization.CurrentLanguageName = Application.systemLanguage.ToString();
				return Application.systemLanguage.ToString();
			}
			else
			{
				var definition = JsonUtility.FromJson<Definition>(json);
				Localization.CurrentLanguageName = definition.currentLanguageName;
				return definition.currentLanguageName;
			}
#else
			Definition definition = StorageManager.Instance.GetObject<Definition>(LocalizationConst.CurrentLanguageKey, true);
			if (definition == null)
			{
				Localizer.CurrentLanguageName = Application.systemLanguage.ToString();
			}
			else
			{
				Localizer.CurrentLanguageName = definition.currentLanguageName;
			}
			return Localizer.CurrentLanguageName;
#endif
		}

		public static void SaveLanguageSettings()
		{
#if UNITY_EDITOR
			var definition = new Definition { currentLanguageName = Localization.CurrentLanguageName };
			var json = JsonUtility.ToJson(definition);
			EditorPrefs.SetString(LocalizationConst.CurrentLanguageKey, json);
#else
			var definition = new Definition { currentLanguageName = Localizer.CurrentLanguageName };
			StorageManager.Instance.SetObject<Definition>(LocalizationConst.CurrentLanguageKey, definition, true);
#endif
		}
	}
}
