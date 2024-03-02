using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    public static class GameObjectExts
    {
        public static void DestroySelf(this GameObject @this)
        {
            GameObject.Destroy(@this);
        }
        public static void DestroySelf(this GameObject @this, float t)
        {
            GameObject.Destroy(@this, t);
        }
        public static void DestroyAllChilds(this GameObject @this)
        {
            var childCount = @this.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject.Destroy(@this.transform.GetChild(i).gameObject);
            }
        }
        public static GameObject Active(this GameObject @this)
        {
            @this.SetActive(true);
            return @this;
        }
        public static GameObject Deactive(this GameObject @this)
        {
            @this.SetActive(false);
            return @this;
        }
        /// <summary>
        /// 优化的设置SetActive方法，可以节约重复设置Active的开销
        /// </summary>
        public static GameObject SetActiveOptimize(this GameObject @this, bool isActive)
        {
            if (@this.activeSelf != isActive)
            {
                @this.SetActive(isActive);
            }
            return @this;
        }
        public static GameObject SetName(this GameObject @this, string name)
        {
            @this.name = name;
            return @this;
        }
        public static GameObject DontDestroy(this GameObject @this)
        {
            GameObject.DontDestroyOnLoad(@this);
            return @this;
        }
        public static T GetOrAddComponent<T>(this GameObject @this) where T : Component
        {
            T component = @this.GetComponent<T>();
            if (component == null)
            {
                component = @this.AddComponent<T>();
            }
            return component;
        }
        public static Component GetOrAddComponent(this GameObject @this, Type type)
        {
            Component component = @this.GetComponent(type);
            if (component == null)
            {
                component = @this.AddComponent(type);
            }
            return component;
        }
        public static T GetComponentInParent<T>(this GameObject @this, string name)
where T : Component
        {
            var parent = @this.GetComponentsInParent<Transform>();
            var length = parent.Length;
            Transform parentTrans = null;
            for (int i = 0; i < length; i++)
            {
                if (parent[i].name == name)
                {
                    parentTrans = parent[i];
                    break;
                }
            }
            if (parentTrans == null)
                return null;
            var comp = parentTrans.GetComponent<T>();
            return comp;
        }
        public static T GetOrAddComponentInParent<T>(this GameObject @this, string name)
where T : Component
        {
            var parent = @this.GetComponentsInParent<Transform>();
            var length = parent.Length;
            Transform parentTrans = null;
            for (int i = 0; i < length; i++)
            {
                if (parent[i].name == name)
                {
                    parentTrans = parent[i];
                    break;
                }
            }
            if (parentTrans == null)
                return null;
            var comp = parentTrans.GetComponent<T>();
            if (comp == null)
            {
                comp = @this.gameObject.AddComponent<T>();
            }
            return comp;
        }
        public static T GetComponentInChildren<T>(this GameObject @this, string name)
            where T : Component
        {
            var childs = @this.GetComponentsInChildren<Transform>();
            var length = childs.Length;
            Transform childTrans = null;
            for (int i = 0; i < length; i++)
            {
                if (childs[i].name == name)
                {
                    childTrans = childs[i];
                    break;
                }
            }
            if (childTrans == null)
                return null;
            var comp = childTrans.GetComponent<T>();
            return comp;
        }
        public static T GetOrAddComponentInChildren<T>(this GameObject @this, string name)
    where T : Component
        {
            var childs = @this.GetComponentsInChildren<Transform>();
            var length = childs.Length;
            Transform childTrans = null;
            for (int i = 0; i < length; i++)
            {
                if (childs[i].name == name)
                {
                    childTrans = childs[i];
                    break;
                }
            }
            if (childTrans == null)
                return null;
            var comp = childTrans.GetComponent<T>();
            if (comp == null)
                comp = childTrans.gameObject.AddComponent<T>();
            return comp;
        }
        public static T GetComponentInPeer<T>(this GameObject @this, string name)
where T : Component
        {
            Transform tran = @this.transform.parent.Find(name);
            if (tran != null)
            {
                return tran.GetComponent<T>();
            }
            return null;
        }
        public static T GetOrAddComponentInPeer<T>(this GameObject @this, string name)
where T : Component
        {
            Transform tran = @this.transform.parent.Find(name);
            if (tran != null)
            {
                var comp = tran.GetComponent<T>();
                if (comp == null)
                    @this.gameObject.AddComponent<T>();
                return comp;
            }
            return null;
        }
        public static T[] GetComponentsInPeer<T>(this GameObject @this, bool includeSrc = false)
