using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public sealed class PoolsPreset : ScriptableObject
    {
        [SerializeField] private List<PoolPreset> _poolPresets = new List<PoolPreset>(256);

        public IReadOnlyList<PoolPreset> Presets => _poolPresets;
    }
}