using UnityEngine;
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
        private Dictionary<string, RedDotNode> _redDotMapping = new Dictionary<string, RedDotNode>();
        private Dictionary<string, List<GameObject>> reddotDic;
        private Dictionary<string, RedDotNode> reddotList;

        /// <summary>
        /// 初始化红点系统。
        /// </summary>
        public void Init()
        {
            reddotDic = new Dictionary<string, List<GameObject>>();
            reddotList = new Dictionary<string, RedDotNode>();

            foreach (var item in _redDotMapping)
            {
                if (!reddotList.ContainsKey(item.Key))
                {
                    reddotList.TryAdd(item.Key, item.Value);
                }
                else
                {
                    reddotList[item.Key].Children = item.Value.Children;
                }

                reddotList[item.Key].State = false;

                if (item.Value.Children != null)
                {
                    foreach (var children in item.Value.Children)
                    {
                        children.State = false;
                        children.Parent = item.Value;

                        reddotList[children.Name] = children;
                    }
                }
            }
        }

        /// <summary>
        /// 添加新的红点配置。
        /// </summary>
        /// <param name="node">要添加的节点。</param>
        /// <param name="parentNode">父节点。</param>
        /// <param name="state">节点的初始状态。</param>
        public void AddRedDotCfg(string node, string parentNode = default, bool state = false)
        {
            string nodeStr = node;
            string parentStr = parentNode;

            if (parentNode == default)
            {
                _redDotMapping[nodeStr] = new RedDotNode(nodeStr);
            }
            else
            {
                if (!_redDotMapping.ContainsKey(parentStr))
                {
                    _redDotMapping.TryAdd(parentStr, new RedDotNode(parentStr));
                }

                List<RedDotNode> childrens = _redDotMapping[parentStr].Children;
                RedDotNode children = new RedDotNode(nodeStr);
                if (!childrens.Contains(children))
                {
                    childrens.Add(children);
                }
            }
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
                    bool state = count > 0;
                    OnStatusChangedEvent(type, state);
                    redDotNode.State = state;
                    redDotNode.Count = count;
                    if (redDotNode.Parent != null)
                    {
                        ParentChange(redDotNode.Parent.Name);
                    }
                }
            }
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
                    bool state = textState != null;
                    OnStatusChangedEvent(type, state);
                    redDotNode.State = state;
                    redDotNode.TextState = textState;
                    if (redDotNode.Parent != null)
                    {
                        ParentChange(redDotNode.Parent.Name);
                    }
                }
            }
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
                    OnStatusChangedEvent(type, state);
                    redDotNode.State = state;
                    if (redDotNode.Parent != null)
                    {
                        ParentChange(redDotNode.Parent.Name);
                    }
                }
            }
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
                    if (item != null)
                    {
                        item.SetActive(status);
                    }
                }
            }
        }

        /// <summary>
        /// 当父节点状态变化时触发的事件。
        /// </summary>
        /// <param name="parentType">父节点类型。</param>
        private void ParentChange(string parentType)
        {
            if (reddotList.TryGetValue(parentType, out RedDotNode redDotNode))
            {
                OnStatusChangedEvent(parentType, GetState(parentType));
                if (redDotNode != null && redDotNode.Parent != null)
                {
                    ParentChange(redDotNode.Parent.Name);
                }
            }
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
                if (redDotNode.Children == null || redDotNode.Children.Count == 0)
                {
                    return redDotNode.State;
                }
                else
                {
                    foreach (var children in redDotNode.Children)
                    {
                        if (GetState(children.Name))
                        {
                            return true;
                        }
                    }

                    return redDotNode.State;
                }
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
                if (redDotNode.Children == null || redDotNode.Children.Count == 0)
                {
                    return redDotNode.Count;
                }
                else
                {
                    int totalCount = redDotNode.Count; // 初始化总数为当前节点的 Count

                    foreach (var children in redDotNode.Children)
                    {
                        // 递归获取子节点的 Count 并累加到总数中
                        totalCount += GetCount(children.Name);
                    }

                    return totalCount;
                }
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
                if (!string.IsNullOrEmpty(redDotNode.TextState))
                {
                    return redDotNode.TextState;
                }
                else if (redDotNode.Children != null)
                {
                    foreach (var children in redDotNode.Children)
                    {
                        string textState = GetTextState(children.Name);
                        if (!string.IsNullOrEmpty(textState))
                        {
                            return textState;
                        }
                    }
                }
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
                    if (reddotDic[redDotType][i] == go)
                    {
                        reddotDic[redDotType].RemoveAt(i);
                    }
                }

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
                foreach (var item in data)
                {
                    if (item != null && item == go)
                    {
                        item.SetActive(false);
                    }
                }
            }
            
            reddotDic.Remove(redDotType);
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
            }

            reddotDic.Clear();
        }
    }
}
