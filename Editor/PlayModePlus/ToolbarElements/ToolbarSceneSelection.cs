using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarSceneSelection : BaseToolbarElement
    {
        private GUIContent _buttonContent;
        private PlayModeManager _playModeManager;
        private SceneListData _sceneData;
        private string _selectedSceneDisplayName;
        private SceneAsset _selectedScene;
        private readonly Texture icon = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomDropdownArrow");
        protected override string Name => "Scene Selection";
        protected override string Tooltip => "Select a scene from the 'Assets/' folder.";

        public override void OnInit()
        {
            this.Width = 25;
            
            // 初始化 PlayModeManager
            _playModeManager = PlayModeManager.Instance;
            
            // 生成场景列表
            RefreshScenesList();
            
            // 设置初始显示文本
            UpdateDisplayText();
            
            _buttonContent = new GUIContent(icon, this.Tooltip);
        }

        private void RefreshScenesList()
        {
            _sceneData = _playModeManager.GenerateSceneList();
            
            // 如果没有场景，显示提示
            if (_sceneData.sceneAssets.Count == 0)
            {
                _selectedSceneDisplayName = "No Scenes";
                _selectedScene = null;
                return;
            }

            // 尝试获取最后选择的场景
            SceneAsset lastScene = _playModeManager.LastScene;
            
            // 如果没有最后选择的场景，尝试获取当前打开的场景
            if (lastScene == null)
            {
                lastScene = GetCurrentSceneAsset();
            }

            // 设置选中的场景
            if (lastScene != null)
            {
                int lastSceneIndex = _sceneData.sceneAssets.IndexOf(lastScene);
                if (lastSceneIndex >= 0 && lastSceneIndex < _sceneData.displayNames.Count)
                {
                    _selectedScene = lastScene;
                    _selectedSceneDisplayName = _sceneData.displayNames[lastSceneIndex];
                    _playModeManager.SelectedScene = lastScene;
                }
                else
                {
                    // 如果最后选择的场景不在列表中，选择第一个场景
                    _selectedScene = _sceneData.sceneAssets[0];
                    _selectedSceneDisplayName = _sceneData.displayNames[0];
                    _playModeManager.SelectedScene = _selectedScene;
                }
            }
            else
            {
                // 选择第一个场景
                _selectedScene = _sceneData.sceneAssets[0];
                _selectedSceneDisplayName = _sceneData.displayNames[0];
                _playModeManager.SelectedScene = _selectedScene;
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

        private void UpdateDisplayText()
        {
            if (_selectedScene != null)
            {
                _selectedSceneDisplayName = _selectedScene.name;
            }
            else
            {
                var currentScene = GetCurrentSceneAsset();
                _selectedSceneDisplayName = string.IsNullOrEmpty(currentScene?.name) ? "No Scene" : currentScene.name;
            }
        }

        public override void OnDrawInToolbar()
        {
            if (_buttonContent == null)
            {
                return;
            }

            // 绘制下拉按钮
            if (EditorGUILayout.DropdownButton(_buttonContent, FocusType.Keyboard, ToolbarStyles.CommandPopupStyle, GUILayout.Width(this.Width)))
            {
                // 点击时刷新场景列表
                RefreshScenesList();
                // 显示场景菜单
                BuildSceneMenu().ShowAsContext();
            }
        }

        private GenericMenu BuildSceneMenu()
        {
            var menu = new GenericMenu();

            // 如果场景列表为空
            if (_sceneData == null || _sceneData.sceneAssets.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Scenes Found"));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Create New Scene..."), false, () =>
                {
                    EditorApplication.ExecuteMenuItem("File/New Scene");
                });
                menu.AddItem(new GUIContent("Refresh List"), false, () =>
                {
                    RefreshScenesList();
                    BuildSceneMenu().ShowAsContext();
                });
                return menu;
            }

            // 添加场景选项
            for (int i = 0; i < _sceneData.sceneAssets.Count; i++)
            {
                SceneAsset sceneAsset = _sceneData.sceneAssets[i];
                string displayName = _sceneData.displayNames[i];
                bool isSelected = _selectedScene == sceneAsset;

                var i1 = i;
                menu.AddItem(new GUIContent(displayName), isSelected, () =>
                {
                    OnSceneSelected(i1);
                });
            }

            menu.AddSeparator("");

            // 添加额外选项
            menu.AddItem(new GUIContent("Refresh List"), false, () =>
            {
                RefreshScenesList();
                // 重新显示菜单
                BuildSceneMenu().ShowAsContext();
            });

            menu.AddItem(new GUIContent("Create New Scene..."), false, () =>
            {
                EditorApplication.ExecuteMenuItem("File/New Scene");
            });

            menu.AddItem(new GUIContent("Open Scene In Explorer"), false, () =>
            {
                if (_selectedScene != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(_selectedScene);
                    string directory = System.IO.Path.GetDirectoryName(scenePath);
                    System.Diagnostics.Process.Start("explorer.exe", directory);
                }
            });

            return menu;
        }

        private void OnSceneSelected(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < _sceneData.sceneAssets.Count)
            {
                _selectedScene = _sceneData.sceneAssets[selectedIndex];
                _selectedSceneDisplayName = _sceneData.displayNames[selectedIndex];
                _playModeManager.SelectedScene = _selectedScene;
                _playModeManager.LastScene = _selectedScene;
                ToolbarSceneName.UpdateSceneLabel();
                // 记录日志
                string scenePath = AssetDatabase.GetAssetPath(_selectedScene) ?? "";
                LogF8.Log($"当前选择场景：{scenePath}");
            }
        }
    }
}