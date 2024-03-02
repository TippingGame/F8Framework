using F8Framework.Core;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class TestItemData : InfiniteScrollData
    {
        public int index = 0;
        public string description = string.Empty;
    }

    public class TestItem : InfiniteScrollItem
    {
        public Text text = null;
        public bool isVertical = true;

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            TestItemData itemData = (TestItemData)scrollData;
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Item : {0} ", itemData.index));
            sb.Append(itemData.description);
            text.text = sb.ToString();
        }

        public void OnClick()
        {
            OnSelect();
        }

        public void OnChangeSizeClick()
        {
            float size = Random.Range(30, 400);

            Vector2 currentSize = ((RectTransform)transform).sizeDelta;
            if (isVertical == true)
            {
                ((RectTransform)transform).sizeDelta = new Vector2(currentSize.x, size);
            }
            else
            {
                ((RectTransform)transform).sizeDelta = new Vector2(size, currentSize.y);
            }

            OnUpdateItemSize();
        }
    }
}