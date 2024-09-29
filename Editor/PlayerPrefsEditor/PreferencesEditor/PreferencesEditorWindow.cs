using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

#if (UNITY_EDITOR_LINUX || UNITY_EDITOR_OSX)
using System.Text;
using System.Globalization;
#endif

namespace F8Framework.Core.Editor
{
    public class PreferencesEditorWindow : EditorWindow
    {
#region ErrorValues
        private readonly int ERROR_VALUE_INT = int.MinValue;
        private readonly string ERROR_VALUE_STR = "<bgTool_error_24072017>";
        #endregion //ErrorValues

        private enum PreferencesEntrySortOrder
        {
            None = 0,
            Asscending = 1,
            Descending = 2
        }

        private static string pathToPrefs = String.Empty;
        private static string platformPathPrefix = @"~";

        private string[] userDef;
        private string[] unityDef;
        private bool showSystemGroup = false;

        private PreferencesEntrySortOrder sortOrder = PreferencesEntrySortOrder.None;

        private SerializedObject serializedObject;
        private ReorderableList userDefList;
        private ReorderableList unityDefList;

        private SerializedProperty[] userDefListCache = new SerializedProperty[0];

        private PreferenceEntryHolder prefEntryHolder;

        private Vector2 scrollPos;
        private float relSpliterPos;
        private bool moveSplitterPos = false;

        private PreferanceStorageAccessor entryAccessor;

        private MySearchField searchfield;
        private string searchTxt;
        private int loadingSpinnerFrame;

        private bool updateView = false;
        private bool monitoring = false;
        private bool showLoadingIndicatorOverlay = false;

        private readonly List<TextValidator> prefKeyValidatorList = new List<TextValidator>()
        {
            new TextValidator(TextValidator.ErrorType.Error, @"Invalid character detected. Only letters, numbers, space and ,.;:<>_|!§$%&/()=?*+~#-]+$ are allowed", @"(^$)|(^[a-zA-Z0-9 ,.;:<>_|!§$%&/()=?*+~#-]+$)"),
            new TextValidator(TextValidator.ErrorType.Warning, @"The given key already exist. The existing entry would be overwritten!", (key) => { return !PlayerPrefs.HasKey(key); })
        };

#if UNITY_EDITOR_LINUX
        private readonly char[] invalidFilenameChars = { '"', '\\', '*', '/', ':', '<', '>', '?', '|' };
#elif UNITY_EDITOR_OSX
        private readonly char[] invalidFilenameChars = { '$', '%', '&', '\\', '/', ':', '<', '>', '|', '~' };
#endif
        [UnityEditor.MenuItem("开发工具/PlayerPrefs Editor", false, 103)]
        static void ShowWindow()
        {
            if (HasOpenInstances<PreferencesEditorWindow>())
            {
                GetWindow<PreferencesEditorWindow>("Prefs Editor").Close();
            }
            else
            {
                PreferencesEditorWindow window = EditorWindow.GetWindow<PreferencesEditorWindow>("Prefs Editor");
                window.minSize = new Vector2(270.0f, 300.0f);
                window.name = "Prefs Editor";

                //window.titleContent = EditorGUIUtility.IconContent("SettingsIcon"); // Icon

                window.Show();
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR_WIN
            pathToPrefs = @"SOFTWARE\Unity\UnityEditor\" + PlayerSettings.companyName + @"\" + PlayerSettings.productName;
            platformPathPrefix = @"<CurrentUser>";
            entryAccessor = new WindowsPrefStorage(pathToPrefs);
#elif UNITY_EDITOR_OSX
            pathToPrefs = @"Library/Preferences/unity." + MakeValidFileName(PlayerSettings.companyName) + "." + MakeValidFileName(PlayerSettings.productName) + ".plist";
            entryAccessor = new MacPrefStorage(pathToPrefs);
            entryAccessor.StartLoadingDelegate = () => { showLoadingIndicatorOverlay = true; };
            entryAccessor.StopLoadingDelegate = () => { showLoadingIndicatorOverlay = false; };
#elif UNITY_EDITOR_LINUX
            pathToPrefs = @".config/unity3d/" + MakeValidFileName(PlayerSettings.companyName) + "/" + MakeValidFileName(PlayerSettings.productName) + "/prefs";
            entryAccessor = new LinuxPrefStorage(pathToPrefs);
#endif
            entryAccessor.PrefEntryChangedDelegate = () => { updateView = true; };

            monitoring = F8EditorPrefs.GetBool("F8Framework.Core.Editor.WatchingForChanges", true);
            if(monitoring)
                entryAccessor.StartMonitoring();

            sortOrder = (PreferencesEntrySortOrder) F8EditorPrefs.GetInt("F8Framework.Core.Editor.SortOrder", 0);
            searchfield = new MySearchField();
            searchfield.DropdownSelectionDelegate = () => { PrepareData(); };

            // Fix for serialisation issue of static fields
            if (userDefList == null)
            {
                InitReorderedList();
                PrepareData();
            }
        }

        // Handel view updates for monitored changes
        // Necessary to avoid main thread access issue
        private void Update()
        {
            if (showLoadingIndicatorOverlay)
            {
                loadingSpinnerFrame = (int)Mathf.Repeat(Time.realtimeSinceStartup * 10, 11.99f);
                PrepareData();
                Repaint();
            }

            if (updateView)
            {
                updateView = false;
                PrepareData();
                Repaint();
            }
        }

        private void OnDisable()
        {
            entryAccessor.StopMonitoring();
        }

        private void InitReorderedList()
        {
            if (prefEntryHolder == null)
            {
                var tmp = Resources.FindObjectsOfTypeAll<PreferenceEntryHolder>();
                if (tmp.Length > 0)
                {
                    prefEntryHolder = tmp[0];
                }
                else
                {
                    prefEntryHolder = ScriptableObject.CreateInstance<PreferenceEntryHolder>();
                }
            }

            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(prefEntryHolder);
            }

            userDefList = new ReorderableList(serializedObject, serializedObject.FindProperty("userDefList"), false, true, true, true);
            unityDefList = new ReorderableList(serializedObject, serializedObject.FindProperty("unityDefList"), false, true, false, false);

            relSpliterPos = F8EditorPrefs.GetFloat("F8Framework.Core.Editor.RelativeSpliterPosition", 100 / position.width);

            userDefList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "User defined");
            };
            userDefList.drawElementBackgroundCallback = OnDrawElementBackgroundCallback;
            userDefList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = GetUserDefListElementAtIndex(index, userDefList.serializedProperty);

