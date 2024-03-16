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
           public string Version;
           public string Size;
           public string MD5;
           public string Package;
       
           public AssetMapping(string abName, string[] assetPath, string version, string size, string md5, string package)
           {
               AbName = abName;
               AssetPath = assetPath;
               Version = version;
               Size = size;
               MD5 = md5;
               Package = package;
           }
           
           public AssetMapping()
           {
               
           }
       }
       
       public static Dictionary<string, AssetMapping> Mappings
       {
           get => mappings;
           set => mappings = value;
       }
       
       private static Dictionary<string, AssetMapping> mappings = new Dictionary<string, AssetMapping> {
       };
   }
}