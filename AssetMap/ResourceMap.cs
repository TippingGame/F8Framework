// code generation.

using System.Collections.Generic;

namespace F8Framework.AssetMap
{
   public static class ResourceMap
   {
       public static Dictionary<string, string> Mappings
       {
           get => mappings;
       }
       
       private static Dictionary<string, string> mappings = new Dictionary<string, string> {
       };
   }
}