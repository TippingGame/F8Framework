using UnityEngine;

namespace F8Framework.Core
{
    public class SDKForAndroid : SDKBase
    {
        private AndroidJavaClass androidJavaClass;
        private AndroidJavaObject androidJavaObject;

        public SDKForAndroid() : base()
        {
            // 默认安卓类
            androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");

            // 你也可以指定一个java类
            // androidJavaObject = new AndroidJavaObject("com.TippingGame.F8Framework.UnityCallJava");
        }

        private void Call(string funcName, params object[] args)
        {
            try
            {
                if (androidJavaObject != null)
                    androidJavaObject.Call(funcName, args);
            }
            catch (System.Exception e)
            {
                LogF8.LogError(e);
            }
        }

        private string CallReturn(string funcName)
        {
            try
            {
                if (androidJavaObject != null)
                {
                    return androidJavaObject.Call<string>(funcName);
                }
                else
                {
                    return "";
                }
            }
            catch (System.Exception e)
            {
                LogF8.LogError(e);
                return "";
            }
        }

        public override void Init()
        {
            Call("SDKInit");
        }

        public override void Login()
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    Call("SDKLogin");
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
                    Call("SDKLogout");
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
                    Call("SDKSwitchAccount");
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
                    Call("SDKLoadVideoAd", id, userId);
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
                    Call("SDKShowVideoAd", id, userId);
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
                    Call("SDKPay", serverNum, serverName, playerId, playerName, amount, extra, orderId,
                        productName, productContent, playerLevel, sign, guid);
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
                    Call("SDKUpdateRole", scenes, serverId, serverName, roleId, roleName, roleLeve, roleCTime,
                        rolePower, guid);
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
                    Call("SDKExit");
                    break;
            }
        }

        public override void Toast(string msg)
        {
            switch ((SDKManager.Instance.platformId, SDKManager.Instance.channelId))
            {
                case ("", ""):
                    break;
                default:
                    Call("AndroidToast", msg);
                    break;
            }
        }
    }
}