                SerializedProperty key = element.FindPropertyRelative("m_key");
                SerializedProperty type = element.FindPropertyRelative("m_typeSelection");

                SerializedProperty value;

                // Load only necessary type
                switch ((PreferenceEntry.PrefTypes)type.enumValueIndex)
                {
                    case PreferenceEntry.PrefTypes.Float:
                        value = element.FindPropertyRelative("m_floatValue");
                        break;
                    case PreferenceEntry.PrefTypes.Int:
                        value = element.FindPropertyRelative("m_intValue");
                        break;
                    case PreferenceEntry.PrefTypes.String:
                        value = element.FindPropertyRelative("m_strValue");
                        break;
                    default:
                        value = element.FindPropertyRelative("This should never happen");
                        break;
                }

                float spliterPos = relSpliterPos * rect.width;
                rect.y += 2;

                EditorGUI.BeginChangeCheck();
                string prefKeyName = key.stringValue;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, spliterPos - 1, EditorGUIUtility.singleLineHeight), new GUIContent(prefKeyName, prefKeyName));
                GUI.enabled = false;
                EditorGUI.EnumPopup(new Rect(rect.x + spliterPos + 1, rect.y, 60, EditorGUIUtility.singleLineHeight), (PreferenceEntry.PrefTypes)type.enumValueIndex);
                GUI.enabled = !showLoadingIndicatorOverlay;
                switch ((PreferenceEntry.PrefTypes)type.enumValueIndex)
                {
                    case PreferenceEntry.PrefTypes.Float:
                        EditorGUI.DelayedFloatField(new Rect(rect.x + spliterPos + 62, rect.y, rect.width - spliterPos - 60, EditorGUIUtility.singleLineHeight), value, GUIContent.none);
                        break;
                    case PreferenceEntry.PrefTypes.Int:
                        EditorGUI.DelayedIntField(new Rect(rect.x + spliterPos + 62, rect.y, rect.width - spliterPos - 60, EditorGUIUtility.singleLineHeight), value, GUIContent.none);
                        break;
                    case PreferenceEntry.PrefTypes.String:
                        EditorGUI.DelayedTextField(new Rect(rect.x + spliterPos + 62, rect.y, rect.width - spliterPos - 60, EditorGUIUtility.singleLineHeight), value, GUIContent.none);
                        break;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    entryAccessor.IgnoreNextChange();

                    switch ((PreferenceEntry.PrefTypes)type.enumValueIndex)
                    {
                        case PreferenceEntry.PrefTypes.Float:
                            PlayerPrefs.SetFloat(key.stringValue, value.floatValue);
                            break;
                        case PreferenceEntry.PrefTypes.Int:
                            PlayerPrefs.SetInt(key.stringValue, value.intValue);
                            break;
                        case PreferenceEntry.PrefTypes.String:
                            PlayerPrefs.SetString(key.stringValue, value.stringValue);
                            break;
                    }

                    PlayerPrefs.Save();
                }
            };
            userDefList.onRemoveCallback = (ReorderableList l) =>
            {
                userDefList.ReleaseKeyboardFocus();
                unityDefList.ReleaseKeyboardFocus();

                string prefKey = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("m_key").stringValue;
                if (EditorUtility.DisplayDialog("Warning!", $"Are you sure you want to delete this entry from PlayerPrefs?\n\nEntry: {prefKey}", "Yes", "No"))
                {
                    entryAccessor.IgnoreNextChange();

                    PlayerPrefs.DeleteKey(prefKey);
                    PlayerPrefs.Save();

                    ReorderableList.defaultBehaviours.DoRemoveButton(l);
                    PrepareData();
                    GUIUtility.ExitGUI();
                }
            };
            userDefList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                foreach (PreferenceEntry.PrefTypes type in Enum.GetValues(typeof(PreferenceEntry.PrefTypes)))
                {
                    menu.AddItem(new GUIContent(type.ToString()), false, () =>
                    {
                        TextFieldDialog.OpenDialog("Create new property", "Key for the new property:", prefKeyValidatorList, (key) => {

                            entryAccessor.IgnoreNextChange();

                            switch (type)
                            {
                                case PreferenceEntry.PrefTypes.Float:
                                    PlayerPrefs.SetFloat(key, 0.0f);

                                    break;
                                case PreferenceEntry.PrefTypes.Int:
                                    PlayerPrefs.SetInt(key, 0);

                                    break;
                                case PreferenceEntry.PrefTypes.String:
                                    PlayerPrefs.SetString(key, string.Empty);

                                    break;
                            }
                            PlayerPrefs.Save();

                            PrepareData();

                            Focus();
                        }, this);

                    });
                }
                menu.ShowAsContext();
            };

            unityDefList.drawElementBackgroundCallback = OnDrawElementBackgroundCallback;
            unityDefList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = unityDefList.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty key = element.FindPropertyRelative("m_key");
                SerializedProperty type = element.FindPropertyRelative("m_typeSelection");

                SerializedProperty value;

                // Load only necessary type
                switch ((PreferenceEntry.PrefTypes)type.enumValueIndex)
                {
                    case PreferenceEntry.PrefTypes.Float:
                        value = element.FindPropertyRelative("m_floatValue");
                        break;
                    case PreferenceEntry.PrefTypes.Int:
                        value = element.FindPropertyRelative("m_intValue");
                        break;
                    case PreferenceEntry.PrefTypes.String:
                        value = element.FindPropertyRelative("m_strValue");
                        break;
                    default:
                        value = element.FindPropertyRelative("This should never happen");
                        break;
                }

                float spliterPos = relSpliterPos * rect.width;
                rect.y += 2;

                GUI.enabled = false;
                string prefKeyName = key.stringValue;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, spliterPos - 1, EditorGUIUtility.singleLineHeight), new GUIContent(prefKeyName, prefKeyName));
                EditorGUI.EnumPopup(new Rect(rect.x + spliterPos + 1, rect.y, 60, EditorGUIUtility.singleLineHeight), (PreferenceEntry.PrefTypes)type.enumValueIndex);

                switch ((PreferenceEntry.PrefTypes)type.enumValueIndex)
                {
                    case PreferenceEntry.PrefTypes.Float:
                        EditorGUI.DelayedFloatField(new Rect(rect.x + spliterPos + 62, rect.y, rect.width - spliterPos - 60, EditorGUIUtility.singleLineHeight), value, GUIContent.none);
                        break;
                    case PreferenceEntry.PrefTypes.Int:
                        EditorGUI.DelayedIntField(new Rect(rect.x + spliterPos + 62, rect.y, rect.width - spliterPos - 60, EditorGUIUtility.singleLineHeight), value, GUIContent.none);
                        break;
                    case PreferenceEntry.PrefTypes.String:
                        EditorGUI.DelayedTextField(new Rect(rect.x + spliterPos + 62, rect.y, rect.width - spliterPos - 60, EditorGUIUtility.singleLineHeight), value, GUIContent.none);
                        break;
                }
                GUI.enabled = !showLoadingIndicatorOverlay;
            };
            unityDefList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Unity defined");
            };
        }

        private void OnDrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (Event.current.type == EventType.Repaint)
            {
                ReorderableList.defaultBehaviours.elementBackground.Draw(rect, false, isActive, isActive, isFocused);
            }

            Rect spliterRect = new Rect(rect.x + relSpliterPos * rect.width, rect.y, 2, rect.height);
            EditorGUIUtility.AddCursorRect(spliterRect, MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDown && spliterRect.Contains(Event.current.mousePosition))
            {
                moveSplitterPos = true;
            }
            if(moveSplitterPos)
            {
                if (Event.current.mousePosition.x > 100 && Event.current.mousePosition.x<rect.width - 120)
                {
                    relSpliterPos = Event.current.mousePosition.x / rect.width;
                    Repaint();
                }
            }
            if (Event.current.type == EventType.MouseUp)
            {
                moveSplitterPos = false;
                F8EditorPrefs.SetFloat("F8Framework.Core.Editor.RelativeSpliterPosition", relSpliterPos);
            }
        }

        void OnGUI()
        {
            // Need to catch 'Stack empty' error on linux
            try
            {
                if (showLoadingIndicatorOverlay)
                {
                    GUI.enabled = false;
                }

                Color defaultColor = GUI.contentColor;
                if (!EditorGUIUtility.isProSkin)
                {
                    GUI.contentColor = Styles.Colors.DarkGray;
                }

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal(EditorStyles.toolbar);

                EditorGUI.BeginChangeCheck();
                searchTxt = searchfield.OnToolbarGUI(searchTxt);
                if (EditorGUI.EndChangeCheck())
                {
                    PrepareData(false);
                }

                GUILayout.FlexibleSpace();

                EditorGUIUtility.SetIconSize(new Vector2(14.0f, 14.0f));

                GUIContent sortOrderContent;
                switch (sortOrder)
                {
                    case PreferencesEntrySortOrder.Asscending:
                        sortOrderContent = new GUIContent(ImageManager.SortAsscending, "Ascending sorted");
                        break;
                    case PreferencesEntrySortOrder.Descending:
                        sortOrderContent = new GUIContent(ImageManager.SortDescending, "Descending sorted");
                        break;
                    case PreferencesEntrySortOrder.None:
                    default:
                        sortOrderContent = new GUIContent(ImageManager.SortDisabled, "Not sorted");
                        break;
                }

                if (GUILayout.Button(sortOrderContent, EditorStyles.toolbarButton))
                {
                    
                    sortOrder++;
                    if((int) sortOrder >= Enum.GetValues(typeof(PreferencesEntrySortOrder)).Length)
                    {
                        sortOrder = 0;
                    }
                    F8EditorPrefs.SetInt("F8Framework.Core.Editor.SortOrder", (int) sortOrder);
                    PrepareData(false);
                }

                GUIContent watcherContent = (entryAccessor.IsMonitoring()) ? new GUIContent(ImageManager.Watching, "Watching changes") : new GUIContent(ImageManager.NotWatching, "Not watching changes");
                if (GUILayout.Button(watcherContent, EditorStyles.toolbarButton))
                {
                    monitoring = !monitoring;

                    F8EditorPrefs.SetBool("F8Framework.Core.Editor.WatchingForChanges", monitoring);

                    if (monitoring)
                        entryAccessor.StartMonitoring();
                    else
                        entryAccessor.StopMonitoring();

                    Repaint();
                }
                if (GUILayout.Button(new GUIContent(ImageManager.Refresh, "Refresh"), EditorStyles.toolbarButton))
                {
                    PlayerPrefs.Save();
                    PrepareData();
                }
                if (GUILayout.Button(new GUIContent(ImageManager.Trash, "Delete all"), EditorStyles.toolbarButton))
                {
                    if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete ALL entries from PlayerPrefs?\n\nUse with caution! Unity defined keys are affected too.", "Yes", "No"))
                    {
                        PlayerPrefs.DeleteAll();
                        PrepareData();
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUIUtility.SetIconSize(new Vector2(0.0f, 0.0f));

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Box(ImageManager.GetOsIcon(), Styles.icon);
                GUILayout.TextField(platformPathPrefix + Path.DirectorySeparatorChar + pathToPrefs, GUILayout.MinWidth(200));

                GUILayout.EndHorizontal();

                scrollPos = GUILayout.BeginScrollView(scrollPos);
                serializedObject.Update();
                userDefList.DoLayoutList();
                serializedObject.ApplyModifiedProperties();

                GUILayout.FlexibleSpace();

                showSystemGroup = EditorGUILayout.Foldout(showSystemGroup, new GUIContent("Show System"));
                if (showSystemGroup)
                {
                    unityDefList.DoLayoutList();
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                GUI.enabled = true;

                if (showLoadingIndicatorOverlay)
                {
                    GUILayout.BeginArea(new Rect(position.size.x * 0.5f - 30, position.size.y * 0.5f - 25, 60, 50), GUI.skin.box);
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageManager.SpinWheelIcons[loadingSpinnerFrame], Styles.icon);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Loading");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.FlexibleSpace();
                    GUILayout.EndArea();
                }

                GUI.contentColor = defaultColor;
            }
            catch (InvalidOperationException)
            { }
        }

        private void PrepareData(bool reloadKeys = true)
        {
            prefEntryHolder.ClearLists();

            LoadKeys(out userDef, out unityDef, reloadKeys);

            CreatePrefEntries(userDef, ref prefEntryHolder.userDefList);
            CreatePrefEntries(unityDef, ref prefEntryHolder.unityDefList);

            // Clear cache
            userDefListCache = new SerializedProperty[prefEntryHolder.userDefList.Count];
        }

        private void CreatePrefEntries(string[] keySource, ref List<PreferenceEntry> listDest)
        {
            if (!string.IsNullOrEmpty(searchTxt) && searchfield.SearchMode == MySearchField.SearchModePreferencesEditorWindow.Key)
            {
                keySource = keySource.Where((keyEntry) => keyEntry.ToLower().Contains(searchTxt.ToLower())).ToArray();
            }

            foreach (string key in keySource)
            {
                var entry = new PreferenceEntry();
                entry.m_key = key;

                string s = PlayerPrefs.GetString(key, ERROR_VALUE_STR);

                if (s != ERROR_VALUE_STR)
                {
                    entry.m_strValue = s;
                    entry.m_typeSelection = PreferenceEntry.PrefTypes.String;
                    listDest.Add(entry);
                    continue;
                }

                float f = PlayerPrefs.GetFloat(key, float.NaN);
                if (!float.IsNaN(f))
                {
                    entry.m_floatValue = f;
                    entry.m_typeSelection = PreferenceEntry.PrefTypes.Float;
                    listDest.Add(entry);
                    continue;
                }

                int i = PlayerPrefs.GetInt(key, ERROR_VALUE_INT);
                if (i != ERROR_VALUE_INT)
                {
                    entry.m_intValue = i;
                    entry.m_typeSelection = PreferenceEntry.PrefTypes.Int;
                    listDest.Add(entry);
                    continue;
                }
            }

            if (!string.IsNullOrEmpty(searchTxt) && searchfield.SearchMode == MySearchField.SearchModePreferencesEditorWindow.Value)
            {
                listDest = listDest.Where((preferenceEntry) => preferenceEntry.ValueAsString().ToLower().Contains(searchTxt.ToLower())).ToList<PreferenceEntry>();
            }

            switch(sortOrder)
            {
                case PreferencesEntrySortOrder.Asscending:
                    listDest.Sort((PreferenceEntry x, PreferenceEntry y) => { return x.m_key.CompareTo(y.m_key); });
                    break;
                case PreferencesEntrySortOrder.Descending:
                    listDest.Sort((PreferenceEntry x, PreferenceEntry y) => { return y.m_key.CompareTo(x.m_key); });
                    break;
            }
        }

        private void LoadKeys(out string[] userDef, out string[] unityDef, bool reloadKeys)
        {
            string[] keys = entryAccessor.GetKeys(reloadKeys);

            //keys.ToList().ForEach( e => { Debug.Log(e); } );

            // Seperate keys int unity defined and user defined
            Dictionary<bool, List<string>> groups = keys
                .GroupBy( (key) => key.StartsWith("unity.") || key.StartsWith("UnityGraphicsQuality") )
                .ToDictionary( (g) => g.Key, (g) => g.ToList() );

            unityDef = (groups.ContainsKey(true)) ? groups[true].ToArray() : new string[0];
            userDef = (groups.ContainsKey(false)) ? groups[false].ToArray() : new string[0];
        }

        private SerializedProperty GetUserDefListElementAtIndex(int index, SerializedProperty ListProperty)
        {
            UnityEngine.Assertions.Assert.IsTrue(ListProperty.isArray, "Given 'ListProperts' is not type of array");

            if (userDefListCache[index] == null)
            {
                userDefListCache[index] = ListProperty.GetArrayElementAtIndex(index);
            }
            return userDefListCache[index];
        }

#if (UNITY_EDITOR_LINUX || UNITY_EDITOR_OSX)
        private string MakeValidFileName(string unsafeFileName)
        {
            string normalizedFileName = unsafeFileName.Trim().Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            // We need to use a TextElementEmumerator in order to support UTF16 characters that may take up more than one char(case 1169358)
            TextElementEnumerator charEnum = StringInfo.GetTextElementEnumerator(normalizedFileName);
            while (charEnum.MoveNext())
            {
                string c = charEnum.GetTextElement();
                if (c.Length == 1 && invalidFilenameChars.Contains(c[0]))
                {
                    stringBuilder.Append('_');
                    continue;
                }
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c, 0);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
#endif
    }
}

public class MySearchField : SearchField
{
    public enum SearchModePreferencesEditorWindow { Key, Value }

    public SearchModePreferencesEditorWindow SearchMode { get; private set; }

    public Action DropdownSelectionDelegate;

    public new string OnGUI(
        Rect rect,
        string text,
        GUIStyle style,
        GUIStyle cancelButtonStyle,
        GUIStyle emptyCancelButtonStyle)
    {
        style.padding.left = 17;
        Rect ContextMenuRect = new Rect(rect.x, rect.y, 10, rect.height);

        // Add interactive area
        EditorGUIUtility.AddCursorRect(ContextMenuRect, MouseCursor.Text);
        if (Event.current.type == EventType.MouseDown && ContextMenuRect.Contains(Event.current.mousePosition))
        {
            void OnDropdownSelection(object parameter)
            {
                SearchMode = (SearchModePreferencesEditorWindow) Enum.Parse(typeof(SearchModePreferencesEditorWindow), parameter.ToString());
                DropdownSelectionDelegate();
            }

            GenericMenu menu = new GenericMenu();
            foreach(SearchModePreferencesEditorWindow EnumIt in Enum.GetValues(typeof(SearchModePreferencesEditorWindow)))
            {
                String EnumName = Enum.GetName(typeof(SearchModePreferencesEditorWindow), EnumIt);
                menu.AddItem(new GUIContent(EnumName), SearchMode == EnumIt, OnDropdownSelection, EnumName);
            }

            menu.DropDown(rect);
        }

        // Render original search field
        String result = base.OnGUI(rect, text, style, cancelButtonStyle, emptyCancelButtonStyle);

        // Render additional images
        GUIStyle ContexMenuOverlayStyle = GUIStyle.none;
        ContexMenuOverlayStyle.contentOffset = new Vector2(9, 5);
        GUI.Box(new Rect(rect.x, rect.y, 5, 5), EditorGUIUtility.IconContent("d_ProfilerTimelineDigDownArrow@2x"), ContexMenuOverlayStyle);

        if (!HasFocus() && String.IsNullOrEmpty(text))
        {
            GUI.enabled = false;
            GUI.Label(new Rect(rect.x + 14, rect.y, 40, rect.height), Enum.GetName(typeof(SearchModePreferencesEditorWindow), SearchMode));
            GUI.enabled = true;
        }
        ContexMenuOverlayStyle.contentOffset = new Vector2();
        return result;
    }

    public new string OnToolbarGUI(string text, params GUILayoutOption[] options) => this.OnToolbarGUI(GUILayoutUtility.GetRect(29f, 200f, 18f, 18f, EditorStyles.toolbarSearchField, options), text);
    public new string OnToolbarGUI(Rect rect, string text) => this.OnGUI(rect, text, EditorStyles.toolbarSearchField, EditorStyles.toolbarButton, EditorStyles.toolbarButton);
}
