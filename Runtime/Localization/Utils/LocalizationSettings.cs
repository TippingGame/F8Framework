using UnityEngine;

namespace F8Framework.Core
{
    public static class LocalizationSettings
    {
        public static string LoadLanguageSettings()
        {
#if UNITY_EDITOR
            string languageName = F8EditorPrefs.GetString(LocalizationConst.CurrentLanguageKey, "");
#else
            string languageName = PlayerPrefs.GetString(LocalizationConst.CurrentLanguageKey, "");
#endif

            if (string.IsNullOrEmpty(languageName))
            {
                languageName = Application.systemLanguage.ToString();
            }

            Localization.EditorInstance.CurrentLanguageName = languageName;
            return languageName;
        }

        public static void SaveLanguageSettings()
        {
            string languageName = Localization.EditorInstance.CurrentLanguageName;

#if UNITY_EDITOR
            F8EditorPrefs.SetString(LocalizationConst.CurrentLanguageKey, languageName);
#else
            PlayerPrefs.SetString(LocalizationConst.CurrentLanguageKey, languageName);
            PlayerPrefs.Save();
#endif
        }
    }
}