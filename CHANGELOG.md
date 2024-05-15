# Changelog

## [1.0.10] - 2024-05-16
1.AssetBundleMap.json中只保留一个AssetPath，改为从AssetBundle中GetAllAssetNames方法获取
2.由于GetAllAssetNames只能获取小写的AssetPath，需修改GetLoadProgress和GetAssetObject方法

## [1.0.9] - 2024-05-12
1.Network模块代码优化GC

## [1.0.8] - 2024-05-09
1.输入模块增加GetAxisRaw

## [1.0.7] - 2024-05-09
1.Network模块代码优化

## [1.0.6] - 2024-05-09
1.本地化表移除实时监控

## [1.0.5] - 2024-05-08
1.本地化表TextID可为空

## [1.0.4] - 2024-05-08
1.热更新增加校验本地资源md5
2.分包下载增加断点续传

## [1.0.3] - 2024-05-02
1.InitializeOnLoadMethod统一管理调用顺序

## [1.0.2] - 2024-04-09
1.LitJson序列化增加UnityEngine基础类型，增加跳过序列化特性[JsonIgnore]

## [1.0.1] - 2024-04-03
1.添加UI红点组件及示例

## [1.0.0] - 2023-12-10
UnityF8Framework核心功能
