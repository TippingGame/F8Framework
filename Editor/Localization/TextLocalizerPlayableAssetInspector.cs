﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
	[CustomEditor(typeof(TextLocalizerPlayableAsset))]
	public class TextLocalizerPlayableAssetInspector : UnityEditor.Editor
	{
		TextLocalizerPlayableAsset localizer;

		void OnEnable()
		{
			localizer = target as TextLocalizerPlayableAsset;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUI.skin.GetStyle("HelpBox").richText = true;
			Localization.Instance.LoadInEditor();
			var keys = Localization.Instance.GetAllIds();

			if (keys.Count == 0)
			{
				EditorGUILayout.HelpBox("没有可用的数据。\n请将LocalizedStrings.tsv放在StreamingAssets/localization文件夹中。", MessageType.Info);
				return;
			}

			if (string.IsNullOrEmpty(localizer.textId))
			{
				EditorGUILayout.HelpBox($"请输入 Text ID", MessageType.Info);

				var postfix = keys.Count > 5 ? $"\n\n<i>还有更多 (共 {keys.Count.ToString()} 个ID)</i>" : "";
				ShowSuggestion(keys.ToList(), 5, postfix);
				return;
			}

			var dict = Localization.Instance.GetDictionaryFromId(localizer.textId);
			if (dict != null)
			{
				var helpText = dict.Aggregate("", (current, item) => current + $"{item.Key}: {item.Value}\n");
				helpText = helpText.TrimEnd('\n');
				EditorGUILayout.HelpBox($"{helpText}", MessageType.Info);
			}
			else
			{
				EditorGUILayout.HelpBox($"Text ID: {localizer.textId} 不可用。", MessageType.Error);
			}

			var suggestions = keys.Where(key => key.StartsWith(localizer.textId)).ToList();
			ShowSuggestion(suggestions);
		}

		void ShowSuggestion(IReadOnlyCollection<string> suggestions, int limit = int.MaxValue, string postfix = "")
		{
			var noSuggestion = suggestions.Count == 0;
			var exactMatch = suggestions.Count == 1 && suggestions.First() == localizer.textId;
			if (noSuggestion || exactMatch) return;

			var text = suggestions.Take(limit)
			                      .Aggregate("\n<b>ID 索引</b>\n", (current, item) => $"{current}\n- {GetMarkedIdRepresentation(item)}");
			text += string.IsNullOrEmpty(postfix) ? "" : postfix;
			EditorGUILayout.HelpBox($"{text}\n", MessageType.Info);

			string GetMarkedIdRepresentation(string id)
			{
				if (string.IsNullOrEmpty(localizer.textId))
				{
					return id;
				}
				else
				{
					return $"<color=green>{id.Insert(localizer.textId.Length, "</color>")}";
				}
			}
		}
	}
}
