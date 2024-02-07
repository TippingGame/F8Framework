using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = Constants.PoolsPresetComponentPath, fileName = "Pools Preset", order = 0)]
#endif
    public sealed class PoolsPreset : ScriptableObject
    {
        [SerializeField] private List<PoolPreset> _poolPresets = new List<PoolPreset>(256);

        public IReadOnlyList<PoolPreset> Presets => _poolPresets;
    }
}