using System;
using kcp2k;

namespace F8Framework.Core
{
    //================================================
    /*
    *1、ServerChannel启动后，接收并维护remote进入的连接;
    *
    *2、当有请求进入并成功建立连接时，触发OnConnected，分发参数分别为
    *NetworkChannelKey以及建立连接的conv;
    *
    *3、当请求断开连接，触发OnDisconnected，分发NetworkChannelKey以及
    *断开连接的conv;
    *
    *4、已连接对象发来数据时，触发OnDataReceived，分发NetworkChannelKey
    *以及发送来数据的conv;
    */
    //================================================
    /// <summary>
    /// / KCP服务端通道；
    /// </summary>
    public class KcpServerChannel : INetworkServerChannel
    {
        KcpServerEndPoint server;

        Action<int, string> onConnected;
        Action<int> onDisconnected;
        Action<int, byte[]> onDataReceived;
        Action<int, string> onError;
        public event Action<int, string> OnConnected
        {
            add { onConnected += value; }
            remove { onConnected -= value; }
        }
        public event Action<int> OnDisconnected
        {
            add { onDisconnected += value; }
            remove { onDisconnected -= value; }
        }
        public event Action<int, byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public event Action<int, string> OnError
        {
            add { onError += value; }
            remove { onError -= value; }
        }
        
        public int Port { get; private set; }
        
        public bool Active { get { return server.IsActive(); } }
        
        public string ChannelName { get; set; }
        
        public string Host { get { return server.IPAddress; } }
        
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
        
        public KcpServerChannel(string channelName, ushort port)
        {
            this.ChannelName = channelName;
            Telepathy.Log.Info = (s) => LogF8.LogNet(s);
            Telepathy.Log.Warning = (s) => LogF8.LogWarning(s);
            Telepathy.Log.Error = (s) => LogF8.LogError(s);
            this.Port = port;
            server = new KcpServerEndPoint(
                (connectionId, ipEndPoint) => onConnected?.Invoke(connectionId, ipEndPoint.ToString()),
                OnReceiveDataHandler,
                (connectionId) => onDisconnected?.Invoke(connectionId),
                OnErrorHandler,
                config
            );
        }
        
        public bool Start()
        {
            if (Active)
                return false;
            server.Start((ushort)Port);
            return true;
        }
        
        public void TickRefresh()
        {
            server.Tick();
        }
        
        public bool Disconnect(int connectionId)
        {
            server.Disconnect(connectionId);
            return true;
        }
        
        public bool SendMessage(int connectionId, byte[] data)
        {
            return SendMessage(KcpReliableType.Reliable, connectionId, data);
        }
        public bool SendMessage(KcpReliableType reliableType, int connectionId, byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            var byteType = (byte)reliableType;
            var channelId = (KcpChannel)byteType;
            switch (channelId)
            {
                case KcpChannel.Unreliable:
                    server.Send(connectionId, segment, KcpChannel.Unreliable);
                    break;
                default:
                    server.Send(connectionId, segment, KcpChannel.Reliable);
                    break;
            }
            return true;
        }
        
        public string GetConnectionAddress(int connectionId)
        {
            return server.GetClientEndPoint(connectionId).Address.ToString();
        }
        
        public void Close()
        {
            server.Stop();
        }
        void OnErrorHandler(int connectionId, ErrorCode error, string reason)
        {
            onError?.Invoke(connectionId, $"{error}-{reason}");
        }
        void OnReceiveDataHandler(int conv, ArraySegment<byte> arrSeg, KcpChannel Channel)
        {
            var rcvLen = arrSeg.Count;
            var rcvData = new byte[rcvLen];
            Array.Copy(arrSeg.Array, 1, rcvData, 0, rcvLen);
            onDataReceived?.Invoke(conv, rcvData);
        }
    }
}
