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
            GenerateAutoBindComponentsCode(false);
        }

        public void BindAllUIComponents()
        {
            GenerateAutoBindComponentsCode(true);
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

        private void GenerateAutoBindComponentsCode(bool bindAllUIComponents)
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

                // 当前节点如果挂了 BaseItem 或其子类，则不再遍历它的子物体
                if (current != transform && current.GetComponent<BaseItem>() != null)
                {
                    continue;
                }
                
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

                System.Collections.Generic.List<(string componentType, string componentKey)> bindTargets =
                    bindAllUIComponents ? GetAllUIBindTargets(child) : GetConventionBindTargets(child);

                foreach (var bindTarget in bindTargets)
                {
                    string componentType = bindTarget.componentType;
                    string normalizeName = RemoveSpecialCharacters(child.gameObject.name);
                    string normalizeKey = RemoveSpecialCharacters(bindTarget.componentKey);

                    if (!tempDic.TryAdd($"{normalizeName}_{normalizeKey}", 1))
                    {
                        tempDic[$"{normalizeName}_{normalizeKey}"] += 1;
                        normalizeKey += _division + tempDic[$"{normalizeName}_{normalizeKey}"];
                    }

                    string fieldName = $"{normalizeName}_{normalizeKey}";

                    generatedCode.Append($"\t[SerializeField] private {componentType} {fieldName};\n");
                    string childPath = GetChildPath(child, prefab.transform);
                    if (componentType == typeof(UnityEngine.GameObject).ToString())
                    {
                        referenceCode.Append($"\t\t{fieldName} = transform.Find(\"{SelectiveEscape(childPath)}\").gameObject;\n");
                    }
                    else
                    {
                        referenceCode.Append($"\t\t{fieldName} = transform.Find(\"{SelectiveEscape(childPath)}\").GetComponent<{componentType}>();\n");
                    }

                    if (ShouldGenerateListener(componentType))
                    {
                        listenerComponents.Add((fieldName, componentType));
                    }

                    System.Text.RegularExpressions.Match matchArray =
                        System.Text.RegularExpressions.Regex.Match(child.gameObject.name, @"^(.*?)\[\d+\]$");
                    if (matchArray.Success)
                    {
                        string baseName = matchArray.Groups[1].Value;
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
                generatedCode.Append($"\t[SerializeField] private {componentType}[] {arrayName} = new {componentType}[{arraySize}];\n");

                // 生成数组元素赋值代码（使用元素自身的索引）
                foreach (var item in indexedItems)
                {
                    if (componentType == typeof(UnityEngine.GameObject).ToString())
                    {
                        referenceCode.Append($"\t\t{arrayName}[{item.index}] = transform.Find(\"{SelectiveEscape(item.path)}\").gameObject;\n");
                    }
                    else
                    {
                        referenceCode.Append($"\t\t{arrayName}[{item.index}] = transform.Find(\"{SelectiveEscape(item.path)}\").GetComponent<{componentType}>();\n");
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
                        listenerFieldCode.Append($"\t{listenerField}\n");
                    }
                }
                
                listenerCode.Append(listenerFieldCode);
                listenerCode.Append("\t// 自动生成\n");
                listenerCode.Append("\tprotected override void OnAddUIComponentListener()\n");
                listenerCode.Append("\t{\n");

                foreach (var component in listenerComponents)
                {
                    string listenerCall = GenerateListenerCall(component.fieldName, component.componentType);
                    if (!string.IsNullOrEmpty(listenerCall))
                    {
                        listenerCode.Append($"\t\t{listenerCall}\n");
                    }
                }

                listenerCode.Append("\t}\n");
            }

            // 将生成的代码插入到脚本中
            string scriptContent;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(scriptPath))
            {
                scriptContent = reader.ReadToEnd();
                // 关闭文件句柄
                reader.Close();
            }
            
            scriptContent = scriptContent.Replace("\r\n", "\n").Replace("\r", "\n");

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
                    $"// 自动获取组件（自动生成，不能删除）" +
                    $"\n\t[Header(\"===== 自动生成，组件绑定 =====\")]" +
                    $"\n{generatedCode}" +
                    $"\n{listenerCode}" +
                    $"\n#if UNITY_EDITOR" +
                    $"\n\t// 自动生成" +
                    $"\n\tprotected override void SetComponents()" +
                    $"\n\t{{" +
                    $"\n{referenceCode}" +
                    $"\t}}" +
                    $"\n#endif" +
                    $"\n\t// 自动获取组件（自动生成，不能删除）");
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
                LogF8.Log("在脚本中找不到插入标记。请在要生成代码的位置添加【// 自动获取组件（自动生成，不能删除）(.*?)// 自动获取组件（自动生成，不能删除）】");
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
                "UnityEngine.UI.ScrollRect",
                "TMPro.TMP_Dropdown",
                "TMPro.TMP_InputField"
            };

            return Array.Exists(listenerTypes, componentType.Equals);
        }

        private System.Collections.Generic.List<(string componentType, string componentKey)> GetConventionBindTargets(Transform child)
        {
            System.Collections.Generic.List<(string componentType, string componentKey)> bindTargets =
                new System.Collections.Generic.List<(string componentType, string componentKey)>();
            string[] nameDivision = child.gameObject.name.Split(_division);

            foreach (var name in nameDivision)
            {
                foreach (var pair in DefaultCodeBindNameTypeConfig.BindNameTypeDict)
                {
                    if (!name.Contains(pair.Key))
                        continue;
                    if (ContainsBindTarget(bindTargets, pair.Value))
                        continue;
                    if (!HasComponent(child, pair.Value))
                        continue;
                    bindTargets.Add((pair.Value, pair.Key));
                }
            }

            return bindTargets;
        }

        private System.Collections.Generic.List<(string componentType, string componentKey)> GetAllUIBindTargets(Transform child)
        {
            System.Collections.Generic.List<(string componentType, string componentKey)> bindTargets =
                new System.Collections.Generic.List<(string componentType, string componentKey)>();

            foreach (var pair in DefaultCodeBindNameTypeConfig.BindNameTypeDict)
            {
                if (NotUIBindingType(pair.Value))
                    continue;
                if (ContainsBindTarget(bindTargets, pair.Value))
                    continue;
                if (!HasComponent(child, pair.Value))
                    continue;

                bindTargets.Add((pair.Value, GetDefaultFieldSuffix(pair.Value, pair.Key)));
            }

            return bindTargets;
        }

        private bool ContainsBindTarget(System.Collections.Generic.List<(string componentType, string componentKey)> bindTargets, string componentType)
        {
            foreach (var bindTarget in bindTargets)
            {
                if (bindTarget.componentType == componentType)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasComponent(Transform child, string componentType)
        {
            string extension = System.IO.Path.GetExtension(componentType);
            string typeToCheck = string.IsNullOrEmpty(extension) ? componentType : extension[1..];
            return componentType == typeof(UnityEngine.GameObject).ToString() || child.GetComponent(typeToCheck);
        }

        private bool NotUIBindingType(string componentType)
        {
            return componentType == typeof (UnityEngine.GameObject).ToString() ||
                   componentType == typeof(UnityEngine.Transform).ToString();
        }

        private string GetDefaultFieldSuffix(string componentType, string fallbackKey)
        {
            int lastDotIndex = componentType.LastIndexOf('.');
            string suffix = lastDotIndex >= 0 ? componentType[(lastDotIndex + 1)..] : componentType;
            suffix = suffix.Replace("TMP_", "TMP");
            suffix = RemoveSpecialCharacters(suffix);
            return string.IsNullOrEmpty(suffix) ? fallbackKey : suffix;
        }

        // 生成监听事件字段
        private string GenerateListenerField(string fieldName, string componentType)
        {
            string normalizedFieldName = fieldName.Replace("[", "_").Replace("]", "_");

            if (componentType.Equals("UnityEngine.UI.Button"))
            {
                return $"private UnityEngine.Events.UnityAction unityAction_{normalizedFieldName};";
            }
            else if (componentType.Equals("UnityEngine.UI.Slider"))
            {
                return $"private UnityEngine.Events.UnityAction<float> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Equals("UnityEngine.UI.Scrollbar"))
            {
                return $"private UnityEngine.Events.UnityAction<float> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Equals("UnityEngine.UI.Dropdown"))
            {
                return $"private UnityEngine.Events.UnityAction<int> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Equals("UnityEngine.UI.Toggle"))
            {
                return $"private UnityEngine.Events.UnityAction<bool> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Equals("UnityEngine.UI.InputField"))
            {
                return $"private UnityEngine.Events.UnityAction<string> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Equals("UnityEngine.UI.ScrollRect"))
            {
                return $"private UnityEngine.Events.UnityAction<Vector2> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Equals("TMPro.TMP_Dropdown"))
            {
                return $"private UnityEngine.Events.UnityAction<int> unityAction_{normalizedFieldName};";
            }
            else if (componentType.Equals("TMPro.TMP_InputField"))
            {
                return $"private UnityEngine.Events.UnityAction<string> unityAction_{normalizedFieldName};";
            }

            return string.Empty;
        }

        // 生成监听事件调用代码
        private string GenerateListenerCall(string fieldName, string componentType)
        {
            string normalizedFieldName = fieldName.Replace("[", "_").Replace("]", "_");

            if (componentType.Equals("UnityEngine.UI.Button"))
            {
                return 
                    $"unityAction_{normalizedFieldName} = () => ButtonClick({fieldName});\n\t\t{fieldName}?.onClick.AddListener(unityAction_{normalizedFieldName});";
            }
            else if (componentType.Equals("UnityEngine.UI.Slider") ||
                     componentType.Equals("UnityEngine.UI.Scrollbar") ||
                     componentType.Equals("UnityEngine.UI.Dropdown") ||
                     componentType.Equals("UnityEngine.UI.Toggle") ||
                     componentType.Equals("UnityEngine.UI.InputField") ||
                     componentType.Equals("UnityEngine.UI.ScrollRect") ||
                     componentType.Equals("TMPro.TMP_Dropdown") ||
                     componentType.Equals("TMPro.TMP_InputField"))
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