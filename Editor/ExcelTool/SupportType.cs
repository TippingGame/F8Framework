using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using F8Framework.Core;
using F8Framework.Core.Editor;
using F8Framework.ExcelData;

//脚本生成器
namespace F8Framework.ExcelData.Editor
{
    public class ScriptGenerator
    {
        private const string LocalizationTableName = "LocalizedStrings";

        private string[] Names;
        private string[] Types;
        private List<ReadExcel.ConfigData> ConfigDatas;
        private string ClassName;
        private string InputPath;
        private readonly Dictionary<string, EnumDefinition> enumDefinitions = new Dictionary<string, EnumDefinition>();
        private readonly List<string> enumDefinitionOrder = new List<string>();

        private class EnumDefinition
        {
            public string Source;
            public bool HasFullDefinition;
        }

        public ScriptGenerator(string inputPath, string className, List<ReadExcel.ConfigData> configDatas)
        {
            InputPath = inputPath;
            ClassName = className;
            Names = configDatas.Select(x => x.Name).ToArray();
            Types = configDatas.Select(x => x.Type).ToArray();
            ConfigDatas = configDatas;
        }

        //开始生成脚本
        public string Generate()
        {
            if (Types == null || Names == null || ClassName == null)
            {
                throw new Exception("表名:" + ClassName +
                                    "\n表名为空:" + (ClassName == null) +
                                    "\n字段类型为空:" + (Types == null) +
                                    "\n字段名为空:" + (Names == null));
            }
            if (!string.Equals(ClassName, LocalizationTableName, StringComparison.Ordinal) &&
                string.Equals(ClassName, LocalizationTableName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"本地化表 Sheet 名大小写必须精确为 {LocalizationTableName}。");
            }
            // 使用LINQ的GroupBy和Any来找出重复的元素  
            var duplicates = Names
                .Where(name => !string.IsNullOrEmpty(name))  // 检查是否为 null 或空字符串
                .GroupBy(name => name)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();
            if (duplicates.Count > 0)
            {
                throw new Exception("表名为：" + ClassName + "，字段名重复：" + string.Join("，",  duplicates));
            }
            return CreateCode(ClassName, Types, Names);
        }
        
        private string GetIdType()
        {
            for (int i = 0; i < Names.Length; i++)
            {
                if (Names[i].Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    return ReadExcel.GetTrueType(Types[i], ClassName, InputPath);
                }
            }

            return "int";
        }
        
