using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F8Framework.F8ExcelDataClass;
using UnityEngine;

namespace F8Framework.Core
{
    // 本地化工具类
    public class Localization : ModuleSingleton<Localization>, IModule
    {
        // 本地化器列表
        static readonly List<LocalizerBase> Localizers = new List<LocalizerBase>();
        // 语言列表
        public static List<string> LanguageList { get; private set; } = new List<string>();
        // 本地化字符串字典
        static readonly Dictionary<string, List<string>> LocalizedStrings = new Dictionary<string, List<string>>();
        // 当前语言名称
        public static string CurrentLanguageName { get; set; }
        // 当前语言索引
        public static int CurrentLanguageIndex => GetLanguageIndex(CurrentLanguageName);

        private LocalizedStrings p_LocalizedStrings;
        public void OnInit(object createParam)
        {
            Load();
        }
        
        /// <summary>
        /// 加载本地化字符串到内存。
        /// </summary>
        public static void Load(bool force = false)
        {
            if (LocalizedStrings.Count > 0 && force == false)
            {
                return;
            }
            LocalizedStrings.Clear();
            
            ReadExcel.Instance.LoadAllExcelData();
            
            Dictionary<int, LocalizedStringsItem> tb;
            string _class = "F8DataManager";
            string method= "GetLocalizedStrings";
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var type = allAssemblies.SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type1 => type1.Name == _class);
            if (type == null)
            {
                LogF8.LogError("需要检查是否有正确生成F8DataManager.cs!");
                return;
            }
            else
            {
                //通过反射，获取单例的实例
                var property = type.BaseType.GetProperty("Instance");
                var instance = property.GetValue(null,null);
                var myMethodExists = type.GetMethod(method);
                tb = myMethodExists.Invoke(instance, null) as Dictionary<int, LocalizedStringsItem>;
                LogF8.LogConfig("<color=green>获取本地化表格成功！</color>");
            }
            
            foreach (var item in tb.Values)
            {
                List<string> list = new List<string>();
            
                Type type2 = item.GetType();
                FieldInfo[] fields = type2.GetFields();
        
                foreach (var field in fields)
                {
                    // 排除 id 和 TextID 字段
                    if (field.Name != "id" && field.Name != "TextID")
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
                            LogF8.LogConfig($"字段：\"<b>{item.TextID}</b>\" 的值为空");
                        }
                    }
                }
                if (LocalizedStrings.ContainsKey(item.TextID))
                {
                    LogF8.LogError($"本地化表 ID \"<b>{item.TextID}</b>\" 出现重复，请修改。");
                    continue;
                }
                LocalizedStrings.TryAdd(item.TextID, list);
            }

            LocalizationSettings.LoadLanguageSettings();
            ChangeLanguage(CurrentLanguageName ?? "");
        }

        /// <summary>
        /// 更改当前语言。
        /// </summary>
        /// <param name="languageName">例如："日语"，"英语"</param>
        public static void ChangeLanguage(string languageName)
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
        public static string ActivatePreviousLanguage()
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
        public static string ActivateNextLanguage()
        {
            var nextIndex = (int)Mathf.Repeat(CurrentLanguageIndex + 1, LanguageList.Count);
            ChangeLanguage(LanguageList[nextIndex]);
            InjectAll();
            return LanguageList[nextIndex];
        }

        // 获取语言索引
        static int GetLanguageIndex(string languageName)
        {
            var i = LanguageList.FindIndex(s => s.Contains(languageName));
            if (i == -1) LogF8.LogError($"不可用的语言名称: {languageName}");
            return i;
        }

        // 添加本地化器
        public static void AddLocalizer(LocalizerBase localizer)
        {
            Localizers.Add(localizer);
        }

        // 移除本地化器
        public static void RemoveLocalizer(LocalizerBase localizer)
        {
            Localizers.Remove(localizer);
        }

        /// <summary>
        /// 重新注入所有注入器的字符串。
        /// </summary>
        public static void InjectAll()
        {
            foreach (var localizer in Localizers) localizer.Localize();
        }

        /// <summary>
        /// 检查当前数据库是否具有特定的文本 ID。
        /// </summary>
        /// <param name="id">文本 ID</param>
        /// <returns></returns>
        public static bool Has(string id)
        {
            return LocalizedStrings.ContainsKey(id);
        }

        /// <summary>
        /// 根据文本 ID 获取本地化字符串。
        /// </summary>
        /// <param name="id">文本 ID</param>
        /// <param name="p">Format</param>
        /// <returns>本地化文本</returns>
        public static string GetTextFromId(string id, params object[] p)
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
        public static string GetTextFromIdLanguage(string id, string languageName, params object[] p)
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
        public static Dictionary<string, string> GetDictionaryFromId(string id)
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
        public static List<string> GetAllIds()
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