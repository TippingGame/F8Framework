using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
	[CustomEditor(typeof(AudioLocalizer))]
	public class AudioLocalizerInspector : UnityEditor.Editor
	{
		AudioLocalizer localizer;
		SerializedProperty localizedTextID;
		SerializedProperty playFromSamePositionWhenInject;

		void OnEnable()
		{
			localizer = (AudioLocalizer)target;
			playFromSamePositionWhenInject = serializedObject.FindProperty("playFromSamePositionWhenInject");
			localizedTextID = serializedObject.FindProperty("localizedTextID");
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
				EditorGUILayout.HelpBox($"输入 Text ID 或 拖拽音频 到上方", MessageType.Info);

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
			EditorGUILayout.PropertyField(playFromSamePositionWhenInject, new GUIContent("从同一位置播放"));
			
			if (!localizedTextID.stringValue.IsNullOrEmpty())
			{
				return;
			}

			if (localizer.clips == null)
			{
				localizer.clips = new AudioClip[langCount];
			}
			else if (localizer.clips.Length != langCount)
			{
				var oldClips = localizer.clips;
				localizer.clips = new AudioClip[langCount];
				for (var i = 0; i < langCount; i++)
				{
					localizer.clips[i] = oldClips[i];
				}
			}

			for (var i = 0; i < langCount; i++)
			{
				var clip = EditorGUILayout.ObjectField(Localization.Instance.LanguageList[i], localizer.clips[i], typeof(AudioClip), false) as AudioClip;
				if (localizer.clips[i] != clip)
				{
					localizer.clips[i] = clip;
					EditorUtility.SetDirty(localizer);
				}
			}
		}
	}
}
