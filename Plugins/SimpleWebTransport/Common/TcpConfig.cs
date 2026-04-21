using System.Net.Sockets;
using UnityEngine;

namespace JamesFrowen.SimpleWeb
{
    [System.Serializable]
    public struct TcpConfig
    {
        public readonly bool noDelay;
        [Tooltip("in milliseconds, (0 means no timeout)")]
        public readonly int sendTimeout;
        [Tooltip("in milliseconds, (0 means no timeout)")]
        public readonly int receiveTimeout;

        public TcpConfig(bool noDelay, int sendTimeout, int receiveTimeout)
        {
            this.noDelay = noDelay;
            this.sendTimeout = sendTimeout;
            this.receiveTimeout = receiveTimeout;
        }

        public void ApplyTo(TcpClient client)
        {
            client.SendTimeout = sendTimeout;
            client.ReceiveTimeout = receiveTimeout;
            client.NoDelay = noDelay;
        }
    }
}
