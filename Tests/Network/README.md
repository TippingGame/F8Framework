# F8 Network

[![license](http://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Unity Version](https://img.shields.io/badge/unity-2021.3.15f1-blue)](https://unity.com) 
[![Platform](https://img.shields.io/badge/platform-Win%20%7C%20Android%20%7C%20iOS%20%7C%20Mac%20%7C%20Linux%20%7C%20WebGL-orange)]() 

## 简介（希望自己点击F8，就能开始制作游戏，不想多余的事）
Unity F8 Network(互联)网络组件。
1. 使用 KCP / TCP / WebSocket 网络通讯协议建立长连接通道，支持Client端和Server端。

## 导入插件（需要首先导入核心）
注意！内置在->F8Framework核心：https://github.com/TippingGame/F8Framework.git  
方式一：直接下载文件，放入Unity  
方式二：Unity->点击菜单栏->Window->Package Manager->点击+号->Add Package from git URL->输入：https://github.com/TippingGame/F8Framework.git  

### 使用方法
#### kcp 或 tcp
* kcp 或 tcp 的 Client 使用示例 [MultiNetworkClient.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/MultiNetworkChannel/MultiNetworkClient.cs)  
```C#
/*----------------------------tcp/kcp客户端使用----------------------------*/
    // 创建tcp通道
    TcpClientChannel tcpClientChannel = new TcpClientChannel("TEST_TCP_CLIENT");
    // 创建kcp通道
    KcpClientChannel kcpClientChannel = new KcpClientChannel("TEST_KCP_CLIENT");
    
    // 设置回调
    tcpClientChannel.OnConnected += TcpClient_OnConnected;
    tcpClientChannel.OnDataReceived += TcpClient_OnDataReceived;
    tcpClientChannel.OnDisconnected += TcpClient_OnDisconnected;
    
    // 可选，开启多线程（注意：WebGL不支持多线程）
    // FF8.Network.StartThread();
    
    // 添加通道
    FF8.Network.AddChannel(tcpClientChannel);
    
    // 连接通道
    tcpClientChannel.Connect("127.0.0.1", 8010);
    
    // 发送数据
    tcpClientChannel.SendMessage(Encoding.UTF8.GetBytes("数据信息"));
    
    // 断开连接
    tcpClientChannel.Disconnect();
    
    // 关闭通道
    tcpClientChannel.Close();

void TcpClient_OnConnected()
{
    LogF8.LogNet($"TCP_CLIENT Connected");
}

void TcpClient_OnDataReceived(byte[] data)
{
    LogF8.LogNet($"TCP_CLIENT receive data: {Encoding.UTF8.GetString(data)}");
}

void TcpClient_OnDisconnected()
{
    LogF8.LogNet($"TCP_CLIENT Disconnected");
}
```
* kcp 或 tcp 的 Server 使用示例 [MultiNetworkServer.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/MultiNetworkChannel/MultiNetworkServer.cs)
```C#
/*----------------------------tcp/kcp服务器使用----------------------------*/
    // 创建tcp通道
    TcpServerChannel tcpServerChannel = new TcpServerChannel("TEST_TCP_SERVER", 8010);
    // 创建kcp通道
    KcpServerChannel kcpServerChannel = new KcpServerChannel("TEST_KCP_SERVER", 8020);
    
    // 设置回调
    tcpServerChannel.OnConnected += TcpServer_OnConnected;
    tcpServerChannel.OnDisconnected += TcpServer_OnDisconnected;
    tcpServerChannel.OnDataReceived += TcpServer_OnDataReceived;
    
    // 可选，开启多线程（注意：WebGL不支持多线程）
    // FF8.Network.StartThread();
    
    // 添加通道
    FF8.Network.AddChannel(tcpServerChannel);
    
    // 开始监听端口
    tcpServerChannel.Start();
    
    // 关闭通道
    tcpServerChannel.Close();

void TcpServer_OnConnected(int conv, string ip)
{
    LogF8.LogNet($"TCP_SERVER conv: {conv} Connected, ip: {ip}");
}

void TcpServer_OnDataReceived(int conv, byte[] data)
{
    LogF8.LogNet($"TCP_SERVER receive data from conv: {conv} . Data: {Encoding.UTF8.GetString(data)}");
}

void TcpServer_OnDisconnected(int conv)
{
    LogF8.LogNet($"TCP_SERVER conv: {conv} Disconnected");
}
```
---
#### websocket
* websocket 的 Client 使用示例 [ExampleWebClient.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/SimpleWebTransport/ExampleWebClient.cs)  
```C#
/*----------------------------websocket客户端使用----------------------------*/
// 参数设置
private string _address = "ws://127.0.0.1:7778";
private int _maxMessageSize = 32000;
private bool _noDelay = true;
private int _sendTimeout = 5000;
private int _receiveTimeout = 5000;
private int _maxMessagePerTick = 500;

// 将连接方案设置为wss，注意：如果sslEnabled为true，则clientUseWss也为true
public bool clientUseWss = false;

private bool echo = false;
private SimpleWebClient client;
private float keepAlive;

// 连接
private void Connect()
{
    TcpConfig tcpConfig = new TcpConfig(_noDelay, _sendTimeout, _receiveTimeout);
    client = SimpleWebClient.Create(_maxMessageSize, _maxMessagePerTick, tcpConfig);

    client.onConnect += () => LogF8.LogNet($"Connected to Server");
    client.onDisconnect += () => LogF8.LogNet($"Disconnected from Server");
    client.onData += OnData;
    client.onError += (exception) => LogF8.LogNet($"Error because of Server, Error:{exception}");

    UriBuilder builder = new UriBuilder(_address)
    {
        Scheme = clientUseWss ? "wss" : "ws"
    };

    client.Connect(builder.Uri);
}

// 更新
private void Update()
{
    client?.ProcessMessageQueue();
    if (keepAlive < Time.time)
    {
        client?.Send(new ArraySegment<byte>(new byte[1] { 0 }));
        keepAlive = Time.time + 1;
    }
}

// 断开连接
private void OnDestroy()
{
    client?.Disconnect();
}

// 接收数据
private void OnData(ArraySegment<byte> data)
{
    LogF8.LogNet($"Data from Server, length:{data.Count}");
    if (echo)
    {
        if (client is WebSocketClientStandAlone standAlone)
            standAlone.Send(data);
        else
            client.Send(data);
    }
}
```
* websocket 的 Server 使用示例 [ExampleWebServer.cs](https://github.com/TippingGame/F8Framework/blob/main/Tests/Network/SimpleWebTransport/ExampleWebServer.cs)  
```C#
/*----------------------------websocket服务器使用----------------------------*/
// 参数设置
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

// 创建服务器
private IEnumerator Start()
{
    TcpConfig tcpConfig = new TcpConfig(_noDelay, _sendTimeout, _receiveTimeout);

    SslConfig sslConfig = SslConfigLoader.Load(sslEnabled, sslCertJson, sslProtocols);
    server = new SimpleWebServer(_maxMessagePerTick, tcpConfig, _maxMessageSize, _maxHandShakeSize, sslConfig);

    server.onConnect += (id, ip) => { connection = true; LogF8.LogNet($"New Client connected, id:{id}, ip:{ip}"); };
    server.onDisconnect += (id) => LogF8.LogNet($"Client disconnected, id:{id}");
    server.onData += OnData;
    server.onError += (id, exception) => LogF8.LogNet($"Error because of Client, id:{id}, Error:{exception}");

    // add events then start
    server.Start(checked((ushort)_port));

    yield return new WaitUntil(() => connection);

    for (int i = 1; i < 200; i++)
    {
        yield return Send(i * 1000);
    }
}

// 更新
private void Update()
{
    server?.ProcessMessageQueue();
}

// 断开连接
private void OnDestroy()
{
    server?.Stop();
}

// 接收数据
private void OnData(int id, ArraySegment<byte> data)
{
    LogF8.LogNet($"Data from Client, id:{id}, length:{data.Count}");

    byte[] received = data.Array;
    int length = data.Count;
    if (length == 1)
        return;
    byte[] bytes = sent[length];

    for (int i = 0; i < length; i++)
    {
        if (bytes[i] != received[i])
            throw new Exception("Data not equal");
    }

    sent.Remove(length);
}

// 发送数据
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

