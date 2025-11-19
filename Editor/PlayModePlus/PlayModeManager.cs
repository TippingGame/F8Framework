using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class PlayModeManager
    {
        public SceneAsset SelectedScene;
        public List<SceneAsset> ScenesInProject;
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

        public SceneListData GenerateSceneList()
        {
            var scenePaths = AssetDatabase.FindAssets("t:SceneAsset")
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToList();

            ScenesInProject = scenePaths
                .Select(AssetDatabase.LoadAssetAtPath<SceneAsset>)
                .ToList();

            var displayNames = GenerateDisplayNames(scenePaths);

            return new SceneListData
            {
                sceneAssets = ScenesInProject,
                displayNames = displayNames
            };
        }

        public SceneListData GetSceneData()
        {
            if (ScenesInProject == null || ScenesInProject.Count == 0)
            {
                return GenerateSceneList();
            }
            return new SceneListData
            {
                sceneAssets = ScenesInProject,
                displayNames = GenerateDisplayNames(ScenesInProject.Select(AssetDatabase.GetAssetPath).ToList())
            };
        }

        private List<string> GenerateDisplayNames(List<string> scenePaths)
        {
            var sceneNames = scenePaths.Select(path => System.IO.Path.GetFileNameWithoutExtension(path)).ToList();
            var displayNames = new List<string>();

            // 检查是否有重名场景
            var duplicateNames = sceneNames
                .GroupBy(name => name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            for (int i = 0; i < scenePaths.Count; i++)
            {
                string sceneName = sceneNames[i];
                string scenePath = scenePaths[i];

                if (duplicateNames.Contains(sceneName))
                {
                    // 对于重名场景，添加父路径直到能区分
                    displayNames.Add(GetUniqueDisplayName(scenePath, scenePaths));
                }
                else
                {
                    // 不重名的场景直接显示场景名
                    displayNames.Add(sceneName);
                }
            }

            return displayNames;
        }

        private string GetUniqueDisplayName(string targetPath, List<string> allPaths)
        {
            var targetParts = targetPath.Split('/').ToList();
            targetParts.RemoveAt(targetParts.Count - 1); // 移除文件名部分
            
            var conflictingPaths = allPaths
                .Where(path => System.IO.Path.GetFileNameWithoutExtension(path) == 
                              System.IO.Path.GetFileNameWithoutExtension(targetPath))
                .ToList();

            // 从父目录开始逐级添加，直到能区分所有冲突路径
            for (int level = 1; level <= targetParts.Count; level++)
            {
                var displayNameBuilder = new List<string>();
                
                // 添加父路径部分（从末尾开始）
                for (int j = Mathf.Max(0, targetParts.Count - level); j < targetParts.Count; j++)
                {
                    displayNameBuilder.Add(targetParts[j]);
                }
                
                // 添加场景名
                displayNameBuilder.Add(System.IO.Path.GetFileNameWithoutExtension(targetPath));
                
                string candidateName = string.Join(" > ", displayNameBuilder);
                
                // 检查这个显示名是否能唯一标识该场景
                bool isUnique = true;
                foreach (var conflictingPath in conflictingPaths)
                {
                    if (conflictingPath != targetPath)
                    {
                        var conflictParts = conflictingPath.Split('/').ToList();
                        conflictParts.RemoveAt(conflictParts.Count - 1);
                        
                        var conflictBuilder = new List<string>();
                        for (int j = Mathf.Max(0, conflictParts.Count - level); j < conflictParts.Count; j++)
                        {
                            conflictBuilder.Add(conflictParts[j]);
                        }
                        conflictBuilder.Add(System.IO.Path.GetFileNameWithoutExtension(conflictingPath));
                        
                        string conflictName = string.Join(" > ", conflictBuilder);
                        
                        if (candidateName == conflictName)
                        {
                            isUnique = false;
                            break;
                        }
                    }
                }
                
                if (isUnique)
                {
                    return candidateName;
                }
            }
            
            // 如果所有父级都相同，显示完整路径（用 > 分隔）
            var fullPathBuilder = new List<string>(targetPath.Split('/'));
            fullPathBuilder[fullPathBuilder.Count - 1] = System.IO.Path.GetFileNameWithoutExtension(targetPath);
            return string.Join(" > ", fullPathBuilder);
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

    public class SceneListData
    {
        public List<SceneAsset> sceneAssets;
        public List<string> displayNames;
    }
}