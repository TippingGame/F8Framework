using UnityEngine;
using UnityEngine.EventSystems;

namespace F8Framework.Core
{
    public class DraggableRect : DragaEventHandler
    {
        public RectTransform dragRectTransform;

        private Canvas m_Canvas;

        public Canvas canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = GetComponentInParent<Canvas>();
                }

                return m_Canvas;
            }
        }

        protected override void OnEnable()
        {
            if (dragRectTransform == null)
            {
                dragRectTransform = gameObject.GetComponent<RectTransform>();
            }

            if (dragRectTransform == null || canvas == null)
            {
                enabled = false;
            }

            onDrag.AddListener(OnDragMove);
        }

        protected override void OnDisable()
        {
            onDrag.RemoveListener(OnDragMove);
        }

        public void OnDragMove(PointerEventData eventData)
        {
            dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        protected override void OnCanvasHierarchyChanged()
        {
            m_Canvas = GetComponentInParent<Canvas>();
        }
    }
}