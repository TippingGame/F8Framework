using UnityEngine;

namespace F8Framework.Core
{
    public static class RectTransformExts
    {
		public static void SetSizeDeltaWidth(this RectTransform @this, float width)
		{
			Vector2 size = @this.sizeDelta;
			size.x = width;
			@this.sizeDelta = size;
		}
		public static void SetSizeDeltaHeight(this RectTransform @this, float height)
		{
			Vector2 size = @this.sizeDelta;
			size.y = height;
			@this.sizeDelta = size;
		}
		public static void SetAnchoredPositionX(this RectTransform @this, float pos)
		{
			Vector3 temp = @this.anchoredPosition;
			temp.x = pos;
			@this.anchoredPosition = temp;
		}
		public static void SetAnchoredPositionY(this RectTransform @this, float pos)
		{
			Vector3 temp = @this.anchoredPosition;
			temp.y = pos;
			@this.anchoredPosition = temp;
		}
		public static void SetAnchoredPositionZ(this RectTransform @this, float pos)
		{
			Vector3 temp = @this.anchoredPosition;
			temp.z = pos;
			@this.anchoredPosition = temp;
		}
	}
}
