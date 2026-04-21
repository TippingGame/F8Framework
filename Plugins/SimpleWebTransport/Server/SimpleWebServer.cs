using System;
using System.Collections.Generic;

namespace JamesFrowen.SimpleWeb
{
    public class SimpleWebServer
    {
        public event Action<IConnection> onConnect;
        public event Action<IConnection> onDisconnect;
        public event Action<IConnection, ArraySegment<byte>> onData;
        public event Action<IConnection, Exception> onError;

        readonly int maxMessagesPerTick;
        readonly WebSocketServer server;
        readonly BufferPool bufferPool;
        List<IConnection> sendAllConnCache;
        List<int> sendAllIdCache;

        public bool Active { get; private set; }

        public SimpleWebServer(int maxMessagesPerTick, TcpConfig tcpConfig, int maxMessageSize, int handshakeMaxSize, SslConfig sslConfig, int maxSendQueueSize)
        {
            this.maxMessagesPerTick = maxMessagesPerTick;
            // use max because bufferpool is used for both messages and handshake
            int max = Math.Max(maxMessageSize, handshakeMaxSize);
            bufferPool = new BufferPool(5, 20, max);
            server = new WebSocketServer(tcpConfig, maxMessageSize, handshakeMaxSize, sslConfig, bufferPool, maxSendQueueSize);
        }

        public void Start(ushort port)
        {
            server.Listen(port);
            Active = true;
        }

        public void Stop()
        {
            server.Stop();
            Active = false;
        }

        /// <summary>
        /// Sends to a list of connections, use <see cref="List{IConnection}"/> version to avoid foreach allocation
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="source"></param>
        public void SendAll(List<IConnection> connections, ArraySegment<byte> source)
        {
            if (connections.Count == 0)
                return;

            ArrayBuffer buffer = bufferPool.Take(source.Count);
            buffer.CopyFrom(source);
            buffer.SetReleasesRequired(connections.Count);

            foreach (IConnection conn in connections)
                server.Send(conn, buffer);
        }

        /// <summary>
        /// Sends to a list of connections, use <see cref="ICollection{IConnection}"/> version when you are using a non-list collection (will allocate in foreach)
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="source"></param>
        public void SendAll(ICollection<IConnection> connections, ArraySegment<byte> source)
        {
            if (connections.Count == 0)
                return;

            ArrayBuffer buffer = bufferPool.Take(source.Count);
            buffer.CopyFrom(source);
            buffer.SetReleasesRequired(connections.Count);

            foreach (IConnection conn in connections)
                server.Send(conn, buffer);
        }

        /// <summary>
        /// Sends to a list of connections, use <see cref="IEnumerable{IConnection}"/> version in cases where you want to use LINQ to get connections (will allocate from LINQ functions and foreach)
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="source"></param>
        public void SendAll(IEnumerable<IConnection> connections, ArraySegment<byte> source)
        {
            sendAllConnCache ??= new();
            sendAllConnCache.Clear();
            // copy to list incase IEnumerable is unstable (different result each loop)
            sendAllConnCache.AddRange(connections);
            SendAll(sendAllConnCache, source);
        }

        public void SendOne(IConnection conn, ArraySegment<byte> source)
        {
            ArrayBuffer buffer = bufferPool.Take(source.Count);
            buffer.CopyFrom(source);
            server.Send(conn, buffer);
        }

        public bool KickClient(IConnection conn) => server.CloseConnection(conn);

        public void GetClientEndPoint(IConnection conn, out string address, out int port) => server.GetClientEndPoint(conn, out address, out port);

        public string GetClientAddress(IConnection conn) => server.GetClientAddress(conn);

        public Request GetClientRequest(IConnection conn) => server.GetClientRequest(conn);

