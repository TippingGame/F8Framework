using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Excel;
using Assembly = System.Reflection.Assembly;

namespace F8Framework.Core
{
    public class SupportType
    {
        public const string INT = "int";
        public const string LONG = "long";
        public const string FLOAT = "float";
        public const string DOUBLE = "double";
        public const string STRING = "str";
        public const string STRINGFULL = "string";
        public const string OBJ = "obj";
        public const string OBJFULL = "object";
       
        public const string ARRAY_INT = "int[]";
        public const string ARRAY_LONG = "long[]";
        public const string ARRAY_FLOAT = "float[]";
        public const string ARRAY_DOUBLE = "double[]";
        public const string ARRAY_STRING = "str[]";
        public const string ARRAY_STRINGFULL = "string[]";
        public const string ARRAY_OBJ = "obj[]";
        public const string ARRAY_OBJFULL = "object[]";
        
        public const string ARRAY_ARRAY_INT = "int[][]";
        public const string ARRAY_ARRAY_LONG = "long[][]";
        public const string ARRAY_ARRAY_FLOAT = "float[][]";
        public const string ARRAY_ARRAY_DOUBLE = "double[][]";
        public const string ARRAY_ARRAY_STRING = "str[][]";
        public const string ARRAY_ARRAY_STRINGFULL = "string[][]";
        public const string ARRAY_ARRAY_OBJ = "obj[][]";
        public const string ARRAY_ARRAY_OBJFULL = "object[][]";
    }

    public class ReadExcel : Singleton<ReadExcel>
    {
        private const string CODE_NAMESPACE = "F8Framework.F8ExcelDataClass"; //由表生成的数据类型均在此命名空间内
        private string ExcelPath = "config"; //需要导表的目录
        private Dictionary<string, List<ConfigData[]>> dataDict; //存放所有数据表内的数据，key：类名  value：数据

        // 设置实时读取Excel路径，只限在StreamingAssets目录下
        public void SetRunTimeExcelPath(string path)
        {
            ExcelPath = path;
        }
        
        public void LoadAllExcelData()
        {
#if UNITY_EDITOR
        string INPUT_PATH = UnityEditor.EditorPrefs.GetString(UnityEngine.Application.dataPath.GetHashCode() + "ExcelPath", default);
#elif UNITY_STANDALONE
        string INPUT_PATH = URLSetting.CS_STREAMINGASSETS_URL + ExcelPath;
#elif UNITY_ANDROID
        string INPUT_PATH = UnityEngine.Application.persistentDataPath + "/" + ExcelPath;
#elif UNITY_IPHONE || UNITY_IOS
        string INPUT_PATH = URLSetting.CS_STREAMINGASSETS_URL + ExcelPath;
#elif UNITY_WEBGL
        string INPUT_PATH = URLSetting.CS_STREAMINGASSETS_URL + ExcelPath;
        LogF8.LogError("WebGL导出后暂不支持实时读取Excel");
#else
        string INPUT_PATH = URLSetting.CS_STREAMINGASSETS_URL + ExcelPath;
#endif
            LogF8.LogConfig(INPUT_PATH);
            if (string.IsNullOrEmpty(INPUT_PATH))
            {
                throw new Exception("请先设置数据表路径！");
            }

            var files = Directory.GetFiles(INPUT_PATH, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".xls") || s.EndsWith(".xlsx")).ToArray();
            if (files == null || files.Length == 0)
            {
                throw new Exception("暂无可以导入的数据表！首次F8请手动导入，【DemoWorkSheet.xlsx / Localization.xlsx】两个表格！" + INPUT_PATH + " 目录");
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
            foreach (string item in files)
            {
                step++;
                GetExcelData(item);
            }

            var assembly = Assembly.Load(CODE_NAMESPACE);
            step = 1;
            Dictionary<String, System.Object> objs = new Dictionary<string, object>();
            foreach (KeyValuePair<string, List<ConfigData[]>> each in dataDict)
            {
                step++;
                Type temp = assembly.GetType(CODE_NAMESPACE + "." + each.Key + "Item");
                object container = assembly.CreateInstance(CODE_NAMESPACE + "." + each.Key);
                //序列化数据
                Serialize(container, temp, each.Value);
                objs.Add(each.Key, container);
            }
            string _class = "F8DataManager";
            string method= "RuntimeLoadAll";
            object[] parameters = new object[] { objs };
            Util.Assembly.InvokeMethod(_class, method, parameters);
            LogF8.LogConfig("<color=green>运行时导表成功！</color>");
        }

        //数据表内每一格数据
        class ConfigData
        {
            public string Type; //数据类型
            public string Name; //字段名
            public string Data; //数据值
        }

