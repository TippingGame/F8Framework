using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JamesFrowen.SimpleWeb
{
    /// <summary>
    /// Represents a handle to a remote connection.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// An opaque application-defined context associated with this connection.
        /// <para>Use this to store references to high-level objects like a Player or Session.</para>
        /// <remarks>This value is typically cleared by the transport when the connection is disconnected.</remarks>
        /// </summary>
        object Context { get; set; }

        /// <summary>
        /// The unique underlying identifier for this connection.
        /// </summary>
        int Id { get; }
    }

    sealed class Connection : IDisposable, IConnection
    {
        public const int IdNotSet = -1;
        readonly object disposedLock = new object();

        public object Context { get; set; }
        public int Id => connId;

        public TcpClient client;

        public int connId = IdNotSet;

        /// <summary>
        /// Connect request, sent from client to start handshake
        /// <para>Only valid on server</para>
        /// </summary>
        public Request request;
        /// <summary>
        /// RemoteEndpoint address or address from request header
        /// <para>Only valid on server</para>
        /// </summary>
        public string remoteAddress;
        public int remotePort;

        public Stream stream;
        public Thread receiveThread;
        public Thread sendThread;

        ManualResetEventSlim sendPending = new ManualResetEventSlim(false);
        ConcurrentQueue<ArrayBuffer> sendQueue = new ConcurrentQueue<ArrayBuffer>();
        Action<Connection> onSendQueueFull;
        readonly int maxSendQueueSize;
        public bool needsPong;

        Action<Connection> onDispose;
        volatile bool hasDisposed;

        public Connection(TcpClient client, Action<Connection> onDispose, Action<Connection> onSendQueueFull, int maxSendQueueSize)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.onDispose = onDispose;
            this.onSendQueueFull = onSendQueueFull;
            this.maxSendQueueSize = maxSendQueueSize;
        }

        public void SetNeedsPong()
        {
            // Set flag to send pong response
            needsPong = true;
            sendPending.Set();
        }

        public void QueueSend(ArrayBuffer buffer)
        {
            // note: need to check disposedLock, so we done Enqueue while Dispose is running
            //       Dispose will empty and release buffers in sendQueue
            //       we want to make sure we do no queue after that
            bool queueFull = false;
            lock (disposedLock)
            {
                if (hasDisposed)
                {
                    Log.Warn($"Message sent to id={connId} after it was been disposed");
                    buffer.Release();
                }
                else if (sendQueue.Count >= maxSendQueueSize)
                {
                    queueFull = true;
                    buffer.Release();
                }
                else
                {
                    sendQueue.Enqueue(buffer);
                    sendPending.Set();
                }
            }

            if (queueFull)
            {
                Log.Warn($"Send queue was over {maxSendQueueSize} for {ToString()}, kicking connection.");
                onSendQueueFull?.Invoke(this);
                Dispose();
            }
        }

        public (ManualResetEventSlim sendPending, ConcurrentQueue<ArrayBuffer> sendQueue) GetSendQueue()
        {
            return (sendPending, sendQueue);
        }

        /// <summary>
        /// disposes client and stops threads
        /// </summary>
        public void Dispose()
        {
            Log.Verbose($"Dispose {ToString()}");

            // check hasDisposed first to stop ThreadInterruptedException on lock
            if (hasDisposed) return;

            Log.Info($"Connection Close: {ToString()}");

            lock (disposedLock)
            {
                // check hasDisposed again inside lock to make sure no other object has called this
                if (hasDisposed) return;

                hasDisposed = true;

                // stop threads first so they don't try to use disposed objects
                receiveThread.Interrupt();
                sendThread?.Interrupt();

                try
                {
                    // stream 
                    stream?.Dispose();
                    stream = null;
                    client.Dispose();
                    client = null;
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }

                sendPending.Dispose();

                // release all buffers in send queue
                while (sendQueue.TryDequeue(out ArrayBuffer buffer))
                    buffer.Release();

                onDispose.Invoke(this);
            }
        }

        public override string ToString()
        {
            if (hasDisposed)
                return $"[Conn:{connId}, Disposed]";
            else
                try
                {
                    System.Net.EndPoint endpoint = client?.Client?.RemoteEndPoint;
                    return $"[Conn:{connId}, endPoint:{endpoint}]";
                }
                catch (SocketException)
                {
                    return $"[Conn:{connId}, endPoint:n/a]";
                }
        }

        /// <summary>
        /// Gets the address based on the <see cref="request"/> and RemoteEndPoint
        /// <para>Called after ServerHandShake is accepted</para>
        /// </summary>
        internal void CalculateEndPoint(out string address, out int port)
        {
            if (request.Headers.TryGetValue("X-Forwarded-For", out string forwardFor))
            {
                string actualClientIP = forwardFor.ToString().Split(',').First();
                // Remove the port number from the address
                address = actualClientIP.Split(':').First();
                port = 0;
            }
            else
            {
                IPEndPoint ipEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                IPAddress ipAddress = ipEndPoint.Address;
                if (ipAddress.IsIPv4MappedToIPv6)
                    ipAddress = ipAddress.MapToIPv4();

                address = ipAddress.ToString();
                port = ipEndPoint.Port;
            }
        }
    }
}