        //创建代码。   
        private string CreateCode(string ClassName, string[] types, string[] fields)
        {
            //生成类
            StringBuilder classSource = new StringBuilder();
            classSource.Append("/*Auto create\n");
            classSource.Append("Don't Edit it*/\n");
            classSource.Append("\n");
            classSource.Append("using System;\n");
            classSource.Append("using System.Collections.Generic;\n");
            classSource.Append("using F8Framework.Core;\n");
            classSource.Append("using UnityEngine.Scripting;\n");
            classSource.Append("using UnityEngine;\n\n");
            classSource.Append("namespace " + ExcelDataTool.CODE_NAMESPACE + "\n");
            classSource.Append("{\n");
            classSource.Append("\t[Serializable]\n");
            bool isLocalizationItem = string.Equals(
                ClassName,
                LocalizationTableName,
                StringComparison.Ordinal);
            classSource.Append("\tpublic class " + ClassName + "Item"); //表里每一条数据的类型名为表类型名加Item
            if (isLocalizationItem)
            {
                classSource.Append(" : ILocalizationItem");
            }
            classSource.Append("\n");
            classSource.Append("\t{\n");
            enumDefinitions.Clear();
            enumDefinitionOrder.Clear();

            string exportFormat = ExcelDataSettings.ExportFormat;
            Dictionary<string, string> binaryRegisters = exportFormat == ExcelDataSettings.BinaryFormat ? new Dictionary<string, string>() : null;
            List<string> generatedFields = new List<string>();
            bool hasVariant = false;
            //设置成员
            for (int i = 0; i < fields.Length; ++i)
            {
                // 检查这个字段是否有变体
                if (ConfigDatas[i].VariantInfo != null)
                {
                    if (ConfigDatas[i].VariantInfo.HasVariant != true) continue;
                    hasVariant = true;
                    string fieldType = ReadExcel.GetTrueType(types[i], ClassName, InputPath, true, binaryRegisters);
                    classSource.Append("\t\t[Preserve]\n");
                    classSource.Append("\t\tpublic Dictionary<System.String, " + fieldType + "> _" + fields[i] + "Variants = new Dictionary<System.String, " + fieldType + ">();\n");
                    classSource.Append("\t\t[F8Framework.Core.BinaryIgnore]\n");
                    classSource.Append("\t\t[LitJson.JsonIgnore]\n");
                    classSource.Append("\t\t[Preserve]\n");
                    classSource.Append($"\t\tpublic {fieldType} {fields[i] } => _{fields[i]}Variants.GetValueOrDefault(!string.IsNullOrEmpty({ExcelDataTool.CODE_NAMESPACE}.{ClassName}.VariantName) ? {ExcelDataTool.CODE_NAMESPACE}.{ClassName}.VariantName : (F8DataManager.Instance.VariantName ?? string.Empty), _{fields[i]}Variants[string.Empty]);\n");
                    binaryRegisters?.TryAdd("System.Collections.Generic.Dictionary<System.String, " + fieldType + ">", SupportType.DICTIONARY);
                    generatedFields.Add(fields[i]);
                }
                else
                {
                    // 普通字段
                    string propertySource = PropertyString(types[i], fields[i], binaryRegisters);
                    classSource.Append(propertySource);
                    if (!string.IsNullOrEmpty(propertySource))
                    {
                        generatedFields.Add(fields[i]);
                    }
                }
                
                // 枚举定义
                if (!string.IsNullOrEmpty(types[i]))
                {
                    PropertyEnum(types[i], ClassName, InputPath, true, binaryRegisters);
                }
            }

            if (isLocalizationItem)
            {
                AppendLocalizationItemImplementation(classSource, ClassName, generatedFields);
            }
            
            classSource.Append("\t}\n");

            //生成Container
            classSource.Append("\t\n");
            classSource.Append("\t[Serializable]\n");
            classSource.Append("\tpublic class " + ClassName + "\n");
            classSource.Append("\t{\n");
            string idType = "int";
            for (int i = 0; i < fields.Length; i++)
            {
                if (string.Equals(fields[i], "id", StringComparison.OrdinalIgnoreCase))
                {
                    idType = ReadExcel.GetTrueType(types[i], ClassName, InputPath);
                    break;
                }
            }

            if (hasVariant)
            {
                classSource.Append("\t\t[Preserve]\n");
                classSource.Append("\t\tpublic static string VariantName;\n");
            }
            classSource.Append("\t\t[Preserve]\n");
            classSource.Append("\t\tpublic static void PreRegister()\n");
            classSource.Append("\t\t{\n");
            binaryRegisters?.TryAdd($"{ExcelDataTool.CODE_NAMESPACE}.{ClassName}Item", SupportType.OBJ);
            binaryRegisters?.TryAdd($"System.Collections.Generic.Dictionary<{idType}, {ExcelDataTool.CODE_NAMESPACE}.{ClassName}Item>", SupportType.DICTIONARY);
            binaryRegisters?.TryAdd($"{ExcelDataTool.CODE_NAMESPACE}.{ClassName}", SupportType.OBJ);
            if (binaryRegisters != null)
            {
                foreach (var item in binaryRegisters)
                {
                    if (item.Value == SupportType.ARRAY)
                    {
                        string arrayType = item.Key; // 例如 "float[]"
                        string elementType = arrayType.EndsWith("[]") 
                            ? arrayType.Substring(0, arrayType.Length - 2) 
                            : arrayType;
                        classSource.Append(
                            $"\t\t\tTypeHandlerFactory.PreRegister<{arrayType}>(new F8Framework.Core.ArrayHandler<{elementType}>());\n");
                    }
                    else if (item.Value == SupportType.LIST)
                    {
                        string listType = item.Key; // 例如 "List<int>"
                        int start = listType.IndexOf('<') + 1;
                        int end = listType.LastIndexOf('>');
                        string elementType = listType.Substring(start, end - start);
                        classSource.Append(
                            $"\t\t\tTypeHandlerFactory.PreRegister<{listType}>(new F8Framework.Core.ListHandler<{elementType}>());\n");
                    }
                    else if (item.Value == SupportType.DICTIONARY)
                    {
                        string dictType = item.Key; // 例如 "Dictionary<int, LocalizedStringsItem>"
                        int start = dictType.IndexOf('<') + 1;
                        int end = dictType.LastIndexOf('>');
                        string inner = dictType.Substring(start, end - start);
                        string[] parts = inner.Split(new char[] { ',' }, 2);
                        string keyType = parts[0].Trim();
                        string valueType = parts[1].Trim();
                        classSource.Append(
                            $"\t\t\tTypeHandlerFactory.PreRegister<{dictType}>(new F8Framework.Core.DictionaryHandler<{keyType}, {valueType}>());\n");
                    }
                    else if (item.Value == SupportType.HASHSET)
                    {
                        string hashSetType = item.Key; // 例如 "HashSet<int>"
                        int start = hashSetType.IndexOf('<') + 1;
                        int end = hashSetType.LastIndexOf('>');
                        string elementType = hashSetType.Substring(start, end - start);
                        classSource.Append(
                            $"\t\t\tTypeHandlerFactory.PreRegister<{hashSetType}>(new F8Framework.Core.HashSetHandler<{elementType}>());\n");
                    }
                    else if (item.Value == SupportType.ENUM)
                    {
                        string enumType = item.Key; // 例如 "Sheet11.MyEnum"
                        classSource.Append(
                            $"\t\t\tTypeHandlerFactory.PreRegister<{enumType}>(new F8Framework.Core.EnumHandler<{enumType}>());\n");
                    }
                    else if (item.Value == SupportType.VALUETUPLE)
                    {
                        string tupleType = item.Key; // 例如 "ValueTuple<int, string>"
                        classSource.Append(
                            $"\t\t\tTypeHandlerFactory.PreRegister<{tupleType}>(new F8Framework.Core.ValueTupleHandler<{tupleType}>());\n");
                    }
                    else if (item.Value == SupportType.OBJ)
                    {
                        string classType = item.Key; // 自定义类
                        classSource.Append(
                            $"\t\t\tTypeHandlerFactory.PreRegister<{classType}>(new F8Framework.Core.ObjectHandler<{classType}>());\n");
                    }
                }
            }
            classSource.Append("\t\t}\n");
            classSource.Append("\t\t[Preserve]\n");
            classSource.Append("\t\tpublic " + "Dictionary<" + idType + ", " + ExcelDataTool.CODE_NAMESPACE + "." + ClassName + "Item" + "> " + "Dict" +
                               " = new Dictionary<" + idType + ", " + ExcelDataTool.CODE_NAMESPACE + "." + ClassName + "Item" + ">();\n");
            foreach (string enumName in enumDefinitionOrder)
            {
                classSource.Append(enumDefinitions[enumName].Source);
            }
            classSource.Append("\t}\n");
            classSource.Append("}\n");
            return classSource.ToString();
            /*  //生成的条目数据类
                namespace F8ExcelDataClass
                {
                    public class testItem
                    {
                        public int id;
                        public float m_float;
                        public string str;
                        public test();
                    }
                }
                //生成的表数据类
                using System.Collections.Generic;
                {
                    public class test
                    {
                        public Dictionary<int, test> Dict;
                        public testContainer();
                    }
                }
             */
        }

