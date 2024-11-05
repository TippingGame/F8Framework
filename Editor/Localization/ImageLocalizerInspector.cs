using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core.Editor
{
	[CustomEditor(typeof(ImageLocalizer))]
	public class ImageLocalizerInspector : UnityEditor.Editor
	{
		ImageLocalizer localizer;
		SerializedProperty localizedTextID;
		SerializedProperty propertyName;

		void OnEnable()
		{
			localizer = (ImageLocalizer)target;
			propertyName = serializedObject.FindProperty("propertyName");
			localizedTextID = serializedObject.FindProperty("localizedTextID");
		}

		public override void OnInspectorGUI()
		{
			// base.OnInspectorGUI();

			Localization.Instance.LoadInEditor();
			serializedObject.Update();

			var langCount = Localization.Instance.LanguageList.Count;

			var component = ComponentFinder.Find<Image, RawImage, SpriteRenderer, Renderer>(localizer);
			
			if (component is Image)
			{
				UpdateSpriteInspector(langCount);
			}
			else if (component is RawImage)
			{
				UpdateTextureInspector(langCount);
			}
			else if (component is SpriteRenderer)
			{
				UpdateSpriteInspector(langCount);
			}
			else if (component is Renderer)
			{
				UpdateTexture2DInspector(langCount);
			}

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
				EditorGUILayout.HelpBox($"输入 Text ID 或 拖拽图片 到上方", MessageType.Info);

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
		
		void UpdateTexture2DInspector(int langCount)
		{
			EditorGUILayout.PropertyField(localizedTextID);
			EditorGUILayout.PropertyField(propertyName);
			
			if (!localizedTextID.stringValue.IsNullOrEmpty())
			{
				return;
			}
			
			if (localizer.texture2Ds == null)
			{
				localizer.texture2Ds = new Texture2D[langCount];
			}
			else if (localizer.texture2Ds.Length != langCount)
			{
				var oldTexture2Ds = localizer.texture2Ds;
				localizer.texture2Ds = new Texture2D[langCount];
				for (var i = 0; i < langCount; i++)
				{
					localizer.texture2Ds[i] = oldTexture2Ds[i];
				}
			}

			for (var i = 0; i < langCount; i++)
			{
				var tex = EditorGUILayout.ObjectField(Localization.Instance.LanguageList[i], localizer.texture2Ds[i], typeof(Texture2D), false) as Texture2D;
				if (localizer.texture2Ds[i] != tex)
				{
					localizer.texture2Ds[i] = tex;
					EditorUtility.SetDirty(localizer);
				}
			}
		}

		void UpdateTextureInspector(int langCount)
		{
			EditorGUILayout.PropertyField(localizedTextID);
			
			if (!localizedTextID.stringValue.IsNullOrEmpty())
			{
				return;
			}
			
			if (localizer.textures == null)
			{
				localizer.textures = new Texture[langCount];
			}
			else if (localizer.textures.Length != langCount)
			{
				var oldTextures = localizer.textures;
				localizer.textures = new Texture[langCount];
				for (var i = 0; i < langCount; i++)
				{
					localizer.textures[i] = oldTextures[i];
				}
			}

			for (var i = 0; i < langCount; i++)
			{
				var tex = EditorGUILayout.ObjectField(Localization.Instance.LanguageList[i], localizer.textures[i], typeof(Texture), false) as Texture;
				if (localizer.textures[i] != tex)
				{
					localizer.textures[i] = tex;
					EditorUtility.SetDirty(localizer);
				}
			}
		}

		void UpdateSpriteInspector(int langCount)
		{
			EditorGUILayout.PropertyField(localizedTextID);
			
			if (!localizedTextID.stringValue.IsNullOrEmpty())
			{
				return;
			}
			
			if (localizer.sprites == null)
			{
				localizer.sprites = new Sprite[langCount];
			}
			else if (localizer.sprites.Length != langCount)
			{
				var oldSprites = localizer.sprites;
				localizer.sprites = new Sprite[langCount];
				for (var i = 0; i < langCount; i++)
				{
					localizer.sprites[i] = oldSprites[i];
				}
			}

			for (var i = 0; i < langCount; i++)
			{
				var sprite = EditorGUILayout.ObjectField(Localization.Instance.LanguageList[i], localizer.sprites[i], typeof(Sprite), false) as Sprite;
				if (localizer.sprites[i] != sprite)
				{
					localizer.sprites[i] = sprite;
					EditorUtility.SetDirty(localizer);
				}
			}
		}
	}
}
