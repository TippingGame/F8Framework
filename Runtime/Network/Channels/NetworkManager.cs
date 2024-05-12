using System.Collections.Generic;
using System.Threading;

namespace F8Framework.Core
{
    [UpdateRefresh]
    public sealed class NetworkManager : ModuleSingleton<NetworkManager>, IModule
    {
        List<INetworkChannel> channelDict;
        List<INetworkChannel> channelDictRemove;
        private Thread netRun;
        
        public int NetworkChannelCount => channelDict.Count;

        public void AddChannel(INetworkChannel channel)
        {
            channelDict.Add(channel);
        }

        public void RemoveChannel(string channelName)
        {
            for (int i = 0; i < channelDict.Count; i++)
            {
                if (channelDict[i].ChannelName == channelName)
                {
                    channelDictRemove.Add(channelDict[i]);
                    return;
                }
            }
            LogF8.LogError("不存在：" + channelName);
        }
        
        public void RemoveChannel(INetworkChannel channel)
        {
            for (int i = 0; i < channelDict.Count; i++)
            {
                if (channelDict[i] == channel)
                {
                    channelDictRemove.Add(channelDict[i]);
                    return;
                }
            }
            LogF8.LogError("不存在：" + channel.ChannelName);
        }
        
        public bool CloseChannel(string channelName)
        {
            for (int i = 0; i < channelDict.Count; i++)
            {
                if (channelDict[i].ChannelName == channelName)
                {
                    channelDict[i].Close();
                    return true;
                }
            }
            
            return false;
        }
        
        public bool CloseChannel(INetworkChannel channel)
        {
            for (int i = 0; i < channelDict.Count; i++)
            {
                if (channelDict[i] == channel)
                {
                    channelDict[i].Close();
                    return true;
                }
            }
            
            return false;
        }

        public INetworkChannel PeekChannel(string channelName)
        {
            for (int i = 0; i < channelDict.Count; i++)
            {
                if (channelDict[i].ChannelName == channelName)
                {
                    return channelDict[i];
                }
            }
            
            return null;
        }
        
        public bool HasChannel(string channelName)
        {
            for (int i = 0; i < channelDict.Count; i++)
            {
                if (channelDict[i].ChannelName == channelName)
                {
                    return true;
                }
            }
            
            return false;
        }

        public List<INetworkChannel> GetAllChannels()
        {
            return channelDict;
        }

        public void OnInit(object createParam)
        {
            channelDict = new List<INetworkChannel>();
            channelDictRemove = new List<INetworkChannel>();
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
            for (int i = 0; i < channelDictRemove.Count; i++)
            {
                channelDict.Remove(channelDictRemove[i]);
            }
            channelDictRemove.Clear();
            
            for (int i = 0; i < channelDict.Count; i++)
            {
                channelDict[i].TickRefresh();
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
            for (int i = 0; i < channelDict.Count; i++)
            {
                channelDict[i].Close();
            }

            channelDict.Clear();
        }
    }
}