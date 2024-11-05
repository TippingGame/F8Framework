using System;
using F8Framework.Core;
using Mirror.SimpleWeb;
using UnityEngine;

namespace F8Framework.Tests
{
    public class ExampleWebClient : MonoBehaviour
    {
        [SerializeField] private string _address = "ws://127.0.0.1:7778";
        [SerializeField] private int _maxMessageSize = 32000;

        [SerializeField] private bool _noDelay = true;
        [SerializeField] private int _sendTimeout = 5000;
        [SerializeField] private int _receiveTimeout = 5000;

        [SerializeField] private int _maxMessagePerTick = 500;


        [Header("Ssl Settings")]
        [Tooltip("Sets connect scheme to wss. Useful when client needs to connect using wss when TLS is outside of transport.\nNOTE: if sslEnabled is true clientUseWss is also true")]
        public bool clientUseWss;


        private bool echo;
        private SimpleWebClient client;
        private float keepAlive;

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
        private void Update()
        {
            client?.ProcessMessageQueue();
            if (keepAlive < Time.time)
            {
                client?.Send(new ArraySegment<byte>(new byte[1] { 0 }));
                keepAlive = Time.time + 1;
            }
        }
        private void OnDestroy()
        {
            client?.Disconnect();
        }

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

        private void OnGUI()
        {
            if (GUILayout.Button("Connect", GUILayout.Width(400), GUILayout.Height(100)))
            {
                Connect();
            }

            echo = GUILayout.Toggle(echo, "Echo", GUILayout.Width(200));
        }
    }
}
