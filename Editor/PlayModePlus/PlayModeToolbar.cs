using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace F8Framework.Core.Editor
{
    public class PlayModeToolbar : VisualElement
    {
        private readonly PlayModeManager _playModeManager = new();
        private readonly BuildManager _buildManager = new();

        private readonly Button _playButton;
        private readonly Button _buildButton;
        private readonly DropdownField _selectSceneDropdown;
        private readonly DropdownField _selectPlayModeSettingsDropdown;
        private readonly DropdownField _selectPlayerSettingsPresetsDropdown;

        private readonly Texture2D _playButtonTexture = 
            Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomPlayButton");
        
        private readonly Texture2D _playStopButtonTexture =
            Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomPlayStopButton");

        public PlayModeToolbar()
        {
#if UNITY_6000_0_OR_NEWER
            var visualTree = Resources.Load<VisualTreeAsset>("com.disillusion.play-mode-plus/PlayModePlusToolbar6000");
#else
            var visualTree = Resources.Load<VisualTreeAsset>("com.disillusion.play-mode-plus/PlayModePlusToolbar");
#endif
            visualTree.CloneTree(this);

            style.flexGrow = 1;

            _playButton = this.Q<Button>("play-button");
            _playButton.clicked -= OnPlayButtonClicked;
            _playButton.clicked += OnPlayButtonClicked;

            _buildButton = this.Q<Button>("build-button");
            _buildButton.clicked -= OnBuildClicked;
            _buildButton.clicked += OnBuildClicked;
            
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            OnPlayModeChanged(PlayModeStateChange.EnteredEditMode);
            
            #region Select Scene Dropdown

            _selectSceneDropdown = this.Q<DropdownField>("select-scene-dropdown");
            _selectSceneDropdown.RegisterCallback<FocusEvent>(evt =>
                _selectSceneDropdown.choices = _playModeManager.GenerateSceneList());
            _selectSceneDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                _playModeManager.SelectedScene =
                    _playModeManager.ScenesInProject.FirstOrDefault(scenesInProject =>
                        scenesInProject.name == evt.newValue);
                _playModeManager.LastScene =
                    _playModeManager.ScenesInProject.FirstOrDefault(scenesInProject =>
                        scenesInProject.name == evt.newValue);
            });

            #endregion

            #region Play Mode Settings Dropdown

            _selectPlayModeSettingsDropdown = this.Q<DropdownField>("play-mode-settings-dropdown");
            _selectPlayModeSettingsDropdown.RegisterCallback<FocusEvent>(evt =>
                _selectSceneDropdown.choices = _playModeManager.GenerateSceneList());
            _selectPlayModeSettingsDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                var editorSettings = new SerializedObject(
                    AssetDatabase.LoadAssetAtPath("ProjectSettings/EditorSettings.asset", typeof(Object)));
                var enterPlayModeOptionsEnabled =
                    editorSettings.FindProperty("m_EnterPlayModeOptionsEnabled");
                var enterPlayModeOptions = editorSettings.FindProperty("m_EnterPlayModeOptions");

                switch (_selectPlayModeSettingsDropdown.value)
                {
                    case "Default (Reload Domain, Reload Scene":
                        enterPlayModeOptionsEnabled.boolValue = false;
                        enterPlayModeOptions.intValue = (int) EnterPlayModeOptions.None;
                        break;
                    case "Disable Reload Domain":
                        enterPlayModeOptionsEnabled.boolValue = true;
                        enterPlayModeOptions.intValue = (int) EnterPlayModeOptions.DisableDomainReload;
                        break;
                    case "Disable Reload Scene":
                        enterPlayModeOptionsEnabled.boolValue = true;
                        enterPlayModeOptions.intValue = (int) EnterPlayModeOptions.DisableSceneReload;
                        break;
                    case "Disable All":
                        enterPlayModeOptionsEnabled.boolValue = true;
                        enterPlayModeOptions.intValue |= (int) EnterPlayModeOptions.DisableDomainReload;
                        enterPlayModeOptions.intValue |= (int) EnterPlayModeOptions.DisableSceneReload;
                        break;
                    default:
                        enterPlayModeOptionsEnabled.boolValue = false;
                        enterPlayModeOptions.intValue = (int) EnterPlayModeOptions.None;
                        break;
                }

                editorSettings.ApplyModifiedProperties();
                EditorUtility.SetDirty(editorSettings.targetObject);
                AssetDatabase.SaveAssets();
            });

            #endregion

            #region Build Settings Dropdown

            _selectPlayerSettingsPresetsDropdown = this.Q<DropdownField>("build-settings-dropdown");
            _selectPlayerSettingsPresetsDropdown.RegisterCallback<FocusEvent>(evt =>
                _selectPlayerSettingsPresetsDropdown.choices = _buildManager.GenerateBuildSettingsList());
            _selectPlayerSettingsPresetsDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                _buildManager.SelectedBuildPreset =
                    _buildManager.PlayerSettingsPresetsInProject.FirstOrDefault(presets =>
                        presets.name == evt.newValue);
                BuildManager.ApplyPreset(_buildManager.SelectedBuildPreset);
            });

            #endregion

            // Set the play mode scene for Default play button
            PlayModeManager.EditorStartScene();

            // Populate Dropdowns
            _selectSceneDropdown.choices = _playModeManager.GenerateSceneList();
            _selectPlayModeSettingsDropdown.choices = _playModeManager.GeneratePlayModeSettingsList();
            _selectPlayerSettingsPresetsDropdown.choices = _buildManager.GenerateBuildSettingsList();

            // Set Select Scene Dropdown value to Last Scene which was opened
            _selectSceneDropdown.value = _playModeManager.LastScene?.name;
            _playModeManager.SelectedScene = _selectSceneDropdown.value == null
                ? _playModeManager.ScenesInProject.FirstOrDefault()
                : _playModeManager.ScenesInProject.FirstOrDefault(scenesInProject =>
                    scenesInProject.name == _selectSceneDropdown.value);
        }

        private void OnPlayButtonClicked()
        {
            _playModeManager.PlayScene();
            _playModeManager.LastScene = _playModeManager.SelectedScene;
        }

        private void OnPlayModeChanged(PlayModeStateChange obj)
        {
            _playButton.style.backgroundImage = EditorApplication.isPlaying
                ? new StyleBackground(_playStopButtonTexture)
                : new StyleBackground(_playButtonTexture);
        }

        private void OnBuildClicked() => _buildManager.OpenBuildWindow();
    }
}