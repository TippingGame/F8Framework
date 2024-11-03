using UnityEngine;

namespace F8Framework.Core
{
    internal static class Constants
    {
        public const string NULL = "<NULL>";
        public const string NONE = "<NONE>";
        public const string NULLABLE = "<NULLABLE>";
        public const string UNKONM = "<UNKONW> ";
        internal const BehaviourOnCapacityReached DefaultBehaviourOnCapacityReached = BehaviourOnCapacityReached.InstantiateWithCallbacks;
        internal const DespawnType DefaultDespawnType = DespawnType.DeactivateAndHide;
        internal const CallbacksType DefaultCallbacksType = CallbacksType.Interfaces;
        internal const ReactionOnRepeatedDelayedDespawn DefaultDelayedDespawnHandleType = ReactionOnRepeatedDelayedDespawn.ResetDelay;
        internal const F8PoolMode DefaultF8PoolMode = F8PoolMode.Performance;
        internal const string OnSpawnMessageName = "OnSpawn";
        internal const string OnDespawnMessageName = "OnDespawn";
        internal const int DefaultPoolsMapCapacity = 64;
        internal const int DefaultPersistentPoolsCapacity = 8;
        internal const int DefaultClonesCapacity = 128;
        internal const int DefaultDespawnRequestsCapacity = 32;
        internal const int DefaultPoolableInterfacesCapacity = 16;
        internal const int DefaultPoolCapacity = 32;
        internal const int DefaultPoolablesListCapacity = 32;
        internal const bool DefaultSendWarningsStatus = true;
        internal const bool DefaultPoolPersistenceStatus = true;
        internal const int NewPoolPreloadSize = 0;

        internal static readonly Vector3 Vector3One = Vector3.one;
        internal static readonly Vector3 DefaultPosition = Vector3.zero;
        internal static readonly Quaternion DefaultRotation = Quaternion.identity;

        internal static class Tooltips
        {
            internal const string CallbacksType = "是否在克隆物体生成和回收时进行回调？\n\n" +
                                              "None - 禁用回调；\n\n" +
                                              "Interfaces - 使用 GetComponents 找到 ISpawnable、IDespawnable 或 IPoolable 接口并调用它们；\n\n" +
                                              "Interfaces In Children - 使用 GetComponentsInChildren 找到 ISpawnable、IDespawnable 或 IPoolable 接口并调用它们；\n\n" +
                                              "Send Message - 通过 GameObject.SendMessage 发送 OnSpawn 和 OnDespawn 消息；\n\n" +
                                              "Broadcast Message - 通过 GameObject.BroadcastMessage 广播 OnSpawn 和 OnDespawn 消息。";

            internal const string DespawnType = "克隆物体应如何回收？\n\n" +
                                                "Only Deactivate - 仅停用克隆物体并放回池中；\n\n" +
                                                "Deactivate And Set Null Parent - 与第一种相同，但还将父对象设置为空；\n\n" +
                                                "Deactivate And Hide - 与第一种相同，但还将父对象设置为池。";

            internal const string OverflowBehaviour = "池溢出时的处理方式是什么？\n\n" +
                                             "Return Nullable Clone - 返回可空的克隆物体；\n\n" +
                                             "Instantiate - 实例化克隆物体，它将不会被缓存到池中。这样的克隆物体忽略所有回调；\n\n" +
                                             "Instantiate With Callbacks - 实例化克隆物体，它将不会被缓存到池中；\n\n" +
                                             "Recycle - 新的克隆物体会强制旧的克隆物体回收；\n\n" +
                                             "Throw Exception - 抛出异常。";
            
            internal const string Capacity = "在运行时创建的池的默认容量。";
            internal const string Persistent = "在运行时创建的池是否应该是持久的？";
            internal const string Warnings = "在运行时创建的池是否默认查找问题并记录警告？";
            internal const string DelayedDespawnReaction = "如果尝试延迟销毁同一克隆物体多次，将采取什么措施？\n\n" +
                                                         "Ignore - 忽略对克隆物体重复延迟销毁的操作；\n\n" +
                                                         "Reset Delay - 重置克隆物体延迟销毁的时间；\n\n" +
                                                         "Reset Delay If New Time Is Less - 如果新时间较短，则重置克隆物体延迟销毁的时间；\n\n" +
                                                         "Reset Delay If New Time Is Greater - 如果新时间较长，则重置克隆物体延迟销毁的时间；\n\n" +
                                                         "Throw Exception - 抛出异常。";

            internal const string F8PoolMode = "Performance - 比 Safety 更快，但所有池都必须具有失真缩放等于 Vector3.one；\n\n" +
                                                  "Safety - 性能较差，但允许池设置任何比例。";
            internal const string DespawnPersistentClonesOnDestroy = "在销毁时是否应该回收持久化的克隆物体？";
            internal const string CheckClonesForNull = "在生成时是否应该检查克隆物体是否为空？";
            internal const string CheckForPrefab = "在设置期间是否应该检查池预设物体，以查看它是否真的是预设物体？";
            internal const string ClearEventsOnDestroy = "在销毁时是否应该清除 F8Pool 静态事件？";
            internal const string GlobalUpdateType = "此组件的 UpdateType。处理延迟销毁。";
            internal const string GlobalPreloadType = "下面的 PoolsPreset 中池的预加载类型。";
            internal const string PoolsToPreload = "要预加载的池。";
            internal const string PoolName = "池名字。";
            internal const string PoolEnabled = "池是否启用。";
            internal const string PreloadSize = "要预加载的个数。";
        }
    }
}