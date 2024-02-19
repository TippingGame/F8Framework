using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core
{
    public static class DefaultCodeBindNameTypeConfig
    {
        public static readonly Dictionary<string, string> BindNameTypeDict = new Dictionary<string, string>()
        {
            { "GameObject", nameof (GameObject) },{ "go", nameof (UnityEngine.GameObject) },
            { "Transform", nameof (UnityEngine.Transform) },{ "tf", nameof (UnityEngine.Transform) },
            { "Animation", nameof (UnityEngine.Animation) },
            { "Animator", nameof (UnityEngine.Animator) },
            { "RectTransform", nameof (UnityEngine.RectTransform) },
            { "Canvas", nameof (UnityEngine.Canvas) },
            { "CanvasGroup", nameof (UnityEngine.CanvasGroup) },
            { "SpriteRenderer", nameof (UnityEngine.SpriteRenderer) },{ "spr", nameof (UnityEngine.SpriteRenderer) },
            { "VerticalLayoutGroup", nameof (UnityEngine.UI.VerticalLayoutGroup) },
            { "HorizontalLayoutGroup", nameof (UnityEngine.UI.HorizontalLayoutGroup) },
            { "GridLayoutGroup", nameof (UnityEngine.UI.GridLayoutGroup) },
            { "ToggleGroup", nameof (UnityEngine.UI.ToggleGroup) },
            { "Button", nameof (UnityEngine.UI.Button) },{ "btn", nameof (UnityEngine.UI.Button) },
            { "Image", nameof (UnityEngine.UI.Image) },{ "img", nameof (UnityEngine.UI.Image) },
            { "RawImage", nameof (UnityEngine.UI.RawImage) },{ "rimg", nameof (UnityEngine.UI.RawImage) },
            { "Text (Legacy)", nameof (UnityEngine.UI.Text) },{ "txt", nameof (UnityEngine.UI.Text) },
            { "InputField", nameof (UnityEngine.UI.InputField) },
            { "Slider", nameof (UnityEngine.UI.Slider) },
            { "Mask", nameof (UnityEngine.UI.Mask) },
            { "RectMask2D", nameof (UnityEngine.UI.RectMask2D) },
            { "Toggle", nameof (UnityEngine.UI.Toggle) },
            { "Scrollbar", nameof (UnityEngine.UI.Scrollbar) },
            { "ScrollRect", nameof (UnityEngine.UI.ScrollRect) },
            { "Dropdown", nameof (UnityEngine.UI.Dropdown) },
            { "Text (TMP)", "TMPro.TMP_Text" },{ "tmp", "TMPro.TMP_Text" },
        };
    }
}