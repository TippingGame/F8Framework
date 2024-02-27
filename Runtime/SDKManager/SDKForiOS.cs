using System.Runtime.InteropServices;

namespace F8Framework.Core
{
    public class SDKForiOS : SDKBase
    {
#if UNITY_IPHONE || UNITY_IOS
        [DllImport("__Internal")]
        private static extern void SDKInit();

        [DllImport("__Internal")]
        private static extern void SDKLogin();

        [DllImport("__Internal")]
        private static extern void SDKLogout();
        
        [DllImport("__Internal")]
        private static extern void SDKSwitchAccount();
        
        [DllImport("__Internal")]
        private static extern void SDKLoadVideoAd(string id, string userId);
        
        [DllImport("__Internal")]
        private static extern void SDKShowVideoAd(string id, string userId);
        
        [DllImport("__Internal")]
        private static extern void SDKPay(string serverNum, string serverName, string playerId, string playerName,
            string amount, string extra, string orderId,
            string productName, string productContent, string playerLevel, string sign, string guid);

        
        [DllImport("__Internal")]
        private static extern void SDKUpdateRole(string scenes, string serverId, string serverName, string roleId,
            string roleName, string roleLeve, string roleCTime);
        
        [DllImport("__Internal")]
        private static extern void SDKExitGame();
        
        [DllImport("__Internal")]
        private static extern void SDKToast(string msg);
#else
        private void SDKInit() { }
        private void SDKLogin() { }
        private void SDKLogout() { }
        private void SDKSwitchAccount() { }
        private void SDKLoadVideoAd(string id, string userId) { }
        private void SDKShowVideoAd(string id, string userId) { }

        private void SDKPay(string serverNum, string serverName, string playerId, string playerName,
            string amount, string extra, string orderId,
            string productName, string productContent, string playerLevel, string sign, string guid) { }

        private void SDKUpdateRole(string scenes, string serverId, string serverName, string roleId,
            string roleName, string roleLeve, string roleCTime) { }

        private void SDKExitGame() { }
        private void SDKToast(string msg) { }
#endif

        public override void Init()
        {
            SDKInit();
        }

        public override void Login()
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    SDKLogin();
                    break;
            }
        }

        public override void Logout()
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    SDKLogout();
                    break;
            }
        }

        public override void SwitchAccount()
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    SDKSwitchAccount();
                    break;
            }
        }

        public override void LoadVideoAd(string id, string userId)
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    SDKLoadVideoAd(id, userId);
                    break;
            }
        }

        public override void ShowVideoAd(string id, string userId)
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    SDKShowVideoAd(id, userId);
                    break;
            }
        }

        public override void Pay(string serverNum, string serverName, string playerId, string playerName, string amount,
            string extra, string orderId,
            string productName, string productContent, string playerLevel, string sign, string guid)
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    SDKPay(serverNum, serverName, playerId, playerName, amount, extra, orderId, productName,
                        productContent, playerLevel, sign, guid);
                    break;
            }
        }

        public override void UpdateRole(string scenes, string serverId, string serverName, string roleId,
            string roleName,
            string roleLeve, string roleCTime, string rolePower, string guid)
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    SDKUpdateRole(scenes, serverId, serverName, roleId, roleName, roleLeve, roleCTime);
                    break;
            }
        }

        public override void ExitGame()
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    SDKExitGame();
                    break;
            }
        }

        public override void Toast(string msg)
        {
            SDKToast(msg);
        }
    }
}