using System;
using System.Collections.Generic;

namespace F8Framework.Core
{
    [Serializable]
    public class Blackboard
    {
        // 基础接口
        private interface IValueContainer
        {
            Type ValueType { get; }
            object GetValueAsObject();
        }

        // 泛型值容器
        [Serializable]
        private class ValueContainer<T> : IValueContainer
        {
            public T value;

            public Type ValueType => typeof(T);
            public object GetValueAsObject() => value;

            public ValueContainer(T value)
            {
                this.value = value;
            }
        }
        
        private Dictionary<string, IValueContainer> valueDictionary = new Dictionary<string, IValueContainer>();

        // 泛型事件系统
        private class ValueChangedEvent<T>
        {
            public event Action<string, T> OnValueChanged;

            public void Invoke(string key, T value)
            {
                OnValueChanged?.Invoke(key, value);
            }
        }

        // 为每种类型存储单独的事件
        private Dictionary<Type, object> valueChangedEvents = new Dictionary<Type, object>();
        private event Action<string> OnValueRemovedGeneric;

        // 注册值改变事件
        public void RegisterValueChanged<T>(Action<string, T> callback)
        {
            Type type = typeof(T);
            if (!valueChangedEvents.TryGetValue(type, out var eventObj))
            {
                eventObj = new ValueChangedEvent<T>();
                valueChangedEvents[type] = eventObj;
            }

            var typedEvent = (ValueChangedEvent<T>)eventObj;
            typedEvent.OnValueChanged += callback;
        }

        // 取消注册值改变事件
        public void UnregisterValueChanged<T>(Action<string, T> callback)
        {
            if (valueChangedEvents.TryGetValue(typeof(T), out var eventObj))
            {
                var typedEvent = (ValueChangedEvent<T>)eventObj;
                typedEvent.OnValueChanged -= callback;
            }
        }

        // 注册值移除事件
        public void RegisterValueRemoved(Action<string> callback)
        {
            OnValueRemovedGeneric += callback;
        }

        // 取消注册值移除事件
        public void UnregisterValueRemoved(Action<string> callback)
        {
            OnValueRemovedGeneric -= callback;
        }

        // 设置值
        public void SetValue<T>(string key, T value)
        {
            if (valueDictionary.TryGetValue(key, out var container))
            {
                if (container is ValueContainer<T> typedContainer)
                {
                    typedContainer.value = value;
                }
                else
                {
                    // 类型不匹配，替换容器
                    valueDictionary[key] = new ValueContainer<T>(value);
                }
            }
            else
            {
                valueDictionary.Add(key, new ValueContainer<T>(value));
            }

            // 触发泛型事件
            TriggerValueChangedEvent(key, value);
        }

        // 触发值改变事件
        private void TriggerValueChangedEvent<T>(string key, T value)
        {
            if (valueChangedEvents.TryGetValue(typeof(T), out var eventObj))
            {
                var typedEvent = (ValueChangedEvent<T>)eventObj;
                typedEvent.Invoke(key, value);
            }
        }

        // 获取值
        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (valueDictionary.TryGetValue(key, out var container) &&
                container is ValueContainer<T> typedContainer)
            {
                return typedContainer.value;
            }

            return defaultValue;
        }

        // 尝试获取值
        public bool TryGetValue<T>(string key, out T value)
        {
            if (valueDictionary.TryGetValue(key, out var container) &&
                container is ValueContainer<T> typedContainer)
            {
                value = typedContainer.value;
                return true;
            }

            value = default;
            return false;
        }

        // 检查是否存在某个键
        public bool HasValue(string key)
        {
            return valueDictionary.ContainsKey(key);
        }

        // 检查是否存在特定类型的值
        public bool HasValue<T>(string key)
        {
            return valueDictionary.TryGetValue(key, out var container) &&
                   container is ValueContainer<T>;
        }

        // 移除值
        public void RemoveValue(string key)
        {
            if (valueDictionary.Remove(key))
            {
                OnValueRemovedGeneric?.Invoke(key);
            }
        }

        // 清空所有值
        public void Clear()
        {
            valueDictionary.Clear();
        }

        // 获取所有键
        public List<string> GetAllKeys()
        {
            return new List<string>(valueDictionary.Keys);
        }

        // 获取值类型
        public Type GetValueType(string key)
        {
            return valueDictionary.TryGetValue(key, out var container) ? container.ValueType : null;
        }
    }
}