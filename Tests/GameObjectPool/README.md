# F8 GameObjectPool

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 GameObjectPool组件，对象池管理，预加载池化，生成/销毁/延迟销毁，生命周期事件监听  

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 代码使用方法
```C#
    // 预制体或者组件
    private GameObject _gameObjectPrefab;
    private DemoGameObjectPool _componentPrefab;
    // 粒子特效
    private ParticleSystem _particleSystemPrefab;
    // 预设对象池ScriptableObject
    private PoolsPreset _poolsPreset;
    void Start()
    {
        /*------------------------------使用GameObjectPool对象池------------------------------*/
        
        // 使用名称或GameObject或者Component创建对象
        GameObject spawnedClone = FF8.GameObjectPool.Spawn("name");
        GameObject spawnedClone = FF8.GameObjectPool.Spawn(_gameObjectPrefab);
        DemoGameObjectPool component = FF8.GameObjectPool.Spawn(_componentPrefab, Vector3.zero, Quaternion.identity, this.transform);
        
        // 销毁
        FF8.GameObjectPool.Despawn(gameObject, delay: 0.5f);
        
        // 粒子特效播放完成后立即销毁
        FF8.GameObjectPool
            .Spawn(_particleSystemPrefab)
            .DespawnOnComplete();
        
        // 如何获取对象池
        F8GameObjectPool _pool = FF8.GameObjectPool.GetPoolByPrefab(_gameObjectPrefab);
        F8GameObjectPool _pool = FF8.GameObjectPool.GetPoolByPrefabName(_gameObjectPrefab.name);
        
        // 对每个池执行操作。
        FF8.GameObjectPool.ForEachPool(LogF8.Log);

        // 对每个克隆执行操作。
        FF8.GameObjectPool.ForEachClone(LogF8.Log);

        // 尝试获取克隆的状态（已生成 / 已回收 / 已生成超过容量）。
        PoolableStatus cloneStatus = FF8.GameObjectPool.GetCloneStatus(spawnedClone);

        // 游戏对象是否是克隆（使用 GameObjectPool 生成）？
        bool isClone = FF8.GameObjectPool.IsClone(spawnedClone);
        
        // 如果要销毁克隆但不回收克隆，请使用此方法以避免错误！
        FF8.GameObjectPool.DestroyClone(spawnedClone);
        
        // 销毁所有池。
        FF8.GameObjectPool.DestroyAllPools(immediately: false);
        
        
        
        /*------------------------------GameObjectPool对象池内功能------------------------------*/
        
        // 手动初始化池。如果您通过 Awake 方法访问池，而该方法在池初始化之前被调用，则可能需要这样做。
        _pool.Init();
        
        _pool.Init(_gameObjectPrefab);

        // 填充池。
        _pool.PopulatePool(16);
        
        // 设置池的容量。
        _pool.SetCapacity(32);
        
        // 设置池的溢出行为。
        _pool.SetBehaviourOnCapacityReached(BehaviourOnCapacityReached.Recycle);
        
        // 设置池中游戏对象的回收类型。
        _pool.SetDespawnType(DespawnType.DeactivateAndHide);
        
        // 设置池的回调类型，用于游戏对象的生成或回收。
        _pool.SetCallbacksType(CallbacksType.Interfaces);
        
        // 设置池的警告是否激活。
        _pool.SetWarningsActive(true);
        
        // 对池中的每个克隆执行操作。
        _pool.ForEachClone(LogF8.Log);
        
        // 对池中的每个已生成的克隆执行操作。
        _pool.ForEachSpawnedClone(LogF8.Log);
        
        // 对池中的每个已回收的克隆执行操作。
        _pool.ForEachDespawnedClone(LogF8.Log);
        
        // 销毁已生成的克隆。
        _pool.DestroySpawnedClones();
        
        // 销毁已回收的克隆。
        _pool.DestroyDespawnedClones();

        // 销毁池中的所有克隆。
        _pool.DestroyAllClones();

        // 立即销毁已生成的克隆。
        _pool.DestroySpawnedClonesImmediate();
        
        // 立即销毁已回收的克隆。
        _pool.DestroyDespawnedClonesImmediate();

        // 立即销毁池中的所有克隆。
        _pool.DestroyAllClonesImmediate();
        
        // 销毁池。
        _pool.DestroyPool();
        
        // 立即销毁池。
        _pool.DestroyPoolImmediate();
        
        // 回收池中的所有克隆。
        _pool.DespawnAllClones();

        // 清除池。
        _pool.Clear();
        
        // 对象池事件
        void DoSomething(GameObject go)
        {
            
        }
        // 监听
        _pool.GameObjectInstantiated.AddListener(DoSomething);
        _pool.GameObjectSpawned.AddListener(DoSomething);
        _pool.GameObjectDespawned.AddListener(DoSomething);
        // 移除
        _pool.GameObjectInstantiated.RemoveListener(DoSomething);
        _pool.GameObjectSpawned.RemoveListener(DoSomething);
        _pool.GameObjectDespawned.RemoveListener(DoSomething);
    }
    
    /*------------------------------继承IPoolable的物体拥有回调------------------------------*/
    public class DemoPoolCallBack : MonoBehaviour, IPoolable
    {
        public void OnSpawn()
        {
            // Do something on spawn.
        }
    
        public void OnDespawn()
        {
            // Do something on despawn.
        }
    }
```
## 拓展功能
1. 使用预加载池
![image](https://tippinggame-1257018413.cos.ap-guangzhou.myqcloud.com/TippingGame/GameObjectPool/ui_20240302154233.png)