        /// <summary>
        /// Processes all messages while <paramref name="keepProcessing"/> is null or returns true
        /// </summary>
        /// <param name="behaviour"></param>
        public void ProcessMessageQueue(Func<bool> keepProcessing = null)
        {
            int processedCount = 0;
            // check enabled every time in case behaviour was disabled after data
            while (
                (keepProcessing?.Invoke() ?? true) &&
                processedCount < maxMessagesPerTick &&
                // Dequeue last
                server.receiveQueue.TryDequeue(out Message next)
                )
            {
                processedCount++;

                switch (next.type)
                {
                    case EventType.Connected:
                        onConnect?.Invoke(next.conn);
                        break;
                    case EventType.Data:
                        using (next.data)
                        {
                            onData?.Invoke(next.conn, next.data.ToSegment());
                        }
                        break;
                    case EventType.Disconnected:
                        onDisconnect?.Invoke(next.conn);
                        break;
                    case EventType.Error:
                        onError?.Invoke(next.conn, next.exception);
                        break;
                }
            }

            if (server.receiveQueue.Count > 0)
            {
                Log.Warn($"SimpleWebServer ProcessMessageQueue has {server.receiveQueue.Count} remaining.");
            }
        }

        /// <summary>get IConnection back using id</summary>
        public bool TryGetConnection(int id, out IConnection connection) => server.TryGetConnection(id, out connection);

        #region legacy helper methods
        /// <summary>
        /// Sends to a list of connections, use <see cref="List{int}"/> version to avoid foreach allocation
        /// </summary>
        /// <param name="connectionIds"></param>
        /// <param name="source"></param>
        public void SendAll(List<int> connectionIds, ArraySegment<byte> source)
        {
            if (connectionIds.Count == 0)
                return;

            ArrayBuffer buffer = bufferPool.Take(source.Count);
            buffer.CopyFrom(source);
            buffer.SetReleasesRequired(connectionIds.Count);

            foreach (int id in connectionIds)
                server.Send(id, buffer);
        }
        /// <summary>
        /// Sends to a list of connections, use <see cref="ICollection{int}"/> version when you are using a non-list collection (will allocate in foreach)
        /// </summary>
        /// <param name="connectionIds"></param>
        /// <param name="source"></param>
        public void SendAll(ICollection<int> connectionIds, ArraySegment<byte> source)
        {
            if (connectionIds.Count == 0)
                return;

            ArrayBuffer buffer = bufferPool.Take(source.Count);
            buffer.CopyFrom(source);
            buffer.SetReleasesRequired(connectionIds.Count);

            foreach (int id in connectionIds)
                server.Send(id, buffer);
        }
        /// <summary>
        /// Sends to a list of connections, use <see cref="IEnumerable{int}"/> version in cases where you want to use LINQ to get connections (will allocate from LINQ functions and foreach)
        /// </summary>
        /// <param name="connectionIds"></param>
        /// <param name="source"></param>
        public void SendAll(IEnumerable<int> connectionIds, ArraySegment<byte> source)
        {
            sendAllIdCache ??= new();
            sendAllIdCache.Clear();
            // copy to list incase IEnumerable is unstable (different result each loop)
            sendAllIdCache.AddRange(connectionIds);
            SendAll(sendAllIdCache, source);
        }
        public void SendOne(int connectionId, ArraySegment<byte> source)
        {
            if (TryGetConnection(connectionId, out IConnection conn))
            {
                SendOne(conn, source);
            }
            else
            {
                Log.Warn($"Cant send message to {connectionId} because connection was not found in dictionary. Maybe it disconnected.");
            }
        }
        public bool KickClient(int connectionId)
        {
            if (TryGetConnection(connectionId, out IConnection conn))
            {
                return KickClient(conn);
            }
            else
            {
                Log.Warn($"Failed to kick {connectionId} because id not found");
                return false;
            }
        }
        public void GetClientEndPoint(int connectionId, out string address, out int port)
        {
            if (TryGetConnection(connectionId, out IConnection conn))
            {
                GetClientEndPoint(conn, out address, out port);
            }
            else
            {
                Log.Error($"Cant get address of connection {connectionId} because connection was not found in dictionary");
                address = null;
                port = 0;
            }
        }
        public string GetClientAddress(int connectionId)
        {
            if (TryGetConnection(connectionId, out IConnection conn))
            {
                return GetClientAddress(conn);
            }
            else
            {
                Log.Error($"Cant get address of connection {connectionId} because connection was not found in dictionary");
                return null;
            }
        }
        public Request GetClientRequest(int connectionId)
        {
            if (TryGetConnection(connectionId, out IConnection conn))
            {
                return GetClientRequest(conn);
            }
            else
            {
                Log.Error($"Cant get request of connection {connectionId} because connection was not found in dictionary");
                return null;
            }
        }
        #endregion
    }
}
