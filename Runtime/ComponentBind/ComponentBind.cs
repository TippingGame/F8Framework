using System;
using UnityEngine;

namespace F8Framework.Core
{
    public class ComponentBind : MonoBehaviour
    {
        protected virtual void SetComponents()
        {
#if UNITY_EDITOR
            LogF8.Log("首次添加UI代码生成组件绑定脚本，并绑定组件：" + this);
#endif
        }
        
#if UNITY_EDITOR
        // 分隔符
        private string _division = "_";
        
        public void Bind()
        {
            GenerateAutoBindComponentsCode();
        }
        
        // 屏蔽自动获取组件
        // protected void Reset()
        // {
        //     Bind();
        // }
        //
        // protected void OnValidate()
        // {
        //     if (!Application.isPlaying)
        //     {
        //         Bind();
        //     }
        // }

        private void GenerateAutoBindComponentsCode()
        {
            string scriptPath = GetScriptPath(this);

            // 不是C#脚本就返回
            if (!System.IO.Path.GetExtension(scriptPath).Equals(".cs", System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Load the prefab
            GameObject prefab = gameObject;

            // 自动生成的代码将会放在这个字符串中
            System.Text.StringBuilder generatedCode = new System.Text.StringBuilder();
            // 自动生成的引用代码
            System.Text.StringBuilder referenceCode = new System.Text.StringBuilder();
            // 自动生成的监听事件代码
            System.Text.StringBuilder listenerCode = new System.Text.StringBuilder();
            // 自动生成的监听事件字段
            System.Text.StringBuilder listenerFieldCode = new System.Text.StringBuilder();

            // 存储需要生成监听事件的组件
            System.Collections.Generic.List<(string fieldName, string componentType)> listenerComponents =
                new System.Collections.Generic.List<(string fieldName, string componentType)>();

            System.Collections.Generic.Queue<Transform> transforms = new System.Collections.Generic.Queue<Transform>();
            System.Collections.Generic.Queue<Transform> childs = new System.Collections.Generic.Queue<Transform>();
            transforms.Enqueue(gameObject.transform);

            while (transforms.Count > 0)
            {
                Transform current = transforms.Dequeue();

                childs.Enqueue(current);

                // 将当前物体的子物体加入队列
                foreach (Transform child in current)
                {
                    transforms.Enqueue(child);
                }
            }

            System.Collections.Generic.Dictionary<string, int> tempDic =
                new System.Collections.Generic.Dictionary<string, int>();
            // 用于存储数组组件的字典
            System.Collections.Generic.Dictionary<string,
                System.Collections.Generic.List<(string path, string componentType)>> arrayComponents =
                new System.Collections.Generic.Dictionary<string,
                    System.Collections.Generic.List<(string path, string componentType)>>();

            // 遍历Prefab中的所有物体
            foreach (Transform child in childs)
            {
                if (child.Equals(transform)) // 忽略自身
                    continue;

                System.Collections.Generic.List<string> componentNames = null;

                string[] nameDivision = child.gameObject.name.Split(_division);

                foreach (var name in nameDivision)
                {
                    // 检查物体名字的一部分是否包含在字典的key中
                    foreach (var key in DefaultCodeBindNameTypeConfig.BindNameTypeDict.Keys)
                    {
                        if (!name.Contains(key))
                            continue;
                        string componentType = DefaultCodeBindNameTypeConfig.BindNameTypeDict[key];
                        if (componentNames == null)
                            componentNames = new System.Collections.Generic.List<string>();

                        if (componentNames.Contains(componentType))
                            continue;
                        componentNames.Add(componentType);

                        string extension = System.IO.Path.GetExtension(componentType);
                        string typeToCheck = string.IsNullOrEmpty(extension) ? componentType : extension[1..];
                        if (componentType != typeof(UnityEngine.GameObject).ToString() &&
                            !child.GetComponent(typeToCheck))
                            continue;

                        string normalizeName = RemoveSpecialCharacters(child.gameObject.name);

                        string normalizeKey = RemoveSpecialCharacters(key);

                        if (!tempDic.TryAdd($"{normalizeName}_{normalizeKey}", 1))
                        {
                            tempDic[$"{normalizeName}_{normalizeKey}"] += 1;
                            // LogF8.LogView("有重名物体：" + normalizeKey);
                            normalizeKey += _division + tempDic[$"{normalizeName}_{normalizeKey}"];
                        }

                        string fieldName = $"{normalizeName}_{normalizeKey}";

                        // 生成自动获取组件的代码
                        generatedCode.AppendLine($"\t[SerializeField] private {componentType} {fieldName};");
                        // 生成引用代码
                        string childPath = GetChildPath(child, prefab.transform);
                        if (componentType == typeof(UnityEngine.GameObject).ToString())
                        {
                            referenceCode.AppendLine(
                                $"\t\t{fieldName} = transform.Find(\"{SelectiveEscape(childPath)}\").gameObject;");
                        }
                        else
                        {
                            referenceCode.AppendLine(
                                $"\t\t{fieldName} = transform.Find(\"{SelectiveEscape(childPath)}\").GetComponent<{componentType}>();");
                        }

                        // 检查是否需要生成监听事件
                        if (ShouldGenerateListener(componentType))
                        {
                            listenerComponents.Add((fieldName, componentType));
                        }

                        // 检查是否是数组元素（名字以[数字]结尾）
                        System.Text.RegularExpressions.Match matchArray =
                            System.Text.RegularExpressions.Regex.Match(child.gameObject.name, @"^(.*?)\[\d+\]$");
                        if (matchArray.Success)
                        {
                            string baseName = matchArray.Groups[1].Value;
                            // 添加到数组字典
                            string arrayKey = baseName + "_" + normalizeKey;
                            if (!arrayComponents.ContainsKey(arrayKey))
                            {
                                arrayComponents[arrayKey] =
                                    new System.Collections.Generic.List<(string path, string componentType)>();
                            }

                            arrayComponents[arrayKey].Add((childPath, componentType));
                        }
                    }
                }
            }

            // 处理数组组件
            foreach (var arrayItem in arrayComponents)
            {
                string arrayName = RemoveSpecialCharacters(arrayItem.Key);
                string componentType = arrayItem.Value[0].componentType;

                // 提取所有索引并找到最大值
                int maxIndex = 0;
                var indexedItems = new System.Collections.Generic.List<(int index, string path)>();
                foreach (var item in arrayItem.Value)
                {
                    var matchArray = System.Text.RegularExpressions.Regex.Match(item.path, @"\[(\d+)\]$");
                    if (matchArray.Success)
                    {
                        int currentIndex = int.Parse(matchArray.Groups[1].Value);
                        indexedItems.Add((currentIndex, item.path));
                        if (currentIndex > maxIndex)
                        {
                            maxIndex = currentIndex;
                        }
                    }
                }

                // 数组大小 = 最大索引 + 1
                int arraySize = maxIndex + 1;

                // 生成数组声明
                generatedCode.AppendLine(
                    $"\t[SerializeField] private {componentType}[] {arrayName} = new {componentType}[{arraySize}];");

                // 生成数组元素赋值代码（使用元素自身的索引）
                foreach (var item in indexedItems)
                {
                    if (componentType == typeof(UnityEngine.GameObject).ToString())
                    {
                        referenceCode.AppendLine(
                            $"\t\t{arrayName}[{item.index}] = transform.Find(\"{SelectiveEscape(item.path)}\").gameObject;");
                    }
                    else
                    {
                        referenceCode.AppendLine(
                            $"\t\t{arrayName}[{item.index}] = transform.Find(\"{SelectiveEscape(item.path)}\").GetComponent<{componentType}>();");
                    }
                }

                // 检查数组组件是否需要生成监听事件
                if (ShouldGenerateListener(componentType))
                {
                    // 为数组中的每个元素生成监听
                    for (int i = 0; i < arraySize; i++)
                    {
                        listenerComponents.Add(($"{arrayName}[{i}]", componentType));
                    }
                }
            }

            // 生成监听事件代码
            if (listenerComponents.Count > 0)
            {
                // 生成监听事件字段
                foreach (var component in listenerComponents)
                {
                    string listenerField = GenerateListenerField(component.fieldName, component.componentType);
                    if (!string.IsNullOrEmpty(listenerField))
                    {
                        listenerFieldCode.AppendLine($"\t{listenerField}");
                    }
                }
                
                listenerCode.AppendLine(listenerFieldCode.ToString());
                listenerCode.AppendLine("\t// 自动生成");
                listenerCode.AppendLine("\tprotected override void OnAddUIComponentListener()");
                listenerCode.AppendLine("\t{");

                foreach (var component in listenerComponents)
                {
                    string listenerCall = GenerateListenerCall(component.fieldName, component.componentType);
                    if (!string.IsNullOrEmpty(listenerCall))
                    {
                        listenerCode.AppendLine($"\t\t{listenerCall}");
                    }
                }

                listenerCode.AppendLine("\t}");
            }

            // 将生成的代码插入到脚本中
            string scriptContent;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(scriptPath))
            {
                scriptContent = reader.ReadToEnd();
                // 关闭文件句柄
                reader.Close();
            }

            // 使用正则表达式匹配并替换注释之间的内容
            string pattern = @"// 自动获取组件（自动生成，不能删除）(.*?)// 自动获取组件（自动生成，不能删除）";
            System.Text.RegularExpressions.Regex regex =
                new System.Text.RegularExpressions.Regex(pattern,
                    System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Match match = regex.Match(scriptContent);

            if (match.Success)
            {
                // 替换注释之间的内容，包含头尾的注释
                scriptContent = scriptContent.Remove(match.Groups[0].Index, match.Groups[0].Length);
                scriptContent = scriptContent.Insert(match.Groups[0].Index,
                    $"// 自动获取组件（自动生成，不能删除）\n{generatedCode}" +
                    $"\n{listenerCode}" +
                    $"\n#if UNITY_EDITOR" +
                    $"\n\t// 自动生成" +
                    $"\n\tprotected override void SetComponents()" +
                    $"\n\t{{" +
                    $"\n{referenceCode}" +
                    $"\t}}" +
                    $"\n#endif" +
                    $"\n\t// 自动获取组件（自动生成，不能删除）");

                // 将所有换行符替换为 UNIX 风格的 "\n"
                scriptContent = scriptContent.Replace("\r\n", "\n").Replace("\r", "\n");

                // 保存脚本文件
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(scriptPath, false))
                {
                    writer.Write(scriptContent);
                    // 关闭文件句柄
                    writer.Close();
                }

                UnityEditor.AssetDatabase.Refresh();

                try
                {
                    SetComponents();
                }
                catch (Exception e)
                {
                    LogF8.LogView(e);
                }

                UnityEditor.EditorUtility.SetDirty(this);
            }
            else
            {
                LogF8.Log("在脚本中找不到插入标记。请在要生成代码的位置添加“// 自动获取组件（自动生成，不能删除）”。");
            }
        }

        // 检查组件类型是否需要生成监听事件
        private bool ShouldGenerateListener(string componentType)
        {
            string[] listenerTypes =
            {
                "UnityEngine.UI.Button",
                "UnityEngine.UI.Slider",
                "UnityEngine.UI.Scrollbar",
                "UnityEngine.UI.Dropdown",
                "UnityEngine.UI.Toggle",
                "UnityEngine.UI.InputField",
                "UnityEngine.UI.ScrollRect"
            };

            return Array.Exists(listenerTypes, type => componentType.Contains(type));
        }

        // 生成监听事件字段
        private string GenerateListenerField(string fieldName, string componentType)
        {
            string normalizedFieldName = fieldName.Replace("[", "_").Replace("]", "_");

            if (componentType.Contains("UnityEngine.UI.Button"))
            {
                return $"private UnityAction unityAction_{normalizedFieldName};";
            }
            else if (componentType.Contains("UnityEngine.UI.Slider"))
            {
                return $"private UnityAction<float> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Contains("UnityEngine.UI.Scrollbar"))
            {
                return $"private UnityAction<float> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Contains("UnityEngine.UI.Dropdown"))
            {
                return $"private UnityAction<int> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Contains("UnityEngine.UI.Toggle"))
            {
                return $"private UnityAction<bool> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Contains("UnityEngine.UI.InputField"))
            {
                return $"private UnityAction<string> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Contains("UnityEngine.UI.ScrollRect"))
            {
                return $"private UnityAction<Vector2> unityAction_{normalizedFieldName};";
            }

            return string.Empty;
        }

        // 生成监听事件调用代码
        private string GenerateListenerCall(string fieldName, string componentType)
        {
            string normalizedFieldName = fieldName.Replace("[", "_").Replace("]", "_");

            if (componentType.Contains("UnityEngine.UI.Button"))
            {
                return 
                    $"unityAction_{normalizedFieldName} = () => ButtonClick({fieldName});\n\t\t{fieldName}?.onClick.AddListener(unityAction_{normalizedFieldName});";
            }
            else if (componentType.Contains("UnityEngine.UI.Slider") ||
                     componentType.Contains("UnityEngine.UI.Scrollbar") ||
                     componentType.Contains("UnityEngine.UI.Dropdown") ||
                     componentType.Contains("UnityEngine.UI.Toggle") ||
                     componentType.Contains("UnityEngine.UI.InputField") ||
                     componentType.Contains("UnityEngine.UI.ScrollRect"))
            {
                return
                    $"unityAction_{normalizedFieldName} = (value) => ValueChange({fieldName}, value);\n\t\t{fieldName}?.onValueChanged.AddListener(unityAction_{normalizedFieldName});";
            }

            return string.Empty;
        }
        
        private string SelectiveEscape(string input)
        {
            // 定义需要转义的特殊字符
            string[] specialCharsToEscape = { "\\", "\"" };

            foreach (var specialChar in specialCharsToEscape)
            {
                // 在特殊字符前添加反斜杠
                input = input.Replace(specialChar, "\\" + specialChar);
            }

            return input;
        }
        
        private string RemoveSpecialCharacters(string input)
        {
            // 定义正则表达式，只允许字母、数字、下划线和中文字符
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9_\\u4e00-\\u9fa5]");
            string cleaned = regex.Replace(input, "");
            // 确保不以数字开头
            if (cleaned.Length > 0 && char.IsDigit(cleaned[0]))
            {
                cleaned = "_" + cleaned; // 在数字前添加下划线
            }
            return cleaned;
        }
        
        // 获取子物体的路径
        private string GetChildPath(Transform child, Transform root)
        {
            if (child == null || root == null)
            {
                LogF8.LogError("child或者root为空");
                return "";
            }
            
            System.Text.StringBuilder path = new System.Text.StringBuilder(child.name);

            while (child.parent != null && child.parent != root)
            {
                child = child.parent;
                if (!string.IsNullOrEmpty(child.name)) // 确保子对象的父级名称不为空
                {
                    path.Insert(0, $"{child.name}/");
                }
                else
                {
                    LogF8.LogError("child.name为空");
                }
            }


            return path.ToString();
        }
        
        private string GetScriptPath(MonoBehaviour script)
        {
            UnityEditor.MonoScript monoScript = UnityEditor.MonoScript.FromMonoBehaviour(script);
            string scriptRelativePath = UnityEditor.AssetDatabase.GetAssetPath(monoScript);
            // 获取绝对路径并规范化
            string scriptPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", scriptRelativePath));
            return FileTools.FormatToUnityPath(scriptPath);
        }
#endif
    }
}