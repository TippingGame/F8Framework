using UnityEditor;
using UnityEngine;

namespace F8Framework.Core
{
	[CustomEditor(typeof(AudioLocalizer))]
	public class AudioLocalizerInspector : UnityEditor.Editor
	{
		AudioLocalizer localizer;
		SerializedProperty playFromSamePositionWhenInject;

		void OnEnable()
		{
			localizer = (AudioLocalizer)target;
			playFromSamePositionWhenInject = serializedObject.FindProperty("注入时从同一位置播放");
		}

		public override void OnInspectorGUI()
		{
			// base.OnInspectorGUI();

			Localizer.Load();
			serializedObject.Update();

			EditorGUILayout.PropertyField(playFromSamePositionWhenInject, new GUIContent("从同一位置播放"));

			var langCount = Localizer.LanguageList.Count;

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
				var clip = EditorGUILayout.ObjectField(Localizer.LanguageList[i], localizer.clips[i], typeof(AudioClip), false) as AudioClip;
				if (localizer.clips[i] != clip)
				{
					localizer.clips[i] = clip;
					EditorUtility.SetDirty(localizer);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
