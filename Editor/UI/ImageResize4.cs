using System.IO;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class ImageResize4 : UnityEditor.Editor
    {
        public static bool IsToBig = true; //向着更大缩放

        [MenuItem("Assets/（F8UI界面管理功能）/（图片尺寸设为4的倍数）", false, -4)]
        public static void ResizeImages()
        {
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;

            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ExTex(assetPath);
            }

            AssetDatabase.Refresh();

            LogF8.Log($"图片尺寸设为4的倍数完成！是否放大图片：{IsToBig}");
        }

        public static void ExTex(string path)
        {
            if (AssetDatabase.LoadMainAssetAtPath(path) is Texture2D tex)
            {
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    return;
                }

                //更改图片属性，可读，否则无法获取Pixel
                importer.isReadable = true;
                importer.SaveAndReimport();

                if (tex.width % 4 == 0 && tex.height % 4 == 0)
                {
                    return;
                }

                Vector2Int v2 = GetFourSize(tex.width, tex.height);
                var texCopy = new Texture2D(v2.x, v2.y);
                //从原来图像上根据现在的大小计算像素点
                for (int h = 0; h < v2.y; h++)
                {
                    for (int w = 0; w < v2.x; w++)
                    {
                        var pixel = tex.GetPixelBilinear(w / (v2.x * 1.0f), h / (v2.y * 1.0f));

                        /*if (info.IsContain(i, j))
                        {
                            pixel.a = 0;
                        }*/

                        texCopy.SetPixel(w, h, pixel);
                    }
                }

                texCopy.Apply();
                File.WriteAllBytes(path, texCopy.EncodeToPNG());
                //恢复不可读
                importer.isReadable = false;
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 目标尺寸，宽高整数4处理
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Vector2Int GetFourSize(int width, int height)
        {
            if (IsToBig)
            {
                while (width % 4 != 0)
                {
                    width++;
                }

                while (height % 4 != 0)
                {
                    height++;
                }
            }
            else
            {
                while (width % 4 != 0)
                {
                    width--;
                }

                while (height % 4 != 0)
                {
                    height--;
                }
            }

            return new Vector2Int(Mathf.Max(4, width), Mathf.Max(4, height));
        }
    }
}