        private void GetExcelData(string inputPath)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(inputPath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
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
                        if (dataDict.ContainsKey(className))
                        {
                            throw new Exception("类名重复: " + className + " ,路径:  " + inputPath);
                        }

                        dataDict.Add(className, dataList);
                        LogF8.LogConfig(className);
                    }
                } while (excelReader.NextResult()); //excelReader.NextResult() Excel表下一个sheet页有没有数据
            }

            stream.Dispose();
            stream.Close();
        }

        //序列化对象
        private static void Serialize(object container, Type temp, List<ConfigData[]> dataList)
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
                        info.SetValue(t, ParseValue(data.Type, data.Data, temp.Name));
                    }
                    else
                    {
                        //2019.4.28f1,2020.3.33f1都出现的BUG（2021.3.8f1测试通过），编译dll后没及时刷新，导致修改name或id后读取失败，需要二次编译
                        LogF8.LogConfig("info是空的：" + data.Name);
                    }
                }

                // FieldInfo.GetValue 获取对象内指定名称的字段的值
                FieldInfo fieldInfoId = null;
                foreach (var field in temp.GetFields())
                {
                    if (string.Equals(field.Name, "id", StringComparison.OrdinalIgnoreCase))
                    {
                        fieldInfoId = field;
                        break;
                    }  
                }
                object id = fieldInfoId.GetValue(t); //获取id
                FieldInfo dictInfo = container.GetType().GetField("Dict");
                object dict = dictInfo.GetValue(container);

                bool isExist = (bool)dict.GetType().GetMethod("ContainsKey").Invoke(dict, new System.Object[] { id });
                if (isExist)
                {
                    throw new Exception("ID重复：" + id + "，类型： " + container.GetType().Name);
                }

                dict.GetType().GetMethod("Add").Invoke(dict, new System.Object[] { id, t });
            }
        }

        private static void DebugError(string type, string data, string classname)
        {
            LogF8.LogError(string.Format("数据类型错误：类型：{0}  数据：{1}  类名：{2}", type, data, classname));
        }

        public static object ParseValue(string type, string data, string classname)
        {
            object o = null;
            try
            {
                switch (type)
                {
                    case SupportType.INT:
                        int INT_int;
                        if (int.TryParse(data, out INT_int) == false)
                        {
                            DebugError(type, data, classname);
                            o = 0;
                        }

                        o = INT_int;
                        break;
                    case SupportType.LONG:
                        long LONG_long;
                        if (long.TryParse(data, out LONG_long) == false)
                        {
                            DebugError(type, data, classname);
                            o = 0;
                        }

                        o = LONG_long;
                        break;
                    case SupportType.FLOAT:
                        float FLOAT_float;
                        if (float.TryParse(data, out FLOAT_float) == false)
                        {
                            DebugError(type, data, classname);
                            o = 0;
                        }

                        o = FLOAT_float;
                        break;
                    case SupportType.DOUBLE:
                        double DOUBLE_double;
                        if (double.TryParse(data, out DOUBLE_double) == false)
                        {
                            DebugError(type, data, classname);
                            o = 0;
                        }

                        o = DOUBLE_double;
                        break;
                    case SupportType.STRING or SupportType.STRINGFULL:
                        o = data;
                        break;
                    case SupportType.OBJ or SupportType.OBJFULL:
                        o = data as System.Object;
                        break;
                    case SupportType.ARRAY_OBJ or SupportType.ARRAY_OBJFULL:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        var ts = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int tsLength = ts.Length;
                        System.Object[] obj = new System.Object[tsLength];
                        for (int i = 0; i < tsLength; i++)
                        {
                            if (ts[i].EndsWith("\"") && ts[i].StartsWith("\""))
                            {
                                string str = ts[i].Substring(1);
                                string str2 = str.Substring(0, str.Length - 1);
                                obj[i] = str2;
                            }
                            else if (ts[i].Contains("."))
                            {
                                obj[i] = (float)ParseValue(SupportType.FLOAT, ts[i], classname);
                            }
                            else
                            {
                                obj[i] = (int)ParseValue(SupportType.INT, ts[i], classname);
                            }
                        }

                        o = obj;
                        break;
                    case SupportType.ARRAY_INT:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        var ints = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int intsLength = ints.Length;
                        int[] array = new int[intsLength];
                        for (int i = 0; i < intsLength; i++)
                        {
                            array[i] = (int)ParseValue(SupportType.INT, ints[i], classname);
                        }

                        o = array;
                        break;
                    case SupportType.ARRAY_LONG:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        var longs = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int longsLength = longs.Length;
                        long[] list = new long[longsLength];
                        for (int i = 0; i < longsLength; i++)
                        {
                            list[i] = (long)ParseValue(SupportType.LONG, longs[i], classname);
                        }

                        o = list;
                        break;
                    case SupportType.ARRAY_FLOAT:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        var floats = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int floatsLength = floats.Length;
                        float[] list2 = new float[floatsLength];
                        for (int i = 0; i < floatsLength; i++)
                        {
                            list2[i] = (float)ParseValue(SupportType.FLOAT, floats[i], classname);
                        }

                        o = list2;
                        break;
                    case SupportType.ARRAY_DOUBLE:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        var dounbles = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int dounbles9Length = dounbles.Length;
                        double[] list9 = new double[dounbles9Length];
                        for (int i = 0; i < dounbles9Length; i++)
                        {
                            list9[i] = (double)ParseValue(SupportType.DOUBLE, dounbles[i], classname);
                        }

                        o = list9;
                        break;
                    case SupportType.ARRAY_STRING or SupportType.ARRAY_STRINGFULL:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        var strs = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int strsLength = strs.Length;
                        string[] list3 = new string[strsLength];
                        for (int i = 0; i < strsLength; i++)
                        {
                            list3[i] = strs[i];
                        }

                        o = list3;
                        break;
                    case SupportType.ARRAY_ARRAY_INT:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        //匹配[]内的内容，并忽略""内的[]，考虑了逗号出现在引号内的情况。它会匹配不在引号内的内容，并且会忽略引号内部的逗号
                        var arr4 = Regex.Matches(data, @"\[[^\[\]\""]*(?:(?:""[^""]*""|'[^']*')[^\[\]\""]*)*\]")
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int arr4Length = arr4.Length;
                        int[][] list4 = new int[arr4Length][];
                        for (int i = 0; i < arr4Length; i++)
                        {
                            list4[i] = (int[])ParseValue(SupportType.ARRAY_INT, arr4[i], classname);
                        }

                        o = list4;
                        break;
                    case SupportType.ARRAY_ARRAY_LONG:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        //匹配[]内的内容，并忽略""内的[]，考虑了逗号出现在引号内的情况。它会匹配不在引号内的内容，并且会忽略引号内部的逗号
                        var arr8 = Regex.Matches(data, @"\[[^\[\]\""]*(?:(?:""[^""]*""|'[^']*')[^\[\]\""]*)*\]")
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int arr8Length = arr8.Length;
                        long[][] list8 = new long[arr8Length][];
                        for (int i = 0; i < arr8Length; i++)
                        {
                            list8[i] = (long[])ParseValue(SupportType.ARRAY_LONG, arr8[i], classname);
                        }

                        o = list8;
                        break;
                    case SupportType.ARRAY_ARRAY_FLOAT:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        //匹配[]内的内容，并忽略""内的[]，考虑了逗号出现在引号内的情况。它会匹配不在引号内的内容，并且会忽略引号内部的逗号
                        var arr5 = Regex.Matches(data, @"\[[^\[\]\""]*(?:(?:""[^""]*""|'[^']*')[^\[\]\""]*)*\]")
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int arr5Length = arr5.Length;
                        float[][] list5 = new float[arr5Length][];
                        for (int i = 0; i < arr5Length; i++)
                        {
                            list5[i] = (float[])ParseValue(SupportType.ARRAY_FLOAT, arr5[i], classname);
                        }

                        o = list5;
                        break;
                    case SupportType.ARRAY_ARRAY_DOUBLE:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        //匹配[]内的内容，并忽略""内的[]，考虑了逗号出现在引号内的情况。它会匹配不在引号内的内容，并且会忽略引号内部的逗号
                        var arr9 = Regex.Matches(data, @"\[[^\[\]\""]*(?:(?:""[^""]*""|'[^']*')[^\[\]\""]*)*\]")
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int arr10Length = arr9.Length;
                        double[][] list10 = new double[arr10Length][];
                        for (int i = 0; i < arr10Length; i++)
                        {
                            list10[i] = (double[])ParseValue(SupportType.ARRAY_DOUBLE, arr9[i], classname);
                        }

                        o = list10;
                        break;
                    case SupportType.ARRAY_ARRAY_STRING or SupportType.ARRAY_ARRAY_STRINGFULL:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        //匹配[]内的内容，并忽略""内的[]，考虑了逗号出现在引号内的情况。它会匹配不在引号内的内容，并且会忽略引号内部的逗号
                        var arr6 = Regex.Matches(data, @"\[[^\[\]\""]*(?:(?:""[^""]*""|'[^']*')[^\[\]\""]*)*\]")
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int arr6Length = arr6.Length;
                        string[][] list6 = new string[arr6Length][];
                        for (int i = 0; i < arr6Length; i++)
                        {
                            list6[i] = (string[])ParseValue(SupportType.ARRAY_STRING, arr6[i], classname);
                        }

                        o = list6;
                        break;
                    case SupportType.ARRAY_ARRAY_OBJ or SupportType.ARRAY_ARRAY_OBJFULL:
                        data = data.Substring(1, data.Length - 2); //移除 '['   ']'
                        //匹配[]内的内容，并忽略""内的[]，考虑了逗号出现在引号内的情况。它会匹配不在引号内的内容，并且会忽略引号内部的逗号
                        var arr7 = Regex.Matches(data, @"\[[^\[\]\""]*(?:(?:""[^""]*""|'[^']*')[^\[\]\""]*)*\]")
                            .Cast<Match>()
                            .Select(m => m.Value)
                            .ToArray();
                        int arr7Length = arr7.Length;
                        System.Object[][] list7 = new System.Object[arr7Length][];
                        for (int i = 0; i < arr7Length; i++)
                        {
                            list7[i] = (System.Object[])ParseValue(SupportType.ARRAY_OBJ, arr7[i], classname);
                        }

                        o = list7;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("\n错误的数据值:" + data + "\n位于:" + classname, ex);
            }

            return o;
        }
    }
}