using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Tests
{
    public class Entity : MonoBehaviour
    {
        public ContentSizeSetterSample control;

        public Text valueText;
        public Text childExpand;

        public GameObject childContainer;
        public RectTransform childRoot;

        public void Init(string text, ContentSizeSetterSample control)
        {
            this.valueText.text = text;
            this.control = control;
        }

        public void OnEnable()
        {
            SetExpandText();
        }

        public void ChildToggle()
        {
            childContainer.SetActive(childContainer.activeSelf == false);

            SetExpandText();
        }

        private void SetExpandText()
        {
            if (childRoot.childCount > 0)
            {
                if (childContainer.activeSelf == true)
                {
                    childExpand.text = "-";
                }
                else
                {
                    childExpand.text = "+";
                }
            }
            else
            {
                childExpand.text = string.Empty;
            }
        }

        public void AddChild()
        {
            control.AddEntity(childRoot);

            childContainer.SetActive(true);
            SetExpandText();
        }

        public void Delete()
        {
            GameObject.Destroy(gameObject);
        }
    }
}