using System.Text;
using UnityEngine;
using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class MultiNetworkClient : MonoBehaviour
    {
        TcpClientChannel tcpClientChannel;
        KcpClientChannel kcpClientChannel;

        [Header("TCP")] [SerializeField] Button btnTcpConnectC2S;
        [SerializeField] Button btnTcpDisconnectC2S;
        [SerializeField] InputField iptTcp;
        [SerializeField] Button btnTcpSendMessageC2S;
        [Header("KCP")] [SerializeField] Button btnKcpConnectC2S;
        [SerializeField] Button btnKcpDisconnectC2S;
        [SerializeField] InputField iptKcp;
        [SerializeField] Button btnKcpSendMessageC2S;

        void Start()
        {
            tcpClientChannel = new TcpClientChannel("TEST_TCP_CLIENT");
            kcpClientChannel = new KcpClientChannel("TEST_KCP_CLIENT");

            tcpClientChannel.OnConnected += TcpClient_OnConnected;
            tcpClientChannel.OnDataReceived += TcpClient_OnDataReceived;
            tcpClientChannel.OnDisconnected += TcpClient_OnDisconnected;

            kcpClientChannel.OnConnected += KcpClient_OnConnected;
            kcpClientChannel.OnDataReceived += KcpClient_OnDataReceived;
            kcpClientChannel.OnDisconnected += KcpClient_OnDisconnected;

            // 可选
            // FF8.Network.StartThread();
            
            //channel的TickRefresh函数可自定义管理轮询，networkManager的作用是存放通道并调用TickRefresh。
            //由于存在多种网络方案的原因，通道对应的具体事件需要由使用者自定义解析，框架不提供具体数据。
            //这里将client加入networkManager，由networkManager管理通道的轮询
            FF8.Network.AddChannel(tcpClientChannel);
            FF8.Network.AddChannel(kcpClientChannel);
            
            btnTcpConnectC2S?.onClick.AddListener(TcpConnectC2S);
            btnTcpDisconnectC2S?.onClick.AddListener(TcpDisconnectC2S);
            btnTcpSendMessageC2S?.onClick.AddListener(TcpSendMessageC2S);

            btnKcpConnectC2S?.onClick.AddListener(KcpConnectC2S);
            btnKcpDisconnectC2S?.onClick.AddListener(KcpDisconnectC2S);
            btnKcpSendMessageC2S?.onClick.AddListener(KcpSendMessageC2S);
        }

        void OnDestroy()
        {
            tcpClientChannel.Close();
            kcpClientChannel.Close();
        }

        #region TCP_CLIENT

        void TcpConnectC2S()
        {
            tcpClientChannel.Connect(MultiNetworkConstants.LOCALHOSET, MultiNetworkConstants.TCP_PORT);
        }

        void TcpDisconnectC2S()
        {
            tcpClientChannel.Disconnect();
        }

        void TcpSendMessageC2S()
        {
            var msg = iptTcp?.text;
            if (!string.IsNullOrEmpty(msg))
                tcpClientChannel.SendMessage(Encoding.UTF8.GetBytes(msg));
        }

        void TcpClient_OnConnected()
        {
            LogF8.LogNet($"TCP_CLIENT Connected");
        }

        void TcpClient_OnDataReceived(byte[] data)
        {
        }

        void TcpClient_OnDisconnected()
        {
            LogF8.LogNet($"TCP_CLIENT Disconnected");
        }

        #endregion

        #region KCP_CLIENT

        void KcpConnectC2S()
        {
            kcpClientChannel.Connect(MultiNetworkConstants.LOCALHOSET, MultiNetworkConstants.KCP_PORT);
        }

        void KcpDisconnectC2S()
        {
            kcpClientChannel.Close();
        }

        void KcpSendMessageC2S()
        {
            var msg = iptKcp?.text;
            if (!string.IsNullOrEmpty(msg))
                kcpClientChannel.SendMessage(Encoding.UTF8.GetBytes(msg));
        }

        void KcpClient_OnConnected()
        {
            LogF8.LogNet($"KCP_CLIENT Connected");
        }

        void KcpClient_OnDataReceived(byte[] data)
        {
        }

        void KcpClient_OnDisconnected()
        {
            LogF8.LogNet($"KCP_CLIENT Disconnected");
        }

        #endregion
    }
}
