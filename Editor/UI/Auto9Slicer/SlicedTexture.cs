using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class SlicedTexture
    {
        public SlicedTexture(Texture2D texture, Border border)
        {
            Texture = texture;
            Border = border;
        }

        public Texture2D Texture { get; }
        public Border Border { get; }
    }

    public struct Border
    {
        public Border(int left, int bottom, int right, int top)
        {
            Left = left;
            Bottom = bottom;
            Right = right;
            Top = top;
        }

        public Vector4 ToVector4()
        {
            return new Vector4(Left, Bottom, Right, Top);
        }

        public int Left { get; }
        public int Bottom { get; }
        public int Right { get; }
        public int Top { get; }
    }
}
