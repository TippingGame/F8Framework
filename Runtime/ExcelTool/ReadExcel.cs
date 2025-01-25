using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Excel;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace F8Framework.Core
{
    public class SupportType
    {
        // 基础类型
        public const string BOOL = "bool";
        public const string BYTE = "byte";
        public const string SHORT = "short";
        public const string INT = "int";
        public const string LONG = "long";
        public const string FLOAT = "float";
        public const string DOUBLE = "double";
        public const string DECIMAL = "decimal";
        public const string STRING = "str";
        public const string STRINGFULL = "string";
        public const string OBJ = "obj";
        public const string OBJFULL = "object";
        public const string VECTOR2 = "vec2";
        public const string VECTOR3 = "vec3";
        public const string VECTOR4 = "vec4";
        public const string VECTOR2FULL = "vector2";
        public const string VECTOR3FULL = "vector3";
        public const string VECTOR4FULL = "vector4";
        public const string VECTOR2INT = "vec2int";
        public const string VECTOR3INT = "vec3int";
        public const string VECTOR2INTFULL = "vector2int";
        public const string VECTOR3INTFULL = "vector3int";
        public const string QUATERNION = "quat";
        public const string QUATERNIONFULL = "quaternion";
        public const string COLOR = "color";
        
        // 容器类型
        public const string ARRAY = "[]";
        public const string LIST = "list<";
        public const string DICTIONARY = "dict<";
        public const string DICTIONARYFULL = "dictionary<";
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
            LogF8.LogError(string.Format("数据类型错误，类型：{0}  数据：{1}  类名：{2}", type, data, classname));
        }
        
        public static object ParseValue(string type, string data, string classname)
        {
            object o = null;
            try
            {
                if (type.EndsWith(SupportType.ARRAY))
                {
                    string innerType = type.Substring(0, type.Length - 2);
                    data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                    var elements = ParseElements(data).ToArray();
                    int elementsLength = elements.Length;
                    var array = (Array)Activator.CreateInstance(Type.GetType(GetTrueType(innerType) + "[]"), elementsLength);
                    for (int i = 0; i < elementsLength; i++)
                    {
                        // 递归解析内层元素
                        array.SetValue(ParseValue(innerType, elements[i], classname), i);
                    }

                    o = array;
                }
                else if (type.StartsWith(SupportType.LIST) && type.EndsWith(">"))
                {
                    string innerType = type.Substring(5, type.Length - 6);
                    data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                    var elements = ParseElements(data).ToArray();
                    int elementsLength = elements.Length;
                    Type elementType = Type.GetType(GetTrueType(innerType, "", "", false));
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType), elementsLength);
                    for (int i = 0; i < elementsLength; i++)
                    {
                        list.Add(ParseValue(innerType, elements[i], classname));
                    }
                    
                    o = list;
                }
                else if ((type.StartsWith(SupportType.DICTIONARY) || type.StartsWith(SupportType.DICTIONARYFULL)) && type.EndsWith(">"))
                {
                    data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                    int commaIndex = type.IndexOf(',');
                    if (commaIndex == -1)
                    {
                        throw new Exception("Dictionary 类型必须包含两个用逗号分隔的类型");
                    }
                    string keyType = null;
                    if (type.StartsWith(SupportType.DICTIONARY))
                    {
                        keyType = type.Substring(5, commaIndex - 5);
                    }
                    else if(type.StartsWith(SupportType.DICTIONARYFULL))
                    {
                        keyType = type.Substring(11, commaIndex - 11);
                    }
                    string valueType = type.Substring(commaIndex + 1, type.Length - commaIndex - 2);
                    var elements = ParseElements(data).ToArray();
                    int elementsLength = elements.Length;
                    Type keyElementType = Type.GetType(GetTrueType(keyType));
                    Type valueElementType = Type.GetType(GetTrueType(valueType, "", "", false));
                    var dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyElementType, valueElementType));
                    for (int i = 0; i < elementsLength; i += 2)
                    {
                        object key = ParseValue(keyType, elementsLength >= i + 1 ? elements[i] : null, classname);
                        object value = ParseValue(valueType, elementsLength >= i + 2 ? elements[i + 1] : null, classname);
                        if (dictionary.Contains(key))
                        {
                            LogF8.LogError("Dictionary 重复Key值：{0}  Value值：{1}  类型：{2}  数据：{3}  类名：{4}",key, value, type, data, classname);
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                    }
                    
                    o = dictionary;
                }
                else
                {
                    switch (type)
                    {
                        case SupportType.BOOL:
                            string trueString = "true";
                            string trueString2 = "1";
                            bool BOOL_bool = trueString.Equals(data, StringComparison.OrdinalIgnoreCase) || trueString2.Equals(data, StringComparison.OrdinalIgnoreCase);
                            
                            o = BOOL_bool;
                            break;
                        case SupportType.BYTE:
                            byte BYTE_byte;
                            if (byte.TryParse(data, out BYTE_byte) == false)
                            {
                                DebugError(type, data, classname);
                                o = 0;
                                break;
                            }

                            o = BYTE_byte;
                            break;
                        case SupportType.SHORT:
                            short SHORT_short;
                            if (short.TryParse(data, out SHORT_short) == false)
                            {
                                DebugError(type, data, classname);
                                o = 0;
                                break;
                            }

                            o = SHORT_short;
                            break;
                        case SupportType.INT:
                            int INT_int;
                            if (int.TryParse(data, out INT_int) == false)
                            {
                                DebugError(type, data, classname);
                                o = 0;
                                break;
                            }

                            o = INT_int;
                            break;
                        case SupportType.LONG:
                            long LONG_long;
                            if (long.TryParse(data, out LONG_long) == false)
                            {
                                DebugError(type, data, classname);
                                o = 0;
                                break;
                            }

                            o = LONG_long;
                            break;
                        case SupportType.FLOAT:
                            float FLOAT_float;
                            if (float.TryParse(data, out FLOAT_float) == false)
                            {
                                DebugError(type, data, classname);
                                o = 0f;
                                break;
                            }

                            o = FLOAT_float;
                            break;
                        case SupportType.DOUBLE:
                            double DOUBLE_double;
                            if (double.TryParse(data, out DOUBLE_double) == false)
                            {
                                DebugError(type, data, classname);
                                o = 0d;
                                break;
                            }

                            o = DOUBLE_double;
                            break;
                        case SupportType.DECIMAL:
                            decimal DECIMAL_decimal;
                            if (decimal.TryParse(data, out DECIMAL_decimal) == false)
                            {
                                DebugError(type, data, classname);
                                o = 0m;
                                break;
                            }

                            o = DECIMAL_decimal;
                            break;
                        case SupportType.STRING or SupportType.STRINGFULL:
                            o = data;
                            break;
                        case SupportType.OBJ or SupportType.OBJFULL:
                            // 检查是否为带引号的字符串
                            if (data.StartsWith("\"") && data.EndsWith("\""))
                            {
                                o = data.Trim('"');
                            }
                            // 使用正则表达式检查是否为浮点数字符串
                            else if (Regex.IsMatch(data, @"[.\eE]"))
                            {
                                if (float.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                                {
                                    // 检查是否存在精度丢失
                                    if (floatValue.ToString(NumberFormatInfo.InvariantInfo) == data)
                                    {
                                        o = floatValue;
                                    }
                                    else
                                    {
                                        if (double.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
                                        {
                                            o = doubleValue;
                                        }
                                        else
                                        {
                                            o = data;
                                        }
                                    }
                                }
                                else
                                {
                                    o = data;
                                }
                            }
                            // 尝试转换为 int 类型
                            else if (int.TryParse(data, out int intValue))
                            {
                                o = intValue;
                            }
                            // 尝试转换为 long 类型
                            else if (long.TryParse(data, out long longValue))
                            {
                                o = longValue;
                            }
                            else
                            {
                                o = data;
                            }

                            break;
                        case SupportType.VECTOR2 or SupportType.VECTOR2FULL:
                            data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                            var vector2 = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                                .Cast<Match>()
                                .Select(m => m.Value)
                                .ToArray();
                            var vector22 = new Vector2();
                            vector22.x = (float)ParseValue(SupportType.FLOAT, vector2.Length >= 1 ? vector2[0] : "0",
                                classname);
                            vector22.y = (float)ParseValue(SupportType.FLOAT, vector2.Length >= 2 ? vector2[1] : "0",
                                classname);
                            o = vector22;
                            break;
                        case SupportType.VECTOR3 or SupportType.VECTOR3FULL:
                            data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                            var vector3 = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                                .Cast<Match>()
                                .Select(m => m.Value)
                                .ToArray();
                            var vector33 = new Vector3();
                            vector33.x = (float)ParseValue(SupportType.FLOAT, vector3.Length >= 1 ? vector3[0] : "0",
                                classname);
                            vector33.y = (float)ParseValue(SupportType.FLOAT, vector3.Length >= 2 ? vector3[1] : "0",
                                classname);
                            vector33.z = (float)ParseValue(SupportType.FLOAT, vector3.Length >= 3 ? vector3[2] : "0",
                                classname);
                            o = vector33;
                            break;
                        case SupportType.VECTOR4 or SupportType.VECTOR4FULL:
                            data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                            var vector4 = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                                .Cast<Match>()
                                .Select(m => m.Value)
                                .ToArray();
                            var vector44 = new Vector4();
                            vector44.x = (float)ParseValue(SupportType.FLOAT, vector4.Length >= 1 ? vector4[0] : "0",
                                classname);
                            vector44.y = (float)ParseValue(SupportType.FLOAT, vector4.Length >= 2 ? vector4[1] : "0",
                                classname);
                            vector44.z = (float)ParseValue(SupportType.FLOAT, vector4.Length >= 3 ? vector4[2] : "0",
                                classname);
                            vector44.w = (float)ParseValue(SupportType.FLOAT, vector4.Length >= 4 ? vector4[3] : "0",
                                classname);
                            o = vector44;
                            break;
                        case SupportType.VECTOR2INT or SupportType.VECTOR2INTFULL:
                            data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                            var vector2int = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                                .Cast<Match>()
                                .Select(m => m.Value)
                                .ToArray();
                            var vector22int = new Vector2Int();
                            vector22int.x = (int)ParseValue(SupportType.INT,
                                vector2int.Length >= 1 ? vector2int[0] : "0", classname);
                            vector22int.y = (int)ParseValue(SupportType.INT,
                                vector2int.Length >= 2 ? vector2int[1] : "0", classname);
                            o = vector22int;
                            break;
                        case SupportType.VECTOR3INT or SupportType.VECTOR3INTFULL:
                            data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                            var vector3int = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                                .Cast<Match>()
                                .Select(m => m.Value)
                                .ToArray();
                            var vector33int = new Vector3Int();
                            vector33int.x = (int)ParseValue(SupportType.INT,
                                vector3int.Length >= 1 ? vector3int[0] : "0", classname);
                            vector33int.y = (int)ParseValue(SupportType.INT,
                                vector3int.Length >= 2 ? vector3int[1] : "0", classname);
                            vector33int.z = (int)ParseValue(SupportType.INT,
                                vector3int.Length >= 3 ? vector3int[2] : "0", classname);
                            o = vector33int;
                            break;
                        case SupportType.QUATERNION or SupportType.QUATERNIONFULL:
                            data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                            var quaternion = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                                .Cast<Match>()
                                .Select(m => m.Value)
                                .ToArray();
                            var quaternion1 = new Quaternion();
                            quaternion1.x = (float)ParseValue(SupportType.FLOAT,
                                quaternion.Length >= 1 ? quaternion[0] : "0", classname);
                            quaternion1.y = (float)ParseValue(SupportType.FLOAT,
                                quaternion.Length >= 2 ? quaternion[1] : "0", classname);
                            quaternion1.z = (float)ParseValue(SupportType.FLOAT,
                                quaternion.Length >= 3 ? quaternion[2] : "0", classname);
                            quaternion1.w = (float)ParseValue(SupportType.FLOAT,
                                quaternion.Length >= 4 ? quaternion[3] : "0", classname);
                            o = quaternion1;
                            break;
                        case SupportType.COLOR:
                            data = RemoveOuterBracketsIfPaired(data); // 移除最外层的 '[' 和 ']' '{' 和 '}'
                            var color = Regex.Matches(data, "(?:\"(?:[^\"]|\"\")*\"|[^,]+)") //逗号分隔
                                .Cast<Match>()
                                .Select(m => m.Value)
                                .ToArray();
                            var color1 = new Color();
                            color1.r = (float)ParseValue(SupportType.FLOAT, color.Length >= 1 ? color[0] : "0",
                                classname);
                            color1.g = (float)ParseValue(SupportType.FLOAT, color.Length >= 2 ? color[1] : "0",
                                classname);
                            color1.b = (float)ParseValue(SupportType.FLOAT, color.Length >= 3 ? color[2] : "0",
                                classname);
                            color1.a = (float)ParseValue(SupportType.FLOAT, color.Length >= 4 ? color[3] : "0",
                                classname);
                            o = color1;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("\n错误的数据值:" + data + "\n位于:" + classname, ex);
            }

            return o;
        }
        
        public static string GetTrueType(string type, string className = "", string inputPath = "", bool writtenForm = true)
        {
            if (type.EndsWith(SupportType.ARRAY))
            {
                string innerType = type.Substring(0, type.Length - 2);
                return GetTrueType(innerType) + "[]";
            }
            else if (type.StartsWith(SupportType.LIST) && type.EndsWith(">"))
            {
                string innerType = type.Substring(5, type.Length - 6);
                if (writtenForm)
                {
                    return "System.Collections.Generic.List<" + GetTrueType(innerType) + ">";
                }
                else
                {
                    return "System.Collections.Generic.List`1[" + GetTrueType(innerType) + "]";
                }
            }
            else if ((type.StartsWith(SupportType.DICTIONARY) || type.StartsWith(SupportType.DICTIONARYFULL)) && type.EndsWith(">"))
            {
                int commaIndex = type.IndexOf(',');
                if (commaIndex == -1)
                {
                    throw new Exception("Dictionary 类型必须包含两个用逗号分隔的类型");
                }
                string keyType = null;
                if (type.StartsWith(SupportType.DICTIONARY))
                {
                    keyType = type.Substring(5, commaIndex - 5);
                }
                else if(type.StartsWith(SupportType.DICTIONARYFULL))
                {
                    keyType = type.Substring(11, commaIndex - 11);
                }
                string valueType = type.Substring(commaIndex + 1, type.Length - commaIndex - 2);
                if (writtenForm)
                {
                    return "System.Collections.Generic.Dictionary<" + GetTrueType(keyType) + "," + GetTrueType(valueType) + ">";
                }
                else
                {
                    return "System.Collections.Generic.Dictionary`2[" + GetTrueType(keyType) + "," + GetTrueType(valueType) + "]";
                }
            }
            else
            {
                return ParseType(type, className, inputPath);
            }
        }
        
        private static string ParseType(string type, string className, string inputPath)
        {
            switch (type)
            {
                case SupportType.BOOL:
                    type = "System.Boolean";
                    break;
                case SupportType.BYTE:
                    type = "System.Byte";
                    break;
                case SupportType.SHORT:
                    type = "System.Int16";
                    break;
                case SupportType.INT:
                    type = "System.Int32";
                    break;
                case SupportType.LONG:
                    type = "System.Int64";
                    break;
                case SupportType.FLOAT:
                    type = "System.Single";
                    break;
                case SupportType.DOUBLE:
                    type = "System.Double";
                    break;
                case SupportType.DECIMAL:
                    type = "System.Decimal";
                    break;
                case SupportType.STRING or SupportType.STRINGFULL:
                    type = "System.String";
                    break;
                case SupportType.OBJ or SupportType.OBJFULL:
                    type = "System.Object";
                    break;
                case SupportType.VECTOR2 or SupportType.VECTOR2FULL:
                    type = "UnityEngine.Vector2";
                    break;
                case SupportType.VECTOR3 or SupportType.VECTOR3FULL:
                    type = "UnityEngine.Vector3";
                    break;
                case SupportType.VECTOR4 or SupportType.VECTOR4FULL:
                    type = "UnityEngine.Vector4";
                    break;
                case SupportType.VECTOR2INT or SupportType.VECTOR2INTFULL:
                    type = "UnityEngine.Vector2Int";
                    break;
                case SupportType.VECTOR3INT or SupportType.VECTOR3INTFULL:
                    type = "UnityEngine.Vector3Int";
                    break;
                case SupportType.QUATERNION or SupportType.QUATERNIONFULL:
                    type = "UnityEngine.Quaternion";
                    break;
                case SupportType.COLOR:
                    type = "UnityEngine.Color";
                    break;
                default:
                    throw new Exception("输入了错误的数据类型:  " + type + ", 类名:  " + className + ", 位于:  " + inputPath);
            }

            return type;
        }
        
        private static string RemoveOuterBracketsIfPaired(string data)
        {
            if (string.IsNullOrEmpty(data))
                return data;

            if ((data.StartsWith("[") && data.EndsWith("]")) || (data.StartsWith("{") && data.EndsWith("}")))
            {
                Stack<char> stack = new Stack<char>();
                char openingBracket = data[0];
                char closingBracket = openingBracket == '[' ? ']' : '}';

                // 先假设最外层括号可删除
                bool canRemoveOuter = true;

                // 从第二个字符开始遍历，到倒数第二个字符结束
                for (int i = 1; i < data.Length - 1; i++)
                {
                    char c = data[i];
                    if (c == openingBracket)
                    {
                        stack.Push(c);
                    }
                    else if (c == closingBracket)
                    {
                        if (stack.Count == 0)
                        {
                            // 遇到右括号但栈为空，说明括号不匹配，不能删除最外层括号
                            canRemoveOuter = false;
                            break;
                        }
                        stack.Pop();
                    }
                }

                // 遍历结束后，若栈不为空，也不能删除最外层括号
                if (stack.Count > 0)
                {
                    canRemoveOuter = false;
                }

                if (canRemoveOuter)
                {
                    return data.Substring(1, data.Length - 2);
                }
            }

            return data;
        }
        
        private static List<string> ParseElements(string data)
        {
            List<string> elements = new List<string>();
            string currentElement = "";
            int bracketDepth = 0;
            bool inQuotes = false;

            foreach (char c in data)
            {
                if (c == '"')
                {
                    // 遇到引号，切换引号状态
                    inQuotes = !inQuotes;
                    currentElement += c;
                }
                else if (c == '[')
                {
                    // 遇到左括号，增加括号深度
                    bracketDepth++;
                    currentElement += c;
                }
                else if (c == ']')
                {
                    // 遇到右括号，减少括号深度
                    bracketDepth--;
                    currentElement += c;
                }
                else if (c == ',' && !inQuotes && bracketDepth == 0)
                {
                    // 如果不在引号内且括号深度为 0，遇到逗号则分割元素
                    elements.Add(currentElement);
                    currentElement = "";
                }
                else
                {
                    // 其他字符直接添加到当前元素
                    currentElement += c;
                }
            }

            // 添加最后一个元素
            if (!string.IsNullOrEmpty(currentElement))
            {
                elements.Add(currentElement);
            }

            return elements;
        }
    }
}