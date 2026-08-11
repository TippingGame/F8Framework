using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System;
using System.Linq;
using System.Text;
using F8Framework.Core;
using F8Framework.Core.Editor;
using F8Framework.ExcelData;
using UnityEngine;
using UnityEditor;
using Excel;
using Assembly = System.Reflection.Assembly;

namespace F8Framework.ExcelData.Editor
{
    public class ExcelDataTool : ScriptableObject
    {
        public const string CODE_NAMESPACE = "F8Framework.F8ExcelDataClass"; //由表生成的数据类型均在此命名空间内

        public const string BinDataFolder = "/AssetBundles/Config/BinConfigData"; //序列化的数据文件默认目录，可在F5打包界面修改
        public const string DataManagerFolder = "/F8Framework/ConfigData/F8DataManager"; //Data代码路径
        public const string DataManagerName = "F8DataManager.cs"; //Data代码脚本名
        public const string ExcelPath = "/StreamingAssets/config"; //需要导表的目录
        public const string DLLFolder = "/F8Framework/ConfigData"; //存放dll目录
        public const string FileIndexFile = "config/fileindex.txt"; //fileindex文件目录
        private static Dictionary<string, ScriptGenerator> codeList; //存放所有生成的类的代码

        private static Dictionary<string, List<ReadExcel.ConfigData[]>> dataDict; //存放所有数据表内的数据，key：类名  value：数据
        
        private static string GetScriptPath()
        {
            MonoScript monoScript = MonoScript.FromScriptableObject(CreateInstance<ExcelDataTool>());

            // 获取脚本在 Assets 中的相对路径
            string scriptRelativePath = AssetDatabase.GetAssetPath(monoScript);

            // 获取绝对路径并规范化
            string scriptPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", scriptRelativePath));

            return scriptPath;
        }
        
        private static bool CreateAsmdefFile()
        {
            string asmdefPath = Application.dataPath + DLLFolder + "/" + CODE_NAMESPACE + ".asmdef";
            string asmdefContent = @"{
    ""name"": ""F8Framework.F8ExcelDataClass"",
    ""references"": [
        ""F8Framework.Core"",
        ""LitJson""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}";

            bool changed = WriteTextIfChanged(asmdefPath, asmdefContent);
            LogF8.LogConfig(
                (changed ? "已更新程序集定义 " : "程序集定义无需更新 ") +
                Application.dataPath + DLLFolder + "/<color=#FF9E59>" +
                CODE_NAMESPACE + ".asmdef</color>");
            return changed;
        }

        public static void LoadAllExcelData()
        {
            ExcelDataSettings.EnsureDefaults();
            F8EditorPipelineBuilder builder = new F8EditorPipelineBuilder("Excel 导表");
            builder.Add(
                ExcelPipelineStepIds.GenerateCode,
                F8BuildPipelineOrder.ConfigurationGenerate,
                "从 Excel 生成配置代码");
            builder.Add(
                ExcelPipelineStepIds.SerializeData,
                F8BuildPipelineOrder.ConfigurationSerialize,
                "序列化 Excel 配置数据");
            F8EditorPipeline.Start(builder);
        }

        internal static bool GenerateCode()
        {
            ExcelDataSettings.EnsureDefaults();
            string lastExcelPath = ExcelDataSettings.SourcePath;
            
            string INPUT_PATH = lastExcelPath;

            FileTools.CheckDirAndCreateWhenNeeded(INPUT_PATH);
            
            string[] files = GetExcelFiles(INPUT_PATH);
            if (files.Length == 0)
            {
                FileTools.SafeCopyFile(
                    FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) +
                    "/Runtime/ExcelTool/StreamingAssets_config/DemoWorkSheet.xlsx",
                    lastExcelPath + "/DemoWorkSheet.xlsx");
                FileTools.SafeCopyFile(
                    FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) +
                    "/Runtime/Localization/StreamingAssets_config/Localization.xlsx",
                    lastExcelPath + "/Localization.xlsx");
                files = GetExcelFiles(INPUT_PATH);
                LogF8.LogError("暂无可以导入的数据表！自动为你创建：【DemoWorkSheet.xlsx / Localization.xlsx】两个表格！" + lastExcelPath + " 目录");
            }

