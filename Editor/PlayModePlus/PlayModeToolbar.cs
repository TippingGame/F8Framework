using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
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
        private readonly Label _sceneLabel;

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
            
            _sceneLabel = this.Q<Label>("scene-name");
            _sceneLabel.RegisterCallback<MouseEnterEvent>(evt =>
            {
                _sceneLabel.style.color = new StyleColor(new Color(89f / 255f, 158f / 255f, 94f / 255f));
                _sceneLabel.style.cursor = new StyleCursor((StyleKeyword)MouseCursor.Link);
            });
    
            _sceneLabel.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                _sceneLabel.style.color = new StyleColor(new Color(238f / 255f, 238f / 255f, 238f / 255f));
                _sceneLabel.style.cursor = new StyleCursor((StyleKeyword)MouseCursor.Arrow);
            });
            _sceneLabel.RegisterCallback<ClickEvent>(OnSceneLabelClicked);
                
            #region Select Scene Dropdown

            _selectSceneDropdown = this.Q<DropdownField>("select-scene-dropdown");
            _selectSceneDropdown.RegisterCallback<FocusEvent>(evt =>
            {
                var sceneData = _playModeManager.GenerateSceneList();
                _selectSceneDropdown.choices = sceneData.displayNames;
            });
            _selectSceneDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                var sceneData = _playModeManager.GetSceneData();
                int selectedIndex = sceneData.displayNames.IndexOf(evt.newValue);
                if (selectedIndex >= 0 && selectedIndex < sceneData.sceneAssets.Count)
                {
                    _playModeManager.SelectedScene = sceneData.sceneAssets[selectedIndex];
                    _playModeManager.LastScene = sceneData.sceneAssets[selectedIndex];
                }
                
                UpdateSceneLabel();
                string scenePath = AssetDatabase.GetAssetPath(_playModeManager.SelectedScene) ?? "";
                LogF8.Log("当前选择场景：" + scenePath);
            });

            #endregion

            #region Play Mode Settings Dropdown

            _selectPlayModeSettingsDropdown = this.Q<DropdownField>("play-mode-settings-dropdown");
            _selectPlayModeSettingsDropdown.RegisterCallback<FocusEvent>(evt =>
            {
                var sceneData = _playModeManager.GenerateSceneList();
                _selectSceneDropdown.choices = sceneData.displayNames;
            });
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
            var sceneData = _playModeManager.GenerateSceneList();
            _selectSceneDropdown.choices = sceneData.displayNames;
            _selectPlayModeSettingsDropdown.choices = _playModeManager.GeneratePlayModeSettingsList();
            _selectPlayerSettingsPresetsDropdown.choices = _buildManager.GenerateBuildSettingsList();

            // Set Select Scene Dropdown value to Last Scene which was opened
            var currentSceneData = _playModeManager.GetSceneData();
            int lastSceneIndex = currentSceneData.sceneAssets.IndexOf(_playModeManager.LastScene);
            if (lastSceneIndex >= 0 && lastSceneIndex < currentSceneData.displayNames.Count)
            {
                _selectSceneDropdown.value = currentSceneData.displayNames[lastSceneIndex];
            }
            
            _playModeManager.SelectedScene = _selectSceneDropdown.value == null
                ? GetCurrentSceneAsset()
                : _playModeManager.LastScene;
            
            UpdateSceneLabel();
        }

        private void OnSceneLabelClicked(ClickEvent evt)
        {
            if (_playModeManager.SelectedScene != null)
            {
                var projectWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
                var projectWindow = EditorWindow.GetWindow(projectWindowType);
                if (projectWindow != null)
                {
                    projectWindow.Focus();
                }
                EditorGUIUtility.PingObject(_playModeManager.SelectedScene);
            }
        }
        
        private SceneAsset GetCurrentSceneAsset()
        {
            var currentScene = EditorSceneManager.GetActiveScene();
            if (currentScene.IsValid() && !string.IsNullOrEmpty(currentScene.path))
            {
                return AssetDatabase.LoadAssetAtPath<SceneAsset>(currentScene.path);
            }
            return null;
        }
        
        private void UpdateSceneLabel()
        {
            if (_playModeManager.SelectedScene != null)
            {
                _sceneLabel.text = _playModeManager.SelectedScene.name;
            }
            else if (_playModeManager.LastScene != null)
            {
                _sceneLabel.text = _playModeManager.LastScene.name;
            }
            else
            {
                var currentScene = GetCurrentSceneAsset();
                _sceneLabel.text = string.IsNullOrEmpty(currentScene?.name) ? "No Scene1" : currentScene.name;
            }
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