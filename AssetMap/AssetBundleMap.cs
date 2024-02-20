// code generation.

using System.Collections.Generic;

namespace F8Framework.AssetMap
{
   public static class AssetBundleMap
   {
       public class AssetMapping
       {
           public string AbName;
           public string[] AssetPath;
       
           public AssetMapping(string abName, string[] assetPath)
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
          {"Cube", new AssetMapping("prefabs/cube", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab"})},
          {"Cube1", new AssetMapping("prefabs/cube", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab"})},
          {"Cube2", new AssetMapping("prefabs/cube", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab"})},
          {"LocalizedStrings", new AssetMapping("config/binconfigdata/localizedstrings", new []{"Assets/AssetBundles/Config/BinConfigData/LocalizedStrings.bytes"})},
          {"Sheet1", new AssetMapping("config/binconfigdata/sheet1", new []{"Assets/AssetBundles/Config/BinConfigData/Sheet1.bytes"})},
          {"Sheet2", new AssetMapping("config/binconfigdata/sheet2", new []{"Assets/AssetBundles/Config/BinConfigData/Sheet2.bytes"})},
          {"Config", new AssetMapping("config", new []{""})},
          {"Prefabs", new AssetMapping("prefabs", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab"})},
          {"BinConfigData", new AssetMapping("config/binconfigdata", new []{"Assets/AssetBundles/Config/BinConfigData/LocalizedStrings.bytes", "Assets/AssetBundles/Config/BinConfigData/Sheet1.bytes", "Assets/AssetBundles/Config/BinConfigData/Sheet2.bytes"})},
       };
   }
}