using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace F8Framework.Core
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class LayoutUpdater : UIBehaviour
    {
        private RectTransform m_Parent;

        public RectTransform rectParent
        {
            get
            {
                if (m_Parent == null)
                {
                    if (transform.parent != null)
                    {
                        m_Parent = transform.parent.GetComponentInParent<RectTransform>();
                    }
                }

                return m_Parent;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            SetDirty(true);
        }

        protected void SetDirty(bool force = false)
        {
            RectTransform parent = rectParent;
            if (parent == null)
            {
                return;
            }

            if (force == true)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
            }
            else
            {
                LayoutRebuilder.MarkLayoutForRebuild(parent);
            }
        }

        protected override void OnTransformParentChanged()
        {
            m_Parent = transform.parent.GetComponentInParent<RectTransform>();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty(false);
        }
#endif
    }
}