            ResetGeneratedData();
            
            foreach (string item in files)
            {
                GetExcelData(item);
            }

            if (codeList.Count == 0)
            {
                EditorUtility.DisplayDialog("注意！！！", "\n暂无可以导入的数据表！", "确定");
                throw new Exception("暂无可以导入的数据表！");
            }
            
            bool scriptsChanged = false;
            string F8ExcelDataClassPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/ConfigData/F8ExcelDataClass";
            FileTools.CheckDirAndCreateWhenNeeded(F8ExcelDataClassPath);
            
            scriptsChanged |= GenerateCodeFiles(codeList);
            
            string F8DataManagerPath = FileTools.FormatToUnityPath(FileTools.TruncatePath(GetScriptPath(), 3)) + "/ConfigData/F8DataManager";
            FileTools.CheckDirAndCreateWhenNeeded(F8DataManagerPath);
            scriptsChanged |= DeleteUnexpectedGeneratedFiles(
                F8DataManagerPath,
                new[] { Path.Combine(F8DataManagerPath, DataManagerName) });
            
            scriptsChanged |= ScriptGenerator.CreateDataManager(codeList);
            
            string obsoleteAsmrefPath = Application.dataPath + DataManagerFolder + "/F8DataManager.asmref";
            scriptsChanged |= DeleteGeneratedFileWithMeta(obsoleteAsmrefPath);
            scriptsChanged |= CreateAsmdefFile();
            WriteFileIndex(INPUT_PATH, files);

