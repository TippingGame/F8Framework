namespace F8Framework.Core
{
    /// <summary>
    /// 基础SDK类，定义了SDK的基本操作接口
    /// </summary>
    public class SDKBase
    {
        public SDKBase()
        {
        }

        /// <summary>
        /// 初始化SDK
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// 登录SDK
        /// </summary>
        public virtual void Login()
        {
        }

        /// <summary>
        /// 登出SDK
        /// </summary>
        public virtual void Logout()
        {
        }

        /// <summary>
        /// 切换账号
        /// </summary>
        public virtual void SwitchAccount()
        {
        }

        /// <summary>
        /// 加载视频广告
        /// </summary>
        /// <param name="id">广告ID</param>
        /// <param name="userId">用户ID</param>
        public virtual void LoadVideoAd(string id, string userId)
        {
        }

        /// <summary>
        /// 显示视频广告
        /// </summary>
        /// <param name="id">广告ID</param>
        /// <param name="userId">用户ID</param>
        public virtual void ShowVideoAd(string id, string userId)
        {
        }
        
        /// <summary>
        /// 进行支付操作
        /// </summary>
        public virtual void Pay(string serverNum, string serverName, string playerId, string playerName, string amount, string extra, string orderId,
            string productName, string productContent, string playerLevel, string sign, string guid)
        {
        }
        
        /// <summary>
        /// 更新角色信息
        /// </summary>
        public virtual void UpdateRole(string scenes, string serverId, string serverName, string roleId, string roleName,
            string roleLeve, string roleCTime, string rolePower, string guid)
        {
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public virtual void ExitGame()
        {
        }
        
        /// <summary>
        /// 弹出消息提示
        /// </summary>
        /// <param name="msg">提示消息</param>
        public virtual void Toast(string msg)
        {
           
        }
    }
}