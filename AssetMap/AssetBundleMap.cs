// code generation.

using System.Collections.Generic;

namespace F8Framework.AssetMap
{
   public static class AssetBundleMap
   {
       public class AssetMapping
       {
           public string AbName;
           public string AssetPath;
       
           public AssetMapping(string abName, string assetPath)
           {
               AbName = abName;
               AssetPath = assetPath;
           }
       }
       
       public static Dictionary<string, AssetMapping> Mappings
       {
           get => mappings;
       }
       
       private static Dictionary<string, AssetMapping> mappings = new Dictionary<string, AssetMapping> {
          {"99", new AssetMapping("audio/99", "Assets/AssetBundles/Audio/99.mp3")},
          {"click11", new AssetMapping("audio/click11", "Assets/AssetBundles/Audio/click11.wav")},
          {"混浊的风声-YS070529", new AssetMapping("audio/混浊的风声-ys070529", "Assets/AssetBundles/Audio/混浊的风声-YS070529.wav")},
          {"蓝火焰-WQ20070429", new AssetMapping("audio/蓝火焰-wq20070429", "Assets/AssetBundles/Audio/蓝火焰-WQ20070429.wav")},
          {"SourceHanSansCN-Normal", new AssetMapping("font/sourcehansanscn-normal", "Assets/AssetBundles/Font/SourceHanSansCN-Normal.ttf")},
          {"SourceHanSerifCN-Heavy", new AssetMapping("font/sourcehanserifcn-heavy", "Assets/AssetBundles/Font/SourceHanSerifCN-Heavy.ttf")},
          {"New Material 1", new AssetMapping("mat/new material 1", "Assets/AssetBundles/Mat/New Material 1.mat")},
          {"New Material", new AssetMapping("mat/new material", "Assets/AssetBundles/Mat/New Material.mat")},
          {"Cube2323", new AssetMapping("new folder/cube2323", "Assets/AssetBundles/New Folder/Cube2323.controller")},
          {"New Animation", new AssetMapping("new folder/new animation", "Assets/AssetBundles/New Folder/New Animation.anim")},
          {"Cylinder", new AssetMapping("nis/cylinder", "Assets/AssetBundles/NIS/Cylinder.prefab")},
          {"Cube", new AssetMapping("prefabs/cube", "Assets/AssetBundles/Prefabs/Cube.prefab")},
          {"Sphere", new AssetMapping("prefabs/sphere", "Assets/AssetBundles/Prefabs/Sphere.prefab")},
          {"Text", new AssetMapping("text/text", "Assets/AssetBundles/Text/Text.prefab")},
          {"141823ehfwgcecgfq8wiyq", new AssetMapping("texture/141823ehfwgcecgfq8wiyq", "Assets/AssetBundles/Texture/141823ehfwgcecgfq8wiyq.jpg")},
          {"Image", new AssetMapping("ui/image", "Assets/AssetBundles/UI/Image.prefab")},
          {"Image12", new AssetMapping("ui/image12", "Assets/AssetBundles/UI/Image12.prefab")},
          {"Text (Legacy)", new AssetMapping("ui/text (legacy)", "Assets/AssetBundles/UI/Text (Legacy).prefab")},
          {"Canvas", new AssetMapping("ui/ui2/canvas", "Assets/AssetBundles/UI/UI2/Canvas.prefab")},
       };
   }
}