using System;
using System.Collections.Generic;
using System.Reflection;

namespace F8Framework.Core
{
    public abstract class CustomModule
    {
        // 用于存储子类的元数据，使用子类的类型作为索引
        private static Dictionary<Type, CustomModule> _customModules;
  
        static CustomModule()
        {
            // 在静态构造函数中初始化字典
            InitializeCustomModules();
        }

        public static Dictionary<Type, CustomModule> GetCustomModule()
        {
            return CustomModule._customModules;
        }
        
        // 通过子类的类型获取 CustomModules 中的某一个 CustomModule
        public static CustomModule GetCustomModuleByType<T>() where T : CustomModule
        {
            return GetCustomModuleByType(typeof(T));
        }
        
        // 通过子类的类型获取CustomModules中的某一个CustomModule
        public static CustomModule GetCustomModuleByType(Type type)
        {
            if (_customModules.TryGetValue(type, out var customModule))
            {
                return customModule;
            }

            // 处理未找到的情况，你可以根据需求进行适当的处理
            return null;
        }
        
        // 获取特定基类的所有子类，并调用它们的构造函数和Init函数
        private static void InitializeCustomModules()
        {
            // 获取特定基类的所有子类
            _customModules = new Dictionary<Type, CustomModule>();

            Type baseType = typeof(CustomModule);
            foreach (Type type in Assembly.GetAssembly(baseType).GetTypes())
            {
                if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                {
                    // 使用 Activator.CreateInstance 创建实例
                    CustomModule customModule = (CustomModule)Activator.CreateInstance(type);
                    
                    // 将子类的类型作为索引，实例作为值存储到字典中
                    _customModules[type] = customModule;
                }
            }
        }
        
        // 将 Instance 方法移到父类中
        protected static T GetInstance<T>() where T : CustomModule, new()
        {
            Type type = typeof(T);
            if (_customModules.TryGetValue(type, out var customModule))
            {
                return (T)customModule;
            }

            T instance = new T();
            _customModules[type] = instance;
            instance.Init();
            return instance;
        }
        
        protected abstract void Init();

        public abstract void OnEnterGame();

        public abstract void OnQuitGame();

       
    }
}