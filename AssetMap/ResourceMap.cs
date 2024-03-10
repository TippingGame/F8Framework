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
          {"LineBreaking Following Characters", "LineBreaking Following Characters"},
          {"LineBreaking Leading Characters", "LineBreaking Leading Characters"},
          {"TMP Settings", "TMP Settings"},
          {"LiberationSans SDF - Drop Shadow", "Fonts & Materials/LiberationSans SDF - Drop Shadow"},
          {"LiberationSans SDF - Fallback", "Fonts & Materials/LiberationSans SDF - Fallback"},
          {"LiberationSans SDF - Outline", "Fonts & Materials/LiberationSans SDF - Outline"},
          {"LiberationSans SDF", "Fonts & Materials/LiberationSans SDF"},
          {"EmojiOne", "Sprite Assets/EmojiOne"},
          {"Default Style Sheet", "Style Sheets/Default Style Sheet"},
       };
   }
}