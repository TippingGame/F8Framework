using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    sealed internal class ToolbarScreenshot : BaseToolbarElement
    {
        private GUIContent _buttonContent;
        private const string ScreenshotFolderPath = "Screenshots";

        protected override string Name => "Screenshot";
        protected override string Tooltip => "Screenshot options";

        public override void OnInit()
        {
            Texture icon = EditorGUIUtility.IconContent("d_FrameCapture").image;
            _buttonContent = new GUIContent(icon, this.Tooltip);
        }

        public override void OnDrawInToolbar()
        {
            if (EditorGUILayout.DropdownButton(_buttonContent, FocusType.Passive, ToolbarStyles.CommandButtonStyle,
                    GUILayout.Width(this.Width)))
            {
                var menu = new GenericMenu();

                // --- Game View Options ---
                menu.AddItem(new GUIContent("Capture Game View/Current Resolution"), false,
                    static () => CaptureGameView(1));
                menu.AddSeparator("Capture Game View/");
                menu.AddItem(new GUIContent("Capture Game View/HD (1920x1080)"), false,
                    static () => CaptureGameViewAtResolution(1920, 1080));
                menu.AddItem(new GUIContent("Capture Game View/4K (3840x2160)"), false,
                    static () => CaptureGameViewAtResolution(3840, 2160));

                // --- Scene View Options ---
                bool isSceneViewAvailable = SceneView.lastActiveSceneView != null;

                if (isSceneViewAvailable)
                {
                    menu.AddItem(new GUIContent("Capture Scene View/Opaque Background"), false,
                        static () => CaptureSceneView(false));
                    menu.AddItem(new GUIContent("Capture Scene View/Transparent Background"), false,
                        static () => CaptureSceneView(true));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Capture Scene View/Opaque Background (No Scene View active)"));
                    menu.AddDisabledItem(
                        new GUIContent("Capture Scene View/Transparent Background (No Scene View active)"));
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Open Screenshots Folder"), false, OpenScreenshotsFolder);
                menu.ShowAsContext();
            }
        }

        private static void OpenScreenshotsFolder()
        {
            EnsureFolderExists();

            Application.OpenURL(Path.GetFullPath(ScreenshotFolderPath));
        }

        private static void CaptureGameView(int resolutionMultiplier)
        {
            EnsureFolderExists();
            string fullPath = GetUniqueScreenshotPath("GameView");
            ScreenCapture.CaptureScreenshot(fullPath, resolutionMultiplier);

            EditorApplication.delayCall += () => { LogScreenshot(fullPath); };
        }

        private static void CaptureGameViewAtResolution(int width, int height)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                LogF8.LogWarning(
                    "[CustomToolbar] Cannot capture Game View at resolution: no main camera found in scene.");

                return;
            }

            EnsureFolderExists();
            string fullPath = GetUniqueScreenshotPath($"GameView_{width}x{height}");

            var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var screenShot = new Texture2D(width, height, TextureFormat.ARGB32, false);

            RenderTexture prevTargetTexture = mainCamera.targetTexture;
            mainCamera.targetTexture = rt;
            mainCamera.Render();
            mainCamera.targetTexture = prevTargetTexture;

            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();
            RenderTexture.active = null;

            byte[] bytes = screenShot.EncodeToPNG();
            File.WriteAllBytes(fullPath, bytes);

            UnityEngine.Object.DestroyImmediate(rt);
            UnityEngine.Object.DestroyImmediate(screenShot);

            LogScreenshot(fullPath);
        }

        private static void CaptureSceneView(bool withTransparency)
        {
            var sceneView = SceneView.lastActiveSceneView;

            if (sceneView == null)
            {
                LogF8.LogWarning(
                    "[CustomToolbar] Cannot capture Scene View: no scene view is currently active or focused.");

                return;
            }

            Camera sceneCamera = sceneView.camera;

            RenderTexture prevTargetTexture = sceneCamera.targetTexture;
            CameraClearFlags prevClearFlags = sceneCamera.clearFlags;
            Color prevBackgroundColor = sceneCamera.backgroundColor;

            RenderTextureFormat rtFormat = withTransparency ? RenderTextureFormat.ARGB32 : RenderTextureFormat.Default;
            TextureFormat texFormat = withTransparency ? TextureFormat.ARGB32 : TextureFormat.RGB24;

            var renderTexture = new RenderTexture(sceneCamera.pixelWidth, sceneCamera.pixelHeight, 24, rtFormat);
            var texture2D = new Texture2D(renderTexture.width, renderTexture.height, texFormat, false);

            sceneCamera.targetTexture = renderTexture;

            if (withTransparency)
            {
                sceneCamera.clearFlags = CameraClearFlags.SolidColor;
                sceneCamera.backgroundColor = new Color(0, 0, 0, 0);
            }

            sceneCamera.Render();

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;

            sceneCamera.targetTexture = prevTargetTexture;
            sceneCamera.clearFlags = prevClearFlags;
            sceneCamera.backgroundColor = prevBackgroundColor;

            byte[] bytes = texture2D.EncodeToPNG();
            EnsureFolderExists();
            string fileName = withTransparency ? "SceneView_Transparent" : "SceneView_Opaque";
            string fullPath = GetUniqueScreenshotPath(fileName);
            File.WriteAllBytes(fullPath, bytes);

            UnityEngine.Object.DestroyImmediate(texture2D);
            UnityEngine.Object.DestroyImmediate(renderTexture);

            LogScreenshot(fullPath);
        }

        private static void EnsureFolderExists()
        {
            if (!Directory.Exists(ScreenshotFolderPath))
            {
                Directory.CreateDirectory(ScreenshotFolderPath);
            }
        }

        private static string GetUniqueScreenshotPath(string prefix)
        {
            string timestamp = $"{DateTimeOffset.Now:yyyy-MM-dd_HH-mm-ss}";

            return Path.Combine(ScreenshotFolderPath, $"{prefix}_{timestamp}.png");
        }

        private static void LogScreenshot(string path)
        {
            AssetDatabase.Refresh();
            LogF8.Log($"Screenshot saved to: <a href=\"{path}\">{path}</a>",
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
        }
    }
}