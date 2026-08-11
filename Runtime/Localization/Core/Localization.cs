using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace F8Framework.Core
{
    // 本地化工具类
    public class Localization : ModuleSingleton<Localization>, IModule
    {
        // 本地化器列表
        readonly List<LocalizerBase> Localizers = new List<LocalizerBase>();
        // 语言列表
        public List<string> LanguageList { get; private set; } = new List<string>();
        // 本地化字符串字典
        readonly Dictionary<string, List<string>> LocalizedStrings = new Dictionary<string, List<string>>();
        // 当前语言名称
        public string CurrentLanguageName { get; internal set; }
        // 当前语言索引
        public int CurrentLanguageIndex => GetLanguageIndex(CurrentLanguageName);
        
        public void OnInit(object createParam)
        {
            Load(createParam as IDictionary);
        }

        public void LoadInEditor()
        {
#if UNITY_EDITOR
            if (LocalizedStrings.Count > 0)
            {
                return;
            }
            LocalizedStrings.Clear();
            LanguageList.Clear();
            IDictionary tb = null;
            if (Application.isPlaying)
            {
                try
                {
                    Util.Assembly.InvokeMethod("F8DataManager", "LoadLocalizedStrings", "EditorInstance", new object[] { });
                    tb = Util.Assembly.InvokeMethod("F8DataManager", "GetLocalizedStrings", "EditorInstance", new object[] { }) as IDictionary;
                }
                catch
                {
                    LogF8.LogError("缺少本地化表或加载本地化表失败！");
                }
            }
            else
            {
                try
                {
                    if (!ConfigDataSourceRegistry.TryLoadAll(out Dictionary<string, object> configData))
                    {
                        throw new InvalidOperationException("没有可用的配置数据源。");
                    }

                    Util.Assembly.InvokeMethod(
                        "F8DataManager",
                        "RuntimeLoadAll",
                        "EditorInstance",
                        new object[] { configData });
                    tb = Util.Assembly.InvokeMethod("F8DataManager", "GetLocalizedStrings", "EditorInstance", new object[] { }) as IDictionary;
                }
                catch
                {
                    LogF8.LogError("缺少本地化表或加载本地化表失败！");
                }
            }
            
            LoadSuccess(tb);
#endif
        }
        
        /// <summary>
        /// 加载本地化字符串到内存。
        /// </summary>
        public void Load(IDictionary createParam)
        {
            if (LocalizedStrings.Count > 0)
            {
                return;
            }
            LocalizedStrings.Clear();
            LanguageList.Clear();
            
            // 必须先加载本地化配置表
            LoadSuccess(createParam);
        }

        private void LoadSuccess(IDictionary tb)
        {
            if (tb == null)
            {
                LogF8.LogError("缺少本地化表，需在初始化本地化模块时传入，参考 GameLauncher.cs");
                return;
            }
            LogF8.LogConfig("<color=green>获取本地化表格成功！</color>");
            
            foreach (object value in tb.Values)
            {
                if (!(value is ILocalizationItem item))
                {
                    LogF8.LogError("本地化配置项未实现 ILocalizationItem，请重新生成配置代码。");
                    continue;
                }

                string textID = item.TextId;
                if (string.IsNullOrEmpty(textID))
                {
                    LogF8.LogError("本地化配置项的 TextID 不能为空。");
                    continue;
                }

                IReadOnlyList<string> languageNames = item.LanguageNames;
                IReadOnlyList<string> languageValues = item.LanguageValues;
                if (languageNames == null || languageValues == null)
                {
                    LogF8.LogError($"本地化表id：\"<b>{item.Id}</b>\"，字段：\"<b>{textID}</b>\" 缺少语言数据。");
                    continue;
                }

                if (languageNames.Count == 0 || languageNames.Count != languageValues.Count)
                {
                    LogF8.LogError($"本地化表id：\"<b>{item.Id}</b>\"，字段：\"<b>{textID}</b>\" 的语言名称和值数量不一致。");
                    continue;
                }

                if (LanguageList.Count == 0)
                {
                    LanguageList.AddRange(languageNames);
                }
                else
                {
                    bool languageOrderMatches = LanguageList.Count == languageNames.Count;
                    for (int i = 0; languageOrderMatches && i < languageNames.Count; i++)
                    {
                        languageOrderMatches = string.Equals(
                            LanguageList[i],
                            languageNames[i],
                            StringComparison.Ordinal);
                    }

                    if (!languageOrderMatches)
                    {
                        LogF8.LogError($"本地化表id：\"<b>{item.Id}</b>\"，字段：\"<b>{textID}</b>\" 的语言顺序不一致。");
                        continue;
                    }
                }

                if (LocalizedStrings.ContainsKey(textID))
                {
                    LogF8.LogError($"本地化表id：\"<b>{item.Id}</b>\"，字段：\"<b>{textID}</b>\" 出现重复，请修改。");
                    continue;
                }

                List<string> localizedValues = new List<string>(languageValues.Count);
                for (int i = 0; i < languageValues.Count; i++)
                {
                    string languageValue = languageValues[i];
                    if (languageValue == null)
                    {
                        languageValue = string.Empty;
                        LogF8.LogConfig($"本地化表id：\"<b>{item.Id}</b>\"，字段：\"<b>{textID}</b>\"，语言：\"<b>{languageNames[i]}</b>\" 的值为空");
                    }

                    localizedValues.Add(languageValue);
                }

                LocalizedStrings.Add(textID, localizedValues);
            }

            if (LanguageList.Count > 0)
            {
                LocalizationSettings.LoadLanguageSettings();
                ChangeLanguage(CurrentLanguageName ?? "");
            }
        }
        
        /// <summary>
        /// 更改当前语言。
        /// </summary>
        /// <param name="languageName">例如："日语"，"英语"</param>
        public void ChangeLanguage(string languageName)
        {
            var languageIndex = 0;
            if (languageName != "")
            {
                languageIndex = GetLanguageIndex(languageName);
            }

            CurrentLanguageName = LanguageList[languageIndex];
            LocalizationSettings.SaveLanguageSettings();
            InjectAll();
        }

        /// <summary>
        /// 激活上一个语言。
        /// </summary>
        /// <returns>激活的语言名称</returns>
        public string ActivatePreviousLanguage()
        {
            var prevIndex = (int)Mathf.Repeat(CurrentLanguageIndex - 1, LanguageList.Count);
            ChangeLanguage(LanguageList[prevIndex]);
            return LanguageList[prevIndex];
        }

        /// <summary>
        /// 激活下一个语言。
        /// </summary>
        /// <returns>激活的语言名称</returns>
        public string ActivateNextLanguage()
        {
            var nextIndex = (int)Mathf.Repeat(CurrentLanguageIndex + 1, LanguageList.Count);
            ChangeLanguage(LanguageList[nextIndex]);
            return LanguageList[nextIndex];
        }

        // 获取语言索引
        int GetLanguageIndex(string languageName)
        {
            var i = LanguageList.FindIndex(s => s.Contains(languageName));
            if (i == -1)
            {
                LogF8.LogError($"不可用的语言名称: {languageName}");
                return 0;
            }
            return i;
        }

        // 添加本地化器
        public void AddLocalizer(LocalizerBase localizer)
        {
            Localizers.Add(localizer);
        }

        // 移除本地化器
        public void RemoveLocalizer(LocalizerBase localizer)
        {
            Localizers.Remove(localizer);
        }

        /// <summary>
        /// 重新注入所有注入器的字符串。
        /// </summary>
        public void InjectAll()
        {
            foreach (var localizer in Localizers) localizer.Localize();
        }

        /// <summary>
        /// 检查当前数据库是否具有特定的文本 ID。
        /// </summary>
        /// <param name="id">文本 ID</param>
        /// <returns></returns>
        public bool Has(string id)
        {
            return LocalizedStrings.ContainsKey(id);
        }

        /// <summary>
        /// 根据文本 ID 获取本地化字符串。
        /// </summary>
        /// <param name="id">文本 ID</param>
        /// <param name="p">Format</param>
        /// <returns>本地化文本</returns>
        public string GetTextFromId(string id, params object[] p)
        {
            return GetTextFromIdLanguage(id, CurrentLanguageName, p);
        }

        /// <summary>
        /// 根据文本 ID 和特定语言获取本地化字符串。
        /// </summary>
        /// <param name="id">文本 ID</param>
        /// <param name="languageName">语言名称</param>
        /// <param name="p">Format</param>
        /// <returns>本地化文本</returns>
        public string GetTextFromIdLanguage(string id, string languageName, params object[] p)
        {
            if (!LocalizedStrings.ContainsKey(id)) return null;
            var languageIndex = GetLanguageIndex(languageName);

            if (p is { Length: > 0 })
                return string.Format(LocalizedStrings[id][languageIndex], p);
            else
                return LocalizedStrings[id][languageIndex];
        }

        /// <summary>
        /// 获取包含特定 ID 的所有语言的字符串字典。
        /// </summary>
        /// <param name="id">文本 ID</param>
        /// <returns>包含本地化字符串的字典</returns>
        public Dictionary<string, string> GetDictionaryFromId(string id)
        {
            if (!LocalizedStrings.ContainsKey(id)) return null;

            var dict = new Dictionary<string, string>();

            foreach (var language in LanguageList)
            {
                var text = GetTextFromIdLanguage(id, language);
                dict.Add(language, text);
            }

            return dict;
        }

        // 获取所有 ID 列表
        public List<string> GetAllIds()
        {
            return LocalizedStrings.Keys.ToList();
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnTermination()
        {
            Localizers.Clear();
            LanguageList.Clear();
            LocalizedStrings.Clear();
            CurrentLanguageName = null;
            base.Destroy();
        }
    }
}
