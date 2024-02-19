using UnityEngine;

namespace F8Framework.Core
{
    public class ComponentBind : MonoBehaviour
    {
        protected virtual void SetComponents()
        {
            LogF8.Log("首次添加需要点击组件绑定按钮");
        }
        
#if UNITY_EDITOR
        // 分隔符
        private string _division = "_";
        
        public void Bind()
        {
            GenerateAutoBindComponentsCode();
        }
        
        public void Reset()
        {
            Bind();
        }
        
        private void GenerateAutoBindComponentsCode()
        {
            string scriptPath = GetScriptPath(this);

            // Load the prefab
            GameObject prefab = gameObject;

            // 自动生成的代码将会放在这个字符串中
            System.Text.StringBuilder generatedCode = new System.Text.StringBuilder();
            // 自动生成的引用代码
            System.Text.StringBuilder referenceCode = new System.Text.StringBuilder();
            
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
            // 遍历Prefab中的所有物体
            foreach (Transform child in childs)
            {
                if (child.Equals(transform))// 忽略自身
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

                        if (!child.GetComponent(componentType))
                            continue;
                            
                        string normalizeName = RemoveSpecialCharacters(child.gameObject.name);

                        string normalizeKey = RemoveSpecialCharacters(key);
                        // 生成自动获取组件的代码
                        generatedCode.AppendLine($"    [SerializeField] private {componentType} {normalizeName}_{normalizeKey};");
                        // 生成引用代码
                        string childPath = GetChildPath(child, prefab.transform);
                        if (componentType == "GameObject")
                        {
                            referenceCode.AppendLine($"        {normalizeName}_{normalizeKey} = transform.Find(\"{SelectiveEscape(childPath)}\").gameObject;");
                        }
                        else
                        {
                            referenceCode.AppendLine($"        {normalizeName}_{normalizeKey} = transform.Find(\"{SelectiveEscape(childPath)}\").GetComponent<{componentType}>();");
                        }
                    }
                }
            }

            // 将生成的代码插入到脚本中
            string scriptContent = System.IO.File.ReadAllText(scriptPath);

            // 使用正则表达式匹配并替换注释之间的内容
            string pattern = @"// Auto Bind Components(.*?)// Auto Bind Components";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Match match = regex.Match(scriptContent);

            if (match.Success)
            {
                // 替换注释之间的内容，包含头尾的注释
                scriptContent = scriptContent.Remove(match.Groups[0].Index, match.Groups[0].Length);
                scriptContent = scriptContent.Insert(match.Groups[0].Index, $"// Auto Bind Components\n{generatedCode}" +
                                                                            $"\n#if UNITY_EDITOR" +
                                                                            $"\n    protected override void SetComponents()" +
                                                                            $"\n    {{" +
                                                                            $"\n{referenceCode}" +
                                                                            $"\n    }}" +
                                                                            $"\n#endif" +
                                                                            $"\n    // Auto Bind Components");
                
                // 将所有换行符替换为 UNIX 风格的 "\n"
                scriptContent = scriptContent.Replace("\r\n", "\n").Replace("\r", "\n");
                
                // 保存脚本文件
                System.IO.File.WriteAllText(scriptPath, scriptContent);

                UnityEditor.AssetDatabase.Refresh();
                
                SetComponents();
                
                UnityEditor.EditorUtility.SetDirty(this);
            }
            else
            {
                LogF8.Log("在脚本中找不到插入标记。请在要生成代码的位置添加“// Auto Bind Components”。");
            }
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
            return regex.Replace(input, "");
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