using UnityEngine.EventSystems;

namespace F8Framework.Core
{
    public class TabButton : Tab, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

    }
}
