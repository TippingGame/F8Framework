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
          {"TImerGo", "TImerGo"},
          {"Entities1", "BinConfigData/Entities1"},
          {"Entities2", "BinConfigData/Entities2"},
          {"table1", "BinConfigData/table1"},
          {"table2", "BinConfigData/table2"},
       };
   }
}