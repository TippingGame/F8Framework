using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Text;

namespace F8Framework.Core
{
    public static class TypeExts
    {
        public static BindingFlags bindFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        public static bool IsAssignableTo(this Type @this, Type type)
        {
            return type.IsAssignableFrom(@this);
        }
        public static bool Is(this Type @this, Type type)
        {
            return @this.IsAssignableTo(type);
        }
        public static bool Is<T>(this Type @this)
        {
            return @this.IsAssignableTo(typeof(T));
        }
        public static string GetShortAssemblyName(this Type @this)
        {
            return @this.Assembly.GetName().Name;
        }
        public static IEnumerable<Type> GetConcreteSubtypes(this Type @this, IEnumerable<Type> types = null)
        {
            if (types == null)
                types = @this.Assembly.GetTypes();
            return @this.IsClass // else is interface
                ? types.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(@this))
                : types.Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(@this));
        }
        #region 属性字段设置
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="this">反射对象</param>
        /// <param name="methodName">方法名，区分大小写</param>
        /// <param name="args">方法参数</param>
        /// <typeparam name="T">约束返回的T必须是引用类型</typeparam>
        /// <returns>T类型</returns>
        public static T InvokeMethod<T>(this object @this, string methodName, object[] args)
        {
            return (T)@this.GetType().GetMethod(methodName, args.Select(o => o.GetType()).ToArray()).Invoke(@this, args);
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="this">反射对象</param>
        /// <param name="methodName">方法名，区分大小写</param>
        /// <param name="args">方法参数</param>
        /// <returns>T类型</returns>
        public static void InvokeMethod(this object @this, string methodName, object[] args)
        {
            var type = @this.GetType();
            type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray()).Invoke(@this, args);
        }
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="this">反射对象</param>
        /// <param name="name">字段名</param>
        /// <typeparam name="T">约束返回的T必须是引用类型</typeparam>
        /// <returns>T类型</returns>
        public static T GetField<T>(this object @this, string name)
        {
            return GetProperty<T>(@this, name);
        }

        /// <summary>
        /// 获取所有的字段信息
        /// </summary>
        /// <param name="this">反射对象</param>
        /// <returns>字段信息</returns>
        public static FieldInfo[] GetFields(this object @this)
        {
            FieldInfo[] fieldInfos = @this.GetType().GetFields(bindFlags);
            return fieldInfos;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="this">反射对象</param>
        /// <param name="name">属性名</param>
        /// <typeparam name="T">约束返回的T必须是引用类型</typeparam>
        /// <returns>T类型</returns>
        public static T GetProperty<T>(this object @this, string name)
        {
            var parameter = Expression.Parameter(@this.GetType(), "e");
            var property = Expression.PropertyOrField(parameter, name);
            return (T)Expression.Lambda(property, parameter).Compile().DynamicInvoke(@this);
        }

        /// <summary>
        /// 获取所有的属性信息
        /// </summary>
        /// <param name="this">反射对象</param>
        /// <returns>属性信息</returns>
        public static PropertyInfo[] GetProperties(this object @this)
        {
            PropertyInfo[] propertyInfos = @this.GetType().GetProperties(bindFlags);
            return propertyInfos;
        }

        #endregion 属性字段设置

        #region 获取Description


        /// <summary>
        ///	根据成员信息获取Description信息
        /// </summary>
        /// <param name="this">成员信息</param>
        /// <returns>如果未找到DescriptionAttribute则返回null或返回类型描述</returns>
        public static string GetDescription(this MemberInfo @this)
        {
            return GetDescription(@this, null);
        }

        /// <summary>
        /// 根据成员信息获取Description信息
        /// </summary>
        /// <param name="this">成员信息</param>
        /// <param name="args">格式化占位对象</param>
        /// <returns>如果未找到DescriptionAttribute则返回null或返回类型描述</returns>
        public static string GetDescription(this MemberInfo @this, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            return @this.IsDefined(typeof(DescriptionAttribute), false) ? @this.GetAttribute<DescriptionAttribute>().Description : string.Empty;
        }

        #endregion 获取Description

        /// <summary>
        /// 获取对象的Attribute
        /// </summary>
        /// <returns></returns>
        public static T GetAttribute<T>(this ICustomAttributeProvider @this) where T : Attribute
        {
            var attributes = @this.GetCustomAttributes(typeof(T), true);
            return attributes.Length > 0 ? attributes[0] as T : null;
        }

        #region 资源获取

        /// <summary>
        ///  获取程序集资源的文本资源
        /// </summary>
        /// <param name="this">程序集中的某一对象类型</param>
        /// <param name="resName">资源项名称</param>
        /// <param name="resourceHolder">资源的根名称。例如，名为“MyResource.en-US.resources”的资源文件的根名称为“MyResource”。</param>
        public static string GetStringRes(this Type @this, string resName, string resourceHolder)
        {
            Assembly thisAssembly = Assembly.GetAssembly(@this);
            ResourceManager rm = new ResourceManager(resourceHolder, thisAssembly);
            return rm.GetString(resName);
        }

        /// <summary>
        /// 获取程序集嵌入资源的文本形式
        /// </summary>
        /// <param name="this">程序集中的某一对象类型</param>
        /// <param name="charset">字符集编码</param>
        /// <param name="resName">嵌入资源相对路径</param>
        /// <returns>如没找到该资源则返回空字符</returns>
        public static string GetManifestString(this Type @this, string charset, string resName)
        {
            Assembly asm = Assembly.GetAssembly(@this);
            Stream st = asm.GetManifestResourceStream(string.Concat(@this.Namespace, ".", resName.Replace("/", ".")));
            if (st == null)
            {
                return "";
            }

            int iLen = (int)st.Length;
            byte[] bytes = new byte[iLen];
            st.Read(bytes, 0, iLen);
            return Encoding.GetEncoding(charset).GetString(bytes);
        }

        #endregion 资源获取
    }
}
