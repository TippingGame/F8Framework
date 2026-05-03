using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F8Framework.Core
{
    /// <summary>
    /// 表示红点系统中的节点。
    /// </summary>
    public class RedDotNode
    {
        /// <summary>
        /// 节点的名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 此节点的父节点。
        /// </summary>
        public RedDotNode Parent { get; set; }

        /// <summary>
        /// 节点的状态。
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 与节点关联的计数。
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 与节点关联的文本状态。
        /// </summary>
        public string TextState { get; set; }

        /// <summary>
        /// 节点及其子树的聚合状态缓存。
        /// </summary>
        public bool AggregatedState { get; set; }

        /// <summary>
        /// 节点及其子树的聚合计数缓存。
        /// </summary>
        public int AggregatedCount { get; set; }

        /// <summary>
        /// 节点及其子树的聚合文本状态缓存。
        /// </summary>
        public string AggregatedTextState { get; set; }

        /// <summary>
        /// 子节点列表。
        /// </summary>
        public List<RedDotNode> Children { get; set; }

        /// <summary>
        /// 初始化 RedDotNode 类的新实例。
        /// </summary>
        /// <param name="name">节点的名称。</param>
        /// <param name="parent">父节点。</param>
        /// <param name="state">节点的初始状态。</param>
        /// <param name="count">节点的初始计数。</param>
        /// <param name="textState">节点的初始文本状态。</param>
        /// <param name="children">初始子节点列表。</param>
        public RedDotNode(string name = null, RedDotNode parent = null, bool state = false, int count = 0, string textState = null, List<RedDotNode> children = null)
        {
            Name = name;
            Parent = parent;
            State = state;
            Count = count;
            TextState = textState;
            Children = children ?? new List<RedDotNode>();
        }
    }

    /// <summary>
    /// 管理 UI 元素的红点系统。
    /// </summary>
    public class UIRedDot : Singleton<UIRedDot>
    {
        private const int MaxAsyncRefreshPerFrame = 1;
        private Dictionary<string, RedDotNode> _redDotMapping = new Dictionary<string, RedDotNode>();
        private Dictionary<string, List<GameObject>> reddotDic;
        private Dictionary<string, RedDotNode> reddotList;
        private readonly Queue<string> _asyncRefreshQueue = new Queue<string>();
        private readonly HashSet<string> _asyncRefreshSet = new HashSet<string>();
        private Coroutine _asyncChangeCoroutine;

        /// <summary>
        /// 初始化红点系统。
        /// </summary>
        public void Init()
        {
            StopAsyncRefresh();
            _asyncRefreshQueue.Clear();
            _asyncRefreshSet.Clear();
            reddotDic = new Dictionary<string, List<GameObject>>();
            reddotList = new Dictionary<string, RedDotNode>();

            foreach (var item in _redDotMapping)
            {
                GetOrCreateNode(item.Key);
            }

            foreach (var item in reddotList.Values)
            {
                item.Parent = null;
                item.Children.Clear();
            }

            foreach (var item in _redDotMapping)
            {
                RedDotNode parentNode = GetOrCreateNode(item.Key);
                parentNode.State = item.Value.State;
                parentNode.Count = item.Value.Count;
                parentNode.TextState = item.Value.TextState;
                parentNode.AggregatedState = false;
                parentNode.AggregatedCount = 0;
                parentNode.AggregatedTextState = null;

                if (item.Value.Children == null)
                {
                    continue;
                }

                foreach (var child in item.Value.Children)
                {
                    RedDotNode childNode = GetOrCreateNode(child.Name);
                    childNode.Parent = parentNode;
                    if (_redDotMapping.TryGetValue(child.Name, out RedDotNode childCfg))
                    {
                        childNode.State = childCfg.State;
                        childNode.Count = childCfg.Count;
                        childNode.TextState = childCfg.TextState;
                    }
                    else
                    {
                        childNode.State = false;
                        childNode.Count = 0;
                        childNode.TextState = null;
                    }

                    childNode.AggregatedState = false;
                    childNode.AggregatedCount = 0;
                    childNode.AggregatedTextState = null;

                    if (!parentNode.Children.Exists(node => node.Name == childNode.Name))
                    {
                        parentNode.Children.Add(childNode);
                    }
                }
            }

            InitializeAggregateCaches();
        }

        /// <summary>
        /// 添加新的红点配置。
        /// </summary>
        /// <param name="node">要添加的节点。</param>
        /// <param name="parentNode">父节点。</param>
        /// <param name="state">节点的初始状态。</param>
        public void AddRedDotCfg(string node, string parentNode = default, bool state = false)
        {
            RedDotNode configNode = GetOrCreateConfigNode(node);
            configNode.State = state;
            configNode.Count = 0;
            configNode.TextState = null;

            AttachConfigNode(node, parentNode);
        }

        /// <summary>
        /// 添加新的红点配置并设置初始计数。
        /// </summary>
        /// <param name="node">要添加的节点。</param>
        /// <param name="count">节点的初始计数。</param>
        /// <param name="parentNode">父节点。</param>
        public void AddRedDotCfg(string node, int count, string parentNode = default)
        {
            RedDotNode configNode = GetOrCreateConfigNode(node);
            configNode.State = count > 0;
            configNode.Count = count;
            configNode.TextState = null;

            AttachConfigNode(node, parentNode);
        }

        /// <summary>
        /// 添加新的红点配置并设置初始文本状态。
        /// </summary>
        /// <param name="node">要添加的节点。</param>
        /// <param name="textState">节点的初始文本状态。</param>
        /// <param name="parentNode">父节点。</param>
        public void AddRedDotCfgText(string node, string textState, string parentNode = default)
        {
            RedDotNode configNode = GetOrCreateConfigNode(node);
            configNode.State = !string.IsNullOrEmpty(textState);
            configNode.Count = 0;
            configNode.TextState = string.IsNullOrEmpty(textState) ? null : textState;

            AttachConfigNode(node, parentNode);
        }

        /// <summary>
        /// 根据计数更改红点节点的状态。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <param name="count">要设置的计数。</param>
        public void Change(string type, int count)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                if (redDotNode.Count != count)
                {
                    redDotNode.State = count > 0;
                    redDotNode.Count = count;
                    RefreshNodeChain(redDotNode);
                }
            }
        }

        /// <summary>
        /// 根据计数异步更改红点节点的状态，并按帧分批刷新红点。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <param name="count">要设置的计数。</param>
        /// <returns>异步刷新的协程。</returns>
        public Coroutine ChangeAsync(string type, int count)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                if (redDotNode.Count != count)
                {
                    redDotNode.State = count > 0;
                    redDotNode.Count = count;
                    QueueRefresh(type);
                }
            }

            return _asyncChangeCoroutine;
        }

        /// <summary>
        /// 根据文本状态更改红点节点的状态。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <param name="textState">要设置的文本状态。</param>
        public void Change(string type, string textState = null)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                if (redDotNode.TextState != textState)
                {
                    redDotNode.State = textState != null;
                    redDotNode.TextState = textState;
                    RefreshNodeChain(redDotNode);
                }
            }
        }

        /// <summary>
        /// 根据文本状态异步更改红点节点的状态，并按帧分批刷新红点。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <param name="textState">要设置的文本状态。</param>
        /// <returns>异步刷新的协程。</returns>
        public Coroutine ChangeAsync(string type, string textState = null)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                if (redDotNode.TextState != textState)
                {
                    redDotNode.State = textState != null;
                    redDotNode.TextState = textState;
                    QueueRefresh(type);
                }
            }

            return _asyncChangeCoroutine;
        }

        /// <summary>
        /// 更改红点节点的状态。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <param name="state">要设置的状态。</param>
        public void Change(string type, bool state)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                if (redDotNode.State != state)
                {
                    redDotNode.State = state;
                    RefreshNodeChain(redDotNode);
                }
            }
        }

        /// <summary>
        /// 异步更改红点节点的状态，并按帧分批刷新红点。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <param name="state">要设置的状态。</param>
        /// <returns>异步刷新的协程。</returns>
        public Coroutine ChangeAsync(string type, bool state)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                if (redDotNode.State != state)
                {
                    redDotNode.State = state;
                    QueueRefresh(type);
                }
            }

            return _asyncChangeCoroutine;
        }

        /// <summary>
        /// 当状态发生变化时触发的事件。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <param name="status">变化后的状态。</param>
        private void OnStatusChangedEvent(string type, bool status)
        {
            if (reddotDic.TryGetValue(type, out List<GameObject> data))
            {
                foreach (var item in data)
                {
                    if (item != null && item.activeSelf != status)
                    {
                        item.SetActive(status);
                    }
                }
            }
        }

        /// <summary>
        /// 将当前节点加入异步刷新队列。
        /// </summary>
        /// <param name="type">当前红点节点类型。</param>
        private void QueueRefresh(string type)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                EnqueueRefresh(redDotNode.Name);

                if (_asyncChangeCoroutine == null)
                {
                    _asyncChangeCoroutine = Util.Unity.StartCoroutine(ProcessRefreshQueue());
                }
            }
        }

        /// <summary>
        /// 将节点加入刷新队列，避免同一节点重复排队。
        /// </summary>
        /// <param name="type">红点节点类型。</param>
        private void EnqueueRefresh(string type)
        {
            if (_asyncRefreshSet.Add(type))
            {
                _asyncRefreshQueue.Enqueue(type);
            }
        }

        /// <summary>
        /// 分帧处理红点刷新队列。
        /// </summary>
        private IEnumerator ProcessRefreshQueue()
        {
            while (_asyncRefreshQueue.Count > 0)
            {
                int refreshCount = 0;

                while (_asyncRefreshQueue.Count > 0 && refreshCount < MaxAsyncRefreshPerFrame)
                {
                    string type = _asyncRefreshQueue.Dequeue();
                    _asyncRefreshSet.Remove(type);

                    if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
                    {
                        bool aggregateChanged = RecalculateAggregates(redDotNode, out bool stateChanged);

                        if (stateChanged)
                        {
                            OnStatusChangedEvent(type, redDotNode.AggregatedState);
                        }

                        if (aggregateChanged && redDotNode.Parent != null)
                        {
                            EnqueueRefresh(redDotNode.Parent.Name);
                        }
                    }

                    refreshCount++;
                }

                if (_asyncRefreshQueue.Count > 0)
                {
                    yield return null;
                }
            }

            _asyncChangeCoroutine = null;
        }

        private void StopAsyncRefresh()
        {
            if (_asyncChangeCoroutine != null)
            {
                Util.Unity.StopCoroutine(_asyncChangeCoroutine);
                _asyncChangeCoroutine = null;
            }
        }

        /// <summary>
        /// 同步刷新当前节点及其父链上的聚合缓存。
        /// </summary>
        /// <param name="redDotNode">发生变化的节点。</param>
        private void RefreshNodeChain(RedDotNode redDotNode)
        {
            while (redDotNode != null)
            {
                bool aggregateChanged = RecalculateAggregates(redDotNode, out bool stateChanged);

                if (stateChanged)
                {
                    OnStatusChangedEvent(redDotNode.Name, redDotNode.AggregatedState);
                }

                if (!aggregateChanged)
                {
                    break;
                }

                redDotNode = redDotNode.Parent;
            }
        }

        /// <summary>
        /// 重新计算节点的聚合缓存，只读取直接子节点的缓存值。
        /// </summary>
        /// <param name="redDotNode">要计算的节点。</param>
        /// <param name="stateChanged">聚合状态是否发生变化。</param>
        /// <returns>任一聚合缓存是否发生变化。</returns>
        private bool RecalculateAggregates(RedDotNode redDotNode, out bool stateChanged)
        {
            bool oldState = redDotNode.AggregatedState;
            int oldCount = redDotNode.AggregatedCount;
            string oldText = redDotNode.AggregatedTextState;

            bool nextState = redDotNode.State;
            int nextCount = redDotNode.Count;
            string nextText = string.IsNullOrEmpty(redDotNode.TextState) ? null : redDotNode.TextState;

            if (redDotNode.Children != null)
            {
                foreach (var child in redDotNode.Children)
                {
                    nextState |= child.AggregatedState;
                    nextCount += child.AggregatedCount;

                    if (string.IsNullOrEmpty(nextText) && !string.IsNullOrEmpty(child.AggregatedTextState))
                    {
                        nextText = child.AggregatedTextState;
                    }
                }
            }

            redDotNode.AggregatedState = nextState;
            redDotNode.AggregatedCount = nextCount;
            redDotNode.AggregatedTextState = nextText;

            stateChanged = oldState != nextState;
            return stateChanged || oldCount != nextCount || oldText != nextText;
        }

        /// <summary>
        /// 获取或创建规范化节点实例。
        /// </summary>
        /// <param name="nodeName">节点名称。</param>
        /// <returns>规范化后的节点实例。</returns>
        private RedDotNode GetOrCreateNode(string nodeName)
        {
            if (!reddotList.TryGetValue(nodeName, out RedDotNode redDotNode))
            {
                redDotNode = new RedDotNode(nodeName);
                reddotList[nodeName] = redDotNode;
            }

            return redDotNode;
        }

        /// <summary>
        /// 获取或创建配置节点实例。
        /// </summary>
        /// <param name="nodeName">节点名称。</param>
        /// <returns>配置节点实例。</returns>
        private RedDotNode GetOrCreateConfigNode(string nodeName)
        {
            if (!_redDotMapping.TryGetValue(nodeName, out RedDotNode redDotNode))
            {
                redDotNode = new RedDotNode(nodeName);
                _redDotMapping[nodeName] = redDotNode;
            }

            return redDotNode;
        }

        /// <summary>
        /// 将节点挂接到配置树中。
        /// </summary>
        /// <param name="node">节点名称。</param>
        /// <param name="parentNode">父节点名称。</param>
        private void AttachConfigNode(string node, string parentNode)
        {
            if (parentNode == default)
            {
                return;
            }

            RedDotNode parentConfigNode = GetOrCreateConfigNode(parentNode);
            List<RedDotNode> children = parentConfigNode.Children;

            if (!children.Exists(item => item.Name == node))
            {
                children.Add(new RedDotNode(node));
            }
        }

        /// <summary>
        /// 初始化所有节点的聚合缓存。
        /// </summary>
        private void InitializeAggregateCaches()
        {
            foreach (var node in reddotList.Values)
            {
                if (node.Parent == null)
                {
                    RebuildAggregateCaches(node);
                }
            }
        }

        /// <summary>
        /// 自底向上重建节点及其子树的聚合缓存。
        /// </summary>
        /// <param name="redDotNode">要重建的节点。</param>
        private void RebuildAggregateCaches(RedDotNode redDotNode)
        {
            if (redDotNode.Children != null)
            {
                foreach (var child in redDotNode.Children)
                {
                    RebuildAggregateCaches(child);
                }
            }

            RecalculateAggregates(redDotNode, out _);
        }

        /// <summary>
        /// 获取红点节点的状态。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <returns>红点节点的状态。</returns>
        public bool GetState(string type)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                return redDotNode.AggregatedState;
            }

            return false;
        }

        /// <summary>
        /// 获取红点节点的计数。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <returns>红点节点的计数。</returns>
        public int GetCount(string type)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                return redDotNode.AggregatedCount;
            }

            return 0;
        }

        /// <summary>
        /// 获取红点节点的文本状态。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        /// <returns>红点节点的文本状态。</returns>
        public string GetTextState(string type)
        {
            if (reddotList.TryGetValue(type, out RedDotNode redDotNode))
            {
                return redDotNode.AggregatedTextState;
            }

            return null;
        }

        /// <summary>
        /// 绑定红点节点到指定的游戏对象。
        /// </summary>
        /// <param name="redDotType">红点节点的类型。</param>
        /// <param name="go">要绑定的游戏对象。</param>
        /// <param name="onlyGo">是否只绑定指定的游戏对象。</param>
        public void Binding(string redDotType, GameObject go, bool onlyGo = false)
        {
            if (!reddotDic.ContainsKey(redDotType))
            {
                reddotDic[redDotType] = new List<GameObject>();
            }

            if (onlyGo)
            {
                for (int i = reddotDic[redDotType].Count - 1; i >= 0; i--)
                {
                    GameObject item = reddotDic[redDotType][i];
                    if (item != null && item != go)
                    {
                        item.SetActive(false);
                    }
                }

                reddotDic[redDotType].Clear();
                reddotDic[redDotType].Add(go);
            }
            else
            {
                for (int i = reddotDic[redDotType].Count - 1; i >= 0; i--)
                {
                    if (reddotDic[redDotType][i] == null)
                    {
                        reddotDic[redDotType].RemoveAt(i);
                    }
                }

                if (!reddotDic[redDotType].Contains(go))
                {
                    reddotDic[redDotType].Add(go);
                }
            }

            UpdateStats(redDotType);
        }

        /// <summary>
        /// 解除指定红点节点的绑定。
        /// </summary>
        /// <param name="redDotType">红点节点的类型。</param>
        public void UnBinding(string redDotType)
        {
            if (reddotDic.TryGetValue(redDotType, out List<GameObject> data))
            {
                foreach (var item in data)
                {
                    if (item != null)
                    {
                        item.SetActive(false);
                    }
                }
            }
            
            reddotDic.Remove(redDotType);
        }
        
        public void UnBinding(string redDotType, GameObject go)
        {
            if (reddotDic.TryGetValue(redDotType, out List<GameObject> data))
            {
                for (int i = data.Count - 1; i >= 0; i--)
                {
                    GameObject item = data[i];
                    if (item == null || item == go)
                    {
                        if (item != null)
                        {
                            item.SetActive(false);
                        }

                        data.RemoveAt(i);
                    }
                }

                if (data.Count == 0)
                {
                    reddotDic.Remove(redDotType);
                }
            }
        }

        /// <summary>
        /// 更新指定红点节点的状态。
        /// </summary>
        /// <param name="type">红点节点的类型。</param>
        private void UpdateStats(string type)
        {
            OnStatusChangedEvent(type, GetState(type));
        }

        /// <summary>
        /// 移除所有红点节点的状态。
        /// </summary>
        public void RemoveAllRed()
        {
            foreach (var item in reddotList)
            {
                item.Value.State = false;
                item.Value.Count = 0;
                item.Value.TextState = null;
                item.Value.AggregatedState = false;
                item.Value.AggregatedCount = 0;
                item.Value.AggregatedTextState = null;
            }

            foreach (var item in reddotDic)
            {
                foreach (var go in item.Value)
                {
                    if (go != null)
                    {
                        go.SetActive(false);
                    }
                }
            }

            reddotDic.Clear();
            _asyncRefreshQueue.Clear();
            _asyncRefreshSet.Clear();
            StopAsyncRefresh();
        }
    }
}
