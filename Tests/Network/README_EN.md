# F8 Network

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/unity-2021|2022|2023|6000-blue)](https://unity.com)
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]()

## Introduction (Simply press F8 to start game development without distractions)
**Unity F8 Network Component**  
Multi-protocol Networking Solution for Client-Server Communication
1. Multi-protocol Support:
    * KCP Protocol: High-speed reliable UDP with congestion control
    * TCP Protocol: Traditional reliable connection
    * WebSocket: Web-compatible communication
2. Dual-mode Operation:
   * Full Client implementation
   * Complete Server implementation
   * Shared codebase for both endpoints
3. Connection Management:
   * Persistent long-lived connections
   * Automatic reconnection
   * Connection state monitoring

## Plugin Installation (Requires Core Framework First)
Note! Built into → F8Framework Core: https://github.com/TippingGame/F8Framework.git  
Method 1: Download files directly and import to Unity  
Method 2: Unity → Menu Bar → Window → Package Manager → "+" → Add Package from git URL → Enter: https://github.com/TippingGame/F8Framework.git

### Usage Guide
#### KCP/TCP Protocol Usage
* Client Implementation [MultiNetworkClient.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/MultiNetworkChannel/MultiNetworkClient.cs)
```C#
/*----------------------------TCP/KCP Client Usage----------------------------*/
// Create TCP channel
TcpClientChannel tcpClientChannel = new TcpClientChannel("TEST_TCP_CLIENT");
// Create KCP channel 
KcpClientChannel kcpClientChannel = new KcpClientChannel("TEST_KCP_CLIENT");

// Set callbacks
tcpClientChannel.OnConnected += TcpClient_OnConnected;
tcpClientChannel.OnDataReceived += TcpClient_OnDataReceived;
tcpClientChannel.OnDisconnected += TcpClient_OnDisconnected;

// Optional: Enable multithreading (Note: Not supported on WebGL)
// FF8.Network.StartThread();

// Add channel
FF8.Network.AddChannel(tcpClientChannel);

// Connect to server
tcpClientChannel.Connect("127.0.0.1", 8010);

// Send data
tcpClientChannel.SendMessage(Encoding.UTF8.GetBytes("Test Message"));

// Disconnect
tcpClientChannel.Disconnect();

// Close channel
tcpClientChannel.Close();

void TcpClient_OnConnected()
{
    LogF8.LogNet($"TCP_CLIENT Connected");
}

void TcpClient_OnDataReceived(byte[] data)
{
    LogF8.LogNet($"TCP_CLIENT received data: {Encoding.UTF8.GetString(data)}");
}

void TcpClient_OnDisconnected()
{
    LogF8.LogNet($"TCP_CLIENT Disconnected");
}
```
* Server Implementation [MultiNetworkServer.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/MultiNetworkChannel/MultiNetworkServer.cs)
```C#
/*----------------------------TCP/KCP Server Usage----------------------------*/
// Create TCP channel
TcpServerChannel tcpServerChannel = new TcpServerChannel("TEST_TCP_SERVER", 8010);
// Create KCP channel
KcpServerChannel kcpServerChannel = new KcpServerChannel("TEST_KCP_SERVER", 8020);

// Set callbacks
tcpServerChannel.OnConnected += TcpServer_OnConnected;
tcpServerChannel.OnDisconnected += TcpServer_OnDisconnected;
tcpServerChannel.OnDataReceived += TcpServer_OnDataReceived;

// Optional: Enable multithreading (Note: Not supported on WebGL)
// FF8.Network.StartThread();

// Add channel
FF8.Network.AddChannel(tcpServerChannel);

// Start listening
tcpServerChannel.Start();

// Close channel
tcpServerChannel.Close();

void TcpServer_OnConnected(int conv, string ip)
{
    LogF8.LogNet($"TCP_SERVER connection: {conv} Connected, IP: {ip}");
}

void TcpServer_OnDataReceived(int conv, byte[] data)
{
    LogF8.LogNet($"TCP_SERVER received data from connection: {conv}. Data: {Encoding.UTF8.GetString(data)}");
}

void TcpServer_OnDisconnected(int conv)
{
    LogF8.LogNet($"TCP_SERVER connection: {conv} Disconnected");
}
```
---
#### WebSocket Protocol Usage
* Client Implementation [ExampleWebClient.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/SimpleWebTransport/ExampleWebClient.cs)
```C#
/*----------------------------WebSocket Client Usage----------------------------*/
// Configuration parameters
private string _address = "ws://127.0.0.1:7778";
private int _maxMessageSize = 32000;
private bool _noDelay = true;
private int _sendTimeout = 5000;
private int _receiveTimeout = 5000;
private int _maxMessagePerTick = 500;

// Set connection scheme to wss (if sslEnabled is true, clientUseWss should also be true)
public bool clientUseWss = false;

private bool echo = false;
private SimpleWebClient client;
private float keepAlive;

// Connect to server
private void Connect()
{
    TcpConfig tcpConfig = new TcpConfig(_noDelay, _sendTimeout, _receiveTimeout);
    client = SimpleWebClient.Create(_maxMessageSize, _maxMessagePerTick, tcpConfig);

    client.onConnect += () => LogF8.LogNet($"Connected to Server");
    client.onDisconnect += () => LogF8.LogNet($"Disconnected from Server");
    client.onData += OnData;
    client.onError += (exception) => LogF8.LogNet($"Server Error: {exception}");

    UriBuilder builder = new UriBuilder(_address)
    {
        Scheme = clientUseWss ? "wss" : "ws"
    };

    client.Connect(builder.Uri);
}

// Update loop
private void Update()
{
    client?.ProcessMessageQueue();
    if (keepAlive < Time.time)
    {
        client?.Send(new ArraySegment<byte>(new byte[1] { 0 }));
        keepAlive = Time.time + 1;
    }
}

// Cleanup
private void OnDestroy()
{
    client?.Disconnect();
}

// Handle received data
private void OnData(ArraySegment<byte> data)
{
    LogF8.LogNet($"Received data from Server, length:{data.Count}");
    if (echo)
    {
        if (client is WebSocketClientStandAlone standAlone)
            standAlone.Send(data);
        else
            client.Send(data);
    }
}
```
* Server Implementation [ExampleWebServer.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/SimpleWebTransport/ExampleWebServer.cs)
```C#
/*----------------------------WebSocket Server Usage----------------------------*/
// Configuration parameters
private int _port = 7778;
private int _maxMessageSize = 32000;
private int _maxHandShakeSize = 5000;
private bool _noDelay = true;
private int _sendTimeout = 5000;
private int _receiveTimeout = 5000;
private int _maxMessagePerTick = 5000;

private bool sslEnabled;
private string sslCertJson = "./cert.json";
private SslProtocols sslProtocols = SslProtocols.Tls12;

private SimpleWebServer server;
private bool connection;
private Dictionary<int, byte[]> sent = new Dictionary<int, byte[]>();

// Server initialization
private IEnumerator Start()
{
    TcpConfig tcpConfig = new TcpConfig(_noDelay, _sendTimeout, _receiveTimeout);

    SslConfig sslConfig = SslConfigLoader.Load(sslEnabled, sslCertJson, sslProtocols);
    server = new SimpleWebServer(_maxMessagePerTick, tcpConfig, _maxMessageSize, _maxHandShakeSize, sslConfig);

    server.onConnect += (id, ip) => { connection = true; LogF8.LogNet($"New Client connected, id:{id}, ip:{ip}"); };
    server.onDisconnect += (id) => LogF8.LogNet($"Client disconnected, id:{id}");
    server.onData += OnData;
    server.onError += (id, exception) => LogF8.LogNet($"Client Error, id:{id}, Error:{exception}");

    // Start server
    server.Start(checked((ushort)_port));

    yield return new WaitUntil(() => connection);

    for (int i = 1; i < 200; i++)
    {
        yield return Send(i * 1000);
    }
}

// Update loop
private void Update()
{
    server?.ProcessMessageQueue();
}

// Cleanup
private void OnDestroy()
{
    server?.Stop();
}

// Handle received data
private void OnData(int id, ArraySegment<byte> data)
{
    LogF8.LogNet($"Received data from Client, id:{id}, length:{data.Count}");

    byte[] received = data.Array;
    int length = data.Count;
    if (length == 1)
        return;
    byte[] bytes = sent[length];

    for (int i = 0; i < length; i++)
    {
        if (bytes[i] != received[i])
            throw new Exception("Data mismatch");
    }

    sent.Remove(length);
}

// Send data to client
private IEnumerator Send(int size)
{
    byte[] bytes = new byte[size];
    System.Random random = new System.Random();
    random.NextBytes(bytes);

    ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
    sent.Add(size, bytes);
    server.SendOne(1, segment);

    yield return new WaitForSeconds(0.5f);
}
```

