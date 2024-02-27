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
          {"00029-3980031510mat", new AssetMapping("materials/00029-3980031510mat", new []{"Assets/AssetBundles/Materials/00029-3980031510mat.mat"})},
          {"00031-1829375853mat", new AssetMapping("materials/00031-1829375853mat", new []{"Assets/AssetBundles/Materials/00031-1829375853mat.mat"})},
          {"Cube", new AssetMapping("prefabs/cube", new []{"Assets/AssetBundles/Prefabs/Cube.prefab"})},
          {"Cube1", new AssetMapping("prefabs/cube1", new []{"Assets/AssetBundles/Prefabs/Cube1.prefab"})},
          {"Cube2", new AssetMapping("prefabs/cube2", new []{"Assets/AssetBundles/Prefabs/Cube2.prefab"})},
          {"00023-3589759801", new AssetMapping("textures/00023-3589759801", new []{"Assets/AssetBundles/Textures/00023-3589759801.png"})},
          {"00029-3980031510", new AssetMapping("textures/00029-3980031510", new []{"Assets/AssetBundles/Textures/00029-3980031510.png"})},
          {"00031-1829375853", new AssetMapping("textures/00031-1829375853", new []{"Assets/AssetBundles/Textures/00031-1829375853.png"})},
          {"UIPanel", new AssetMapping("ui/uipanel", new []{"Assets/AssetBundles/UI/UIPanel.prefab"})},
          {"LocalizedStrings", new AssetMapping("config/binconfigdata/localizedstrings", new []{"Assets/AssetBundles/Config/BinConfigData/LocalizedStrings.bytes"})},
          {"Sheet1", new AssetMapping("config/binconfigdata/sheet1", new []{"Assets/AssetBundles/Config/BinConfigData/Sheet1.bytes"})},
          {"Sheet2", new AssetMapping("config/binconfigdata/sheet2", new []{"Assets/AssetBundles/Config/BinConfigData/Sheet2.bytes"})},
          {"Config", new AssetMapping("config", new []{""})},
          {"Materials", new AssetMapping("materials", new []{"Assets/AssetBundles/Materials/00029-3980031510mat.mat", "Assets/AssetBundles/Materials/00031-1829375853mat.mat"})},
          {"Prefabs", new AssetMapping("prefabs", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab"})},
          {"Textures", new AssetMapping("textures", new []{"Assets/AssetBundles/Textures/00023-3589759801.png", "Assets/AssetBundles/Textures/00029-3980031510.png", "Assets/AssetBundles/Textures/00031-1829375853.png"})},
          {"UI", new AssetMapping("ui", new []{"Assets/AssetBundles/UI/UIPanel.prefab"})},
          {"BinConfigData", new AssetMapping("config/binconfigdata", new []{"Assets/AssetBundles/Config/BinConfigData/LocalizedStrings.bytes", "Assets/AssetBundles/Config/BinConfigData/Sheet1.bytes", "Assets/AssetBundles/Config/BinConfigData/Sheet2.bytes"})},
       };
   }
}