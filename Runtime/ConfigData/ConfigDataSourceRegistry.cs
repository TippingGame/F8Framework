using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace F8Framework.Core
{
    /// <summary>
    /// 可选配置数据源。实现程序集只依赖 Core，Core 不反向依赖具体的数据格式模块。
    /// </summary>
    public interface IConfigDataSource
    {
        string Id { get; }
        int Priority { get; }
        bool IsAvailable { get; }
        void LoadAll(IDictionary<string, object> destination);
    }

    /// <summary>
    /// 运行时配置数据源注册表。Excel、JSON、远程配置等模块均可按需注册。
    /// </summary>
    public static class ConfigDataSourceRegistry
    {
        private static readonly object SyncRoot = new object();
        private static readonly List<IConfigDataSource> Sources = new List<IConfigDataSource>();

        public static void Register(IConfigDataSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrWhiteSpace(source.Id))
            {
                throw new ArgumentException("配置数据源 Id 不能为空。", nameof(source));
            }

            lock (SyncRoot)
            {
                Sources.RemoveAll(item =>
                    string.Equals(item.Id, source.Id, StringComparison.OrdinalIgnoreCase));
                Sources.Add(source);
                Sources.Sort(CompareSources);
            }
        }

        public static bool Unregister(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            lock (SyncRoot)
            {
                return Sources.RemoveAll(item =>
                    string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase)) > 0;
            }
        }

        public static IReadOnlyList<string> GetAvailableSourceIds()
        {
            IConfigDataSource[] snapshot;
            lock (SyncRoot)
            {
                snapshot = Sources.ToArray();
            }

            List<string> availableSourceIds = new List<string>();
            foreach (IConfigDataSource source in snapshot)
            {
                try
                {
                    if (source.IsAvailable)
                    {
                        availableSourceIds.Add(source.Id);
                    }
                }
                catch (Exception exception)
                {
                    LogF8.LogError($"检查配置数据源可用性失败：{source.Id}\n{exception}");
                }
            }

            return availableSourceIds;
        }

        public static bool TryLoadAll(out Dictionary<string, object> data)
        {
            data = new Dictionary<string, object>();
            return TryLoadAll(data);
        }

        public static bool TryLoadAll(IDictionary<string, object> destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            IConfigDataSource[] snapshot;
            lock (SyncRoot)
            {
                snapshot = Sources.ToArray();
            }

            foreach (IConfigDataSource source in snapshot)
            {
                try
                {
                    if (!source.IsAvailable)
                    {
                        continue;
                    }

                    Dictionary<string, object> loadedData = new Dictionary<string, object>();
                    source.LoadAll(loadedData);
                    foreach (KeyValuePair<string, object> item in loadedData)
                    {
                        destination[item.Key] = item.Value;
                    }

                    LogF8.LogConfig($"已通过配置数据源加载数据：{source.Id}");
                    return true;
                }
                catch (Exception exception)
                {
                    LogF8.LogError($"配置数据源加载失败：{source.Id}\n{exception}");
                }
            }

            return false;
        }

        private static int CompareSources(IConfigDataSource left, IConfigDataSource right)
        {
            int priorityComparison = right.Priority.CompareTo(left.Priority);
            return priorityComparison != 0
                ? priorityComparison
                : string.Compare(left.Id, right.Id, StringComparison.OrdinalIgnoreCase);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            lock (SyncRoot)
            {
                Sources.Clear();
            }
        }
    }
}
