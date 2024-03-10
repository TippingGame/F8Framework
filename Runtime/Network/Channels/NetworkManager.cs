using System.Collections.Concurrent;
using System.Collections.Generic;

namespace F8Framework.Core
{
    [UpdateRefresh]
    public sealed class NetworkManager : ModuleSingleton<NetworkManager>, IModule
    {
        ConcurrentDictionary<string, INetworkChannel> channelDict;
        ConcurrentDictionary<string, INetworkChannel> addChannelDict;
        ConcurrentDictionary<string, INetworkChannel> deleteChannelDict;

        public int NetworkChannelCount => channelDict.Count;

        public bool AddChannel(INetworkChannel channel)
        {
            var channelName = channel.ChannelName;
            return addChannelDict.TryAdd(channelName, channel);
        }

        public bool AddChannel(string channelName, INetworkChannel channel)
        {
            return addChannelDict.TryAdd(channelName, channel);
        }

        public void RemoveChannel(INetworkChannel channel)
        {
            var channelName = channel.ChannelName;
            deleteChannelDict.TryAdd(channelName, channel);
        }

        public void RemoveChannel(string channelName, INetworkChannel channel)
        {
            deleteChannelDict.TryAdd(channelName, channel);
        }

        public bool CloseChannel(string channelName, out INetworkChannel channel)
        {
            if (channelDict.TryRemove(channelName, out channel))
            {
                channel.Close();
                return true;
            }

            return false;
        }

        public bool PeekChannel(string channelName, out INetworkChannel channel)
        {
            return channelDict.TryGetValue(channelName, out channel);
        }
        
        public bool HasChannel(string channelName)
        {
            return channelDict.ContainsKey(channelName);
        }

        public List<INetworkChannel> GetAllChannels()
        {
            List<INetworkChannel> channelList = new List<INetworkChannel>();
    
            foreach (INetworkChannel channel in channelDict.Values)
            {
                channelList.Add(channel);
            }
    
            return channelList;
        }

        public void OnInit(object createParam)
        {
            channelDict = new ConcurrentDictionary<string, INetworkChannel>();
            addChannelDict = new ConcurrentDictionary<string, INetworkChannel>();
            deleteChannelDict = new ConcurrentDictionary<string, INetworkChannel>();
        }
        
        /// <summary>
        /// 利用多核性能
        /// </summary>
        public void ThreadOnUpdate()
        {
            
        }
        
        /// <summary>
        /// Update只能利用单个CPU核心
        /// </summary>
        public void OnUpdate()
        {
            // 首先处理待删除字典
            foreach (var delete in deleteChannelDict)
            {
                delete.Value.Close();
                channelDict.TryRemove(delete.Key, out _);
            }
            deleteChannelDict.Clear();
    
            // 然后处理待添加字典
            foreach (var add in addChannelDict)
            {
                channelDict.TryAdd(add.Key, add.Value);
            }
            addChannelDict.Clear();
    
            // 最后对所有通道进行刷新
            foreach (var channel in channelDict.Values)
            {
                channel.TickRefresh();
            }
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            foreach (var channel in channelDict)
            {
                channel.Value.Close();
            }

            channelDict.Clear();
        }
    }
}