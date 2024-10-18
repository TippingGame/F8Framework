// code generation.

using System.Collections.Generic;
using UnityEngine.Scripting;

namespace F8Framework.Core
{
   public static class AssetBundleMap
   {
       [Preserve]
       public class AssetMapping
       {
           public string AbName;
           public string[] AssetPath;
           public string Version;
           public string Size;
           public string MD5;
           public string Package;
           public string Updated;

           /// <summary>
           /// AB资产信息
           /// </summary>
           /// <param name="abName"></param>
           /// <param name="assetPath"></param>
           /// <param name="version"></param>
           /// <param name="size"></param>
           /// <param name="md5"></param>
           /// <param name="package">使用文件夹区分包，例如Package_0目录下的就是包编号：0。</param>
           /// <param name="updated"></param>
           public AssetMapping(string abName, string[] assetPath, string version, string size, string md5, string package, string updated)
           {
               AbName = abName;
               AssetPath = assetPath;
               Version = version;
               Size = size;
               MD5 = md5;
               Package = package;
               Updated = updated;
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