using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace F8Framework.Core
{
    public static class ComponentExts
    {
        public static T GetOrAddComponent<T>(this Component @this) where T : Component
        {
            T component = @this.GetComponent<T>();
            if (component == null)
            {
                component = @this.gameObject.AddComponent<T>();
            }
            return component;
        }
        public static Component GetOrAddComponent(this Component @this, Type type)
        {
            Component component = @this.GetComponent(type);
            if (component == null)
            {
                component = @this.gameObject.AddComponent(type);
            }
            return component;
        }
        public static T GetComponentInParent<T>(this Component @this, string name)
where T : Component
        {
            Transform parentTrans = @this.transform.FindParent(name);
            if (parentTrans == null)
                return null;
            return parentTrans.GetComponent<T>();
        }
        public static T GetOrAddComponentInParent<T>(this Component @this, string name)
where T : Component
        {
            Transform parentTrans = @this.transform.FindParent(name);
            if (parentTrans == null)
                return null;
            var comp = parentTrans.GetComponent<T>();
            if (comp == null)
            {
                comp = parentTrans.gameObject.AddComponent<T>();
            }
            return comp;
        }
        public static T GetComponentInChildren<T>(this Component @this, string name)
            where T : Component
        {
            Transform childTrans = @this.transform.FindChildren(name);
            if (childTrans == null)
                return null;
            return childTrans.GetComponent<T>();
        }
        public static T GetOrAddComponentInChildren<T>(this Component @this, string name)
    where T : Component
        {
            Transform childTrans = @this.transform.FindChildren(name);
            if (childTrans == null)
                return null;
            var comp = childTrans.GetComponent<T>();
            if (comp == null)
                comp = childTrans.gameObject.AddComponent<T>();
            return comp;
        }
        public static T GetComponentInPeer<T>(this Component @this, string name)
where T : Component
        {
            Transform tran = @this.transform.FindPeer(name);
            return tran == null ? null : tran.GetComponent<T>();
        }
        public static T GetOrAddComponentInPeer<T>(this Component @this, string name)
where T : Component
        {
            Transform tran = @this.transform.FindPeer(name);
            if (tran == null)
                return null;
            var comp = tran.GetComponent<T>();
            if (comp == null)
                comp = tran.gameObject.AddComponent<T>();
            return comp;
        }
        public static T[] GetComponentsInPeer<T>(this Component @this, bool includeSrc = false)
where T : Component
        {
            Transform[] trans = @this.transform.FindPeers(includeSrc);
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
        public static Component TryRemoveComponent<T>(this Component @this) where T : Component
        {
            var t = @this.GetComponent<T>();

            if (t != null)
            {
                Object.Destroy(t);
            }
            return @this;
        }
        public static Component TryRemoveComponent(this Component @this, Type type)
        {
            var t = @this.GetComponent(type);

            if (t != null)
            {
                Object.Destroy(t);
            }
            return @this;
        }
        public static Component TryRemoveComponent(this Component @this, string type)
        {
            var t = @this.GetComponent(type);

            if (t != null)
            {
                Object.Destroy(t);
            }
            return @this;
        }
        public static Component TryRemoveComponents<T>(this Component @this) where T : Component
        {
            var t = @this.GetComponents<T>();

            for (var i = 0; i < t.Length; i++)
            {
                Object.Destroy(t[i]);
            }
            return @this;
        }
        public static Component TryRemoveComponents<T>(this Component @this, Type type)
        {
            var t = @this.GetComponents(type);

            for (var i = 0; i < t.Length; i++)
            {
                Object.Destroy(t[i]);
            }
            return @this;
        }
    }
}
