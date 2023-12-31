namespace F8Framework.Core
{
    public static class FF8
    {
        //相当于重命名
        /* ---核心模块--- */

        //全局消息
        public static MessageManager Message = MessageManager.Instance;

        //本地存储
        public static PlayerPrefsManager PPrefs = PlayerPrefsManager.Instance;
        
        //游戏时间管理
        public static TimerManager Timer = TimerManager.Instance;
        
        //资产管理
        public static AssetManager Asset = AssetManager.Instance;
        
        //音频管理
        public static AudioManager Audio = AudioManager.Instance;


        /* ---可选模块--- */

    }
}

