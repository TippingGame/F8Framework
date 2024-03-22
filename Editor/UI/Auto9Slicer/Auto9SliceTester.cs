using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class Auto9SliceTester : UnityEditor.Editor
    {
        public static SliceOptions Options => options;
        private static SliceOptions options = new SliceOptions();

        public static bool CreateBackup => createBackup;
        private static bool createBackup = false;

        private static List<string> texture2DList;

        [MenuItem("Assets/（F8UI界面管理功能）/（图片自动切割九宫格）", false, -2)]
        public static void Auto9Slice()
        {
            texture2DList = new List<string>();

            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;

            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.LoadMainAssetAtPath(assetPath) is Texture2D tex)
                {
                    if (!assetPath.Contains(".original"))
                    {
                        texture2DList.Add(assetPath);
                    }
                }
            }

            foreach (var target in texture2DList)
            {
                var importer = AssetImporter.GetAtPath(target);
                if (importer is TextureImporter textureImporter)
                {
                    if (textureImporter.spriteBorder != Vector4.zero)
                    {
                        LogF8.Log($"已设置九宫格，跳过 {Path.GetFileName(target)}");
                        continue;
                    }

                    var fullPath = Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? "", target);
                    var bytes = File.ReadAllBytes(fullPath);

                    // バックアップ
                    if (CreateBackup)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(fullPath);
                        File.WriteAllBytes(
                            Path.Combine(Path.GetDirectoryName(fullPath) ?? "",
                                fileName + ".original" + Path.GetExtension(fullPath)), bytes);
                    }

                    // importerのreadable設定に依らずに読み込むために直接読む
                    var targetTexture = new Texture2D(2, 2);
                    targetTexture.LoadImage(bytes);

                    var slicedTexture = Slicer.Slice(targetTexture, Options);
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spriteBorder = slicedTexture.Border.ToVector4();
                    if (fullPath.EndsWith(".png")) File.WriteAllBytes(fullPath, slicedTexture.Texture.EncodeToPNG());
                    if (fullPath.EndsWith(".jpg")) File.WriteAllBytes(fullPath, slicedTexture.Texture.EncodeToJPG());
                    if (fullPath.EndsWith(".jpeg")) File.WriteAllBytes(fullPath, slicedTexture.Texture.EncodeToJPG());

                    LogF8.Log($"图片九宫格切割完成！{Path.GetFileName(target)} = {textureImporter.spriteBorder}");
                }
            }

            AssetDatabase.Refresh();
        }
    }
}