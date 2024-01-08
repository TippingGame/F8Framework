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
          {"Cube", new AssetMapping("prefabs/cube", "Assets/AssetBundles/Prefabs/Cube.prefab")},
          {"Tracking Asset Map", new AssetMapping("text/tracking asset map", "Assets/AssetBundles/Text/Tracking Asset Map.json")},
          {"Image", new AssetMapping("ui/image", "Assets/AssetBundles/UI/Image.prefab")},
          {"Image12", new AssetMapping("ui/image12", "Assets/AssetBundles/UI/Image12.prefab")},
          {"Text (Legacy)", new AssetMapping("ui/text (legacy)", "Assets/AssetBundles/UI/Text (Legacy).prefab")},
       };
   }
}