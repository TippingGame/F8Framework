using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Excel;
#if UNITY_WEBGL
using LitJson;
#else
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace F8Framework.Core.Editor
{
    public class ExcelDataTool : ScriptableObject
    {
        public const string CODE_NAMESPACE = "F8Framework.F8ExcelDataClass"; //由表生成的数据类型均在此命名空间内

        public const string
            BinDataFolder = "/AssetBundles/Config/BinConfigData"; //序列化的数据文件都会放在此文件夹内,此文件夹位于Resources文件夹下用于读取数据
        public const string DataManagerFolder = "/F8Framework/ConfigData/F8DataManager"; //Data代码路径
        public const string DataManagerName = "F8DataManager.cs"; //Data代码脚本名
        public const string ExcelPath = "/StreamingAssets/config"; //需要导表的目录
        public const string DLLFolder = "/F8Framework/ConfigData"; //存放dll目录
        public const string FileIndexFile = "config/fileindex.txt"; //fileindex文件目录
        private static List<string> codeList; //存放所有生成的类的代码

        private static Dictionary<string, List<ConfigData[]>> dataDict; //存放所有数据表内的数据，key：类名  value：数据

        // 使用StringBuilder来优化字符串的重复构造
        private static StringBuilder FileIndex = new StringBuilder();

        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<ExcelDataTool>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);

            // 获取绝对路径并规范化
            string scriptPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", scriptRelativePath));

            return scriptPath;
        }
        
        private static void CreateAsmdefFile()
        {
            // 创建.asmref文件的路径
            string asmrefPath = Application.dataPath + DataManagerFolder + "/F8DataManager.asmref";
            
            FileTools.CheckFileAndCreateDirWhenNeeded(asmrefPath);
            // 创建一个新的.asmdef文件
            string asmdefContent = @"{
    ""reference"": ""F8Framework.Core""
}";

            // 将内容写入.asmdef文件
            FileTools.SafeWriteAllText(asmrefPath, asmdefContent);
        }
        
        public static void LoadAllExcelData()
        {
            string INPUT_PATH = Application.dataPath + ExcelPath;

            FileTools.CheckDirAndCreateWhenNeeded(INPUT_PATH);

            var files = Directory.GetFiles(INPUT_PATH, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".xls") || s.EndsWith(".xlsx")).ToArray();
            if (files == null || files.Length == 0)
            {
                FileTools.SafeCopyFile(
                    FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) +
                    "/Tests/ExcelTool/StreamingAssets_config/Demo工作表.xlsx",
                    Application.streamingAssetsPath + "/config/Demo工作表.xlsx");
                FileTools.SafeCopyFile(
                    FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) +
                    "/Tests/Localization/StreamingAssets_config/本地化.xlsx",
                    Application.streamingAssetsPath + "/config/本地化.xlsx");
                files = Directory.GetFiles(INPUT_PATH, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".xls") || s.EndsWith(".xlsx")).ToArray();
                LogF8.LogError("暂无可以导入的数据表！自动为你创建：【Demo工作表.xlsx / 本地化.xlsx】两个表格！" + ExcelPath + " 目录");
            }
            
            string F8DataManagerPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/ConfigData/F8DataManager";
            FileTools.SafeDeleteDir(F8DataManagerPath);
            FileTools.CheckDirAndCreateWhenNeeded(F8DataManagerPath);
            string F8ExcelDataClassPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/ConfigData/F8ExcelDataClass";
            FileTools.SafeDeleteDir(F8ExcelDataClassPath);
            FileTools.CheckDirAndCreateWhenNeeded(F8ExcelDataClassPath);
            CreateAsmdefFile();
            AssetDatabase.Refresh();
            
            if (codeList == null)
            {
                codeList = new List<string>();
            }
            else
            {
                codeList.Clear();
            }

            if (dataDict == null)
            {
                dataDict = new Dictionary<string, List<ConfigData[]>>();
            }
            else
            {
                dataDict.Clear();
            }

            float step = 1f;
            FileIndex.Clear();
            FileTools.SafeDeleteFile(URLSetting.CS_STREAMINGASSETS_URL + FileIndexFile);
            FileTools.SafeDeleteFile(URLSetting.CS_STREAMINGASSETS_URL + FileIndexFile + ".meta");
            AssetDatabase.Refresh();
            FileTools.CheckFileAndCreateDirWhenNeeded(URLSetting.CS_STREAMINGASSETS_URL + FileIndexFile);
            foreach (string item in files)
            {
                step++;
                GetExcelData(item);
                OnLogCallBack(item.Substring(item.LastIndexOf('\\') + 1));
            }

            if (codeList.Count == 0)
            {
                EditorUtility.DisplayDialog("注意！！！", "\n暂无可以导入的数据表！", "确定");
                throw new Exception("暂无可以导入的数据表！");
            }

            //编译代码,生成包含所有数据表内数据类型的dll
            Assembly assembly = CompileCode(codeList.ToArray());
            AssetDatabase.Refresh();
            //准备序列化数据
            string BinDataPath = Application.dataPath + BinDataFolder; //序列化后的数据存放路径
            if (Directory.Exists(BinDataPath)) Directory.Delete(BinDataPath, true); //删除旧的数据文件
            Directory.CreateDirectory(BinDataPath);
            step = 1;
            foreach (KeyValuePair<string, List<ConfigData[]>> each in dataDict)
            {
                step++;
                //Assembly.CreateInstance 方法 (String) 使用区分大小写的搜索，从此程序集中查找指定的类型，然后使用系统激活器创建它的实例化对象
                object container = assembly.CreateInstance(CODE_NAMESPACE + "." + each.Key);
                Type temp = assembly.GetType(CODE_NAMESPACE + "." + each.Key + "Item");
                //序列化数据
                Serialize(container, temp, each.Value, BinDataPath);
            }
            ScriptGenerator.CreateDataManager(assembly);
            AssetDatabase.Refresh();
            LogF8.LogConfig("<color=yellow>导表成功!</color>");
        }

        [UnityEditor.MenuItem("开发工具/运行时读取Excel _F7", false, 101)]
        public static void ReLoadExcelData()
        {
            ReadExcel.Instance.LoadAllExcelData();
        }

        private static void OnLogCallBack(string condition)
        {
            FileIndex.Append(condition);
            if (FileIndex.Length <= 0) return;
            using (var sw = File.AppendText(URLSetting.CS_STREAMINGASSETS_URL + FileIndexFile))
            {
                sw.WriteLine(FileIndex.ToString());
            }

            FileIndex.Remove(0, FileIndex.Length);
        }

        //数据表内每一格数据
        class ConfigData
        {
            public string Type; //数据类型
            public string Name; //字段名
            public string Data; //数据值
        }

        private static void GetExcelData(string inputPath)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(inputPath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                EditorUtility.DisplayDialog("注意！！！", "\n请关闭 " + inputPath + " 后再导表！", "确定");
                throw new Exception("请关闭 " + inputPath + " 后再导表！");
            }

            IExcelDataReader excelReader = null;
            if (inputPath.EndsWith(".xls")) excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            else if (inputPath.EndsWith(".xlsx")) excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            if (!excelReader.IsValid)
            {
                throw new Exception("无法读取的文件:  " + inputPath);
            }
            else
            {
                do // 读取所有的sheet
                {
                    // sheet name
                    string className = excelReader.Name;
                    string[] types = null; //数据类型
                    string[] names = null; //字段名
                    List<ConfigData[]> dataList = new List<ConfigData[]>();
                    int index = 1;

                    //开始读取
                    while (excelReader.Read())
                    {
                        //这里读取的是每一行的数据
                        string[] datas = new string[excelReader.FieldCount];
                        for (int j = 0; j < excelReader.FieldCount; ++j)
                        {
                            datas[j] = excelReader.GetString(j);
                        }

                        //空行不处理
                        if (datas.Length == 0 || string.IsNullOrEmpty(datas[0]))
                        {
                            ++index;
                            continue;
                        }

                        //第1行表示类型
                        if (index == 1) types = datas;
                        //第2行表示变量名
                        else if (index == 2) names = datas;
                        //后面的表示数据
                        else if (index > 2)
                        {
                            if (types == null || names == null || datas == null){
                                throw new Exception("数据错误！["+ className +"]配置表！第" + index + "行" + inputPath);
                            }
                            //把读取的数据和数据类型,名称保存起来,后面用来动态生成类
                            List<ConfigData> configDataList = new List<ConfigData>();
                            for (int j = 0; j < datas.Length; ++j)
                            {
                                ConfigData data = new ConfigData();
                                data.Type = types[j];
                                data.Name = names[j];
                                data.Data = datas[j];
                                if (string.IsNullOrEmpty(data.Type) || string.IsNullOrEmpty(data.Data))
                                    continue; //空的数据不处理
                                configDataList.Add(data);
                            }

                            dataList.Add(configDataList.ToArray());
                        }

                        ++index;
                    }

                    if (string.IsNullOrEmpty(className))
                    {
                        throw new Exception("空的类名（excel页签名）, 路径:  " + inputPath);
                    }

                    if (names != null && types != null)
                    {
                        //根据刚才的数据来生成C#脚本
                        ScriptGenerator generator = new ScriptGenerator(inputPath, className, names, types);
                        //所有生成的类的代码最终保存在这个链表中
                        codeList.Add(generator.Generate());
                        if (dataDict.ContainsKey(className))
                        {
                            throw new Exception("类名重复: " + className + " ,路径:  " + inputPath);
                        }

                        dataDict.Add(className, dataList);
                    }
                } while (excelReader.NextResult()); //excelReader.NextResult() Excel表下一个sheet页有没有数据
            }

            stream.Dispose();
            stream.Close();
        }

        //编译代码
        private static Assembly CompileCode(string[] scripts)
        {
            string path = Application.dataPath + DLLFolder + "/F8ExcelDataClass";
            if (Directory.Exists(path)) Directory.Delete(path, true); //删除旧dll
            Directory.CreateDirectory(path);
            //编译器实例对象
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            //编译器参数实例对象
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.AddRange(new string[] { "System.dll" }); //添加程序集引用
            objCompilerParameters.OutputAssembly = path + "/" + CODE_NAMESPACE + ".dll"; //设置输出的程序集名
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

                throw new Exception("编译dll出错！请检查配置表格式！");
            }

            LogF8.LogConfig("已编译 " + path + "/<color=#FFFF00>" + CODE_NAMESPACE + ".dll</color>");
            return cr.CompiledAssembly;
        }

        //序列化对象
        private static void Serialize(object container, Type temp, List<ConfigData[]> dataList, string BinDataPath)
        {
            //设置数据
            foreach (ConfigData[] datas in dataList)
            {
                //Type.FullName 获取该类型的完全限定名称，包括其命名空间，但不包括程序集。
                object t = temp.Assembly.CreateInstance(temp.FullName);
                foreach (ConfigData data in datas)
                {
                    //Type.GetField(String) 搜索Type内指定名称的公共字段。
                    FieldInfo info = temp.GetField(data.Name);
                    // FieldInfo.SetValue 设置对象内指定名称的字段的值
                    if (info != null)
                    {
                        info.SetValue(t, ReadExcel.ParseValue(data.Type, data.Data, temp.Name));
                    }
                    else
                    {
                        //2019.4.28f1,2020.3.33f1都出现的BUG（2021.3.8f1测试通过），编译dll后没及时刷新，导致修改name或id后读取失败，需要二次编译
                        LogF8.LogConfig("info是空的：" + data.Name);
                    }
                }

                // FieldInfo.GetValue 获取对象内指定名称的字段的值
                object id = temp.GetField("id").GetValue(t); //获取id
                FieldInfo dictInfo = container.GetType().GetField("Dict");
                object dict = dictInfo.GetValue(container);

                bool isExist = (bool)dict.GetType().GetMethod("ContainsKey").Invoke(dict, new System.Object[] { id });
                if (isExist)
                {
                    EditorUtility.DisplayDialog("注意！！！", "ID重复：" + id + "，类型： " + container.GetType().Name, "确定");
                    throw new Exception("ID重复：" + id + "，类型： " + container.GetType().Name);
                }

                dict.GetType().GetMethod("Add").Invoke(dict, new System.Object[] { id, t });
            }
#if UNITY_WEBGL
            // 序列化对象
            string json = Util.LitJson.ToJson(container);
            // 写入到文件
            string filePath = BinDataPath + "/" + container.GetType().Name + ".json";
            FileTools.SafeWriteAllText(filePath, json);
            // 记录日志
            LogF8.LogConfig("已序列化 " + BinDataPath + "/<color=#FFFF00>" + container.GetType().Name + ".json</color>");
#else
            IFormatter f = new BinaryFormatter();
            Stream s = new FileStream(BinDataPath + "/" + container.GetType().Name + ".bytes", FileMode.OpenOrCreate,
                FileAccess.Write, FileShare.Write);
            f.Serialize(s, container);
            LogF8.LogConfig("已序列化 " + BinDataPath + "/<color=#FFFF00>" + container.GetType().Name + ".bytes</color>");
            s.Close();
#endif
        }
    }
}