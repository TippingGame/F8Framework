using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace F8Framework.Core
{
    [UpdateRefresh]
    public sealed class NetworkManager : ModuleSingleton<NetworkManager>, IModule
    {
        ConcurrentBag<INetworkChannel> channelDict;
        private Thread netRun;
        
        public int NetworkChannelCount => channelDict.Count;

        public void AddChannel(INetworkChannel channel)
        {
            channelDict.Add(channel);
        }

        public void RemoveChannel(string channelName)
        {
            INetworkChannel channel;
            foreach (INetworkChannel item in channelDict)
            {
                if (item.ChannelName == channelName)
                {
                    channel = item;
                    break;
                }
            }
            
            if (!channelDict.TryTake(out channel))
            {
                LogF8.LogError("不存在：" + channelName);
            }
        }
        
        public void RemoveChannel(INetworkChannel channel)
        {
            if (!channelDict.TryTake(out channel))
            {
                LogF8.LogError("不存在：" + channel.ChannelName);
            }
        }
        
        public bool CloseChannel(string channelName)
        {
            foreach (INetworkChannel item in channelDict)
            {
                if (item.ChannelName == channelName)
                {
                    item.Close();
                    return true;
                }
            }

            return false;
        }
        
        public bool CloseChannel(INetworkChannel channel)
        {
            foreach (INetworkChannel item in channelDict)
            {
                if (item == channel)
                {
                    channel.Close();
                    return true;
                }
            }

            return false;
        }

        public INetworkChannel PeekChannel(string channelName)
        {
            foreach (INetworkChannel item in channelDict)
            {
                if (item.ChannelName == channelName)
                {
                    return item;
                }
            }
    
            return null;
        }
        
        public bool HasChannel(string channelName)
        {
            foreach (INetworkChannel item in channelDict)
            {
                if (item.ChannelName == channelName)
                {
                    return true;
                }
            }

            return false;
        }

        public List<INetworkChannel> GetAllChannels()
        {
            List<INetworkChannel> channelList = new List<INetworkChannel>();
    
            foreach (INetworkChannel channel in channelDict)
            {
                channelList.Add(channel);
            }
    
            return channelList;
        }

        public void OnInit(object createParam)
        {
            channelDict = new ConcurrentBag<INetworkChannel>();
        }

        /// <summary>
        /// 开启Update线程
        /// </summary>
        public void StartThread()
        {
#if UNITY_WEBGL
            LogF8.LogError("WebGL不支持.Net多线程");
#endif
            if (netRun == null)
            {
                netRun = new Thread(new ThreadStart(ThreadOnUpdate));
                netRun.Start();
            }
        }   
        
        /// <summary>
        /// Update线程
        /// </summary>
        private void ThreadOnUpdate()
        {
            while (true)
            {
                Update();
                Thread.Sleep(1);
            }
        }
        
        /// <summary>
        /// Update只能利用单个CPU核心
        /// </summary>
        public void OnUpdate()
        {
            if (netRun == null)
            {
                Update();
            }
        }

        private void Update()
        {
            if (channelDict.Count <= 0)
            {
                return;
            }
            // 最后对所有通道进行刷新
            foreach (var channel in channelDict)
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
                channel.Close();
            }

            channelDict.Clear();
        }
    }
}