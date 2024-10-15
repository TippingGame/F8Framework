using System;
using Telepathy;

namespace F8Framework.Core
{
    public class TcpServerChannel : INetworkServerChannel
    {
        Server server;
        
        public bool Active { get { return server.Active; } }
        Action onAbort;
        public event Action OnAbort
        {
            add { onAbort += value; }
            remove { onAbort -= value; }
        }
        public event Action<int, string> OnConnected
        {
            add { server.OnConnected += value; }
            remove { server.OnConnected -= value; }
        }
        Action<int, byte[]> onDataReceived;
        public event Action<int, byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public event Action<int> OnDisconnected
        {
            add { server.OnDisconnected += value; }
            remove { server.OnDisconnected -= value; }
        }
        
        public string ChannelName { get; set; }
        
        public int Port { get; private set; }
        
        public string Host 
        {
            get { return server.listener.LocalEndpoint.ToString(); } 
        }
        public TcpServerChannel(string channelName, int port)
        {
            this.ChannelName = channelName;
            server = new Server(TcpConstants.MaxMessageSize);
            Telepathy.Log.Info = (s) => LogF8.LogNet(s);
            Telepathy.Log.Warning = (s) => LogF8.LogWarning(s);
            Telepathy.Log.Error = (s) => LogF8.LogError(s);
            this.Port = port;
        }
        
        public bool Start()
        {
            if (server.Start(Port))
            {
                server.OnData = OnReceiveDataHandler;
                return true;
            }
            return false;
        }
        
        public bool Disconnect(int connectionId)
        {
            if (server.Disconnect(connectionId))
            {
                return true;
            }
            return false;
        }
        
        public string GetConnectionAddress(int connectionId)
        {
            return server.GetClientAddress(connectionId);
        }
        
        public bool SendMessage(int connectionId, byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            return server.Send(connectionId, segment);
        }
        
        public void TickRefresh()
        {
            server.Tick(100);
        }
        
        public void Close()
        {
            server.Stop();
            server.OnData = null;
            onDataReceived = null;
            onAbort?.Invoke();
            onAbort = null;
        }
        void OnReceiveDataHandler(int conv, ArraySegment<byte> arrSeg)
        {
            onDataReceived?.Invoke(conv, arrSeg.Array);
        }
    }
}
