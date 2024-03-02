using System;
using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public static class TransformExts
    {
        public static void SetPositionX(this Transform @this, float newValue)
        {
            Vector3 v = @this.position;
            v.x = newValue;
            @this.position = v;
        }
        public static void SetPositionY(this Transform @this, float newValue)
        {
            Vector3 v = @this.position;
            v.y = newValue;
            @this.position = v;
        }
        public static void SetPositionZ(this Transform @this, float newValue)
        {
            Vector3 v = @this.position;
            v.z = newValue;
            @this.position = v;
        }
        public static void AddPositionX(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.position;
            v.x += deltaValue;
            @this.position = v;
        }
        public static void AddPositionY(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.position;
            v.y += deltaValue;
            @this.position = v;
        }
        public static void AddPositionZ(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.position;
            v.z += deltaValue;
            @this.position = v;
        }
        public static void SetLocalPositionX(this Transform @this, float newValue)
        {
            Vector3 v = @this.localPosition;
            v.x = newValue;
            @this.localPosition = v;
        }
        public static void SetLocalPositionY(this Transform @this, float newValue)
        {
            Vector3 v = @this.localPosition;
            v.y = newValue;
            @this.localPosition = v;
        }
        public static void SetLocalPositionZ(this Transform @this, float newValue)
        {
            Vector3 v = @this.localPosition;
            v.z = newValue;
            @this.localPosition = v;
        }
        public static void AddLocalPositionX(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localPosition;
            v.x += deltaValue;
            @this.localPosition = v;
        }
        public static void AddLocalPositionY(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localPosition;
            v.y += deltaValue;
            @this.localPosition = v;
        }
        public static void AddLocalPositionZ(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localPosition;
            v.z += deltaValue;
            @this.localPosition = v;
        }
        public static void SetLocalScaleX(this Transform @this, float newValue)
        {
            Vector3 v = @this.localScale;
            v.x = newValue;
            @this.localScale = v;
        }
        public static void SetLocalScaleY(this Transform @this, float newValue)
        {
            Vector3 v = @this.localScale;
            v.y = newValue;
            @this.localScale = v;
        }
        public static void SetLocalScaleZ(this Transform @this, float newValue)
        {
            Vector3 v = @this.localScale;
            v.z = newValue;
            @this.localScale = v;
        }
        public static void AddLocalScaleX(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localScale;
            v.x += deltaValue;
            @this.localScale = v;
        }
        /// <summary>
        /// 增加相对尺寸的 y 分量。
        /// </summary>
        /// <param name="this"><see cref="Transform" /> 对象。</param>
        /// <param name="deltaValue">y 分量增量。</param>
        public static void AddLocalScaleY(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localScale;
            v.y += deltaValue;
            @this.localScale = v;
        }
        /// <summary>
        /// 增加相对尺寸的 z 分量。
        /// </summary>
        /// <param name="this"><see cref="Transform" /> 对象。</param>
        /// <param name="deltaValue">z 分量增量。</param>
        public static void AddLocalScaleZ(this Transform @this, float deltaValue)
        {
            Vector3 v = @this.localScale;
            v.z += deltaValue;
            @this.localScale = v;
        }
        /// <summary>
        /// 二维空间下使 <see cref="Transform" /> 指向指向目标点的算法，使用世界坐标。
        /// </summary>
        /// <param name="this"><see cref="Transform" /> 对象。</param>
        /// <param name="lookAtPoint2D">要朝向的二维坐标点。</param>
        /// <remarks>假定其 forward 向量为 <see cref="Vector3.up" />。</remarks>
        public static void LookAt2D(this Transform @this, Vector2 lookAtPoint2D)
        {
            Vector3 vector = lookAtPoint2D.ConvertToVector3() - @this.position;
            vector.y = 0f;

            if (vector.magnitude > 0f)
            {
                @this.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
            }
        }
        public static void DeleteChildrens(this Transform @this)
        {
            var childCount = @this.childCount;
            if (childCount == 0)
                return;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(@this.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// 查找所有符合名称的子节点
        /// </summary>
        /// <param name="this">目标对象</param>
        /// <param name="name">子级别目标对象名称</param>
        /// <returns>名字符合的对象数组</returns>
        public static Transform[] FindChildrens(this Transform @this, string name)
        {
            var trans = @this.GetComponentsInChildren<Transform>();
            var length = trans.Length;
            var dst = new Transform[length];
            int idx = 0;
            for (int i = 0; i < length; i++)
            {
                if (trans[i].name.Contains(name))
                {
                    dst[idx] = trans[i];
                    idx++;
                }
            }
            Array.Resize(ref dst, idx);
            return dst;
        }
        public static Transform FindChildren(this Transform @this, string name)
        {
            var trans = @this.GetComponentsInChildren<Transform>();
            var length = trans.Length;
            for (int i = 1; i < length; i++)
            {
                if (trans[i].name.Equals(name))
                    return trans[i];
            }
            return null;
        }
        /// <summary>
        /// 查找同级别其他对象；
        /// </summary>
        /// <param name="@this">同级别当前对象</param>
        /// <param name="includeSrc">是否包含本身</param>
        /// <returns>当前级别下除此对象的其他同级的对象</returns>
        public static Transform[] FindPeers(this Transform @this, bool includeSrc = false)
        {
            Transform parentTrans = @this.parent;
            var childTrans = parentTrans.GetComponentsInChildren<Transform>();
            var length = childTrans.Length;
            if (!includeSrc)
                return Util.Algorithm.FindAll(childTrans, t => t.parent == parentTrans && t != @this);
            else
                return Util.Algorithm.FindAll(childTrans, t => t.parent == parentTrans);
        }
        public static Transform FindPeer(this Transform @this, string name)
        {
            Transform tran = @this.parent.Find(name);
            if (tran == null)
                return null;
            return tran;
        }
        public static Transform FindParent(this Transform @this, string name)
        {
            Transform parent = @this.parent;
            while (parent != null)
            {
                if (parent.name == name)
                {
                    break;
                }
                parent = parent.parent;
            }
            return parent;
        }
        public static Transform[] FindParents(this Transform @this, Predicate<Transform> condition)
        {
            List<Transform> transformList = new List<Transform>();
            Transform parent = @this.parent;
            while (parent != null)
            {
                if (condition(parent))
                {
                    transformList.Add(parent);
                }
                parent = parent.parent;
            }
            return transformList.ToArray();
        }
        /// <summary>
        /// 查找同级别下所有目标组件；
        /// 略耗性能；
        /// </summary>
        /// <typeparam name="T">目标组件</typeparam>
        /// <param name="this">同级别当前对象</param>
        /// <param name="includeSrc">包含当前对象</param>
        /// <returns>同级别对象数组</returns>
        public static T[] PeerComponets<T>(this Transform @this, bool includeSrc = false) where T : Component
        {
            Transform parentTrans = @this.parent;
            var childTrans = parentTrans.GetComponentsInChildren<Transform>();
            var length = childTrans.Length;
            Transform[] trans;
            if (!includeSrc)
                trans = Util.Algorithm.FindAll(childTrans, t => t.parent == parentTrans);
            else
                trans = Util.Algorithm.FindAll(childTrans, t => t.parent == parentTrans && t != @this);
            var transLength = trans.Length;
            T[] src = new T[transLength];
            int idx = 0;
            for (int i = 0; i < transLength; i++)
            {
                var comp = trans[i].GetComponent<T>();
                if (comp != null)
                {
                    src[idx] = comp;
                    idx++;
                }
            }
            T[] dst = new T[idx];
            Array.Copy(src, 0, dst, 0, idx);
            return dst;
        }
        public static void DestroyAllChilds(this Transform @this)
        {
            var childCount = @this.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(@this.GetChild(i).gameObject);
            }
        }
        public static void ResetWorldTransform(this Transform @this)
        {
            @this.position = Vector3.zero;
            @this.rotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
        }
        public static void ResetLocalTransform(this Transform @this)
        {
            @this.localPosition = Vector3.zero;
            @this.localRotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
        }
        public static void ResetRectTransform(this RectTransform @this)
        {
            @this.localPosition = Vector3.zero;
            @this.localRotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
            @this.offsetMax = Vector2.zero;
            @this.offsetMin = Vector2.zero;
        }
        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public static void ClearChild(this Transform @this)
        {
            if (@this == null)
                return;
            for (int i = @this.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(@this.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// 获取第一个子物体
        /// </summary>
        /// <param name="findActiveObject">激活条件</param>
        public static Transform GetFirstChild(this Transform @this, bool findActiveObject = true)
        {
            if (@this == null || @this.childCount == 0)
                return null;

            if (findActiveObject == false)
                return @this.GetChild(0);

            for (int i = 0; i < @this.childCount; i++)
            {
                Transform target = @this.GetChild(i);
                if (target.gameObject.activeSelf)
                    return target;
            }
            return null;
        }
        /// <summary>
        /// 获取最后一个子物体
        /// </summary>
        /// <param name="findActiveObject">激活条件</param>
        public static Transform GetLastChild(this Transform @this, bool findActiveObject = true)
        {
            if (@this == null || @this.childCount == 0)
                return null;

            if (findActiveObject == false)
                return @this.GetChild(@this.childCount - 1);

            for (int i = @this.childCount - 1; i >= 0; i--)
            {
                Transform target = @this.GetChild(i);
                if (target.gameObject.activeSelf)
                    return target;
            }
            return null;
        }
        /// <summary>
        /// 设置并对其到父对象；
        /// </summary>
        public static void SetAlignParent(this Transform @this, Transform parent)
        {
            @this.SetParent(parent);
            @this.localPosition = Vector3.zero;
            @this.localRotation = Quaternion.Euler(Vector3.zero);
            @this.localScale = Vector3.one;
        }
        public static Transform SetParentChild(this Transform @this, Transform parent, string name)
        {
            var child = FindChildren(parent, name);
            if (child != null)
            {
                @this.SetParent(child);
                return @this;
            }
            return null;
        }
    }
}
