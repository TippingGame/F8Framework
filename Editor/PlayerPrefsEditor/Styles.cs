using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class Styles
    {
        #region Colors
        public class Colors {
            public static Color DarkGray =  new Color(0.09f, 0.09f, 0.09f);
            public static Color LightGray = new Color(0.65f, 0.65f, 0.65f);
            public static Color Red =       new Color(1.00f, 0.00f, 0.00f);
            public static Color Yellow =    new Color(1.00f, 1.00f, 0.00f);
            public static Color Blue =      new Color(0.00f, 0.63f, 0.99f);
        }
        #endregion // Colors

        #region Texture manager
        static Dictionary<long, Texture2D> mTextures = new Dictionary<long, Texture2D>();

        public static Texture2D GetTexture(long pColorRGBA)
        {
            if (mTextures.ContainsKey(pColorRGBA) && mTextures[pColorRGBA] != null)
                return mTextures[pColorRGBA];

            Color32 c = GetColor(pColorRGBA);

            var texture = new Texture2D(4, 4);
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    texture.SetPixel(x, y, c);
            texture.Apply();
            texture.Compress(true);

            mTextures[pColorRGBA] = texture;

            return texture;
        }

        private static Color32 GetColor(long pColorRGBA)
        {
            byte r = (byte)((pColorRGBA & 0xff000000) >> 24);
            byte g = (byte)((pColorRGBA & 0xff0000) >> 16);
            byte b = (byte)((pColorRGBA & 0xff00) >> 8);
            byte a = (byte)((pColorRGBA & 0xff));

            Color32 c = new Color32(r, g, b, a);
            return c;
        }
        #endregion Texture manager

        static GUIStyle mHSeparator;
        private static GUIStyle hSeparator
        {
            get
            {
                if (mHSeparator == null)
                {
                    mHSeparator = new GUIStyle();
                    mHSeparator.alignment = TextAnchor.MiddleCenter;
                    mHSeparator.stretchWidth = true;
                    mHSeparator.fixedHeight = 1;
                    mHSeparator.margin = new RectOffset(20, 20, 5, 5);
                    mHSeparator.normal.background = (EditorGUIUtility.isProSkin) ? GetTexture(0xb5b5b5ff) : GetTexture(0x000000ff);
                }
                return mHSeparator;
            }
        }

        public static void HorizontalSeparator()
        {
            GUILayout.Label("", hSeparator);
        }

        static GUIStyle Icon;
        public static GUIStyle icon
        {
            get
            {
                if (Icon == null)
                {
                    Icon = new GUIStyle();
                    Icon.fixedWidth = 15.0f;
                    Icon.fixedHeight = 15.0f;
                    Icon.margin = new RectOffset(2, 2, 2, 2);
                }
                return Icon;
            }
        }

        static GUIStyle MiniButton;
        public static GUIStyle miniButton
        {
            get
            {
                if (MiniButton == null)
                {
                    MiniButton = new GUIStyle(GUI.skin.button);
                    MiniButton.fixedWidth = 15.0f;
                    MiniButton.fixedHeight = 15.0f;
                    MiniButton.margin = new RectOffset(2, 2, 2, 2);
                    MiniButton.padding = new RectOffset(2, 2, 2, 2);
                }
                return MiniButton;
            }
        }
    }
}