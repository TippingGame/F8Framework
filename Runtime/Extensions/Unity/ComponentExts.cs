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
        public static T GetOrAddComponentInParent<T>(this Component @this, string name)
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
        public static T GetComponentInChildren<T>(this Component @this, string name)
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
        public static T GetOrAddComponentInChildren<T>(this Component @this, string name)
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
        public static T GetComponentInPeer<T>(this Component @this, string name)
where T : Component
        {
            Transform tran = @this.transform.parent.Find(name);
            if (tran != null)
            {
                return tran.GetComponent<T>();
            }
            return null;
        }
        public static T GetOrAddComponentInPeer<T>(this Component @this, string name)
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
        public static T[] GetComponentsInPeer<T>(this Component @this, bool includeSrc = false)
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
