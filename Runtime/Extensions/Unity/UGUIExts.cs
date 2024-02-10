using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public static class UGUIExts
    {
        public static Button AddButtonClickListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddButtonDownListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerDown);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddButtonUpListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerUp);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddListener(this Button @this, EventTriggerType triggerType, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = triggerType };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button RemoveListener(this Button @this, EventTriggerType triggerType, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveListener(handle);
            return @this;
        }
        public static Button RemoveAllListeners(this Button @this, EventTriggerType triggerType)
        {
            var eventTrigger = @this.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                throw new ArgumentNullException(nameof(eventTrigger));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveAllListeners();
            return @this;
        }
        public static void EnableImage(this Image @this)
        {
            if (@this != null)
            {
                var c = @this.color;
                @this.color = new Color(c.r, c.g, c.b, 1);
            }
        }
        public static void DisableImage(this Image @this)
        {
            if (@this != null)
            {
                var c = @this.color;
                @this.sprite = null;
                @this.color = new Color(c.r, c.g, c.b, 0);
            }
        }
    }
}
