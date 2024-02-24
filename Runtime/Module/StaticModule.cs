using System;
using System.Collections.Generic;
using System.Reflection;

namespace F8Framework.Core
{
    public abstract class StaticModule
    {
        // 用于存储子类的元数据，使用子类的类型作为索引
        private static Dictionary<Type, StaticModule> _staticModules;
  
        static StaticModule()
        {
            // 在静态构造函数中初始化字典
            InitializeStaticModules();
        }

        public static Dictionary<Type, StaticModule> GetStaticModule()
        {
            return StaticModule._staticModules;
        }
        
        // 通过子类的类型获取 StaticModules 中的某一个 StaticModule
        public static StaticModule GetStaticModuleByType<T>() where T : StaticModule
        {
            return GetStaticModuleByType(typeof(T));
        }
        
        // 通过子类的类型获取StaticModules中的某一个StaticModule
        public static StaticModule GetStaticModuleByType(Type type)
        {
            if (_staticModules.TryGetValue(type, out var staticModule))
            {
                return staticModule;
            }

            // 处理未找到的情况，你可以根据需求进行适当的处理
            return null;
        }
        
        // 获取特定基类的所有子类，并调用它们的构造函数和Init函数
        private static void InitializeStaticModules()
        {
            // 获取特定基类的所有子类
            _staticModules = new Dictionary<Type, StaticModule>();

            Type baseType = typeof(StaticModule);
            foreach (Type type in Assembly.GetAssembly(baseType).GetTypes())
            {
                if (type.IsSubclassOf(baseType) && !type.IsAbstract)
                {
                    // 使用 Activator.CreateInstance 创建实例
                    StaticModule staticModule = (StaticModule)Activator.CreateInstance(type);
                    
                    // 将子类的类型作为索引，实例作为值存储到字典中
                    _staticModules[type] = staticModule;
                }
            }
        }
        
        // 将 Instance 方法移到父类中
        protected static T GetInstance<T>() where T : StaticModule, new()
        {
            Type type = typeof(T);
            if (_staticModules.TryGetValue(type, out var staticModule))
            {
                return (T)staticModule;
            }

            T instance = new T();
            _staticModules[type] = instance;
            instance.Init();
            return instance;
        }
        
        protected abstract void Init();

        public abstract void OnEnterGame();

        public abstract void OnQuitGame();

       
    }
}