# F8 ReferencePool

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 ReferencePool Component**  
Efficient object pooling system for managing reusable game objects and components to minimize instantiation overhead.
* Pool Management:
    * Acquire: Retrieve objects from pool
    * Release: Return objects to pool
    * Remove: Empty the pool

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Code Examples
```C#
    // Class implementing IReference interface for pool management
    public class AssetInfo : IReference
    {
        public void Clear()
        {
            
        }
    }

    void Start()
    {
        // Preload pool with 50 instances
        ReferencePool.Add<AssetInfo>(50);
        // Acquire an instance from pool
        AssetInfo assetInfo = ReferencePool.Acquire<AssetInfo>();
        
        // Return instance to pool (makes it available for reuse)
        ReferencePool.Release(assetInfo);
        // Clear all instances of this type from pool
        ReferencePool.RemoveAll(typeof(AssetInfo));
    }
```