        private static void AppendLocalizationItemImplementation(
            StringBuilder classSource,
            string className,
            IReadOnlyList<string> generatedFields)
        {
            List<string> idFields = generatedFields
                .Where(field => string.Equals(field, "id", StringComparison.OrdinalIgnoreCase))
                .ToList();
            List<string> textIdFields = generatedFields
                .Where(field => string.Equals(field, "TextID", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (idFields.Count != 1 || textIdFields.Count != 1)
            {
                throw new InvalidOperationException(
                    $"本地化表 {className} 必须包含 id 和 TextID 字段，且两者都只能出现一次。");
            }

            string idField = idFields[0];
            string textIdField = textIdFields[0];

            List<string> languageFields = generatedFields
                .Where(field =>
                    !string.Equals(field, idField, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(field, textIdField, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (languageFields.Count == 0)
            {
                throw new InvalidOperationException(
                    $"本地化表 {className} 至少需要包含一个语言字段。");
            }

            classSource.Append("\n");
            classSource.Append($"\t\tstring ILocalizationItem.Id => ((System.Object){idField})?.ToString() ?? string.Empty;\n");
            classSource.Append($"\t\tstring ILocalizationItem.TextId => ((System.Object){textIdField})?.ToString();\n");
            classSource.Append("\t\tIReadOnlyList<string> ILocalizationItem.LanguageNames => new string[]\n");
            classSource.Append("\t\t{\n");
            foreach (string languageField in languageFields)
            {
                classSource.Append($"\t\t\tnameof({languageField}),\n");
            }
            classSource.Append("\t\t};\n");
            classSource.Append("\t\tIReadOnlyList<string> ILocalizationItem.LanguageValues => new string[]\n");
            classSource.Append("\t\t{\n");
            foreach (string languageField in languageFields)
            {
                classSource.Append($"\t\t\t((System.Object){languageField})?.ToString(),\n");
            }
            classSource.Append("\t\t};\n");
        }

        private string PropertyString(string type, string propertyName, Dictionary<string, string> binaryRegisters)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(propertyName))
                return null;
            
            type = ReadExcel.GetTrueType(type, ClassName, InputPath, true, binaryRegisters);
            if (!string.IsNullOrEmpty(type))
            {
                StringBuilder sbProperty = new StringBuilder();
                
                sbProperty.Append("\t\t[Preserve]\n");
                sbProperty.Append("\t\tpublic " + type + " " + propertyName + ";\n");
                
                return sbProperty.ToString();
            }
            else
            {
                return "";
            }
        }

        private void PropertyEnum(string type, string className = "", string inputPath = "", bool writtenForm = true, Dictionary<string, string> binaryRegisters = null)
        {
            if (!type.StartsWith(SupportType.ENUM) || (!type.EndsWith(">") && !type.EndsWith("}")))
                return;

            // 1. 检查是否有大括号（完整定义）或只有尖括号（简化定义）
            bool hasBraces = type.Contains('{') && type.Contains('}');
            string enumDefinition;
            string enumValues = "";

            if (hasBraces)
            {
                // 完整定义：enum<...>{...}
                int startBrace = type.IndexOf('{');
                int endBrace = type.LastIndexOf('}');

                if (startBrace == -1 || endBrace == -1)
                {
                    throw new Exception("枚举定义缺少大括号");
                }

                enumDefinition = type.Substring(SupportType.ENUM.Length, startBrace - SupportType.ENUM.Length)
                    .Trim('<', '>', ' ');
                enumValues = type.Substring(startBrace + 1, endBrace - startBrace - 1).Trim();
            }
            else
            {
                // 简化定义：enum<...>
                int startAngle = type.IndexOf('<');
                int endAngle = type.LastIndexOf('>');

                if (startAngle == -1 || endAngle == -1)
                {
                    throw new Exception("枚举定义缺少尖括号");
                }

                enumDefinition = type.Substring(startAngle + 1, endAngle - startAngle - 1).Trim();
            }

            // 2. 分割参数
            string[] enumParams = enumDefinition.Split(',');
            if (enumParams.Length < 1)
            {
                throw new Exception("枚举定义至少需要包含枚举名称");
            }

            string enumName = enumParams[0].Trim();
            string enumSourceName = enumName;

            // 3. 检查类名前缀（如果包含点）
            if (enumName.Contains('.'))
            {
                string[] parts = enumName.Split('.');
                if (parts[0] != className)
                    return;

                enumSourceName = parts[parts.Length - 1].Trim();
            }

            // 4. 设置默认值
            string underlyingType = enumParams.Length > 1 ? enumParams[1].Trim() : "int"; // 默认为int
            bool isFlags = enumParams.Length > 2 &&
                           enumParams[2].Trim().Equals("Flags", StringComparison.OrdinalIgnoreCase);

            // 5. 生成C#枚举代码
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (isFlags)
            {
                sb.Append("\t\t[System.Flags]\n");
            }

            sb.Append("\t\t[Preserve]\n");
            sb.Append("\t\t[Serializable]\n");
            sb.Append("\t\tpublic enum ").Append(enumSourceName).Append(" : ").Append(ReadExcel.GetTrueType(underlyingType, className, inputPath, writtenForm)).Append("\n");
            sb.Append("\t\t{\n");

            // 6. 处理枚举值（如果有）
            if (hasBraces && !string.IsNullOrEmpty(enumValues))
            {
                string[] valuePairs = enumValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in valuePairs)
                {
                    string trimmedPair = pair.Trim();
                    if (!string.IsNullOrEmpty(trimmedPair))
                    {
                        sb.Append("\t\t\t" + trimmedPair + ",\n");
                    }
                }
            }
            else
            {
                // 简化定义时添加一个默认值
                sb.Append("\t\t\tNone = 0,\n");
            }

            sb.Append("\t\t}\n");

            RegisterEnumDefinition(enumSourceName, sb.ToString(), hasBraces, enumDefinition, underlyingType, isFlags, enumValues);
            binaryRegisters?.TryAdd($"{ExcelDataTool.CODE_NAMESPACE}.{className}.{enumSourceName}", SupportType.ENUM);
        }

        private void RegisterEnumDefinition(string enumName, string source, bool hasFullDefinition, string enumDefinition, string underlyingType, bool isFlags, string enumValues)
        {
            if (string.IsNullOrEmpty(enumName) || string.IsNullOrEmpty(source))
            {
                return;
            }

            if (enumDefinitions.TryGetValue(enumName, out EnumDefinition existing))
            {
                if (!existing.HasFullDefinition && hasFullDefinition)
                {
                    existing.Source = source;
                    existing.HasFullDefinition = true;
                }

                return;
            }

            enumDefinitions[enumName] = new EnumDefinition
            {
                Source = source,
                HasFullDefinition = hasFullDefinition
            };
            enumDefinitionOrder.Add(enumName);
        }
        
        //创建数据管理器脚本
        public static bool CreateDataManager(Dictionary<string, ScriptGenerator> codeList)
        {
            List<string> list = new List<string>();
            list.AddRange(codeList.Keys);
            IEnumerable types = list.FindAll(t => true);
            
            StringBuilder source = new StringBuilder();
            source.Append("/*\n");
            source.Append(" *   This file was generated by a tool.\n");
            source.Append(" *   Do not edit it, otherwise the changes will be overwritten.\n");
            source.Append(" */\n");
            source.Append("\n");

            source.Append("using System;\n");
            source.Append("using System.Collections;\n");
            source.Append("using System.Collections.Generic;\n");
            source.Append("using System.Threading.Tasks;\n");
            source.Append("using UnityEngine;\n");
            source.Append("using F8Framework.Core;\n");
            source.Append("using UnityEngine.Scripting;\n\n");
            source.Append("namespace " + ExcelDataTool.CODE_NAMESPACE + "\n");
            source.Append("{\n");
            source.Append("\tpublic class F8DataManager : ModuleSingleton<F8DataManager>, IModule\n");
            source.Append("\t{\n");

            source.Append("\t\tpublic string VariantName;\n");
            source.Append("\t\tprivate AssetLoadTracker _assetLoadTracker;\n");
            source.Append("\t\tprivate AssetLoadTracker AssetLoadTracker => _assetLoadTracker ??= new AssetLoadTracker();\n");
            //定义变量
            foreach (string t in types)
            {
                source.Append("\t\tprivate " + t + " p_" + t + ";\n");
            }

            source.Append("\n");

            bool hasLocalizedStrings = false;
            //定义方法
            foreach (var dict in codeList)
            {
                string t = dict.Key;
                ScriptGenerator sg = dict.Value;
                string typeName = t + "Item"; //类型名
                string typeNameNotItem = t; //类型名没item
                source.Append("\t\t[Preserve]\n");
                source.Append("\t\tpublic " + typeName + " Get" + typeNameNotItem + "ByID" + "(" + sg.GetIdType() + " id)\n");
                source.Append("\t\t{\n");
                source.Append("\t\t\t" + typeName + " t = null;\n");
                source.Append("\t\t\tp_" + t + ".Dict.TryGetValue(id, out t);\n");
                source.Append("\t\t\tif (t == null) LogF8.LogError(" + '"' + "找不到id： " + '"' + " + id " +
                              "+ " +
                              '"' + " ，配置表： " + t + '"' + ");\n");
                source.Append("\t\t\treturn t;\n");
                source.Append("\t\t}\n\n");
                source.Append("\t\t[Preserve]\n");
                source.Append("\t\tpublic Dictionary<" + sg.GetIdType() + ", " + typeName + ">" + " Get" + typeNameNotItem + "()\n");
                source.Append("\t\t{\n");
                source.Append("\t\t\tif (p_" + t + " != null) return p_" + t + ".Dict;\n");
                source.Append("\t\t\tLogF8.LogError(\"未加载配置表： " + t + "\");\n");
                source.Append("\t\t\treturn null;\n");
                source.Append("\t\t}\n\n");
                if (t == LocalizationTableName)
                {
                    hasLocalizedStrings = true;
                }
            }

            if (hasLocalizedStrings)
            {
                //只加载本地化表
                source.Append("\t\t[Preserve]\n");
                source.Append("\t\tpublic void LoadLocalizedStrings()\n");
                source.Append("\t\t{\n");
                source.Append("\t\t\tp_LocalizedStrings = Load<LocalizedStrings>(\"LocalizedStrings\") as LocalizedStrings;\n");
                source.Append("\t\t}\n\n");
            
                source.Append("\t\t[Preserve]\n");
                source.Append("\t\tpublic void LoadLocalizedStringsCallback(Action onLoadComplete)\n");
                source.Append("\t\t{\n");
                source.Append("\t\t\tUtil.Unity.StartCoroutine(LoadLocalizedStringsIEnumerator(onLoadComplete));\n");
                source.Append("\t\t}\n\n");
            
                source.Append("\t\t[Preserve]\n");
                source.Append("\t\tpublic IEnumerator LoadLocalizedStringsIEnumerator(Action onLoadComplete = null)\n");
                source.Append("\t\t{\n");
                source.Append("\t\t\tyield return LoadAsync<LocalizedStrings>(\"LocalizedStrings\", result => p_LocalizedStrings = result as LocalizedStrings);\n");
                source.Append("\t\t\tonLoadComplete?.Invoke();\n");
                source.Append("\t\t}\n\n");
            }

            //加载所有配置表
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic void LoadAll()\n");
            source.Append("\t\t{\n");
            foreach (string t in types)
            {
                source.Append("\t\t\tp_" + t + " = Load<" + t + ">(" + '"' + t + '"' + ") as " + t + ";\n");
            }

            source.Append("\t\t}\n\n");

            //运行时加载所有配置表
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic void RuntimeLoadAll(Dictionary<String, System.Object> objs = null)\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\tif (objs == null)\n");
            source.Append("\t\t\t{\n");
            source.Append("\t\t\t\tif (!ConfigDataSourceRegistry.TryLoadAll(out objs))\n");
            source.Append("\t\t\t\t{\n");
            source.Append("\t\t\t\t\tLogF8.LogError(\"没有可用的配置数据源。请安装并启用一个 IConfigDataSource 实现。\");\n");
            source.Append("\t\t\t\t\treturn;\n");
            source.Append("\t\t\t\t}\n");
            source.Append("\t\t\t}\n");
            foreach (string t in types)
            {
                source.Append("\t\t\tp_" + t + " = objs[" + '"' + t + '"' + "] as " + t + ";\n");
            }

            source.Append("\t\t}\n\n");

            //异步加载所有配置表
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic IEnumerable LoadAllAsync()\n");
            source.Append("\t\t{\n");
            foreach (string t in types)
            {
                source.Append("\t\t\tyield return LoadAsync<" + t + ">("+ '"' + t + '"' + ", result => " +  "p_" + t + " = result" + " as " + t + ");\n");
            }
            source.Append("#if UNITY_EDITOR\n");
            source.Append("\t\t\tif (AssetManager.Instance.IsEditorMode)\n");
            source.Append("\t\t\t{\n");
            source.Append("\t\t\t\tRuntimeLoadAll();\n");
            source.Append("\t\t\t}\n");
            source.Append("#endif\n");
            source.Append("\t\t}\n\n");
            
            //异步加载所有配置表
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic async Task LoadAllAsyncTask()\n");
            source.Append("\t\t{\n");
            foreach (string t in types)
            {
                source.Append("\t\t\tawait LoadAsyncTask<" + t + ">("+ '"' + t + '"' + ", result => " +  "p_" + t + " = result" + " as " + t + ");\n");
            }
            source.Append("#if UNITY_EDITOR\n");
            source.Append("\t\t\tif (AssetManager.Instance.IsEditorMode)\n");
            source.Append("\t\t\t{\n");
            source.Append("\t\t\t\tRuntimeLoadAll();\n");
            source.Append("\t\t\t}\n");
            source.Append("#endif\n");
            source.Append("\t\t}\n\n");
            
            //异步加载所有配置表
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic void LoadAllAsyncCallback(Action onLoadComplete = null)\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\tUtil.Unity.StartCoroutine(LoadAllAsyncIEnumerator(onLoadComplete));\n");
            source.Append("\t\t}\n\n");
            
            //异步加载所有配置表
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic IEnumerator LoadAllAsyncIEnumerator(Action onLoadComplete = null)\n");
            source.Append("\t\t{\n");
            foreach (string t in types)
            {
                source.Append("\t\t\tyield return LoadAsync<" + t + ">("+ '"' + t + '"' + ", result => " +  "p_" + t + " = result" + " as " + t + ");\n");
            }
            source.Append("#if UNITY_EDITOR\n");
            source.Append("\t\t\tif (AssetManager.Instance.IsEditorMode)\n");
            source.Append("\t\t\t{\n");
            source.Append("\t\t\t\tRuntimeLoadAll();\n");
            source.Append("\t\t\t}\n");
            source.Append("#endif\n");
            source.Append("\t\t\tonLoadComplete?.Invoke();\n");
            source.Append("\t\t}\n\n");
            
            //反序列化
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic T Load<T>(string name)\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\tTextAsset textAsset = AssetLoadTracker.Load<TextAsset>(name);\n");
            source.Append("\t\t\tif (textAsset == null)\n");
            source.Append("\t\t\t{\n");
            source.Append("\t\t\t\treturn default(T);\n");
            source.Append("\t\t\t}\n");
            source.Append("\t\t\tUnloadAsset(name, false);\n");
            
            string exportFormat = ExcelDataSettings.ExportFormat;
            if (exportFormat == ExcelDataSettings.BinaryFormat)
            {
                source.Append("\t\t\tT obj = Util.BinarySerializer.Deserialize<T>(textAsset.bytes);\n");
            }else
            {
                source.Append("\t\t\tT obj = Util.LitJson.ToObject<T>(textAsset.text);\n");
            }
            source.Append("\t\t\treturn obj;\n");
            source.Append("\t\t}\n\n");
            
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic IEnumerator LoadAsync<T>(string name, Action<T> callback)\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\tTextAsset textAsset = null;\n");
            source.Append("\t\t\tyield return AssetLoadTracker.LoadAsync<TextAsset>(name, result => textAsset = result);\n");
            source.Append("\t\t\tif (textAsset != null)\n");
            source.Append("\t\t\t{\n");
            source.Append("\t\t\t\tUnloadAsset(name, false);\n");
            if (exportFormat == ExcelDataSettings.BinaryFormat)
            {
                source.Append("\t\t\t\tT obj = Util.BinarySerializer.Deserialize<T>(textAsset.bytes);\n");
            }else
            {
                source.Append("\t\t\t\tT obj = Util.LitJson.ToObject<T>(textAsset.text);\n");
            }
            source.Append("\t\t\t\tcallback?.Invoke(obj);\n");
            source.Append("\t\t\t}\n");
            source.Append("\t\t}\n\n");
            
            source.Append("\t\t[Preserve]\n");
            source.Append("\t\tpublic async Task LoadAsyncTask<T>(string name, Action<T> callback)\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\tTextAsset textAsset = null;\n");
            source.Append("\t\t\tBaseLoader load = AssetLoadTracker.LoadAsync<TextAsset>(name, result => textAsset = result);\n");
            source.Append("\t\t\tawait load;\n");
            source.Append("\t\t\tif (textAsset != null)\n");
            source.Append("\t\t\t{\n");
            source.Append("\t\t\t\tUnloadAsset(name, false);\n");
            if (exportFormat == ExcelDataSettings.BinaryFormat)
            {
                source.Append("\t\t\t\tT obj = Util.BinarySerializer.Deserialize<T>(textAsset.bytes);\n");
            }else
            {
                source.Append("\t\t\t\tT obj = Util.LitJson.ToObject<T>(textAsset.text);\n");
            }
            source.Append("\t\t\t\tcallback?.Invoke(obj);\n");
            source.Append("\t\t\t}\n");
            source.Append("\t\t}\n\n");
            
            source.Append("\t\tpublic void UnloadAsset(string name, bool unloadAllLoadedObjects = false)\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\t_assetLoadTracker?.Release(name, unloadAllLoadedObjects);\n");
            source.Append("\t\t}\n\n");
            
            source.Append("\t\tpublic void ClearAssetLoadTracker(bool unloadAllLoadedObjects = false)\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\tif (_assetLoadTracker == null)\n");
            source.Append("\t\t\t\treturn;\n\n");
            source.Append("\t\t\t_assetLoadTracker.ReleaseAll(unloadAllLoadedObjects);\n");
            source.Append("\t\t\t_assetLoadTracker = null;\n");
            source.Append("\t\t}\n\n");
            
            source.Append("\t\tpublic void OnInit(object createParam)\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\t\n");
            source.Append("\t\t}\n\n");
            source.Append("\t\tpublic void OnUpdate()\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\t\n");
            source.Append("\t\t}\n\n");
            source.Append("\t\tpublic void OnLateUpdate()\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\t\n");
            source.Append("\t\t}\n\n");
            source.Append("\t\tpublic void OnFixedUpdate()\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\t\n");
            source.Append("\t\t}\n\n");
            source.Append("\t\tpublic void OnTermination()\n");
            source.Append("\t\t{\n");
            source.Append("\t\t\tClearAssetLoadTracker(true);\n");
            foreach (string t in types)
            {
                source.Append("\t\t\tp_" + t + " = null;\n");
            }
            source.Append("\t\t\tVariantName = null;\n");
            source.Append("\t\t\tbase.Destroy();\n");
            source.Append("\t\t}\n");
            source.Append("\t}\n");
            source.Append("}");
            // 保存脚本；内容未变化时保留文件时间戳和 Unity meta/GUID。
            string path = Application.dataPath + ExcelDataTool.DataManagerFolder;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string dataManagerPath = path + "/" + ExcelDataTool.DataManagerName;
            bool changed = ExcelDataTool.WriteTextIfChanged(dataManagerPath, source.ToString());
            LogF8.LogConfig(
                (changed ? "已生成代码 " : "代码无需更新 ") +
                path + "/<color=#FF9E59>" + ExcelDataTool.DataManagerName + "</color>");
            return changed;

            /*  //生成的数据管理类如下
                using System;
                using UnityEngine;
                using System.Runtime.Serialization;
                using System.Runtime.Serialization.Formatters.Binary;
                using System.IO;
                using ExcelConfigClass;
                
                public class F8DataManager : Singleton<F8DataManager>
                {
                    public test p_test;
                    public test2 p_test2;
                    public testItem GetTestByID(Int32 id)
                    {
                        testItem t = null;
                        p_test.Dict.TryGetValue(id, out t);
                        if (t == null) LogF8.LogError("can't find the id " + id + " in test");
                        return t;
                    }
                    public test2Item GetTest2ByID(String id)
                    {
                        test2Item t = null;
                        p_test2.Dict.TryGetValue(id, out t);
                        if (t == null) LogF8.LogError("can't find the id " + id + " in test2");
                        return t;
                    }
                    public void LoadAll()
                    {
                        p_test = Load("test") as test;
                        p_test2 = Load("test2") as test2;
                    }
		            private System.Object Load(string name)
		            {
		            	IFormatter f = new BinaryFormatter();
		            	TextAsset text = AssetLoadTracker.Load<TextAsset>(name);
		            	using (MemoryStream memoryStream = new MemoryStream(text.bytes))
		            	{
		            		return f.Deserialize(memoryStream);
		            	}
		            }
                }
            */
        }
    }
}
