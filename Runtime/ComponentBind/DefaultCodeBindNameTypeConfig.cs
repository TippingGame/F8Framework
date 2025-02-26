using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public static class DefaultCodeBindNameTypeConfig
    {
        public static readonly Dictionary<string, string> BindNameTypeDict = new Dictionary<string, string>()
        {
            { "GameObject", typeof (UnityEngine.GameObject).ToString() },{ "go", typeof (UnityEngine.GameObject).ToString() },
            { "Transform", typeof(UnityEngine.Transform).ToString() },{ "tf", typeof(UnityEngine.Transform).ToString() },
            { "Animation", typeof(UnityEngine.Animation).ToString() },
            { "Animator", typeof(UnityEngine.Animator).ToString() },
            { "RectTransform", typeof(UnityEngine.RectTransform).ToString() },
            { "Canvas", typeof(UnityEngine.Canvas).ToString() },
            { "CanvasGroup", typeof(UnityEngine.CanvasGroup).ToString() },
            { "SpriteRenderer", typeof(UnityEngine.SpriteRenderer).ToString() },{ "spr", typeof(UnityEngine.SpriteRenderer).ToString() },
            { "VerticalLayoutGroup", typeof(UnityEngine.UI.VerticalLayoutGroup).ToString() },
            { "HorizontalLayoutGroup", typeof(UnityEngine.UI.HorizontalLayoutGroup).ToString() },
            { "GridLayoutGroup", typeof(UnityEngine.UI.GridLayoutGroup).ToString() },
            { "ToggleGroup", typeof(UnityEngine.UI.ToggleGroup).ToString() },
            { "Button", typeof(UnityEngine.UI.Button).ToString() },{ "btn", typeof(UnityEngine.UI.Button).ToString() },
            { "Image", typeof(UnityEngine.UI.Image).ToString() },{ "img", typeof(UnityEngine.UI.Image).ToString() },
            { "RawImage", typeof(UnityEngine.UI.RawImage).ToString() },{ "rimg", typeof(UnityEngine.UI.RawImage).ToString() },
            { "Text (Legacy)", typeof(UnityEngine.UI.Text).ToString() },{ "txt", typeof(UnityEngine.UI.Text).ToString() },
            { "InputField", typeof(UnityEngine.UI.InputField).ToString() },
            { "Slider", typeof(UnityEngine.UI.Slider).ToString() },
            { "Mask", typeof(UnityEngine.UI.Mask).ToString() },
            { "RectMask2D", typeof(UnityEngine.UI.RectMask2D).ToString() },
            { "Toggle", typeof(UnityEngine.UI.Toggle).ToString() },
            { "Scrollbar", typeof(UnityEngine.UI.Scrollbar).ToString() },
            { "ScrollRect", typeof(UnityEngine.UI.ScrollRect).ToString() },
            { "Dropdown", typeof(UnityEngine.UI.Dropdown).ToString() },
            { "Text (TMP)", "TMPro.TMP_Text" },{ "tmp", "TMPro.TMP_Text" },
        };
    }
}