where T : Component
        {
            Transform parentTrans = @this.transform.parent;
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
        public static GameObject TryRemoveComponent<T>(this GameObject @this) where T : Component
        {
            var t = @this.GetComponent<T>();

            if (t != null)
            {
                Object.Destroy(t);
            }
            return @this;
        }
        public static GameObject TryRemoveComponent(this GameObject @this, Type type)
        {
            var t = @this.GetComponent(type);

            if (t != null)
            {
                Object.Destroy(t);
            }
            return @this;
        }
        public static GameObject TryRemoveComponent(this GameObject @this, string type)
        {
            var t = @this.GetComponent(type);

            if (t != null)
            {
                Object.Destroy(t);
            }
            return @this;
        }
        public static GameObject TryRemoveComponents<T>(this GameObject @this) where T : Component
        {
            var t = @this.GetComponents<T>();

            for (var i = 0; i < t.Length; i++)
            {
                Object.Destroy(t[i]);
            }
            return @this;
        }
        public static GameObject TryRemoveComponents<T>(this GameObject @this, Type type)
        {
            var t = @this.GetComponents(type);

            for (var i = 0; i < t.Length; i++)
            {
                Object.Destroy(t[i]);
            }
            return @this;
        }
        public static GameObject SetLayer(this GameObject @this, int layer)
        {
            var trans = @this.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trans)
            {
                t.gameObject.layer = layer;
            }
            return @this;
        }
        /// <summary>
        /// 设置层级；
        /// 此API会令对象下的所有子对象都被设置层级； 
        /// </summary>
        public static GameObject SetLayer(this GameObject @this, string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            var trans = @this.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trans)
            {
                t.gameObject.layer = layer;
            }
            return @this;
        }
        /// <summary>
        /// 在本帧直接销毁
        /// </summary>
        /// <param name="@this"></param>
        public static void DestroyNow(this GameObject @this)
        {
            GameObject.DestroyImmediate(@this);
        }
        /// <summary>
        /// 获取或创建GameObject
        /// </summary>
        /// <param name="@this"></param>
        public static GameObject FindOrCreateGameObject(this GameObject @this, string name)
        {
            var trans = @this.transform.Find(name);
            if (trans == null)
            {
                var go = new GameObject(name).SetParent(@this);
                return go;
            }
            else
            {
                return trans.gameObject;
            }
        }
        /// <summary>
        /// 获取或创建GameObject
        /// </summary>
        /// <param name="@this"></param>
        public static GameObject FindOrCreateGo(this GameObject @this, string name)
        {
            return @this.FindOrCreateGameObject(name);
        }
        /// <summary>
        /// 获取或创建GameObject
        /// </summary>
        /// <param name="@this"></param>
        public static GameObject FindOrCreateGameObject(this GameObject @this, string name, params Type[] Components)
        {
            var trans = @this.transform.Find(name);
            if (trans == null)
            {
                var go = new GameObject(name, Components).SetParent(@this);
                return go;
            }
            else
            {
                return trans.gameObject;
            }
        }
        public static GameObject CreateGameObject(this GameObject @this, string name)
        {
            var go = new GameObject(name).SetParent(@this);
            return go;
        }
        public static GameObject CreateGameObject(this GameObject @this, string name, params Type[] Components)
        {
            var go = new GameObject(name, Components).SetParent(@this);
            return go;
        }
        /// <summary>
        /// 设置父级GameObject
        /// </summary>
        /// <param name="@this">返回自身</param>
        public static GameObject SetParent(this GameObject @this, GameObject parentGameObject)
        {
            @this.transform.SetParent(parentGameObject.transform);
            return @this;
        }
        public static GameObject SetParent(this GameObject @this, Transform parent)
        {
            @this.transform.SetParent(parent);
            return @this;
        }
        /// <summary>
        /// 设置世界坐标
        /// </summary>
        /// <param name="@this"></param>
        public static GameObject SetPosition(this GameObject @this, Vector3 position)
        {
            if (@this.transform != null)
            {
                @this.transform.position = position;
            }
            return @this;
        }
        /// <summary>
        /// 设置本地坐标
        /// </summary>
        /// <param name="@this"></param>
        public static GameObject SetLocalPosition(this GameObject @this, Vector3 localPosition)
        {
            if (@this.transform != null)
            {
                @this.transform.localPosition = localPosition;

            }
            return @this;
        }
        public static GameObject SetLocalScale(this GameObject @this, Vector3 scaleValue)
        {
            if (@this.transform != null)
            {
                @this.transform.localScale = scaleValue;
            }
            return @this;
        }
        public static GameObject SetRotation(this GameObject @this, Quaternion value)
        {
            if (@this.transform != null)
            {
                @this.transform.rotation = value;
            }
            return @this;
        }
        public static GameObject SetLocalRotation(this GameObject @this, Quaternion value)
        {
            if (@this.transform != null)
            {
                @this.transform.localRotation = value;
            }
            return @this;
        }
        public static GameObject SetEulerAngles(this GameObject @this, Vector3 value)
        {
            if (@this.transform != null)
            {
                @this.transform.eulerAngles = value;
            }
            return @this;
        }
        /// <summary>
        /// 检查组件是否存在
        /// </summary>
        public static bool HasComponent<T>(this GameObject @this) where T : Component
        {
            return @this.GetComponent<T>() != null;
        }
        /// <summary>
        /// 检查组件是否存在
        /// </summary>
        /// <param name="@this">Game object</param>
        /// <param name="type">组件类型</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent(this GameObject @this, string type)
        {
            return @this.GetComponent(type) != null;
        }
        /// <summary>
        /// 检查组件是否存在
        /// </summary>
        /// <param name="@this">Game object</param>
        /// <param name="type">组件类型</param>
        /// <returns>True when component is attached.</returns>
        public static bool HasComponent(this GameObject @this, Type type)
        {
            return @this.GetComponent(type) != null;
        }
    }
}
