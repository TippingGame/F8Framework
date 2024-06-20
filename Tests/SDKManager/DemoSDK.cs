using F8Framework.Core;
using F8Framework.Launcher;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoSDK : MonoBehaviour
    {
        void Start()
        {
            // 启动SDK，平台id，渠道id
            FF8.SDK.SDKStart("1", "1");

            // 登录
            FF8.SDK.SDKLogin();

            // 登出
            FF8.SDK.SDKLogout();

            // 切换账号
            FF8.SDK.SDKSwitchAccount();

            // 加载视频广告
            FF8.SDK.SDKLoadVideoAd("1", "1");

            // 播放视频广告
            FF8.SDK.SDKShowVideoAd("1", "1");

            // 支付
            FF8.SDK.SDKPay("serverNum", "serverName", "playerId", "playerName", "amount", "extra", "orderId",
                "productName", "productContent", "playerLevel", "sign", "guid");

            // 更新用户信息
            FF8.SDK.SDKUpdateRole("scenes", "serverId", "serverName", "roleId", "roleName", "roleLeve", "roleCTime",
                "rolePower", "guid");

            // SDK退出游戏
            FF8.SDK.SDKExitGame();

            // 播放视频广告
            FF8.SDK.SDKToast("Native Toast");
        }
    }
}
