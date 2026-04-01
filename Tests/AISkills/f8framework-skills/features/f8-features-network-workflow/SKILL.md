---
name: f8-features-network-workflow
description: Use when implementing or troubleshooting Network feature workflows — TCP, KCP, WebSocket client and server communication in F8Framework.
---

# Network Feature Workflow

> **⚠️ IMPORTANT**: Before using this feature, you **MUST** formally initialize F8Framework in the launch sequence. Ensure `ModuleCenter.Initialize(this);` has run first, then create the required module, for example `FF8.Network = ModuleCenter.CreateModule<NetworkManager>();`.


## Use this skill when

- The task is about network communication using TCP, KCP, or WebSocket.
- The user needs to create client or server channels.
- Troubleshooting connection, data transfer, or disconnection issues.

## Path resolution

1. Prefer project source at Assets/F8Framework.
2. For usage docs, read: Assets/F8Framework/Tests/Network/README.md

## Sources of truth

- Runtime module: Assets/F8Framework/Runtime/Network
- Test docs: Assets/F8Framework/Tests/Network

## Key classes and interfaces

| Class | Role |
|-------|------|
| `NetworkManager` | Core module. Access via `FF8.Network`. |
| `TcpClientChannel` | TCP client connection. |
| `TcpServerChannel` | TCP server listener. |
| `KcpClientChannel` | KCP client connection (reliable UDP). |
| `KcpServerChannel` | KCP server listener. |
| `SimpleWebClient` | WebSocket client. |
| `SimpleWebServer` | WebSocket server. |

## API quick reference

### TCP/KCP Client
```csharp
TcpClientChannel client = new TcpClientChannel("TCP_CLIENT");
// or KcpClientChannel client = new KcpClientChannel("KCP_CLIENT");

client.OnConnected += () => LogF8.LogNet("Connected");
client.OnDataReceived += (byte[] data) => LogF8.LogNet(Encoding.UTF8.GetString(data));
client.OnDisconnected += () => LogF8.LogNet("Disconnected");

FF8.Network.AddChannel(client);
client.Connect("127.0.0.1", 8010);
client.SendMessage(Encoding.UTF8.GetBytes("Hello"));
client.Disconnect();
client.Close();
```

### TCP/KCP Server
```csharp
TcpServerChannel server = new TcpServerChannel("TCP_SERVER", 8010);

server.OnConnected += (int conv, string ip) => LogF8.LogNet($"Client {conv} connected from {ip}");
server.OnDataReceived += (int conv, byte[] data) => LogF8.LogNet($"Data from {conv}");
server.OnDisconnected += (int conv) => LogF8.LogNet($"Client {conv} disconnected");

FF8.Network.AddChannel(server);
server.Start();
server.Close();
```

### WebSocket Client
```csharp
TcpConfig tcpConfig = new TcpConfig(noDelay: true, sendTimeout: 5000, receiveTimeout: 5000);
SimpleWebClient wsClient = SimpleWebClient.Create(maxMessageSize, maxMessagePerTick, tcpConfig);

wsClient.onConnect += () => { };
wsClient.onDisconnect += () => { };
wsClient.onData += (ArraySegment<byte> data) => { };
wsClient.onError += (Exception e) => { };

wsClient.Connect(new Uri("ws://127.0.0.1:7778")); // or wss://

// Must call in Update:
wsClient.ProcessMessageQueue();

// Send data:
wsClient.Send(new ArraySegment<byte>(bytes));
wsClient.Disconnect();

// Use clientUseWss to toggle ws/wss scheme in UriBuilder
```

### WebSocket Server
```csharp
TcpConfig tcpConfig = new TcpConfig(true, 5000, 5000);
SslConfig sslConfig = SslConfigLoader.Load(sslEnabled, certPath, SslProtocols.Tls12);
SimpleWebServer wsServer = new SimpleWebServer(maxMessagePerTick, tcpConfig, maxMessageSize, maxHandShakeSize, sslConfig);

wsServer.onConnect += (int id, string ip) => { };
wsServer.onDisconnect += (int id) => { };
wsServer.onData += (int id, ArraySegment<byte> data) => { };
wsServer.onError += (int id, Exception e) => { };

wsServer.Start((ushort)port);
wsServer.ProcessMessageQueue(); // In Update
wsServer.SendOne(clientId, segment);
wsServer.Stop();
```

### Optional multi-threading
```csharp
FF8.Network.StartThread(); // Note: WebGL does not support threads
```

## Workflow

1. Choose protocol: TCP (reliable), KCP (reliable UDP, low latency), WebSocket (browser/cross-platform).
2. Create channel with name and port.
3. Register connection callbacks before connecting.
4. Add channel to `FF8.Network`.
5. For WebSocket, call `ProcessMessageQueue()` in Update.
6. Serialize data as `byte[]` for transmission.
7. Handle reconnection logic in `OnDisconnected` callback.

## Common error handling

| Error | Cause | Solution |
|-------|-------|----------|
| Connection refused | Server not running or wrong port | Verify server address and port |
| WebGL thread crash | Using `StartThread()` on WebGL | Don't use multi-threading on WebGL |
| SSL handshake fails | Invalid certificate | Check SSL config and cert.json. Ensure SslProtocols.Tls12 is used. |
| Data mismatch | Serialization error | Ensure client and server use same byte order/format |

## Cross-module dependencies

- None — Network is self-contained.

## Output checklist

- Protocol selected (TCP/KCP/WebSocket).
- Client and/or server channels configured.
- Data serialization format defined.
- Validation status and remaining risks.
