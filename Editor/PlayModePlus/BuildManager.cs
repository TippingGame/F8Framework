using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class BuildManager
    {
        public Preset SelectedBuildPreset;
        public Preset[] PlayerSettingsPresetsInProject;

        public void OpenBuildWindow()
        {
#if UNITY_6000_0_OR_NEWER
            EditorApplication.ExecuteMenuItem("File/Build Profiles");
#else
            var buildPlayerWindow = EditorWindow.GetWindow<BuildPlayerWindow>("Build Settings");
            buildPlayerWindow.Show();
#endif
        }

        public static void ApplyPreset(Preset preset)
        {
            var projectSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();

            foreach (var settings in projectSettings)
            {
                preset.ApplyTo(settings);
            }
        }

        public List<string> GenerateBuildSettingsList()
        {
            PlayerSettingsPresetsInProject = AssetDatabase.FindAssets("t:Preset")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.Contains("PlayerSettings"))
                .Select(AssetDatabase.LoadAssetAtPath<Preset>)
                .ToArray();

            var buildPresetsList = new List<string>(PlayerSettingsPresetsInProject.Select(preset => preset.name));

            return buildPresetsList;
        }
    }
}