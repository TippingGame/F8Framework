using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace F8Framework.Core
{
    public class DragaEventHandler : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public DragEvent onBeginDrag = new DragEvent();
        public DragEvent onDrag = new DragEvent();
        public DragEvent onEndDrag = new DragEvent();

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag.Invoke(eventData);
        }

        [System.Serializable]
        public class DragEvent : UnityEvent<PointerEventData>
        {
            public DragEvent()
            {
            }
        }
    }
}