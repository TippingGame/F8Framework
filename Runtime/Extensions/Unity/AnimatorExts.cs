using UnityEngine;

namespace F8Framework.Core
{
    public static class AnimatorExts
    {
        /// <summary>
        /// 获取动画组件切换进度
        /// </summary>
        public static float GetCrossFadeProgress(this Animator @this, int layer = 0)
        {
            if (@this.GetNextAnimatorStateInfo(layer).shortNameHash == 0)
            {
                return 1;
            }
            return @this.GetCurrentAnimatorStateInfo(layer).normalizedTime % 1;
        }
    }
}
