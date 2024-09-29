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
			var json = UnityEditor.EditorPrefs.GetString(Application.dataPath.GetHashCode() + LocalizationConst.CurrentLanguageKey, "");
			if (json == "")
			{
				// 根据系统语言设置
				Localization.Instance.CurrentLanguageName = Application.systemLanguage.ToString();
				return Application.systemLanguage.ToString();
			}
			else
			{
				var definition = JsonUtility.FromJson<Definition>(json);
				Localization.Instance.CurrentLanguageName = definition.currentLanguageName;
				return definition.currentLanguageName;
			}
#else
			Definition definition = StorageManager.Instance.GetObject<Definition>(LocalizationConst.CurrentLanguageKey, true);
			if (definition == null)
			{
				Localization.Instance.CurrentLanguageName = Application.systemLanguage.ToString();
			}
			else
			{
				Localization.Instance.CurrentLanguageName = definition.currentLanguageName;
			}
			return Localization.Instance.CurrentLanguageName;
#endif
		}

		public static void SaveLanguageSettings()
		{
#if UNITY_EDITOR
			var definition = new Definition { currentLanguageName = Localization.Instance.CurrentLanguageName };
			var json = JsonUtility.ToJson(definition);
			UnityEditor.EditorPrefs.SetString(Application.dataPath.GetHashCode() + LocalizationConst.CurrentLanguageKey, json);
#else
			var definition = new Definition { currentLanguageName = Localization.Instance.CurrentLanguageName };
			StorageManager.Instance.SetObject<Definition>(LocalizationConst.CurrentLanguageKey, definition, true);
#endif
		}
	}
}
