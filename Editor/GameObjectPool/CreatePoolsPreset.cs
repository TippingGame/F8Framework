using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class CreatePoolsPreset
    {
        [MenuItem("Assets/（F8预加载对象池）/（PoolsPreset.asset）", false, -1)]
        private static void CreateScriptObject()
        {
            PoolsPreset config = ScriptableObject.CreateInstance<PoolsPreset>();
            ProjectWindowUtil.CreateAsset(config, "PoolsPreset.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
