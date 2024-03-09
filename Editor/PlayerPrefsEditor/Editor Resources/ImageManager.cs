#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    public class ImageManager
    {
        // Keep this ID unique
        private static readonly string ID = "[PlayerPrefsEditor] com.F8Framework.Core.Editor";

        private static string imageManagerPath;
        private static string GetAssetDir()
        {
            if (imageManagerPath != null)
            {
                return imageManagerPath;
            }

            foreach (string assetGuid in AssetDatabase.FindAssets("ImageManager"))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                string fileName = Path.GetFileName(assetPath);

                if (fileName.Equals("ImageManager.cs"))
                {
                    // Check ID if it's the correct ImageManager
                    if (File.ReadLines(Path.GetFullPath(assetPath)).Any(line => line.Contains(ID)))
                    {
                        imageManagerPath = Path.GetDirectoryName(assetPath) + Path.DirectorySeparatorChar;
                        return imageManagerPath;
                    }
                }
            }
            throw new Exception("Cannot find ImageManager.cs in the project. Are sure all the files in place?");
        }

        public static Texture2D GetOsIcon()
        {
#if UNITY_EDITOR_WIN
            return OsWinIcon;
#elif UNITY_EDITOR_OSX
            return OsMacIcon;
#elif UNITY_EDITOR_LINUX
            return OsLinuxIcon;
#endif
        }

        private static Texture2D osLinuxIcon;
        public static Texture2D OsLinuxIcon
        {
            get
            {
                if (osLinuxIcon == null)
                {
                    osLinuxIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "os_linux_icon.png", typeof(Texture2D));
                }
                return osLinuxIcon;
            }
        }

        private static Texture2D osWinIcon;
        public static Texture2D OsWinIcon
        {
            get
            {
                if (osWinIcon == null)
                {
                    osWinIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "os_win_icon.png", typeof(Texture2D));
                }
                return osWinIcon;
            }
        }

        private static Texture2D osMacIcon;
        public static Texture2D OsMacIcon
        {
            get
            {
                if (osMacIcon == null)
                {
                    osMacIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "os_mac_icon.png", typeof(Texture2D));
                }
                return osMacIcon;
            }
        }

        private static GUIContent[] spinWheelIcons;
        public static GUIContent[] SpinWheelIcons
        {
            get
            {
                if(spinWheelIcons == null)
                {
                    spinWheelIcons = new GUIContent[12];
                    for (int i = 0; i < 12; i++)
                        spinWheelIcons[i] = EditorGUIUtility.IconContent("WaitSpin" + i.ToString("00"));
                }
                return spinWheelIcons;
            }
        }

        private static Texture2D refresh;
        public static Texture2D Refresh
        {
            get
            {
                if (refresh == null)
                {
                    refresh = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "refresh.png", typeof(Texture2D));
                }
                return refresh;
            }
        }

        private static Texture2D trash;
        public static Texture2D Trash
        {
            get
            {
                if (trash == null)
                {
                    trash = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "trash.png", typeof(Texture2D));
                }
                return trash;
            }
        }

        private static Texture2D exclamation;
        public static Texture2D Exclamation
        {
            get
            {
                if(exclamation == null)
                {
                    exclamation = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "exclamation.png", typeof(Texture2D));
                }
                return exclamation;
            }
        }

        private static Texture2D info;
        public static Texture2D Info
        {
            get
            {
                if (info == null)
                {
                    info = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "info.png", typeof(Texture2D));
                }
                return info;
            }
        }

        private static Texture2D watching;
        public static Texture2D Watching
        {
            get
            {
                if(watching == null)
                {
                    watching = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "watching.png", typeof(Texture2D));
                }
                return watching;
            }
        }

        private static Texture2D notWatching;
        public static Texture2D NotWatching
        {
            get
            {
                if (notWatching == null)
                {
                    notWatching = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "not_watching.png", typeof(Texture2D));
                }
                return notWatching;
            }
        }

        private static Texture2D sortDisabled;
        public static Texture2D SortDisabled
        {
            get
            {
                if (sortDisabled == null)
                {
                    sortDisabled = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "sort.png", typeof(Texture2D));
                }
                return sortDisabled;
            }
        }

        private static Texture2D sortAsscending;
        public static Texture2D SortAsscending
        {
            get
            {
                if (sortAsscending == null)
                {
                    sortAsscending = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "sort_asc.png", typeof(Texture2D));
                }
                return sortAsscending;
            }
        }

        private static Texture2D sortDescending;
        public static Texture2D SortDescending
        {
            get
            {
                if (sortDescending == null)
                {
                    sortDescending = (Texture2D)AssetDatabase.LoadAssetAtPath(GetAssetDir() + "sort_desc.png", typeof(Texture2D));
                }
                return sortDescending;
            }
        }
    }
}
#endif
