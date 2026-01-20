using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarSelectPlayModeSettings : BaseToolbarElement
    {
        private static GUIContent buttonContent;
        private string _currentSelection;
        private List<string> _playModeOptions;
        private readonly Texture icon = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomSceneCogButton");
        protected override string Name => "Select Play Mode Settings";
        protected override string Tooltip => "Select Play Mode Settings";

        public override void OnInit()
        {
            // 初始化选项列表
            _playModeOptions = new List<string>
            {
                "Default (Reload Domain, Reload Scene)",
                "Disable Reload Domain",
                "Disable Reload Scene",
                "Disable All"
            };
            
            // 初始化当前选择
            _currentSelection = GetCurrentPlayModeSetting();
            buttonContent = new GUIContent(icon, string.Format(this.Tooltip, _currentSelection));
        }

        public override void OnDrawInToolbar()
        {
            if (buttonContent == null) return;

            // 更新工具提示
            string currentSetting = GetCurrentPlayModeSetting();
            buttonContent.tooltip = string.Format(this.Tooltip, currentSetting);

            // 绘制图标按钮
            if (GUILayout.Button(buttonContent, ToolbarStyles.CommandButtonStyle, GUILayout.Width(this.Width)))
            {
                // 点开下拉列表前刷新当前选中状态
                RefreshCurrentSelection();
                PlayModeSettingsMenu().ShowAsContext();
            }
        }

        private void RefreshCurrentSelection()
        {
            // 刷新当前选中状态
            _currentSelection = GetCurrentPlayModeSetting();
        }

        private string GetCurrentPlayModeSetting()
        {
            var editorSettings = new SerializedObject(
                AssetDatabase.LoadAssetAtPath("ProjectSettings/EditorSettings.asset", typeof(Object)));
            
            var enterPlayModeOptionsEnabled = editorSettings.FindProperty("m_EnterPlayModeOptionsEnabled");
            var enterPlayModeOptions = editorSettings.FindProperty("m_EnterPlayModeOptions");
            
            if (!enterPlayModeOptionsEnabled.boolValue)
            {
                return "Default (Reload Domain, Reload Scene)";
            }
            
            int options = enterPlayModeOptions.intValue;
            
            if ((options & (int)EnterPlayModeOptions.DisableDomainReload) != 0 &&
                (options & (int)EnterPlayModeOptions.DisableSceneReload) != 0)
            {
                return "Disable All";
            }
            else if ((options & (int)EnterPlayModeOptions.DisableDomainReload) != 0)
            {
                return "Disable Reload Domain";
            }
            else if ((options & (int)EnterPlayModeOptions.DisableSceneReload) != 0)
            {
                return "Disable Reload Scene";
            }
            
            return "Default (Reload Domain, Reload Scene)";
        }
        
        private GenericMenu PlayModeSettingsMenu()
        {
            var menu = new GenericMenu();
            
            // 添加所有选项 - 使用最新的 _currentSelection 来判断选中状态
            foreach (string option in _playModeOptions)
            {
                bool isSelected = _currentSelection == option;
                menu.AddItem(new GUIContent(option), isSelected, () =>
                {
                    OnPlayModeSettingSelected(option);
                });
            }
            
            menu.AddSeparator("");
            
            // 添加额外选项
            menu.AddItem(new GUIContent("Open Editor Settings..."), false, () =>
            {
                SettingsService.OpenProjectSettings("Project/Editor");
            });
            
            return menu;
        }
        
        private void OnPlayModeSettingSelected(string selectedOption)
        {
            // 更新当前选择
            _currentSelection = selectedOption;
            
            // 应用设置
            var editorSettings = new SerializedObject(
                AssetDatabase.LoadAssetAtPath("ProjectSettings/EditorSettings.asset", typeof(Object)));
            
            var enterPlayModeOptionsEnabled = editorSettings.FindProperty("m_EnterPlayModeOptionsEnabled");
            var enterPlayModeOptions = editorSettings.FindProperty("m_EnterPlayModeOptions");
            
            switch (selectedOption)
            {
                case "Default (Reload Domain, Reload Scene)":
                    enterPlayModeOptionsEnabled.boolValue = false;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.None;
                    break;
                    
                case "Disable Reload Domain":
                    enterPlayModeOptionsEnabled.boolValue = true;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.DisableDomainReload;
                    break;
                    
                case "Disable Reload Scene":
                    enterPlayModeOptionsEnabled.boolValue = true;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.DisableSceneReload;
                    break;
                    
                case "Disable All":
                    enterPlayModeOptionsEnabled.boolValue = true;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.DisableDomainReload | 
                                                   (int)EnterPlayModeOptions.DisableSceneReload;
                    break;
                    
                default:
                    enterPlayModeOptionsEnabled.boolValue = false;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.None;
                    break;
            }
            
            editorSettings.ApplyModifiedProperties();
            EditorUtility.SetDirty(editorSettings.targetObject);
            AssetDatabase.SaveAssets();
            
            LogF8.Log($"Play Mode settings changed to: {selectedOption}");
        }
    }
}