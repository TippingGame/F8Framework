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
        [Tooltip(Constants.Tooltips.PoolName)]
        [SerializeField] private string _name;
        [Space]
#endif
        [Tooltip(Constants.Tooltips.PoolEnabled)]
        [SerializeField] private bool _enabled = true;
        [Tooltip(Constants.Tooltips.Persistent)]
        [SerializeField] private GameObject _prefab;
        [Tooltip(Constants.Tooltips.OverflowBehaviour)]
        [SerializeField] private BehaviourOnCapacityReached _behaviourOnCapacityReached = Constants.DefaultBehaviourOnCapacityReached;
        [Tooltip(Constants.Tooltips.DespawnType)]
        [SerializeField] private DespawnType _despawnType = Constants.DefaultDespawnType;
        [Tooltip(Constants.Tooltips.CallbacksType)]
        [SerializeField] private CallbacksType _callbacksType = Constants.DefaultCallbacksType;
        [Tooltip(Constants.Tooltips.Capacity)]
        [SerializeField, Min(0)] private int _capacity;
        [Tooltip(Constants.Tooltips.PreloadSize)]
        [SerializeField, Min(0)] private int _preloadSize;
        [Tooltip(Constants.Tooltips.Persistent)]
        [SerializeField] private bool _dontDestroyOnLoad = true;
        [Tooltip(Constants.Tooltips.Warnings)]
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