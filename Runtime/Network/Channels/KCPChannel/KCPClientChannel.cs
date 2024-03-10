using System;
using kcp2k;

namespace F8Framework.Core
{
    //================================================
    /*
    * 1、ChlientChannel启动后，维护并保持与远程服务器的连接。
    * 
    *2、主动连接remote超过20000ms未响应时，触发超时事件被，结束连接并
    *触发onDisconnected，返回参数NetworkChannelKey以及 -1；
    *
    *3、连接成功，触发onConnected并返回参数NetworkChannelKey以及-1；
    *
    *4、从remote接收数据，触发onReceiveData，返回byte[] 数组，-1，以及
    *NetworkChannelKey；
    *
    *5、发送消息到remote，需要通过调用SendMessage方法。
    */
    //================================================
    /// <summary>
    /// KCP客户端通道；
    /// </summary>
    public class KcpClientChannel : INetworkClientChannel
    {
        
        public string ChannelName { get; set; }

        KcpClient client;

        Action onConnected;
        Action onDisconnected;
        Action<byte[]> onDataReceived;
        Action<string> onError;
        public event Action OnConnected
        {
            add { onConnected += value; }
            remove { onConnected -= value; }
        }
        public event Action OnDisconnected
        {
            add { onDisconnected += value; }
            remove { onDisconnected -= value; }
        }
        public event Action<byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public event Action<string> OnError
        {
            add { onError += value; }
            remove { onError -= value; }
        }
        
        public bool IsConnect { get { return client.connected; } }
        
        public int Port { get; private set; }
        
        public string Host { get; private set; }
        
        protected KcpConfig config = new KcpConfig(
            // force NoDelay and minimum interval.
            // this way UpdateSeveralTimes() doesn't need to wait very long and
            // tests run a lot faster.
            NoDelay: true,
            // not all platforms support DualMode.
            // run tests without it so they work on all platforms.
            DualMode: false,
            Interval: 1, // 1ms so at interval code at least runs.
            Timeout: 2000,

            // large window sizes so large messages are flushed with very few
            // update calls. otherwise tests take too long.
            SendWindowSize: Kcp.WND_SND * 1000,
            ReceiveWindowSize: Kcp.WND_RCV * 1000,

            // congestion window _heavily_ restricts send/recv window sizes
            // sending a max sized message would require thousands of updates.
            CongestionWindow: false,

            // maximum retransmit attempts until dead_link detected
            // default * 2 to check if configuration works
            MaxRetransmits: Kcp.DEADLINK * 2
        );
        
        public KcpClientChannel(string channelName)
        {
            this.ChannelName = channelName;
            Telepathy.Log.Info = (s) => LogF8.LogNet(s);
            Telepathy.Log.Warning = (s) => LogF8.LogWarning(s);
            Telepathy.Log.Error = (s) => LogF8.LogError(s);
            client = new KcpClient(
                OnConnectHandler,
                OnReceiveDataHandler,
                OnDisconnectHandler,
                OnErrorHandler,
                config
            );
        }
        
        public void Connect(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            client.Connect(Host, (ushort)port);
        }
        
        public void TickRefresh()
        {
            client?.Tick();
        }
        
        public bool SendMessage(byte[] data)
        {
            return SendMessage(KcpReliableType.Reliable, data);
        }
        /// <summary>
        ///发送消息到remote;
        /// </summary>
        /// <param name="reliableType">消息可靠类型</param>
        /// <param name="data">数据</param>
        public bool SendMessage(KcpReliableType reliableType, byte[] data)
        {
            if (!IsConnect)
                return false;
            var arraySegment = new ArraySegment<byte>(data);
            var byteType = (byte)reliableType;
            var channelId = (KcpChannel)byteType;
            switch (channelId)
            {
                case KcpChannel.Unreliable:
                    client.Send(arraySegment, KcpChannel.Unreliable);
                    break;
                default:
                    client.Send(arraySegment, KcpChannel.Reliable);
                    break;
            }
            return true;
        }
        
        public void Close()
        {
            client.Disconnect();
        }
        void OnDisconnectHandler()
        {
            onDisconnected?.Invoke();
            onConnected = null;
            onDisconnected = null;
            onDataReceived = null;
        }
        void OnConnectHandler()
        {
            onConnected?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg, KcpChannel channel)
        {
            var rcvLen = arrSeg.Count;
            var rcvData = new byte[rcvLen];
            Array.Copy(arrSeg.Array, arrSeg.Offset, rcvData, 0, rcvLen);
            onDataReceived?.Invoke(rcvData);
        }
        void OnErrorHandler(ErrorCode error, string reason)
        {
            onError?.Invoke($"{error}-{reason}");
        }
    }
}
