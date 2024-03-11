using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using UnityEditor;

namespace F8Framework.Core.Editor
{
    /// <summary>
    /// 将代码编译为Dll
    /// </summary>
    public class CompileCodeDll
    {
        // scripts例子
        static string[] serializationCodeArray = new string[]
        {
            @"using System;
using System.Collections.Generic;
using UnityEngine;
using F8Framework.F8ExcelDataClass;

namespace F8Framework.Serialization
{
    public class SerializationDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();
        [SerializeField]
        private List<TValue> values = new List<TValue>();

        private Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> ToDictionary() { return target; }

        public SerializationDictionary()
        {
            this.target = this;
        }

        public void OnBeforeSerialize()
        {
        foreach (var kvp in this.target)
        {
            if (!keys.Contains(kvp.Key))
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
            else
            {
                int idx = keys.IndexOf(kvp.Key);
                if (idx >= values.Count)
                {
                    values.Add(kvp.Value);
                }
                else
                {
                    values[idx] = kvp.Value;
                }
            }
        }
        }

        public void OnAfterDeserialize()
        {
        this.target.Clear();
        int count = Math.Min(keys.Count, values.Count);
        for (int i = 0; i < count; i++)
        {
            this.target[keys[i]] = values[i];
        }
        }
    }
}
"
        };
        
        /// <summary>
        /// 编译dll
        /// </summary>
        /// <param name="scripts">多个代码</param>
        /// <param name="dllName">生成dll名字</param>
        /// <param name="path">生成路径</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Assembly CompileCode(string[] scripts, string dllName, string path)
        {
            //编译器实例对象
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            //编译器参数实例对象
            CompilerParameters objCompilerParameters = new CompilerParameters();
            string unityEnginePath = EditorApplication.applicationContentsPath + "/Managed/UnityEngine.dll";
            objCompilerParameters.ReferencedAssemblies.AddRange(new string[] { "System.dll", unityEnginePath }); // 需要引用的dll
            objCompilerParameters.OutputAssembly = Path.Combine(path, dllName, ".dll"); // 设置输出的程序集名
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;
            //开始编译脚本
            CompilerResults cr = codeProvider.CompileAssemblyFromSource(objCompilerParameters, scripts);
            if (cr.Errors.HasErrors)
            {
                foreach (CompilerError err in cr.Errors)
                {
                    LogF8.LogError("编译dll出错：" + err.ErrorText);
                }
                throw new Exception("编译dll出错！");
            }

            LogF8.Log("已编译 " + path + "/<color=#FFFF00>" + dllName + ".dll</color>");
            AssetDatabase.Refresh();
            return cr.CompiledAssembly;
        }
    }
}