            return scriptsChanged;
        }

        internal static void SerializeGeneratedData()
        {
            LogF8.LogConfig("<color=#FF9E59>导表后脚本编译完成!</color>");
            string lastExcelPath = ExcelDataSettings.SourcePath;
            string INPUT_PATH = lastExcelPath;
            string[] files = GetExcelFiles(INPUT_PATH);
            if (files.Length == 0)
            {
                throw new InvalidOperationException("暂无可以序列化的数据表：" + INPUT_PATH);
            }

            ResetGeneratedData();
            foreach (string item in files)
            {
                GetExcelData(item);
            }

            if (dataDict.Count == 0)
            {
                throw new InvalidOperationException("Excel 中没有可序列化的数据页签：" + INPUT_PATH);
            }

            Assembly assembly = Util.Assembly.GetAssembly(CODE_NAMESPACE);
            if (assembly == null)
            {
                throw new InvalidOperationException(
                    "找不到 Excel 生成程序集：" + CODE_NAMESPACE + "。请确认脚本编译成功后重试。");
            }

            string BinDataPath = ValidateOutputDirectory(
                ExcelDataSettings.OutputPath,
                INPUT_PATH);
            string stagingPath = CreateStagingDirectory(BinDataPath);
            List<string> serializedFiles = new List<string>();
            try
            {
                foreach (KeyValuePair<string, List<ReadExcel.ConfigData[]>> each in
                         dataDict.OrderBy(item => item.Key, StringComparer.Ordinal))
                {
                    string containerTypeName = CODE_NAMESPACE + "." + each.Key;
                    string itemTypeName = containerTypeName + "Item";
                    Type containerType = assembly.GetType(containerTypeName);
                    Type itemType = assembly.GetType(itemTypeName);
                    if (containerType == null || itemType == null)
                    {
                        throw new InvalidOperationException(
                            "Excel 生成类型与当前表格不一致：" +
                            (containerType == null ? containerTypeName : itemTypeName) +
                            "。请重新生成代码并等待编译完成。");
                    }

                    object container;
                    try
                    {
                        container = Activator.CreateInstance(containerType);
                    }
                    catch (Exception exception)
                    {
                        throw new InvalidOperationException(
                            "无法创建 Excel 配置容器：" + containerTypeName,
                            exception);
                    }

                    serializedFiles.Add(Serialize(container, itemType, each.Value, stagingPath));
                }

                CommitGeneratedDirectory(stagingPath, BinDataPath);
            }
            finally
            {
                if (Directory.Exists(stagingPath))
                {
                    Directory.Delete(stagingPath, true);
                }
            }

            foreach (string serializedFile in serializedFiles)
            {
                LogF8.LogConfig(
                    "已序列化 " + BinDataPath + "/<color=#FFFF00>" +
                    Path.GetFileName(serializedFile) + "</color>");
            }

            LogF8.LogConfig("<color=yellow>导表成功!</color>");
            AssetDatabase.Refresh();
        }
        
        [UnityEditor.MenuItem("开发工具/运行时读取Excel _F7", false, 101)]
        public static void ReLoadExcelData()
        {
            ReadExcel.Instance.LoadAllExcelData();
        }

        private static string[] GetExcelFiles(string inputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath) || !Directory.Exists(inputPath))
            {
                return Array.Empty<string>();
            }

            return Directory.EnumerateFiles(inputPath, "*.*", SearchOption.AllDirectories)
                .Where(IsExcelFile)
                .OrderBy(path => GetRelativeExcelPath(inputPath, path), StringComparer.Ordinal)
                .ToArray();
        }

        private static bool IsExcelFile(string path)
        {
            string extension = Path.GetExtension(path);
            return (string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase)) &&
                   !Path.GetFileName(path).StartsWith("~$", StringComparison.Ordinal);
        }

        private static void ResetGeneratedData()
        {
            codeList = new Dictionary<string, ScriptGenerator>(StringComparer.OrdinalIgnoreCase);
            dataDict = new Dictionary<string, List<ReadExcel.ConfigData[]>>(
                StringComparer.OrdinalIgnoreCase);
        }

        private static void WriteFileIndex(string inputPath, IEnumerable<string> files)
        {
            string[] relativePaths = files
                .Select(path => GetRelativeExcelPath(inputPath, path))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
            string content = relativePaths.Length == 0
                ? string.Empty
                : string.Join("\n", relativePaths) + "\n";
            string indexPath = URLSetting.CS_STREAMINGASSETS_URL + FileIndexFile;
            bool changed = WriteTextIfChanged(indexPath, content);
            LogF8.LogConfig(
                (changed ? "已更新文件索引 " : "文件索引无需更新 ") +
                "<color=#FF9E59>" + indexPath + "</color>");
        }

        private static string GetRelativeExcelPath(string inputPath, string filePath)
        {
            string directory = NormalizeDirectoryPath(inputPath);
            string file = Path.GetFullPath(filePath)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            string prefix = directory.EndsWith(Path.DirectorySeparatorChar.ToString(), PathComparison)
                ? directory
                : directory + Path.DirectorySeparatorChar;
            if (!file.StartsWith(prefix, PathComparison))
            {
                throw new InvalidOperationException("Excel 文件不在配置目录内：" + filePath);
            }

            return file.Substring(prefix.Length).Replace('\\', '/');
        }

        private static string ValidateOutputDirectory(string outputPath, string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new InvalidOperationException("请先设置导表目录。");
            }

            string output = NormalizeDirectoryPath(outputPath);
            string projectRoot = NormalizeDirectoryPath(Path.Combine(Application.dataPath, ".."));
            string assetsRoot = NormalizeDirectoryPath(Application.dataPath);
            string pathRoot = NormalizeDirectoryPath(Path.GetPathRoot(output));
            if (string.Equals(output, pathRoot, PathComparison) ||
                IsSameOrParentDirectory(output, projectRoot) ||
                string.Equals(output, assetsRoot, PathComparison))
            {
                throw new InvalidOperationException("导表目录范围过大，禁止替换：" + output);
            }

            if (!string.IsNullOrWhiteSpace(sourcePath) &&
                IsSameOrParentDirectory(output, NormalizeDirectoryPath(sourcePath)))
            {
                throw new InvalidOperationException("导表目录不能包含 Excel 源目录：" + output);
            }

            if (File.Exists(output))
            {
                throw new InvalidOperationException("导表目录被同名文件占用：" + output);
            }

            ValidateGeneratedDirectoryContents(output, "现有导表目录");
            return output;
        }

        private static void ValidateGeneratedDirectoryContents(string path, string displayName)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            string unexpectedDirectory = Directory
                .EnumerateDirectories(path, "*", SearchOption.AllDirectories)
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(unexpectedDirectory))
            {
                throw new InvalidOperationException(
                    displayName + "包含子目录，为避免误删已停止替换：" + unexpectedDirectory);
            }

            string unexpectedFile = Directory
                .EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(file =>
                {
                    string extension = Path.GetExtension(file);
                    return !string.Equals(extension, ".bytes", StringComparison.OrdinalIgnoreCase) &&
                           !string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase) &&
                           !string.Equals(extension, ".meta", StringComparison.OrdinalIgnoreCase);
                });
            if (!string.IsNullOrEmpty(unexpectedFile))
            {
                throw new InvalidOperationException(
                    displayName + "包含非配置文件，为避免误删已停止替换：" + unexpectedFile);
            }
        }

        private static string CreateStagingDirectory(string outputPath)
        {
            string parent = Path.GetDirectoryName(outputPath);
            if (string.IsNullOrEmpty(parent))
            {
                throw new InvalidOperationException("无法确定导表目录的父目录：" + outputPath);
            }

            Directory.CreateDirectory(parent);
            string stagingPath = Path.Combine(
                parent,
                "." + Path.GetFileName(outputPath) +
                ".f8-staging-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(stagingPath);
            return stagingPath;
        }

        private static void CommitGeneratedDirectory(string stagingPath, string outputPath)
        {
            string staging = NormalizeDirectoryPath(stagingPath);
            string output = ValidateOutputDirectory(outputPath, null);
            if (!Directory.Exists(staging))
            {
                throw new DirectoryNotFoundException("找不到待提交的导表目录：" + staging);
            }

            if (!string.Equals(
                    Path.GetDirectoryName(staging),
                    Path.GetDirectoryName(output),
                    PathComparison))
            {
                throw new InvalidOperationException("临时导表目录必须与最终目录位于同一父目录。");
            }

            ValidateGeneratedDirectoryContents(staging, "临时导表目录");
            string backupPath = Path.Combine(
                Path.GetDirectoryName(output),
                "." + Path.GetFileName(output) +
                ".f8-backup-" + Guid.NewGuid().ToString("N"));
            bool existingMoved = false;
            try
            {
                if (Directory.Exists(output))
                {
                    Directory.Move(output, backupPath);
                    existingMoved = true;
                }

                Directory.Move(staging, output);
            }
            catch (Exception commitException)
            {
                if (existingMoved && !Directory.Exists(output) && Directory.Exists(backupPath))
                {
                    try
                    {
                        Directory.Move(backupPath, output);
                    }
                    catch (Exception restoreException)
                    {
                        throw new AggregateException(
                            "提交新配置失败，并且旧配置恢复失败：" + output,
                            commitException,
                            restoreException);
                    }
                }

                throw;
            }

            if (existingMoved && Directory.Exists(backupPath))
            {
                try
                {
                    Directory.Delete(backupPath, true);
                }
                catch (Exception cleanupException)
                {
                    LogF8.LogWarning(
                        "新配置已生效，但旧配置备份清理失败：" +
                        backupPath + "\n" + cleanupException.Message);
                }
            }
        }

        private static string NormalizeDirectoryPath(string path)
        {
            string fullPath = Path.GetFullPath(path)
                .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            string root = Path.GetPathRoot(fullPath);
            return string.Equals(fullPath, root, PathComparison)
                ? fullPath
                : fullPath.TrimEnd(Path.DirectorySeparatorChar);
        }

        private static bool IsSameOrParentDirectory(string parentPath, string childPath)
        {
            if (string.Equals(parentPath, childPath, PathComparison))
            {
                return true;
            }

            string prefix = parentPath.EndsWith(Path.DirectorySeparatorChar.ToString(), PathComparison)
                ? parentPath
                : parentPath + Path.DirectorySeparatorChar;
            return childPath.StartsWith(prefix, PathComparison);
        }

        private static StringComparison PathComparison =>
            Application.platform == RuntimePlatform.WindowsEditor
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
        
        private static void GetExcelData(string inputPath)
        {
            FileStream stream = null;
            IExcelDataReader excelReader = null;
            try
            {
                stream = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                
                string extension = Path.GetExtension(inputPath);
                if (string.Equals(extension, ".xls", StringComparison.OrdinalIgnoreCase))
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                else if (string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                if (excelReader == null || !excelReader.IsValid)
                {
                    throw new Exception("无法读取的文件:  " + inputPath);
                }
                do // 读取所有的sheet
                {
                    // sheet name
                    string className = excelReader.Name;
                    string[] types = null; //数据类型
                    string[] names = null; //字段名
                    List<ReadExcel.ConfigData[]> dataList = new List<ReadExcel.ConfigData[]>();
                    int index = 1;
                    //把读取的数据和数据类型,名称保存起来,后面用来动态生成类
                    List<ReadExcel.ConfigData> configDataList = new List<ReadExcel.ConfigData>();
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
                            if (types == null || names == null || datas == null)
                            {
                                throw new Exception("数据错误！[" + className + "]配置表！第" + index + "行" + inputPath);
                            }
                            
                            configDataList.Clear();
                            for (int j = 0; j < datas.Length; ++j)
                            {
                                if (string.IsNullOrEmpty(types[j]))
                                    continue; //空的数据不处理
                                
                                ReadExcel.ConfigData data = new ReadExcel.ConfigData();
                                data.Type = types[j];
                                data.Name = names[j];
                                data.Data = datas[j];
                                
                                configDataList.Add(data);
                            }

                            ReadExcel.VariantInfoDict(ref configDataList);
                            
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
                        List<ReadExcel.ConfigData> scriptConfigDataList = configDataList.Count > 0
                            ? configDataList
                            : CreateConfigDataList(types, names);
                        //根据刚才的数据来生成C#脚本
                        ScriptGenerator generator = new ScriptGenerator(inputPath, className, scriptConfigDataList);
                        //所有生成的类的代码最终保存在这
                        if (codeList.ContainsKey(className))
                        {
                            throw new Exception("类名重复: " + className + " ,路径:  " + inputPath);
                        }

                        codeList.Add(className, generator);
                        if (dataDict.ContainsKey(className))
                        {
                            throw new Exception("类名重复: " + className + " ,路径:  " + inputPath);
                        }

                        dataDict.Add(className, dataList);
                    }
                } while (excelReader.NextResult()); //excelReader.NextResult() Excel表下一个sheet页有没有数据
            }
            catch (IOException)
            {
                EditorUtility.DisplayDialog("注意！！！", "\n请关闭 " + inputPath + " 后再导表！", "确定");
                throw new Exception("请关闭 " + inputPath + " 后再导表！");
            }
            catch (Exception ex)
            {
                LogF8.LogError($"处理Excel文件失败: {inputPath}, 错误: {ex.Message}");
                throw;
            }
            finally
            {
                excelReader?.Dispose();
                stream?.Dispose();
            }
        }

        private static List<ReadExcel.ConfigData> CreateConfigDataList(string[] types, string[] names)
        {
            List<ReadExcel.ConfigData> configDataList = new List<ReadExcel.ConfigData>();
            int count = Math.Min(types.Length, names.Length);
            for (int i = 0; i < count; i++)
            {
                if (string.IsNullOrEmpty(types[i]))
                    continue;

                configDataList.Add(new ReadExcel.ConfigData
                {
                    Type = types[i],
                    Name = names[i],
                    Data = string.Empty
                });
            }

            ReadExcel.VariantInfoDict(ref configDataList);
            return configDataList;
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

        // 生成代码文件
        public static bool GenerateCodeFiles(Dictionary<string, ScriptGenerator> codeList)
        {
            string path = Application.dataPath + DLLFolder + "/F8ExcelDataClass";
            FileTools.CheckDirAndCreateWhenNeeded(path);
            bool changed = false;
            HashSet<string> expectedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, ScriptGenerator> kvp in
                     codeList.OrderBy(item => item.Key, StringComparer.Ordinal))
            {
                string filePath = $"{path}/{kvp.Key}.cs";
                expectedFiles.Add(Path.GetFullPath(filePath));
                try
                {
                    bool fileChanged = WriteTextIfChanged(filePath, kvp.Value.Generate());
                    changed |= fileChanged;
                    LogF8.LogConfig(
                        (fileChanged ? "已生成代码 " : "代码无需更新 ") +
                        path + "/<color=#FF9E59>" + kvp.Key + ".cs</color>");
                }
                catch (Exception e)
                {
                    LogF8.LogException(e);
                    throw new Exception("表格生成错误，修改后重试F8：" + kvp.Key + ".cs" + "\n");
                }
            }

            changed |= DeleteUnexpectedGeneratedFiles(path, expectedFiles);
            return changed;
        }

        internal static bool WriteTextIfChanged(string path, string content)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            content ??= string.Empty;
            FileTools.CheckFileAndCreateDirWhenNeeded(path);
            bool assetMetadataMissing = path.StartsWith(Application.dataPath, StringComparison.OrdinalIgnoreCase) &&
                                        !File.Exists(path + ".meta");
            if (File.Exists(path) && File.ReadAllText(path) == content)
            {
                return assetMetadataMissing;
            }

            File.WriteAllText(path, content, new UTF8Encoding(false));
            return true;
        }

        private static bool DeleteUnexpectedGeneratedFiles(
            string directory,
            IEnumerable<string> expectedFiles)
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }

            HashSet<string> expected = new HashSet<string>(
                expectedFiles.Select(Path.GetFullPath),
                StringComparer.OrdinalIgnoreCase);
            bool changed = false;

            foreach (string file in Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly))
            {
                if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) ||
                    expected.Contains(Path.GetFullPath(file)))
                {
                    continue;
                }

                changed |= DeleteGeneratedFileWithMeta(file);
            }

            foreach (string metaFile in Directory.GetFiles(directory, "*.meta", SearchOption.TopDirectoryOnly))
            {
                string assetFile = metaFile.Substring(0, metaFile.Length - ".meta".Length);
                if (!File.Exists(assetFile))
                {
                    FileTools.SafeDeleteFile(metaFile);
                    changed = true;
                }
            }

            return changed;
        }

        private static bool DeleteGeneratedFileWithMeta(string path)
        {
            bool existed = File.Exists(path) || File.Exists(path + ".meta");
            FileTools.SafeDeleteFile(path);
            FileTools.SafeDeleteFile(path + ".meta");
            return existed;
        }
        
        //序列化对象
        private static string Serialize(object container, Type temp, List<ReadExcel.ConfigData[]> dataList, string BinDataPath)
        {
            if (container == null)
            {
                throw new InvalidOperationException("Excel 配置容器不能为空。");
            }

            if (temp == null)
            {
                throw new InvalidOperationException("Excel 配置项类型不能为空。");
            }

            const BindingFlags memberFlags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo dictInfo = container.GetType().GetField("Dict", memberFlags);
            if (dictInfo == null ||
                !(dictInfo.GetValue(container) is System.Collections.IDictionary dict))
            {
                throw new InvalidOperationException(
                    "Excel 配置容器缺少可用的 Dict 字段：" + container.GetType().FullName);
            }

            FieldInfo fieldInfoId = temp
                .GetFields(memberFlags)
                .FirstOrDefault(field =>
                    string.Equals(field.Name, "id", StringComparison.OrdinalIgnoreCase));
            PropertyInfo propertyInfoId = fieldInfoId == null
                ? temp.GetProperties(memberFlags).FirstOrDefault(property =>
                    string.Equals(property.Name, "id", StringComparison.OrdinalIgnoreCase) &&
                    property.CanRead && property.GetIndexParameters().Length == 0)
                : null;
            if (fieldInfoId == null && propertyInfoId == null)
            {
                throw new InvalidOperationException(
                    "Excel 配置项缺少 id 字段或属性：" + temp.FullName);
            }

            //设置数据
            foreach (ReadExcel.ConfigData[] datas in dataList)
            {
                //Type.FullName 获取该类型的完全限定名称，包括其命名空间，但不包括程序集。
                object t;
                try
                {
                    t = Activator.CreateInstance(temp);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException(
                        "无法创建 Excel 配置项：" + temp.FullName,
                        exception);
                }

                foreach (ReadExcel.ConfigData data in datas)
                {
                    if (data.VariantInfo != null)
                    {
                        // variant<字段名,变体名> 只为基础字段补充数据，本身不会生成成员。
                        if (!data.VariantInfo.HasVariant)
                        {
                            continue;
                        }

                        string name = "_" + data.Name + "Variants";
                        FieldInfo variantDictField = temp.GetField(name, memberFlags);
                        if (variantDictField == null ||
                            !(variantDictField.GetValue(t) is System.Collections.IDictionary variantDict))
                        {
                            throw new InvalidOperationException(
                                "Excel 变体字段与生成类型不一致：" + temp.FullName + "." + name);
                        }

                        foreach (var variantData in data.VariantInfo.Variants)
                        {
                            object variantValue = ReadExcel.ParseValue(data.Type, variantData.Value, temp.Name);
                            variantDict.Add(variantData.Key, variantValue);
                        }
                        
                        variantDictField.SetValue(t, variantDict);
                    }
                    else
                    {
                        if (data.Name.IsNullOrEmpty())
                        {
                            continue;
                        }
                        FieldInfo info = temp.GetField(data.Name, memberFlags);
                        // FieldInfo.SetValue 设置对象内指定名称的字段的值
                        if (info == null)
                        {
                            throw new InvalidOperationException(
                                "Excel 字段与生成类型不一致：" + temp.FullName + "." + data.Name);
                        }

                        info.SetValue(t, ReadExcel.ParseValue(data.Type, data.Data, temp.Name));
                    }
                }

                // FieldInfo.GetValue 获取对象内指定名称的字段的值
                object id = fieldInfoId != null
                    ? fieldInfoId.GetValue(t)
                    : propertyInfoId.GetValue(t);
                if (id == null)
                {
                    throw new InvalidOperationException(
                        "Excel 配置 ID 不能为空，类型：" + temp.FullName);
                }

                if (dict.Contains(id))
                {
                    if (!Application.isBatchMode)
                    {
                        EditorUtility.DisplayDialog(
                            "注意！！！",
                            "ID重复：" + id + "，类型： " + container.GetType().Name,
                            "确定");
                    }

                    throw new Exception("ID重复：" + id + "，类型： " + container.GetType().Name);
                }

                dict.Add(id, t);
            }

            string exportFormat = ExcelDataSettings.ExportFormat;
            if (exportFormat == ExcelDataSettings.BinaryFormat)
            {
                string filePath = Path.Combine(BinDataPath, container.GetType().Name + ".bytes");
                Util.BinarySerializer.SerializeToFile(container, filePath);
                return filePath;
            }
            else
            {
                string json = Util.LitJson.ToJson(container);
                string filePath = Path.Combine(BinDataPath, container.GetType().Name + ".json");
                FileTools.SafeWriteAllText(filePath, json);
                return filePath;
            }
        }
    }
}
