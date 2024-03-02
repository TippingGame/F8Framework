using UnityEngine;

namespace F8Framework.Tests
{
    public class DragSort : MonoBehaviour
    {
        public void SortChange()
        {
            transform.SetAsLastSibling();
        }
    }
}