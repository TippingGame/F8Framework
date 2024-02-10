using UnityEngine;
namespace F8Framework.Core
{
    public static class VectorExts
    {
        public static Vector2 ConvertToVector2(this Vector3 @this)
        {
            return new Vector2(@this.x, @this.z);
        }
        /// <summary>
        ///返回 (x, 0, y)
        /// </summary>
        public static Vector3 ConvertToVector3(this Vector2 @this)
        {
            return new Vector3(@this.x, 0f, @this.y);
        }
        public static Vector3 ConvertToVector3(this Vector2 @this, float y)
        {
            return new Vector3(@this.x, y, @this.y);
        }
        public static Vector2 Rotate(this Vector2 @this, float angle, Vector2 pivot = default(Vector2))
        {
            Vector2 rotated = Quaternion.Euler(new Vector3(0f, 0f, angle)) * (@this - pivot);
            return rotated + pivot;
        }
    }
}
