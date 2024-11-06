# F8 ReferencePool

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 ReferencePool组件，引用池管理，入池/取出/回收/清空

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git

### 代码使用方法
```C#
    // 使用IReference接口
    public class AssetInfo : IReference
    {
        public void Clear()
        {
            
        }
    }

    void Start()
    {
        // 添加入池50个数据
        ReferencePool.Add<AssetInfo>(50);
        // 取出
        AssetInfo assetInfo = ReferencePool.Acquire<AssetInfo>();
        
        // 回收
        ReferencePool.Release(assetInfo);
        // 清空
        ReferencePool.RemoveAll(typeof(AssetInfo));
    }
```


