using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public string CurrentLanguageName { get; set; }
        // 当前语言索引
        public int CurrentLanguageIndex => GetLanguageIndex(CurrentLanguageName);
        
        public void OnInit(object createParam)
        {
            Load();
        }

        public void LoadInEditor(bool force = false)
        {
#if UNITY_EDITOR
            if (LocalizedStrings.Count > 0 && force == false)
            {
                return;
            }
            LocalizedStrings.Clear();
            
            if (Application.isPlaying)
            {
                Util.Assembly.InvokeMethod("F8DataManager", "LoadLocalizedStrings", new object[] { });
            }
            else
            {
                try
                {
                    Util.Assembly.InvokeMethod("F8DataManager", "GetLocalizedStrings", new object[] { });
                }
                catch
                {
                    ReadExcel.Instance.LoadAllExcelData();
                }
            }
            LoadSuccess();
#endif
        }
        
        /// <summary>
        /// 加载本地化字符串到内存。
        /// </summary>
        public void Load(bool force = false)
        {
            if (LocalizedStrings.Count > 0 && force == false)
            {
                return;
            }
            LocalizedStrings.Clear();
            
            // 必须先加载本地化配置表
#if UNITY_WEBGL
            LogF8.LogConfig("（提示）由于WebGL异步加载完本地化表，请在创建本地化模块之前加上：yield return F8DataManager.Instance.LoadLocalizedStringsIEnumerator();");
            LoadSuccess();
#else
            Util.Assembly.InvokeMethod("F8DataManager", "LoadLocalizedStrings", new object[] { });
            LoadSuccess();
#endif
        }

        private void LoadSuccess()
        {
            var tb = Util.Assembly.InvokeMethod("F8DataManager", "GetLocalizedStrings", new object[] { }) as IDictionary;
            
            LogF8.LogConfig("<color=green>获取本地化表格成功！</color>");
            
            foreach (var item in tb.Values)
            {
                // 反射获取 TextID 属性的值
                Type itemType = item.GetType();
                FieldInfo textIDProperty = itemType.GetField("TextID");
                if (textIDProperty == null)
                {
                    LogF8.LogError("无法获取 TextID 属性，请检查对象类型。");
                    continue;
                }

                object textIDValue = textIDProperty.GetValue(item);
                if (textIDValue == null)
                {
                    continue;
                }

                string id = string.Empty;
                FieldInfo fieldInfoId = null;
                foreach (var field in itemType.GetFields())
                {
                    if (string.Equals(field.Name, "id", StringComparison.OrdinalIgnoreCase))
                    {
                        fieldInfoId = field;
                        break;
                    }  
                }
                FieldInfo idProperty = fieldInfoId;
                object idValue = idProperty?.GetValue(item);
                if (idProperty != null && idValue != null)
                {
                    id = idValue.ToString();
                }
                
                string textID = textIDValue.ToString();

                List<string> list = new List<string>();

                Type type2 = item.GetType();
                FieldInfo[] fields = type2.GetFields();

                foreach (var field in fields)
                {
                    // 排除 id 和 TextID 字段
                    if (!field.Name.Equals("id", StringComparison.OrdinalIgnoreCase) && field.Name != "TextID")
                    {
                        if (!LanguageList.Contains(field.Name))
                        {
                            LanguageList.Add(field.Name);
                        }

                        object value = field.GetValue(item); // 这里传递的是 item 对象
                        if (value != null)
                        {
                            list.Add(value.ToString());
                        }
                        else
                        {
                            // 如果字段的值为 null，你可以选择添加一个默认值，或者进行其他处理
                            list.Add("");
                            LogF8.LogConfig($"本地化表id：\"<b>{id}</b>\"，字段：\"<b>{textID}</b>\" 的值为空");
                        }
                    }
                }

                if (LocalizedStrings.ContainsKey(textID))
                {
                    LogF8.LogError($"本地化表id：\"<b>{id}</b>\"，字段：\"<b>{textID}</b>\" 出现重复，请修改。");
                    continue;
                }

                LocalizedStrings.TryAdd(textID, list);

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
        }

        /// <summary>
        /// 激活上一个语言。
        /// </summary>
        /// <returns>激活的语言名称</returns>
        public string ActivatePreviousLanguage()
        {
            var prevIndex = (int)Mathf.Repeat(CurrentLanguageIndex - 1, LanguageList.Count);
            ChangeLanguage(LanguageList[prevIndex]);
            InjectAll();
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
            InjectAll();
            return LanguageList[nextIndex];
        }

        // 获取语言索引
        int GetLanguageIndex(string languageName)
        {
            var i = LanguageList.FindIndex(s => s.Contains(languageName));
            if (i == -1) LogF8.LogError($"不可用的语言名称: {languageName}");
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
            base.Destroy();
        }
    }
}