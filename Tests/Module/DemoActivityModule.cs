using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class DemoActivityModule : MonoBehaviour
    {
        private void Start()
        {
            LogF8.Log("已发现活动模块数量: " + ActivityModule.GetActivityModuleTypes().Count);

            var levelActivity = LevelUnlockActivityModule.Instance;
            var limitedActivity = LimitedOpenActivityModule.Instance;

            PrintState(levelActivity);
            PrintState(limitedActivity);

            DemoActivityContext.PlayerLevel = 20;
            DemoActivityContext.IsActivitySwitchEnabled = true;
            ActivityModule.RefreshInstantiatedModules();

            PrintState(levelActivity);
            PrintState(limitedActivity);

            ActivityModule.ReleaseActivityModule<LevelUnlockActivityModule>();
            ActivityModule.ReleaseActivityModule(typeof(LimitedOpenActivityModule));
            ActivityModule.ReleaseAllModules();
        }

        private static void PrintState(ActivityModule module)
        {
            LogF8.Log($"{module.ModuleName} -> Unlock:{module.IsUnlocked} Visible:{module.IsVisible} Open:{module.IsOpen}");
        }
    }

    public static class DemoActivityContext
    {
        public static int PlayerLevel = 1;
        public static bool IsActivitySwitchEnabled = false;
        public static bool IsTimeWindowActive = true;
    }

    public class LevelUnlockActivityModule : ActivityModule
    {
        public static LevelUnlockActivityModule Instance => GetInstance<LevelUnlockActivityModule>();

        protected override void Init()
        {
            AddUnlockCondition(() => DemoActivityContext.PlayerLevel >= 10);
        }

        protected override void OnOpenStateChanged(bool previousValue, bool currentValue)
        {
            LogF8.Log($"LevelUnlockActivityModule Open -> {currentValue}");
        }
    }

    public class LimitedOpenActivityModule : ActivityModule
    {
        public static LimitedOpenActivityModule Instance => GetInstance<LimitedOpenActivityModule>();

        protected override void Init()
        {
            AddUnlockCondition(() => DemoActivityContext.PlayerLevel >= 15);
            AddOpenCondition(() => DemoActivityContext.IsActivitySwitchEnabled);
            AddOpenCondition(() => DemoActivityContext.IsTimeWindowActive);
        }

        protected override bool EvaluateVisibleCore(bool isUnlocked)
        {
            return isUnlocked;
        }

        protected override void OnStateChanged(ActivityModuleState previousState, ActivityModuleState currentState)
        {
            LogF8.Log($"LimitedOpenActivityModule State -> Unlock:{currentState.IsUnlocked} Visible:{currentState.IsVisible} Open:{currentState.IsOpen}");
        }
    }
}
