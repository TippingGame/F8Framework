using System;
using System.Net;
using kcp2k;

namespace F8Framework.Core
{
    public class KcpServerEndPoint : KcpServer
    {
        public KcpServerEndPoint(Action<int, IPEndPoint> OnConnected, Action<int, ArraySegment<byte>, KcpChannel> OnData, Action<int> OnDisconnected, Action<int, ErrorCode, string> OnError, KcpConfig config) 
            : base(OnConnected, OnData, OnDisconnected, OnError, config)
        {
        }
        public string IPAddress { get { return socket.LocalEndPoint.ToString(); } }
    }
}
