using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarSelectPlayerSettingsPresets : BaseToolbarElement
    {
        private BuildManager _buildManager;
        private GUIContent _buttonContent;
        private List<string> _buildSettingsList;
        private Preset _selectedBuildPreset;
        private readonly Texture icon = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomDropdownArrow");
        protected override string Name => "Select Player Settings Presets";
        protected override string Tooltip => "Select Player Settings Presets";

        public override void OnInit()
        {
            this.Width = 25;
            _buildManager = BuildManager.Instance;
            _buttonContent = new GUIContent(icon, this.Tooltip);
            // 生成预设列表
            UpdateBuildSettingsList();
        }

        private void UpdateBuildSettingsList()
        {
            _buildSettingsList = _buildManager.GenerateBuildSettingsList();
        }

        public override void OnDrawInToolbar()
        {
            if (_buttonContent == null)
            {
                return;
            }

            // 绘制下拉按钮
            if (EditorGUILayout.DropdownButton(_buttonContent, FocusType.Keyboard,
                    ToolbarStyles.CommandPopupStyle, GUILayout.Width(this.Width)))
            {
                // 点击时更新列表
                UpdateBuildSettingsList();

                // 显示预设菜单
                PresetsMenu().ShowAsContext();
            }

            // 可选：在悬停时显示当前选择的预设信息
            Rect lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition) && _selectedBuildPreset != null)
            {
                GUI.Label(lastRect, new GUIContent("", $"当前预设: {_selectedBuildPreset.name}"));
            }
        }

        private GenericMenu PresetsMenu()
        {
            var menu = new GenericMenu();

            // 如果预设列表为空
            if (_buildSettingsList == null || _buildSettingsList.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Player Settings Presets Found"));
                return menu;
            }

            // 添加预设选项
            foreach (string presetName in _buildSettingsList)
            {
                bool isSelected = _selectedBuildPreset != null && _selectedBuildPreset.name == presetName;

                menu.AddItem(new GUIContent(presetName), isSelected, () => { OnPresetSelected(presetName); });
            }

            menu.AddSeparator("");

            // 添加刷新选项
            menu.AddItem(new GUIContent("Refresh List"), false, () =>
            {
                UpdateBuildSettingsList();
                LogF8.Log("Build preset list refreshed");
            });

            menu.AddItem(new GUIContent("Open Player Settings..."), false, () =>
            {
                SettingsService.OpenProjectSettings("Project/Player");
            });
            
            return menu;
        }

        private void OnPresetSelected(string presetName)
        {
            // 查找对应的 Preset 对象
            _selectedBuildPreset = _buildManager.PlayerSettingsPresetsInProject
                .FirstOrDefault(preset => preset.name == presetName);

            if (_selectedBuildPreset != null)
            {
                // 应用预设
                BuildManager.ApplyPreset(_selectedBuildPreset);

                LogF8.Log($"Applied Player Settings preset: {presetName}");
            }
        }
    }
}