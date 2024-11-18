using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace F8Framework.Core.Editor
{
    public class PlayModeManager
    {
        public SceneAsset SelectedScene;
        public SceneAsset[] ScenesInProject;
        private string[] _playmodeSettings;

        private static string PrefsKey => "playmodeplus-lastScene";
        private SceneAsset _lastScene;

        public static void EditorStartScene() => EditorSceneManager.playModeStartScene = null;

        public SceneAsset LastScene
        {
            get
            {
                if (_lastScene != null) return _lastScene;
                var lastScenePath = F8EditorPrefs.GetString(PrefsKey, null);
                if (lastScenePath != null)
                {
                    _lastScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(lastScenePath);
                }

                return _lastScene;
            }
            set
            {
                _lastScene = value;
                var lastScenePath = AssetDatabase.GetAssetPath(_lastScene);
                F8EditorPrefs.SetString(PrefsKey, lastScenePath);
            }
        }

        public void PlayScene()
        {
            if (!EditorApplication.isPlaying)
            {
                LastScene = SelectedScene;
                EditorSceneManager.playModeStartScene = SelectedScene;
                EditorApplication.isPlaying = true;
            }
            else
                EditorApplication.isPlaying = false;
        }

        public List<string> GenerateSceneList()
        {
            ScenesInProject = AssetDatabase.FindAssets("t:SceneAsset")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SceneAsset>)
                .ToArray();

            var scenesList = new List<string>(ScenesInProject.Select(scene => scene.name));

            return scenesList;
        }

        public List<string> GeneratePlayModeSettingsList()
        {
            _playmodeSettings = new[]
            {
                "Default (Reload Domain, Reload Scene)", "Disable Reload Domain", "Disable Reload Scene", "Disable All"
            };

            var playModeSettingsList = new List<string>(_playmodeSettings);

            return playModeSettingsList;
        }
    }
}