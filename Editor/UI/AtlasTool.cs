using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace F8Framework.Core.Editor
{
    public class AtlasCutting : UnityEditor.Editor
    {
        static List<string> t2dPath;
        
        [MenuItem("Assets/（F8UI界面管理功能）/（图集切割）", false, -3)]
        public static void SliceAtlas()
        {
            t2dPath = new List<string>();
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                DealPng(assetPath);
            }
            
            AssetDatabase.Refresh();
            
            foreach (var path in t2dPath)
            {
                var importer = AssetImporter.GetAtPath(path);
                if (importer is TextureImporter textureImporter)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.SaveAndReimport();
                }
            }
            
            LogF8.Log("图集切割完成！");
        }

        public static void DealPng(string assetPath)
        {
            string outdir = FileTools.FormatToUnityPath(FileTools.TruncatePath(assetPath, 1));

            var assets2 = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            for (int i = 1; i < assets2.Length; i++)
            {
                if (assets2[i] is Sprite)
                {
                    var sp = assets2[i] as Sprite;
                    Texture2D t2d = new Texture2D((int)sp.rect.width, (int)sp.rect.height, TextureFormat.RGBA32, false);
                    var aslasTexture = sp.texture;
                    t2d.SetPixels(aslasTexture.GetPixels((int)sp.rect.x, (int)sp.rect.y, (int)sp.rect.width,
                        (int)sp.rect.height));
                    t2d.Apply();
                    File.WriteAllBytes(outdir + "/" + sp.name + ".png", t2d.EncodeToPNG());
                    
                    t2dPath.Add(outdir + "/" + sp.name + ".png");
                }
                else
                {
                    LogF8.LogError($"{assets2[i].name} 图片类型错误，Texture Type 应为 Sprite(2D and UI)，Sprite Mode 应为 Multiple，Read/Write 应勾选，再点击 Sprite Editor 设置 Slice，图片压缩 None 质量为最佳。");
                }
            }
        }
    }
}