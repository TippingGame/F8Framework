using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#if LOCALIZER_TMP
using TMPro;
#endif

namespace F8Framework.Core.Editor
{
	[CustomEditor(typeof(FontLocalizer))]
	public class FontLocalizerInspector : UnityEditor.Editor
	{
		FontLocalizer localizer;
		SerializedProperty localizedTextID;
		bool hasText;
#if LOCALIZER_TMP
		bool hasTMP_Text;
#endif
		
		void OnEnable()
		{
			localizer = (FontLocalizer)target;
			localizedTextID = serializedObject.FindProperty("localizedTextID");
			hasText = ComponentFinder.Find<TextMesh, Text>(localizer);
#if LOCALIZER_TMP
			hasTMP_Text = ComponentFinder.Find<TMP_Text>(localizer);
#endif
		}

		public override void OnInspectorGUI()
		{
			// base.OnInspectorGUI();

			Localization.Instance.LoadInEditor();
			serializedObject.Update();

			var langCount = Localization.Instance.LanguageList.Count;

			UpdateAudioClipInspector(langCount);

			serializedObject.ApplyModifiedProperties();
			
			GUI.skin.GetStyle("HelpBox").richText = true;
			Localization.Instance.LoadInEditor();
			var keys = Localization.Instance.GetAllIds();

			if (keys.Count == 0)
			{
				EditorGUILayout.HelpBox("没有可用的数据。\n请将本地化表放在Excel存放文件夹中。", MessageType.Info);
				return;
			}

			if (string.IsNullOrEmpty(localizer.localizedTextID))
			{
				EditorGUILayout.HelpBox($"输入 Text ID 或 拖拽字体 到上方", MessageType.Info);

				var postfix = keys.Count > 5 ? $"\n\n<i>还有更多（共 {keys.Count.ToString()} 个ID）</i>" : "";
				ShowSuggestion(keys.ToList(), postfix);
				return;
			}

			var dict = Localization.Instance.GetDictionaryFromId(localizer.localizedTextID);
			if (dict != null)
			{
				var helpText = dict.Aggregate("", (current, item) => current + $"{item.Key}: {item.Value}\n");
				helpText = helpText.TrimEnd('\n');
				EditorGUILayout.HelpBox($"{helpText}", MessageType.Info);
			}
			else
			{
				EditorGUILayout.HelpBox($"Text ID：{localizer.localizedTextID} 不可用。", MessageType.Error);
			}

			var suggestions = keys.Where(key => key.StartsWith(localizer.localizedTextID)).ToList();
			ShowSuggestion(suggestions);
		}
		
		void ShowSuggestion(IReadOnlyCollection<string> suggestions, string postfix = "")
		{
			var noSuggestion = suggestions.Count == 0;
			var exactMatch = suggestions.Count == 1 && suggestions.First() == localizer.localizedTextID;
			if (noSuggestion || exactMatch) return;

			var limit = LocalizationEditorSettings.current.maxSuggestion;
			var text = suggestions.Take(limit)
				.Aggregate("\n<b>ID 索引</b>\n", (current, item) => $"{current}\n- {GetMarkedIdRepresentation(item)}");
			text += string.IsNullOrEmpty(postfix) ? "" : postfix;
			EditorGUILayout.HelpBox($"{text}\n", MessageType.Info);

			string GetMarkedIdRepresentation(string id)
			{
				if (string.IsNullOrEmpty(localizer.localizedTextID))
				{
					return id;
				}
				else
				{
					return $"<color=green>{id.Insert(localizer.localizedTextID.Length, "</color>")}";
				}
			}
		}
		
		void UpdateAudioClipInspector(int langCount)
		{
			EditorGUILayout.PropertyField(localizedTextID);
			
			if (!localizedTextID.stringValue.IsNullOrEmpty())
			{
				return;
			}

			if (hasText)
			{
				if (localizer.fonts == null)
				{
					localizer.fonts = new Font[langCount];
				}
				else if (localizer.fonts.Length != langCount)
				{
					var oldFonts = localizer.fonts;
					localizer.fonts = new Font[langCount];
					for (var i = 0; i < langCount; i++)
					{
						localizer.fonts[i] = oldFonts[i];
					}
				}

				for (var i = 0; i < langCount; i++)
				{
					var font = EditorGUILayout.ObjectField(Localization.Instance.LanguageList[i], localizer.fonts[i], typeof(Font), false) as Font;
					if (localizer.fonts[i] != font)
					{
						localizer.fonts[i] = font;
						EditorUtility.SetDirty(localizer);
					}
				}
				return;
			}

#if LOCALIZER_TMP
			if (hasTMP_Text)
			{
				if (localizer.TMP_fontAsset == null)
				{
					localizer.TMP_fontAsset = new TMP_FontAsset[langCount];
				}
				else if (localizer.TMP_fontAsset.Length != langCount)
				{
					var oldFonts = localizer.TMP_fontAsset;
					localizer.TMP_fontAsset = new TMP_FontAsset[langCount];
					for (var i = 0; i < langCount; i++)
					{
						localizer.TMP_fontAsset[i] = oldFonts[i];
					}
				}

				for (var i = 0; i < langCount; i++)
				{
					var font = EditorGUILayout.ObjectField(Localization.Instance.LanguageList[i], localizer.TMP_fontAsset[i], typeof(TMP_FontAsset), false) as TMP_FontAsset;
					if (localizer.TMP_fontAsset[i] != font)
					{
						localizer.TMP_fontAsset[i] = font;
						EditorUtility.SetDirty(localizer);
					}
				}
				return;
			}
#endif
		}
	}
}
