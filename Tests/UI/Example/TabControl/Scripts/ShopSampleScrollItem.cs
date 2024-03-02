using F8Framework.Core;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class ShopSampleScrollItemData : InfiniteScrollData
    {
        public string name = string.Empty;
        public Sprite image;
        public string description = string.Empty;

        public bool buttonEnabled = false;
        public string buttonText = string.Empty;

        public System.Action<int> buttonEvent;
    }

    public class ShopSampleScrollItem : InfiniteScrollItem
    {
        private ShopSampleScrollItemData itemData;

        public Text itemName = null;
        public Image itemImage = null;
        public Text itemIDescription = null;

        public Button button = null;
        public Text buttonText = null;

        public System.Action<int> buttonEvent;

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            itemData = scrollData as ShopSampleScrollItemData;

            itemName.text = itemData.name;
            itemImage.sprite = itemData.image;
            itemIDescription.text = itemData.description;

            button.interactable = itemData.buttonEnabled;
            buttonText.text = itemData.buttonText;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickEvent);
        }

        private void OnClickEvent()
        {
            if (itemData.buttonEvent != null)
            {
                itemData.buttonEvent(GetDataIndex());
            }
        }
    }
}