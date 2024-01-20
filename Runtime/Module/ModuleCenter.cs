using System;
using System.Collections.Generic;
using System.Reflection;

namespace F8Framework.Core
{
    public abstract class ModuleCenter
    {
        // 用于存储子类的元数据，使用子类的类型作为索引
        private static Dictionary<Type, ModuleCenter> _moduleCenters;
  
        static ModuleCenter()
        {
            // 在静态构造函数中初始化字典
            InitializeModuleCenters();
        }

        public static Dictionary<Type, ModuleCenter> GetSubCenter()
        {
            return ModuleCenter._moduleCenters;
        }
        
        // 通过子类的类型获取ModuleCenters中的某一个center
        public static ModuleCenter GetCenterByType(Type type)
        {
            if (_moduleCenters.TryGetValue(type, out var center))
            {
                return center;
            }

            // 处理未找到的情况，你可以根据需求进行适当的处理
            return null;
        }
        
        // 获取特定基类的所有子类，并调用它们的构造函数和Init函数
        private static void InitializeModuleCenters()
        {
            // 获取特定基类的所有子类
            _moduleCenters = new Dictionary<Type, ModuleCenter>();

            Type baseType = typeof(ModuleCenter);
            foreach (Type type in Assembly.GetAssembly(baseType).GetTypes())
            {
                if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                {
                    // 使用 Activator.CreateInstance 创建实例
                    ModuleCenter moduleCenter = (ModuleCenter)Activator.CreateInstance(type);
                    
                    // 将子类的类型作为索引，实例作为值存储到字典中
                    _moduleCenters[type] = moduleCenter;
                }
            }
        }
        
        // 将 Instance 方法移到父类中
        protected static T GetInstance<T>() where T : ModuleCenter, new()
        {
            Type type = typeof(T);
            if (_moduleCenters.TryGetValue(type, out var center))
            {
                return (T)center;
            }

            T instance = new T();
            _moduleCenters[type] = instance;
            instance.Init();
            return instance;
        }
        
        protected abstract void Init();

        public abstract void OnEnterGame();

        public abstract void OnQuitGame();

       
    }
}