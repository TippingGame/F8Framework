using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace JamesFrowen.SimpleWeb
{
    public class WebSocketServer
    {
        public readonly ConcurrentQueue<Message> receiveQueue = new ConcurrentQueue<Message>();

        readonly TcpConfig tcpConfig;
        readonly int maxMessageSize;
        readonly int maxSendQueueSize;

        TcpListener listener;
        Thread acceptThread;
        bool serverStopped;
        readonly ServerHandshake handShake;
        readonly ServerSslHelper sslHelper;
        readonly BufferPool bufferPool;
        readonly ConcurrentDictionary<int, Connection> connections = new ConcurrentDictionary<int, Connection>();

        int _idCounter = 0;

        public WebSocketServer(TcpConfig tcpConfig, int maxMessageSize, int handshakeMaxSize, SslConfig sslConfig, BufferPool bufferPool, int maxSendQueueSize)
        {
            this.tcpConfig = tcpConfig;
            this.maxMessageSize = maxMessageSize;
            sslHelper = new ServerSslHelper(sslConfig);
            this.bufferPool = bufferPool;
            handShake = new ServerHandshake(this.bufferPool, handshakeMaxSize);
            if (maxSendQueueSize <= 0)
                throw new ArgumentException($"maxSendQueueSize must be positive value", nameof(maxSendQueueSize));
            this.maxSendQueueSize = maxSendQueueSize;
        }

        public void Listen(int port)
        {
            listener = TcpListener.Create(port);
            listener.Start();

            Log.Info($"Server has started on port {port}");

            acceptThread = new Thread(acceptLoop);
            acceptThread.IsBackground = true;
            acceptThread.Start();
        }

        public void Stop()
        {
            serverStopped = true;

            // Interrupt then stop so that Exception is handled correctly
            acceptThread?.Interrupt();
            listener?.Stop();
            acceptThread = null;

            Log.Info("Server stopped, Closing all connections...");
            // make copy so that foreach doesn't break if values are removed
            Connection[] connectionsCopy = connections.Values.ToArray();
            foreach (Connection conn in connectionsCopy)
                conn.Dispose();

            connections.Clear();
        }

        void acceptLoop()
        {
            try
            {
                try
                {
                    while (true)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        tcpConfig.ApplyTo(client);

                        // TODO keep track of connections before they are in connections dictionary
                        //      this might not be a problem as HandshakeAndReceiveLoop checks for stop
                        //      and returns/disposes before sending message to queue
                        Connection conn = new Connection(client, AfterConnectionDisposed, SendQueueFull, maxSendQueueSize);
                        Log.Info($"A client connected {conn}");

                        // handshake needs its own thread as it needs to wait for message from client
                        Thread receiveThread = new Thread(() => HandshakeAndReceiveLoop(conn));

                        conn.receiveThread = receiveThread;

                        receiveThread.IsBackground = true;
                        receiveThread.Start();
                    }
                }
                catch (SocketException)
                {
                    // check for Interrupted/Abort
                    Utils.CheckForInterupt();
                    throw;
                }
            }
            catch (ThreadInterruptedException e) { Log.InfoException(e); }
            catch (ThreadAbortException e) { Log.InfoException(e); }
            catch (Exception e) { Log.Exception(e); }
        }

        void HandshakeAndReceiveLoop(Connection conn)
        {
            try
            {
                bool success = sslHelper.TryCreateStream(conn);
                if (!success)
                {
                    Log.Error($"Failed to create SSL Stream {conn}");
                    conn.Dispose();
                    return;
                }

                success = handShake.TryHandshake(conn);

                if (success)
                {
                    Log.Info($"Sent Handshake {conn}");
                }
                else
                {
                    Log.Error($"Handshake Failed {conn}");
                    conn.Dispose();
                    return;
                }

                // check if Stop has been called since accepting this client
                if (serverStopped)
                {
                    Log.Info("Server stops after successful handshake");
                    return;
                }

                conn.connId = Interlocked.Increment(ref _idCounter);
                connections.TryAdd(conn.connId, conn);

                receiveQueue.Enqueue(new Message(conn, EventType.Connected));

                Thread sendThread = new Thread(() =>
                {
                    SendLoop.Config sendConfig = new SendLoop.Config(
                        conn,
                        bufferSize: Constants.HeaderSize + maxMessageSize,
                        setMask: false);

                    SendLoop.Loop(sendConfig);
                });

                conn.sendThread = sendThread;
                sendThread.IsBackground = true;
                sendThread.Name = $"SendLoop {conn.connId}";
                sendThread.Start();

                ReceiveLoop.Config receiveConfig = new ReceiveLoop.Config(
                    conn,
                    maxMessageSize,
                    expectMask: true,
                    receiveQueue,
                    bufferPool);

                ReceiveLoop.Loop(receiveConfig);
            }
            catch (ThreadInterruptedException e) { Log.InfoException(e); }
            catch (ThreadAbortException e) { Log.InfoException(e); }
            catch (Exception e) { Log.Exception(e); }
            finally
            {
                // close here in case connect fails
                conn.Dispose();
            }
        }

        void AfterConnectionDisposed(Connection conn)
        {
            if (conn.connId != Connection.IdNotSet)
            {
                receiveQueue.Enqueue(new Message(conn, EventType.Disconnected));
                connections.TryRemove(conn.connId, out Connection _);
            }
        }

        void SendQueueFull(Connection conn)
        {
            receiveQueue.Enqueue(new Message(conn, new Exception("Send Queue Full")));
        }

        public void Send(IConnection connection, ArrayBuffer buffer)
        {
            Connection conn = (Connection)connection;
            conn.QueueSend(buffer);
        }

        public bool CloseConnection(IConnection connection)
        {
            Connection conn = (Connection)connection;

            Log.Info($"Kicking connection {conn.connId}");
            conn.Dispose();
            return true;
        }

        public void GetClientEndPoint(IConnection connection, out string address, out int port)
        {
            Connection conn = (Connection)connection;
            address = conn.remoteAddress;
            port = conn.remotePort;
        }

        public string GetClientAddress(IConnection connection)
        {
            GetClientEndPoint(connection, out string address, out _);
            return address;
        }

        public Request GetClientRequest(IConnection connection)
        {
            Connection conn = (Connection)connection;
            return conn.request;
        }

        /// <summary>get IConnection back using id</summary>
        public bool TryGetConnection(int id, out IConnection connection)
        {
            bool found = connections.TryGetValue(id, out Connection conn);
            connection = conn;
            return found;
        }

        #region legacy helper methods
        public void Send(int id, ArrayBuffer buffer)
        {
            if (TryGetConnection(id, out IConnection conn))
            {
                Send(conn, buffer);
            }
            else
            {
                Log.Warn($"Cant send message to {id} because connection was not found in dictionary. Maybe it disconnected.");
                buffer.Release();
            }
        }
        public bool CloseConnection(int id)
        {
            if (TryGetConnection(id, out IConnection conn))
            {
                return CloseConnection(conn);
            }
            else
            {
                Log.Warn($"Failed to kick {id} because id not found");

                return false;
            }
        }
        public void GetClientEndPoint(int id, out string address, out int port)
        {
            if (TryGetConnection(id, out IConnection conn))
            {
                GetClientEndPoint(conn, out address, out port);
            }
            else
            {
                Log.Error($"Cant get address of connection {id} because connection was not found in dictionary");
                address = null;
                port = 0;
            }
        }
        public string GetClientAddress(int id)
        {
            GetClientEndPoint(id, out string address, out _);
            return address;
        }
        public Request GetClientRequest(int id)
        {
            if (TryGetConnection(id, out IConnection conn))
            {
                return GetClientRequest(conn);
            }
            else
            {
                Log.Error($"Cant get request of connection {id} because connection was not found in dictionary");
                return null;
            }
        }
        #endregion
    }
}
