using System;
using UnityEngine;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace F8Framework.Core
{
    [Serializable]
#if ENABLE_IL2CPP
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    public sealed class PoolPreset
    {
#if UNITY_EDITOR
        [SerializeField] private string _name;
        [Space]
#endif
        [SerializeField] private bool _enabled = true;
        [SerializeField] private GameObject _prefab;
        [Tooltip(Constants.Tooltips.OverflowBehaviour)]
        [SerializeField] private BehaviourOnCapacityReached _behaviourOnCapacityReached = Constants.DefaultBehaviourOnCapacityReached;
        [Tooltip(Constants.Tooltips.DespawnType)]
        [SerializeField] private DespawnType _despawnType = Constants.DefaultDespawnType;
        [Tooltip(Constants.Tooltips.CallbacksType)]
        [SerializeField] private CallbacksType _callbacksType = Constants.DefaultCallbacksType;
        [SerializeField, Min(0)] private int _capacity;
        [SerializeField, Min(0)] private int _preloadSize;
        [SerializeField] private bool _dontDestroyOnLoad;
        [SerializeField] private bool _warnings = true;

        public bool Enabled => _enabled;
        public GameObject Prefab => _prefab;
        public BehaviourOnCapacityReached BehaviourOnCapacityReached => _behaviourOnCapacityReached;
        public DespawnType DespawnType => _despawnType;
        public CallbacksType CallbacksType => _callbacksType;
        public int Capacity => _capacity;
        public int PreloadSize => _preloadSize;
        public bool Persistent => _dontDestroyOnLoad;
        public bool Warnings => _warnings;
    }
}