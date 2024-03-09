using System.Collections.Generic;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    [System.Serializable]
    public class PreferenceEntryHolder : ScriptableObject
    {
        public List<PreferenceEntry> userDefList;
        public List<PreferenceEntry> unityDefList;

        private void OnEnable()
        {
            hideFlags = HideFlags.DontSave;
            if (userDefList == null)
                userDefList = new List<PreferenceEntry>();
            if (unityDefList == null)
                unityDefList = new List<PreferenceEntry>();
        }

        public void ClearLists()
        {
            if (userDefList != null)
                userDefList.Clear();
            if (unityDefList != null)
                unityDefList.Clear();
        }
    }
}