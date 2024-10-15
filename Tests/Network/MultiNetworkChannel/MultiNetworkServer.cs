using System.Text;
using UnityEngine;
using UnityEngine.UI;
using F8Framework.Core;
using F8Framework.Launcher;

namespace F8Framework.Tests
{
    public class MultiNetworkServer : MonoBehaviour
    {
        TcpServerChannel tcpServerChannel;
        KcpServerChannel kcpServerChannel;

        [Header("TCP")] [SerializeField] Button btnTcpStartServer;
        [SerializeField] Button btnTcpStopServer;
        [Header("KCP")] [SerializeField] Button btnKcpStartServer;
        [SerializeField] Button btnKcpStopServer;

        void Start()
        {
            tcpServerChannel = new TcpServerChannel("TEST_TCP_SERVER", MultiNetworkConstants.TCP_PORT);
            kcpServerChannel = new KcpServerChannel("TEST_KCP_SERVER", MultiNetworkConstants.KCP_PORT);

            tcpServerChannel.OnConnected += TcpServer_OnConnected;
            tcpServerChannel.OnDisconnected += TcpServer_OnDisconnected;
            tcpServerChannel.OnDataReceived += TcpServer_OnDataReceived;

            kcpServerChannel.OnConnected += KcpServer_OnConnected;
            kcpServerChannel.OnDataReceived += KcpServer_OnDataReceived;
            kcpServerChannel.OnDisconnected += KcpServer_OnDisconnected;
            
            // 可选
            // FF8.Network.StartThread();
            
            //channel的TickRefresh函数可自定义管理轮询，networkManager的作用是存放通道并调用TickRefresh。
            //由于存在多种网络方案的原因，通道对应的具体事件需要由使用者自定义解析，框架不提供具体数据。
            //这里将server加入networkManager，由networkManager管理通道的轮询
            FF8.Network.AddChannel(kcpServerChannel);
            FF8.Network.AddChannel(tcpServerChannel);

            btnTcpStartServer?.onClick.AddListener(TcpStartServer);
            btnTcpStopServer?.onClick.AddListener(TcpStopServer);

            btnKcpStartServer?.onClick.AddListener(KcpStartServer);
            btnKcpStopServer?.onClick.AddListener(KcpStopServer);
        }
        
        #region TCP_SERVER

        void TcpStartServer()
        {
            tcpServerChannel.Start();
        }

        void TcpStopServer()
        {
            tcpServerChannel.Close();
        }

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

        #endregion

        #region KCP_SERVER

        void KcpStartServer()
        {
            kcpServerChannel.Start();
        }

        void KcpStopServer()
        {
            kcpServerChannel.Close();
        }

        void KcpServer_OnConnected(int conv, string ip)
        {
            LogF8.LogNet($"KCP_SERVER conv: {conv} Connected, ip: {ip}");
        }

        void KcpServer_OnDataReceived(int conv, byte[] data)
        {
            LogF8.LogNet($"KCP_SERVER receive data from conv: {conv} . Data: {Encoding.UTF8.GetString(data)}");
        }

        void KcpServer_OnDisconnected(int conv)
        {
            LogF8.LogNet($"KCP_SERVER conv: {conv} Disconnected");
        }

        #endregion
    }
}
