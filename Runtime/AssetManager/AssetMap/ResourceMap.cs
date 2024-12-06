// code generation.

using System.Collections.Generic;

namespace F8Framework.Core
{
   public static class ResourceMap
   {
       public static Dictionary<string, string> Mappings
       {
           get => mappings;
           set => mappings = value;
       }
       
       private static Dictionary<string, string> mappings = new Dictionary<string, string> {
       };
   }
}