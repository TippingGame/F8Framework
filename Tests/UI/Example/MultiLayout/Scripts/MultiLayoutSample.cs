using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Tests
{
    public class MultiLayoutSample : MonoBehaviour
    {
        private MultiLayout multiLayout = null;
        private int layout = 0;

        private void Awake()
        {
            multiLayout = GetComponent<MultiLayout>();
        }

        public void ChangeLayout()
        {
            multiLayout.SelectLayout(layout++ % multiLayout.layout.count);
        }
    }
}