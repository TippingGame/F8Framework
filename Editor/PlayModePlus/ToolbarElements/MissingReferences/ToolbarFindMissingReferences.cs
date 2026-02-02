using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarFindMissingReferences : BaseToolbarElement
    {
        private GUIContent _buttonContent;
        private static bool isScanning;
        private static int currentIndex;
        private static List<GameObject> allObjectsToScan;
        private static Dictionary<GameObject, List<MissingReferenceInfo>> scanResults;
        private readonly Texture icon = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/CustomMissingReferences");

        protected override string Name => "Find Missing References";
        protected override string Tooltip => "Scan the scene and opens a window with the missing references.";

        private readonly static HashSet<string> IgnoredComponentTypes = new()
        {
            "UniversalAdditionalCameraData", "UniversalAdditionalLightData",
        };

        private readonly static HashSet<string> IgnoredFieldNames = new()
        {
            "m_VolumeTrigger"
        };

        public override void OnInit()
        {
            _buttonContent = new GUIContent(icon, this.Tooltip);

            EditorApplication.update += UpdateScan;
        }

        public override void OnDrawInToolbar()
        {
            this.Enabled = !EditorApplication.isPlayingOrWillChangePlaymode && !isScanning;

            using (new EditorGUI.DisabledScope(!this.Enabled))
            {
                GUIContent content = isScanning
                    ? new GUIContent(_buttonContent.image, "Scanning in progress...")
                    : _buttonContent;

                if (GUILayout.Button(content, ToolbarStyles.CommandButtonStyle, GUILayout.Width(this.Width)))
                {
                    StartScan();
                }
            }
        }

        private static void StartScan()
        {
            if (isScanning)
            {
                return;
            }

            Scene scene = SceneManager.GetActiveScene();
            GameObject[] allGameObjects = scene.GetRootGameObjects();

            allObjectsToScan = new List<GameObject>();

            foreach (GameObject rootGo in allGameObjects)
            {
                CollectAllGameObjects(rootGo, allObjectsToScan);
            }

            scanResults = new Dictionary<GameObject, List<MissingReferenceInfo>>();
            currentIndex = 0;
            isScanning = true;
        }

        private static void UpdateScan()
        {
            if (!isScanning)
            {
                return;
            }

            const int objectsPerFrame = 5;
            int endIndex = Mathf.Min(currentIndex + objectsPerFrame, allObjectsToScan.Count);

            for (int i = currentIndex; i < endIndex; i++)
            {
                float progress = (float)i / allObjectsToScan.Count;
                string progressText = $"Scanning objects... ({i + 1}/{allObjectsToScan.Count})";

                if (EditorUtility.DisplayCancelableProgressBar("Finding Missing References", progressText, progress))
                {
                    FinishScan();

                    return;
                }

                ScanSingleGameObject(allObjectsToScan[i], scanResults);
            }

            currentIndex = endIndex;

            if (currentIndex >= allObjectsToScan.Count)
            {
                FinishScan();
            }
        }

        private static void FinishScan()
        {
            EditorUtility.ClearProgressBar();
            isScanning = false;

            MissingReferencesWindow.ShowWindow(scanResults);

            allObjectsToScan = null;
            scanResults = null;
        }

        private static void CollectAllGameObjects(GameObject parent, List<GameObject> collection)
        {
            collection.Add(parent);

            foreach (Transform child in parent.transform)
            {
                CollectAllGameObjects(child.gameObject, collection);
            }
        }

        private static void ScanSingleGameObject(GameObject go,
            Dictionary<GameObject, List<MissingReferenceInfo>> results)
        {
            MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                if (component == null)
                {
                    AddResult(go, new MissingReferenceInfo { IsScriptMissing = true }, results);

                    continue;
                }

                if (IgnoredComponentTypes.Contains(component.GetType().Name))
                {
                    continue;
                }

                var serializedObject = new SerializedObject(component);
                SerializedProperty property = serializedObject.GetIterator();

                while (property.NextVisible(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }
                    
                    if (property.objectReferenceValue == null && 
                        property.objectReferenceInstanceIDValue != 0)
                    {
                        if (IgnoredFieldNames.Contains(property.name))
                        {
                            continue;
                        }

                        AddResult(go, new MissingReferenceInfo
                        {
                            ComponentName = component.GetType().Name,
                            FieldName = property.displayName,
                            IsScriptMissing = false
                        }, results);
                    }
                }
            }
        }

        private static void AddResult(GameObject go, MissingReferenceInfo info,
            Dictionary<GameObject, List<MissingReferenceInfo>> results)
        {
            if (!results.ContainsKey(go))
            {
                results[go] = new List<MissingReferenceInfo>();
            }

            results[go].Add(info);
        }
    }
}