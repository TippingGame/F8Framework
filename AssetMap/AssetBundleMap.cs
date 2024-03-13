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
          {"2852", new AssetMapping("audio/2852", new []{"Assets/AssetBundles/Audio/2852.wav"})},
          {"button1", new AssetMapping("audio/button1", new []{"Assets/AssetBundles/Audio/button1.wav"})},
          {"大风-WQ20070525", new AssetMapping("audio/大风-wq20070525", new []{"Assets/AssetBundles/Audio/大风-WQ20070525.mp3"})},
          {"笛子1-mcx20070508", new AssetMapping("audio/笛子1-mcx20070508", new []{"Assets/AssetBundles/Audio/笛子1-mcx20070508.wav"})},
          {"00029-3980031510mat", new AssetMapping("materials/00029-3980031510mat", new []{"Assets/AssetBundles/Materials/00029-3980031510mat.mat"})},
          {"00031-1829375853mat", new AssetMapping("materials/00031-1829375853mat", new []{"Assets/AssetBundles/Materials/00031-1829375853mat.mat"})},
          {"Cube", new AssetMapping("prefabs/pf2/cube2sdfwe", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube1fsdf.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube2sdfwe.prefab", "Assets/AssetBundles/Prefabs/pf2/Cubeljk.prefab"})},
          {"Cube1", new AssetMapping("prefabs/pf2/cube2sdfwe", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube1fsdf.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube2sdfwe.prefab", "Assets/AssetBundles/Prefabs/pf2/Cubeljk.prefab"})},
          {"Cube2", new AssetMapping("prefabs/pf2/cube2sdfwe", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube1fsdf.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube2sdfwe.prefab", "Assets/AssetBundles/Prefabs/pf2/Cubeljk.prefab"})},
          {"Auto9Slice", new AssetMapping("slicend/auto9slice", new []{"Assets/AssetBundles/Slicend/Auto9Slice.asset"})},
          {"baoshixitong01", new AssetMapping("slicend/baoshixitong01", new []{"Assets/AssetBundles/Slicend/baoshixitong01.png"})},
          {"baoshixitong02", new AssetMapping("slicend/baoshixitong02", new []{"Assets/AssetBundles/Slicend/baoshixitong02.png"})},
          {"baoshixitong05", new AssetMapping("slicend/baoshixitong05", new []{"Assets/AssetBundles/Slicend/baoshixitong05.png"})},
          {"baoshixitong06", new AssetMapping("slicend/baoshixitong06", new []{"Assets/AssetBundles/Slicend/baoshixitong06.png"})},
          {"baoshixitong15", new AssetMapping("slicend/baoshixitong15", new []{"Assets/AssetBundles/Slicend/baoshixitong15.png"})},
          {"baoshixitong21", new AssetMapping("slicend/baoshixitong21", new []{"Assets/AssetBundles/Slicend/baoshixitong21.png"})},
          {"baoshixitong22", new AssetMapping("slicend/baoshixitong22", new []{"Assets/AssetBundles/Slicend/baoshixitong22.png"})},
          {"baoxiangxitong02", new AssetMapping("slicend/baoxiangxitong02", new []{"Assets/AssetBundles/Slicend/baoxiangxitong02.png"})},
          {"baoxiangxitong21", new AssetMapping("slicend/baoxiangxitong21", new []{"Assets/AssetBundles/Slicend/baoxiangxitong21.png"})},
          {"baoxiangxitong23", new AssetMapping("slicend/baoxiangxitong23", new []{"Assets/AssetBundles/Slicend/baoxiangxitong23.png"})},
          {"baoxiangzhuangbei16", new AssetMapping("slicend/baoxiangzhuangbei16", new []{"Assets/AssetBundles/Slicend/baoxiangzhuangbei16.png"})},
          {"baoxiangzhuangbei17", new AssetMapping("slicend/baoxiangzhuangbei17", new []{"Assets/AssetBundles/Slicend/baoxiangzhuangbei17.png"})},
          {"SampleImage", new AssetMapping("slicend/sampleimage", new []{"Assets/AssetBundles/Slicend/SampleImage.png"})},
          {"00023-3589759801", new AssetMapping("textures/00023-3589759801", new []{"Assets/AssetBundles/Textures/00023-3589759801.png"})},
          {"00029-3980031510", new AssetMapping("textures/00029-3980031510", new []{"Assets/AssetBundles/Textures/00029-3980031510.png"})},
          {"00031-1829375853", new AssetMapping("textures/00031-1829375853", new []{"Assets/AssetBundles/Textures/00031-1829375853.png"})},
          {"UIPanel", new AssetMapping("ui/uipanel", new []{"Assets/AssetBundles/UI/UIPanel.prefab"})},
          {"LocalizedStrings", new AssetMapping("config/binconfigdata/localizedstrings", new []{"Assets/AssetBundles/Config/BinConfigData/LocalizedStrings.bytes"})},
          {"Sheet1", new AssetMapping("config/binconfigdata/sheet1", new []{"Assets/AssetBundles/Config/BinConfigData/Sheet1.bytes"})},
          {"Sheet2", new AssetMapping("config/binconfigdata/sheet2", new []{"Assets/AssetBundles/Config/BinConfigData/Sheet2.bytes"})},
          {"Cube1fsdf", new AssetMapping("prefabs/pf2/cube2sdfwe", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube1fsdf.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube2sdfwe.prefab", "Assets/AssetBundles/Prefabs/pf2/Cubeljk.prefab"})},
          {"Cube2sdfwe", new AssetMapping("prefabs/pf2/cube2sdfwe", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube1fsdf.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube2sdfwe.prefab", "Assets/AssetBundles/Prefabs/pf2/Cubeljk.prefab"})},
          {"Cubeljk", new AssetMapping("prefabs/pf2/cube2sdfwe", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube1fsdf.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube2sdfwe.prefab", "Assets/AssetBundles/Prefabs/pf2/Cubeljk.prefab"})},
          {"Audio", new AssetMapping("audio", new []{"Assets/AssetBundles/Audio/2852.wav", "Assets/AssetBundles/Audio/button1.wav", "Assets/AssetBundles/Audio/大风-WQ20070525.mp3", "Assets/AssetBundles/Audio/笛子1-mcx20070508.wav"})},
          {"Config", new AssetMapping("config", new []{""})},
          {"Materials", new AssetMapping("materials", new []{"Assets/AssetBundles/Materials/00029-3980031510mat.mat", "Assets/AssetBundles/Materials/00031-1829375853mat.mat"})},
          {"Prefabs", new AssetMapping("prefabs", new []{"Assets/AssetBundles/Prefabs/Cube.prefab", "Assets/AssetBundles/Prefabs/Cube1.prefab", "Assets/AssetBundles/Prefabs/Cube2.prefab"})},
          {"Slicend", new AssetMapping("slicend", new []{"Assets/AssetBundles/Slicend/Auto9Slice.asset", "Assets/AssetBundles/Slicend/baoshixitong01.png", "Assets/AssetBundles/Slicend/baoshixitong02.png", "Assets/AssetBundles/Slicend/baoshixitong05.png", "Assets/AssetBundles/Slicend/baoshixitong06.png", "Assets/AssetBundles/Slicend/baoshixitong15.png", "Assets/AssetBundles/Slicend/baoshixitong21.png", "Assets/AssetBundles/Slicend/baoshixitong22.png", "Assets/AssetBundles/Slicend/baoxiangxitong02.png", "Assets/AssetBundles/Slicend/baoxiangxitong21.png", "Assets/AssetBundles/Slicend/baoxiangxitong23.png", "Assets/AssetBundles/Slicend/baoxiangzhuangbei16.png", "Assets/AssetBundles/Slicend/baoxiangzhuangbei17.png", "Assets/AssetBundles/Slicend/SampleImage.png"})},
          {"Textures", new AssetMapping("textures", new []{"Assets/AssetBundles/Textures/00023-3589759801.png", "Assets/AssetBundles/Textures/00029-3980031510.png", "Assets/AssetBundles/Textures/00031-1829375853.png"})},
          {"UI", new AssetMapping("ui", new []{"Assets/AssetBundles/UI/UIPanel.prefab"})},
          {"BinConfigData", new AssetMapping("config/binconfigdata", new []{"Assets/AssetBundles/Config/BinConfigData/LocalizedStrings.bytes", "Assets/AssetBundles/Config/BinConfigData/Sheet1.bytes", "Assets/AssetBundles/Config/BinConfigData/Sheet2.bytes"})},
          {"pf2", new AssetMapping("prefabs/pf2", new []{"Assets/AssetBundles/Prefabs/pf2/Cube1fsdf.prefab", "Assets/AssetBundles/Prefabs/pf2/Cube2sdfwe.prefab", "Assets/AssetBundles/Prefabs/pf2/Cubeljk.prefab"})},
       };
   }
}