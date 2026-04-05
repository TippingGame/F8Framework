#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace F8Framework.Core
{
    /// <summary>
    /// PlayerInputManager 的运行时配置
    /// </summary>
    public sealed class PlayerInputManagerConfig
    {
        public GameObject PlayerPrefab { get; set; }
        public PlayerJoinBehavior JoinBehavior { get; set; } = PlayerJoinBehavior.JoinPlayersManually;
        public PlayerNotifications NotificationBehavior { get; set; } = PlayerNotifications.InvokeCSharpEvents;
        public int MaxPlayerCount { get; set; } = -1;
        public bool SplitScreen { get; set; }
        public bool MaintainAspectRatioInSplitScreen { get; set; }
        public int FixedNumberOfSplitScreens { get; set; } = -1;
        public InputAction JoinAction { get; set; }
    }
}
#endif