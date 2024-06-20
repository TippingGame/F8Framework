using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace F8Framework.Core
{
    public static partial class Util
    {
        public static class Assembly
        {
            /// <summary>
            /// 默认使用应用的程序集，若使用了Assembly.Load，则需要更新域程序集；
            /// </summary>
            static System.Reflection.Assembly[] domainAssemblies;

            static Assembly()
            {
                domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            /// <summary>
            /// 设置域程序集
            /// </summary>
            /// <param name="assemblies">程序集</param>
            public static void SetDomainAssemblies(System.Reflection.Assembly[] assemblies)
            {
                domainAssemblies = assemblies;
            }

            /// <summary>
            /// 从域程序集中获取类；
            /// </summary>
            /// <param name="typeName">类型完全限定名</param>
            /// <returns>类型</returns>
            public static Type GetType(string typeName)
            {
                Type type = null;
                foreach (var assembly in domainAssemblies)
                {
                    type = assembly.GetType(typeName);
                    if (type != null)
                        break;
                }

                return type;
            }

            /// <summary>
            /// 获取AppDomain中指定的程序集
            /// </summary>
            /// <param name="assemblyName">程序集名</param>
            /// <returns>程序集</returns>
            public static System.Reflection.Assembly GetAssembly(string assemblyName)
            {
                foreach (var assembly in domainAssemblies)
                {
                    if (assembly.GetName().Name == assemblyName)
                        return assembly;
                }

                return null;
            }

            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="args">构造参数</param>
            /// <returns>实例化后的对象</returns>
            public static object GetTypeInstance(Type type, params object[] args)
            {
                return Activator.CreateInstance(type, args);
            }

            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// 被反射对象必须是具有无参公共构造 
            /// </summary>
            /// <param name="typeName">类型名</param>
            /// <returns>实例化后的对象</returns>
            public static object GetTypeInstance(string typeName)
            {
                object inst = null;
                foreach (var a in domainAssemblies)
                {
                    var dstType = a.GetType(typeName);
                    if (dstType != null)
                    {
                        inst = Activator.CreateInstance(dstType);
                        break;
                    }
                }

                return inst;
            }

            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// </summary>
            /// <param name="typeName">类型名</param>
            /// <param name="args">构造参数</param>
            /// <returns>实例化后的对象</returns>
            public static object GetTypeInstance(string typeName, object[] args)
            {
                object inst = null;
                foreach (var a in domainAssemblies)
                {
                    var dstType = a.GetType(typeName);
                    if (dstType != null)
                    {
                        inst = Activator.CreateInstance(dstType, args);
                        break;
                    }
                }

                return inst;
            }

            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// 被反射对象必须是具有无参公共构造 ，强转至泛型类型。
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="typeName">类型名</param>
            /// <returns>实例化后的对象</returns>
            public static T GetTypeInstance<T>(string typeName)
            {
                T inst = default;
                foreach (var a in domainAssemblies)
                {
                    var dstType = a.GetType(typeName);
                    if (dstType != null)
                    {
                        inst = (T)Activator.CreateInstance(dstType);
                        break;
                    }
                }

                return inst;
            }

            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// 被反射对象必须是具有无参公共构造 
            /// </summary>
            /// <param name="typeName">类型名</param>
            /// <param name="assemblies">程序集集合</param>
            /// <returns>实例化后的对象</returns>
            public static object GetTypeInstance(string typeName, params System.Reflection.Assembly[] assemblies)
            {
                object inst = null;
                foreach (var a in assemblies)
                {
                    var dstType = a.GetType(typeName);
                    if (dstType != null)
                    {
                        inst = Activator.CreateInstance(dstType);
                        break;
                    }
                }

                return inst;
            }

            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// </summary>
            /// <param name="typeName">类型名</param>
            /// <param name="args">构造参数</param>
            /// <param name="assemblies">程序集集合</param>
            /// <returns>实例化后的对象</returns>
            public static object GetTypeInstance(string typeName, object[] args,
                params System.Reflection.Assembly[] assemblies)
            {
                object inst = null;
                foreach (var a in assemblies)
                {
                    var dstType = a.GetType(typeName);
                    if (dstType != null)
                    {
                        inst = Activator.CreateInstance(dstType, args);
                        break;
                    }
                }

                return inst;
            }

            /// <summary>
            /// 获取所有派生类的实例对象
            /// </summary>
            /// <typeparam name="T">目标类型</typeparam>
            /// <param name="assembly">需要检测的程序集</param>
            /// <returns>反射生成后的对象数组</returns>
            public static T[] GetDerivedTypeInstances<T>(System.Reflection.Assembly assembly = null)
                where T : class
            {
                Type type = typeof(T);
                List<T> list = new List<T>();
                var types = GetDerivedTypes(type, assembly);
                var length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    var obj = GetTypeInstance(types[i]) as T;
                    list.Add(obj);
                }

                return list.ToArray();
            }

            /// <returns></returns>
            /// <summary>
            /// 获取目标类的派生对象；
            /// </summary>
            /// <param name="type">基类</param>
            /// <param name="assembly">需要检测的程序集</param>
            /// <returns>实例对象</returns>
            public static object[] GetDerivedTypeInstances(Type type, System.Reflection.Assembly assembly = null)
            {
                List<object> list = new List<object>();
                var types = GetDerivedTypes(type, assembly);
                var length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    var obj = GetTypeInstance(types[i]);
                    list.Add(obj);
                }

                return list.ToArray();
            }

            /// <summary>
            /// 获取变量的名称；
            /// 参考：
            /// object dotNet=new object();
            /// Utility.Assembly.GetValueTypeName(() =>dotNet);
            /// 输出得 dotNet
            /// </summary>
            /// <typeparam name="T">任意类型的变量</typeparam>
            /// <param name="memberExperssion">变量的表达式</param>
            /// <returns>传入变量的名称</returns>
            public static string GetPropertyName<T>(Expression<Func<T>> memberExperssion)
            {
                MemberExpression me = (MemberExpression)memberExperssion.Body;
                return me.Member.Name;
            }

            /// <summary>
            /// 通过特性获取对象实体；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="type">基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象</returns>
            public static object GetInstanceByAttribute<T>(Type type, System.Reflection.Assembly assembly,
                bool inherit = false)
                where T : Attribute
            {
                object obj = default;
                var types = GetDerivedTypes(type, assembly);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        obj = GetTypeInstance(types[i]);
                        return obj;
                    }
                }

                return obj;
            }

            /// <summary>
            /// 通过特性获取对象实体；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <typeparam name="K">基类，new()约束</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象</returns>
            public static K GetInstanceByAttribute<T, K>(System.Reflection.Assembly assembly = null,
                bool inherit = false)
                where T : Attribute
                where K : class
            {
                K obj = default;
                var types = GetDerivedTypes(typeof(K), assembly);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        obj = GetTypeInstance(types[i]) as K;
                        return obj;
                    }
                }

                return obj;
            }

            /// <summary>
            /// 通过特性获取对象实体数组；
            /// 生成的对象必须是无参可构造；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <typeparam name="K">基类，new()约束</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>生成的对象数组</returns>
            public static K[] GetInstancesByAttribute<T, K>(System.Reflection.Assembly assembly)
                where T : Attribute
                where K : class
            {
                return GetInstancesByAttribute<T, K>(false, assembly);
            }

            /// <summary>
            /// 通过特性获取对象实体数组；
            /// 生成的对象必须是无参可构造；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <typeparam name="K">基类，new()约束</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象数组</returns>
            public static K[] GetInstancesByAttribute<T, K>(bool inherit, System.Reflection.Assembly assembly = null)
                where T : Attribute
                where K : class
            {
                List<K> set = new List<K>();
                var types = GetDerivedTypes(typeof(K), assembly);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes<T>(inherit).Count() > 0)
                    {
                        set.Add(GetTypeInstance(types[i]) as K);
                    }
                }

                return set.ToArray();
            }

            /// <summary>
            /// 通过特性获取对象实体数组；
            /// 生成的对象必须是无参可构造；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="type">基类，new()约束</param>
            /// <param name="assembly">查询的程序集</param>
            /// <returns><生成的对象数组/returns>
            public static object[] GetInstancesByAttribute<T>(Type type, System.Reflection.Assembly assembly = null)
                where T : Attribute
            {
                return GetInstancesByAttribute<T>(type, false, assembly);
            }

            /// <summary>
            /// 通过特性获取对象实体数组；
            /// 生成的对象必须是无参可构造；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="type">基类，new()约束</param>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象数组</returns>
            public static object[] GetInstancesByAttribute<T>(Type type, bool inherit,
                System.Reflection.Assembly assembly = null)
                where T : Attribute
            {
                List<object> set = new List<object>();
                var types = GetDerivedTypes(type, assembly);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        set.Add(GetTypeInstance(types[i]));
                    }
                }

                return set.ToArray();
            }

            /// <summary>
            /// 通过特性获取目标派生类的所有可实例化类；
            /// </summary>
            /// <typeparam name="T">特性类型</typeparam>
            /// <typeparam name="K">派生的基类</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类数组</returns>
            public static Type[] GetDerivedTypesByAttribute<T, K>(System.Reflection.Assembly assembly = null)
                where T : Attribute
                where K : class
            {
                return GetDerivedTypesByAttribute<T, K>(false, assembly);
            }

            /// <summary>
            /// 通过特性获取目标派生类的所有可实例化类；
            /// </summary>
            /// <typeparam name="T">特性类型</typeparam>
            /// <typeparam name="K">派生的基类</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>非抽象派生类数组</returns>
            public static Type[] GetDerivedTypesByAttribute<T, K>(bool inherit,
                System.Reflection.Assembly assembly = null)
                where T : Attribute
                where K : class
            {
                return GetDerivedTypesByAttribute<T>(typeof(K), inherit, assembly);
            }

            /// <summary>
            /// 通过特性获取目标派生类的所有可实例化类；
            /// </summary>
            /// <typeparam name="T">特性类型</typeparam>
            /// <param name="type">派生的基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类数组</returns>
            public static Type[] GetDerivedTypesByAttribute<T>(Type type, System.Reflection.Assembly assembly = null)
                where T : Attribute
            {
                return GetDerivedTypesByAttribute<T>(type, false, assembly);
            }

            /// <summary>
            /// 通过特性获取目标派生类的所有可实例化类；
            /// </summary>
            /// <typeparam name="T">特性类型</typeparam>
            /// <param name="type">派生的基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>非抽象派生类数组</returns>
            public static Type[] GetDerivedTypesByAttribute<T>(Type type, bool inherit,
                System.Reflection.Assembly assembly = null)
                where T : Attribute
            {
                Type[] types = assembly.GetTypes();
                return types.Where(t =>
                {
                    return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract &&
                           t.GetCustomAttributes<T>(inherit).Count() > 0;
                }).ToArray();
            }

            /// <summary>
            /// 获取某类型的第一个派生类；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类</returns>
            public static Type GetDerivedType<T>(System.Reflection.Assembly assembly = null)
                where T : class
            {
                return GetDerivedType(typeof(T), assembly);
            }

            /// <summary>
            ///  获取某类型的第一个派生类；
            /// </summary>
            /// <param name="type">基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类</returns>
            public static Type GetDerivedType(Type type, System.Reflection.Assembly assembly = null)
            {
                Type[] types;
                if (assembly == null)
                    types = type.Assembly.GetTypes();
                else
                    types = assembly.GetTypes();
                var rstType = from t in types
                    where type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract
                    select t;
                return rstType.FirstOrDefault();
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类数组；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类</returns>
            public static Type[] GetDerivedTypes<T>(System.Reflection.Assembly assembly = null)
                where T : class
            {
                Type type = typeof(T);
                Type[] types;
                if (assembly == null)
                    types = type.Assembly.GetTypes();
                else
                    types = assembly.GetTypes();
                return types.Where(t => { return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract; }).ToArray();
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类数组；
            /// </summary>
            /// <param name="type">基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类</returns>
            public static Type[] GetDerivedTypes(Type type, System.Reflection.Assembly assembly = null)
            {
                Type[] types;
                if (assembly == null)
                    types = type.Assembly.GetTypes();
                else
                    types = assembly.GetTypes();
                return types.Where(t => { return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract; }).ToArray();
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类数组；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <param name="assemblies">查询的程序集</param>
            /// <returns>非抽象派生类</returns>
            public static Type[] GetDerivedTypes<T>(params System.Reflection.Assembly[] assemblies)
                where T : class
            {
                return GetDerivedTypes(typeof(T), assemblies);
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类数组；
            /// </summary>
            /// <param name="type">基类</param>
            /// <param name="assemblies">查询的程序集集合</param>
            /// <returns>非抽象派生类</returns>
            public static Type[] GetDerivedTypes(Type type, params System.Reflection.Assembly[] assemblies)
            {
                List<Type> types;
                if (assemblies == null)
                    return type.Assembly.GetTypes().Where(t =>
                    {
                        return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract;
                    }).ToArray();
                else
                {
                    types = new List<Type>();
                    foreach (var a in assemblies)
                    {
                        var dstTypes = a.GetTypes().Where(t =>
                        {
                            return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract;
                        });
                        types.AddRange(dstTypes);
                    }

                    return types.ToArray();
                }
            }

            /// <summary>
            /// 获取某类型的第一个派生类完全限定名；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类完全限定名</returns>
            public static string GetDerivedTypeName<T>(System.Reflection.Assembly assembly = null)
                where T : class
            {
                return GetDerivedTypeName(typeof(T), assembly);
            }

            /// <summary>
            ///  获取某类型的第一个派生类完全限定名；
            /// </summary>
            /// <param name="type">基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类完全限定名</returns>
            public static string GetDerivedTypeName(Type type, System.Reflection.Assembly assembly = null)
            {
                Type[] types;
                if (assembly == null)
                    types = type.Assembly.GetTypes();
                else
                    types = assembly.GetTypes();
                var rstType = from t in types
                    where type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract
                    select t;
                return rstType.FirstOrDefault().FullName;
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类完全限定名数组；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类完全限定名</returns>
            public static string[] GetDerivedTypeNames<T>(System.Reflection.Assembly assembly = null)
                where T : class
            {
                Type type = typeof(T);
                Type[] types;
                if (assembly == null)
                    types = type.Assembly.GetTypes();
                else
                    types = assembly.GetTypes();
                return types.Where(t => { return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract; })
                    .Select(t => t.FullName).ToArray();
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类完全限定名数组；
            /// </summary>
            /// <param name="type">基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类完全限定名</returns>
            public static string[] GetDerivedTypeNames(Type type, System.Reflection.Assembly assembly = null)
            {
                Type[] types;
                if (assembly == null)
                    types = type.Assembly.GetTypes();
                else
                    types = assembly.GetTypes();
                return types.Where(t => { return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract; })
                    .Select(t => t.FullName).ToArray();
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类完全限定名数组；
            /// </summary>
            /// <param name="type">基类</param>
            /// <param name="assemblies">查询的程序集集合</param>
            /// <returns>非抽象派生类完全限定名</returns>
            public static string[] GetDerivedTypeNames(Type type, params System.Reflection.Assembly[] assemblies)
            {
                List<string> types;
                if (assemblies == null)
                    return type.Assembly.GetTypes()
                        .Where(t => { return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract; })
                        .Select(t => t.FullName).ToArray();
                else
                {
                    types = new List<string>();
                    foreach (var a in assemblies)
                    {
                        var dstTypes = a.GetTypes().Where(t =>
                        {
                            return type.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract;
                        }).Select(t => t.FullName);
                        types.AddRange(dstTypes);
                    }
                }

                return types.ToArray();
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类完全限定名数组；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <param name="assemblies">查询的程序集集合</param>
            /// <returns>非抽象派生类完全限定名</returns>
            public static string[] GetDerivedTypeNames<T>(params System.Reflection.Assembly[] assemblies)
                where T : class
            {
                return GetDerivedTypeNames(typeof(T), assemblies);
            }

            /// <summary>
            /// 获取某类型在指定程序集的所有派生类完全限定名数组；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <returns>非抽象派生类完全限定名</returns>
            public static string[] GetDerivedTypeNames<T>()
                where T : class
            {
                return GetDerivedTypeNames(typeof(T), domainAssemblies);
            }

            /// <summary>
            /// 将一个对象上的字段值赋予到另一个对象上名字相同的字段上；
            /// 此方法可识别属性与字段，赋值时尽量将属性的索引字段也进行命名统一；
            /// </summary>
            /// <typeparam name="T">需要赋值的源类型</typeparam>
            /// <typeparam name="K">目标类型</typeparam>
            /// <param name="source">源对象</param>
            /// <param name="target">目标对象</param>
            /// <returns>被赋值后的目标对象</returns>
            public static K AssignSameFieldValue<T, K>(T source, K target)
                where T : class
                where K : class
            {
                Type srcType = typeof(T);
                Type tgType = typeof(K);
                var srcFields = srcType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public |
                                                  BindingFlags.GetProperty);
                var tgFields = tgType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public |
                                                BindingFlags.GetProperty);
                var length = srcFields.Length;
                for (int i = 0; i < length; i++)
                {
                    var tgLen = tgFields.Length;
                    for (int j = 0; j < tgLen; j++)
                    {
                        if (srcFields[i].Name == tgFields[j].Name)
                        {
                            try
                            {
                                tgFields[j].SetValue(target, srcFields[i].GetValue(source));
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                return target;
            }

            /// <summary>
            /// 遍历实例对象上的所有字段；
            /// 此方法可识别属性与字段，打印属性时候需要特别注意过滤自动属性的额外字段；
            /// </summary>
            /// <typeparam name="T">实例对象类型</typeparam>
            /// <param name="obj">实例对象</param>
            /// <param name="handler">遍历到一条字段执行的方法</param>
            public static void TraverseInstanceAllFileds<T>(T obj, Action<string, object> handler)
            {
                TraverseInstanceAllFileds(typeof(T), obj, handler);
            }

            /// <summary>
            /// 遍历实例对象上的所有字段；
            /// 此方法可识别属性与字段，打印属性时候需要特别注意过滤自动属性的额外字段；
            /// </summary>
            /// <param name="type">实例对象类型</param>
            /// <param name="obj">实例对象</param>
            /// <param name="handler">遍历到一条字段执行的方法</param>
            public static void TraverseInstanceAllFileds(Type type, object obj, Action<string, object> handler)
            {
                if (type == null)
                    throw new ArgumentNullException("type is invalid");
                if (obj == null)
                    throw new ArgumentNullException("obj is invalid");
                if (handler == null)
                    throw new ArgumentNullException("handler is invalid");
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public |
                                            BindingFlags.GetProperty | BindingFlags.Static);
                foreach (var f in fields)
                {
                    handler.Invoke(f.Name, f.GetValue(obj));
                }
            }

            /// <summary>
            /// 遍历实例对象上的所有属性；
            /// </summary>
            /// <param name="type">实例对象类型</param>
            /// <param name="obj">实例对象</param>
            /// <param name="handler">遍历到一条字段执行的方法</param>
            public static void TraverseInstanceAllProperties(Type type, object obj, Action<string, object> handler)
            {
                if (type == null)
                    throw new ArgumentNullException("type is invalid");
                if (obj == null)
                    throw new ArgumentNullException("obj is invalid");
                if (handler == null)
                    throw new ArgumentNullException("handler is invalid");
                var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance |
                                                    BindingFlags.Public | BindingFlags.Static);
                foreach (var p in properties)
                {
                    handler.Invoke(p.Name, p.GetValue(obj));
                }
            }

            /// <summary>
            /// 遍历type类型上的非对象字段；
            /// 包含静态、常量、属性等；
            /// </summary>
            /// <typeparam name="T">遍历的类型</typeparam>
            /// <param name="handler">遍历到一条字段执行的方法</param>
            public static void TraverseTypeFileds<T>(Action<string, object> handler)
            {
                TraverseTypeFileds(typeof(T), handler);
            }

            /// <summary>
            /// 遍历type类型上的非对象字段；
            /// 包含静态、常量、属性等；
            /// </summary>
            /// <param name="type">遍历的类型</param>
            /// <param name="handler">遍历到一条字段执行的方法</param>
            public static void TraverseTypeFileds(Type type, Action<string, object> handler)
            {
                if (type == null)
                    throw new ArgumentNullException("type is invalid");
                if (handler == null)
                    throw new ArgumentNullException("handler is invalid");
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty |
                                            BindingFlags.Static);
                foreach (var f in fields)
                {
                    handler.Invoke(f.Name, f.GetValue(null));
                }
            }

            /// <summary>
            /// 查询单个类型中存在的目标特性
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="type">目标类型</param>
            /// <param name="inherit">是否查找基类中的特性</param>
            /// <returns>特性数组</returns>
            public static T[] GetAttributes<T>(Type type, bool inherit = false)
                where T : Attribute
            {
                var attributes = type.GetCustomAttributes<T>(inherit).ToArray();
                return attributes;
            }

            /// <summary>
            /// 查询单个类型中存在的目标特性
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <typeparam name="K">目标类型</typeparam>
            /// <param name="inherit">是否查找基类中的特性</param>
            /// <returns>特性数组</returns>
            public static T[] GetAttributes<T, K>(bool inherit = false)
                where T : Attribute
                where K : class
            {
                return GetAttributes<T>(typeof(K), inherit);
            }

            /// <summary>
            /// 获取程序集中所有被挂载的特性数组
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="assembly">目标程序集</param>
            /// <param name="inherit">是否查找基类中的特性</param>
            /// <returns>特性数组</returns>
            public static T[] GetAttributesInAssembly<T>(System.Reflection.Assembly assembly, bool inherit = false)
                where T : Attribute
            {
                var attributes = new List<T>();
                Type[] types = assembly.GetTypes();
                var length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    var atts = GetAttributes<T>(types[i], inherit);
                    attributes.AddRange(atts);
                }

                return attributes.ToArray();
            }

            /// <summary>
            /// 通过特性获取类；
            /// </summary>
            /// <typeparam name="T">查找的指定类型</typeparam>
            /// <param name="assembly">目标程序集</param>
            /// <param name="inherit">是否查找基类中的特性</param>
            /// <returns>查找到的类型</returns>
            public static Type[] GetTypesByAttribute<T>(System.Reflection.Assembly assembly, bool inherit = false)
                where T : Attribute
            {
                var attributes = new List<Type>();
                Type[] types = assembly.GetTypes();
                var length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].IsDefined(typeof(T), inherit))
                    {
                        attributes.Add(types[i]);
                    }
                }

                return attributes.ToArray();
            }

            /// <summary>
            /// 获取指定类型中，挂载了目标特性的方法信息；
            /// </summary>
            /// <typeparam name="T">查找的指定类型</typeparam>
            /// <typeparam name="K">特性类型</typeparam>
            /// <param name="inherit">是否查找基类中的特性</param>
            /// <returns>方法信息数组</returns>
            public static MethodInfo[] GetTypeMethodsByAttribute<T, K>(bool inherit = false)
                where T : class
                where K : Attribute
            {
                return GetTypeMethodsByAttribute(typeof(T), typeof(K), inherit);
            }

            /// <summary>
            ///  获取指定类型中，挂载了目标特性的方法信息；
            /// </summary>
            /// <param name="type">查找的指定类型</param>
            /// <param name="attributeType">特性类型</param>
            /// <param name="inherit">是否查找基类中的特性</param>
            /// <returns>方法信息数组</returns>
            public static MethodInfo[] GetTypeMethodsByAttribute(Type type, Type attributeType, bool inherit = false)
            {
                if (type == null)
                    throw new ArgumentNullException("Type is invalid !");
                if (attributeType == null)
                    throw new ArgumentNullException("AttributeType is invalid !");
                if (!typeof(Attribute).IsAssignableFrom(attributeType))
                    throw new NotImplementedException($"{attributeType} is not inherit from Attribute!");
                return type
                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance |
                                BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttributes(attributeType, inherit).Length > 0).ToArray();
            }

            /// <summary>
            /// 获取指定类型中，挂载了目标特性的方法信息；
            /// </summary>
            /// <typeparam name="T">特性类型</typeparam>
            /// <param name="type">查找的指定类型</param>
            /// <param name="inherit">是否查找基类中的特性</param>
            /// <returns>方法信息数组</returns>
            public static MethodInfo[] GetTypeMethodsByAttribute<T>(Type type, bool inherit = false)
                where T : Attribute
            {
                if (type == null)
                    throw new ArgumentNullException("Type is invalid !");
                return type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance |
                                       BindingFlags.NonPublic).Where(m => m.GetCustomAttributes<T>(inherit).Count() > 0)
                    .ToArray();
            }

            /// <summary>
            /// 获取当前程序集下所有标记指定特效的方法信息；
            /// </summary>
            /// <typeparam name="T">特性类型</typeparam>
            /// <param name="assembly">目标程序集</param>
            /// <returns>方法信息数组</returns>
            public static MethodInfo[] GetAssemblyMethodsByAttribute<T>(System.Reflection.Assembly assembly,
                bool inherit = false)
                where T : Attribute
            {
                if (assembly == null)
                    throw new ArgumentNullException("assembly is invalid !");
                var types = assembly.GetTypes();
                List<MethodInfo> methodSet = new List<MethodInfo>();
                foreach (var t in types)
                {
                    var methods =
                        t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance |
                                     BindingFlags.NonPublic).Where(m => m.GetCustomAttributes<T>(inherit).Count() > 0)
                            .ToArray();
                    methodSet.AddRange(methods);
                }

                return methodSet.ToArray();
            }

            /// <summary>
            /// 执行方法
            /// </summary>
            /// <param name="className">类名</param>
            /// <param name="methodName">方法名</param>
            /// <param name="parameters">方法参数</param>
            /// <returns>返回值</returns>
            public static object InvokeMethod(string className, string methodName, object[] parameters = null)
            {
                // 查找指定类名的类型
                var type = domainAssemblies.SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(t => t.Name == className);
                if (type == null)
                {
                    throw new Exception($"需要检查是否有正确生成{className}类!");
                }

                // 获取单例的实例
                var property = type.BaseType?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    throw new Exception($"无法获取 {className} 的单例实例!");
                }
                var instance = property.GetValue(null, null);

                // 获取方法并执行
                var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (method == null)
                {
                    throw new Exception($"类中 : {type} 找不到此方法 : {methodName}!");
                }

                return method.Invoke(instance, parameters);
            }
            
            /// <summary>
            /// 执行方法；
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <param name="methodName">方法名</param>
            /// <param name="parameters">方法参数</param>
            /// <returns>返回值</returns>
            public static object InvokeMethod(object obj, string methodName, object[] parameters = null)
            {
                if (obj == null)
                    throw new NullReferenceException("Obj is invalid !");
                if (string.IsNullOrEmpty(methodName))
                    throw new ArgumentNullException("MethodName is invalid !");
                Type type = obj.GetType();
                var method = type.BaseType.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (method == null)
                    throw new NullReferenceException($"Type : {type} can not find method : {methodName} !");
                return method.Invoke(obj, parameters);
            }

            /// <summary>
            /// 执行方法；
            /// </summary>
            /// <param name="type">方法所在的type</param>
            /// <param name="obj">目标对象</param>
            /// <param name="methodName">方法名</param>
            /// <param name="parameters">方法参数</param>
            /// <returns>返回值</returns>
            public static object InvokeMethod(Type type, object obj, string methodName, object[] parameters = null)
            {
                if (type == null)
                    throw new ArgumentNullException("Type is invalid !");
                if (obj == null)
                    throw new NullReferenceException("Obj is invalid !");
                if (string.IsNullOrEmpty(methodName))
                    throw new ArgumentNullException("MethodName is invalid !");
                var method = type.GetMethod(methodName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (method == null)
                    throw new NullReferenceException($"Type : {type} can not find method : {methodName} !");
                return method.Invoke(obj, parameters);
            }

            /// <summary>
            /// 设置对象属性值；
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <param name="propertyName">属性名</param>
            /// <param name="newValue">属性值</param>
            public static void SetPropertyValue(object obj, string propertyName, object newValue)
            {
                if (obj == null)
                    throw new NullReferenceException("Obj is invalid !");
                if (string.IsNullOrEmpty(propertyName))
                    throw new ArgumentNullException("PropertyName is invalid !");
                Type type = obj.GetType();
                PropertyInfo prop = type.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                prop.SetValue(obj, newValue, null);
            }

            /// <summary>
            /// 设置对象属性值；
            /// 使用此方法为对象属性进行赋值时,若Type类型赋予正确,则可赋值非public类型的属性值;
            /// </summary>
            /// <param name="type">属性可写的类Type</param>
            /// <param name="obj">目标对象</param>
            /// <param name="propertyName">属性名</param>
            /// <param name="value">属性值</param>
            public static void SetPropertyValue(Type type, object obj, string propertyName, object value)
            {
                if (type == null)
                    throw new ArgumentNullException("Type is invalid !");
                if (obj == null)
                    throw new NullReferenceException("Obj is invalid !");
                if (string.IsNullOrEmpty(propertyName))
                    throw new ArgumentNullException("PropertyName is invalid !");
                PropertyInfo prop = type.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                prop.SetValue(obj, value, null);
            }

            /// <summary>
            /// 设置对象字段值;
            /// </summary>
            /// <param name="type">字段可赋值所在类Type</param>
            /// <param name="obj">目标对象</param>
            /// <param name="fieldName">字段名</param>
            /// <param name="value">字段值</param>
            public static void SetFieldValue(Type type, object obj, string fieldName, object value)
            {
                if (type == null)
                    throw new ArgumentNullException("Type is invalid !");
                if (string.IsNullOrEmpty(fieldName))
                    throw new ArgumentNullException("FieldName is invalid !");
                var field = type.GetField(fieldName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (field == null)
                    throw new NullReferenceException($"Type : {type} can not find field: {fieldName} !");
                field.SetValue(obj, value);
            }

            /// <summary>
            /// 设置对象字段值;
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <param name="fieldName">字段名</param>
            /// <param name="value">字段值</param>
            public static void SetFieldValue(object obj, string fieldName, object value)
            {
                if (obj == null)
                    throw new NullReferenceException("Obj is invalid !");
                if (string.IsNullOrEmpty(fieldName))
                    throw new ArgumentNullException("FieldName is invalid !");
                Type type = obj.GetType();
                var field = type.GetField(fieldName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (field == null)
                    throw new NullReferenceException($"Type : {type} can not find field: {fieldName} !");
                field.SetValue(obj, value);
            }

            /// <summary>
            /// 获取对象的属性值;
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <param name="propertyName">属性名</param>
            /// <returns>属性值</returns>
            public static object GetPropertyValue(object obj, string propertyName)
            {
                if (obj == null)
                    throw new NullReferenceException("Obj is invalid !");
                if (string.IsNullOrEmpty(propertyName))
                    throw new ArgumentNullException("PropertyName is invalid !");
                Type type = obj.GetType();
                var property = type.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (property == null)
                    throw new NullReferenceException($"Type : {type} can not find property: {propertyName} !");
                return property.GetValue(obj);
            }

            /// <summary>
            /// 获取非实例对象属性
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="propertyName">属性名</param>
            /// <returns>属性值</returns>
            public static object GetNonInstancePropertyValue(Type type, string propertyName)
            {
                var property = type.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                return property.GetValue(propertyName);
            }

            /// <summary>
            /// 获取属性
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="propertyName">属性名</param>
            /// <returns>属性信息</returns>
            public static PropertyInfo GetProperty(Type type, string propertyName)
            {
                var property = type.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (property == null)
                    throw new NullReferenceException($"Type : {type} can not find property: {propertyName} !");
                return property;
            }

            /// <summary>
            /// 获取对象字段值;
            /// </summary>
            /// <param name="obj">目标对象</param>
            /// <param name="fieldName">字段名</param>
            /// <returns>字段值</returns>
            public static object GetFieldValue(object obj, string fieldName)
            {
                if (obj == null)
                    throw new NullReferenceException("Obj is invalid !");
                if (string.IsNullOrEmpty(fieldName))
                    throw new ArgumentNullException("FieldName is invalid !");
                Type type = obj.GetType();
                var field = type.GetField(fieldName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (field == null)
                    throw new NullReferenceException($"Type : {type} can not find field: {fieldName} !");
                return field.GetValue(obj);
            }

            /// <summary>
            /// 获取接口实现的最高级类型；
            /// </summary>
            /// <param name="derivedType">派生类</param>
            /// <param name="interfaceType">接口基类</param>
            /// <returns>最高实现的类类型</returns>
            public static Type GetInterfaceHigestImplementedType(Type derivedType, Type interfaceType)
            {
                if (derivedType == null)
                    throw new ArgumentNullException("DerivedType is invalid !");
                if (interfaceType == null)
                    throw new ArgumentNullException("InterfaceType is invalid !");
                if (!interfaceType.IsInterface)
                    throw new ArgumentException($"{interfaceType} is not interface !");
                if (!interfaceType.IsAssignableFrom(derivedType))
                    throw new NotImplementedException($"{derivedType} is not inherit from {interfaceType} !");
                Type type = derivedType;
                while (type.BaseType != null)
                {
                    var currentType = type.BaseType;
                    if (interfaceType.IsAssignableFrom(currentType))
                    {
                        type = currentType;
                    }
                    else
                        break;
                }

                return type;
            }

            /// <summary>
            /// 获取类Type类型中的所有字段名；
            /// </summary>
            /// <typeparam name="T">type类型</typeparam>
            /// <returns>名称数组</returns>
            public static string[] GetTypeAllFields<T>()
            {
                return GetTypeAllString(typeof(T));
            }

            /// <summary>
            /// 获取类Type类型中的所有字段名；
            /// </summary>
            /// <param name="type">type类型</param>
            /// <returns>名称数组</returns>
            public static string[] GetTypeAllString(Type type)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic |
                                            BindingFlags.Static);
                return fields.Select(f => f.Name).ToArray();
            }

            /// <summary>
            /// 获取类Type类型中的所有字段名；
            /// </summary>
            /// <param name="type">type类型</param>
            /// <returns>名称数组</returns>
            public static System.Reflection.FieldInfo[] GetTypeAllFields(Type type)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic |
                                            BindingFlags.Static);
                return fields.Select(f => f).ToArray();
            }
            
            /// <summary>
            /// 获取Type类型中所有属性字段名；
            /// </summary>
            /// <typeparam name="T">type类型</typeparam>
            /// <returns>名称数组</returns>
            public static string[] GetTypeAllProperties<T>()
            {
                return GetTypeAllProperties(typeof(T));
            }

            /// <summary>
            /// 获取Type类型中所有属性字段名；
            /// </summary>
            /// <param name="type">type类型</param>
            /// <returns>名称数组</returns>
            public static string[] GetTypeAllProperties(Type type)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                                    BindingFlags.NonPublic | BindingFlags.Static);
                return properties.Select(f => f.Name).ToArray();
            }

            /// <summary>
            /// 获取Type类型中所有字段名称与字段类型的映射
            /// </summary>
            /// <param name="type">type类型</param>
            /// <returns>名称与类型的映射</returns>
            public static IDictionary<string, Type> GetTypeFieldsNameAndTypeMapping(Type type)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic |
                                            BindingFlags.Static);
                return fields.ToDictionary(f => f.Name, t => t.FieldType);
            }

            /// <summary>
            /// 获取Type类型中所有属性名称与字段类型的映射
            /// </summary>
            /// <param name="type">type类型</param>
            /// <returns>名称与类型的映射</returns>
            public static IDictionary<string, Type> GetTypePropertyNameAndTypeMapping(Type type)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                                    BindingFlags.NonPublic | BindingFlags.Static);
                return properties.ToDictionary(f => f.Name, t => t.PropertyType);
            }

            /// <summary>
            /// 检测是否是引用类型
            /// </summary>
            /// <param name="obj">传入的对象</param>
            /// <returns>是否是引用类型</returns>
            public static bool IsReferenceType(object obj)
            {
                return !obj.GetType().IsValueType;
            }
        }
    }
}