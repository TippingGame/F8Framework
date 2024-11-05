using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using F8Framework.Core;
using Mirror.SimpleWeb;
using UnityEngine;

namespace F8Framework.Tests
{
    public class ExampleWebServer : MonoBehaviour
    {
        [SerializeField] private int _port = 7778;
        [SerializeField] private int _maxMessageSize = 32000;
        [SerializeField] private int _maxHandShakeSize = 5000;

        [SerializeField] private bool _noDelay = true;
        [SerializeField] private int _sendTimeout = 5000;
        [SerializeField] private int _receiveTimeout = 5000;

        [SerializeField] private int _maxMessagePerTick = 5000;

        [Header("Ssl Settings")]
        [SerializeField] private bool sslEnabled;
        [Tooltip("See .cert.example.Json for example")]
        [SerializeField] private string sslCertJson = "./cert.json";
        [SerializeField] private SslProtocols sslProtocols = SslProtocols.Tls12;


        private SimpleWebServer server;
        private bool connection;
        private Dictionary<int, byte[]> sent = new Dictionary<int, byte[]>();

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
        private void Update()
        {
            server?.ProcessMessageQueue();
        }
        private void OnDestroy()
        {
            server?.Stop();
        }

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
    }
}
