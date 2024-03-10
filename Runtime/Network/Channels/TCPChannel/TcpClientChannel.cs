using System;
using Telepathy;
namespace F8Framework.Core
{
    public class TcpClientChannel : INetworkClientChannel
    {
        Client client;
        Action onAbort;
        ///<inheritdoc/>
        public string ChannelName { get; set; }
        ///<inheritdoc/>
        public bool IsConnect { get { return client.Connected; } }
        public event Action OnAbort
        {
            add { onAbort += value; }
            remove { onAbort -= value; }
        }
        public event Action OnConnected
        {
            add { client.OnConnected += value; }
            remove { client.OnConnected -= value; }
        }
        event Action<byte[]> onDataReceived;
        public event Action<byte[]> OnDataReceived
        {
            add { onDataReceived += value; }
            remove { onDataReceived -= value; }
        }
        public event Action OnDisconnected
        {
            add { client.OnDisconnected += value; }
            remove { client.OnDisconnected -= value; }
        }
        ///<inheritdoc/>
        public int Port { get; private set; }
        ///<inheritdoc/>
        public string Host { get; private set; }
        public TcpClientChannel(string channelName)
        {
            this.ChannelName = channelName;
            client = new Client(TcpConstants.MaxMessageSize);
            Telepathy.Log.Info = (s) => LogF8.LogNet(s);
            Telepathy.Log.Warning = (s) => LogF8.LogWarning(s);
            Telepathy.Log.Error = (s) => LogF8.LogError(s);
        }
        ///<inheritdoc/>
        public void Connect(string host, int port)
        {
            this.Host = host;
            this.Port = port;
            client.Connect(Host, Port);
            client.OnData = OnReceiveDataHandler;
        }
        ///<inheritdoc/>
        public void TickRefresh()
        {
            client.Tick(100);
        }
        ///<inheritdoc/>
        public bool SendMessage(byte[] data)
        {
            var segment = new ArraySegment<byte>(data);
            return client.Send(segment);
        }
        ///<inheritdoc/>
        public void Disconnect()
        {
            client.Disconnect();
            client.OnData = null;
            onDataReceived = null;
        }
        ///<inheritdoc/>
        public void Close()
        {
            Disconnect();
            onAbort?.Invoke();
        }
        void OnReceiveDataHandler(ArraySegment<byte> arrSeg)
        {
            onDataReceived?.Invoke(arrSeg.Array);
        }
    }
}
