#if !(UNITY_2018_3_OR_NEWER && NET_STANDARD_2_0)

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace MessagePack.Internal
{
    internal class DynamicAssembly
    {
#if NETFRAMEWORK // We don't ship a net472 target, but we might add one for debugging purposes
        private readonly string moduleName;
#endif
        readonly AssemblyBuilder assemblyBuilder;
        readonly ModuleBuilder moduleBuilder;

        // don't expose ModuleBuilder
        // public ModuleBuilder ModuleBuilder { get { return moduleBuilder; } }

        readonly object gate = new object();

        public DynamicAssembly(string moduleName)
        {
#if NETFRAMEWORK // We don't ship a net472 target, but we might add one for debugging purposes
            AssemblyBuilderAccess builderAccess = AssemblyBuilderAccess.RunAndSave;
            this.moduleName = moduleName;
#else
            AssemblyBuilderAccess builderAccess = AssemblyBuilderAccess.RunAndCollect;
#endif
            this.assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(moduleName), builderAccess);
            this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(moduleName + ".dll");
        }

        // requires lock on mono environment. see: https://github.com/neuecc/MessagePack-CSharp/issues/161

        public TypeBuilder DefineType(string name, TypeAttributes attr) => this.moduleBuilder.DefineType(name, attr);

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent) => this.moduleBuilder.DefineType(name, attr, parent);

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces) => this.moduleBuilder.DefineType(name, attr, parent, interfaces);

#if NETFRAMEWORK

        public AssemblyBuilder Save()
        {
            assemblyBuilder.Save(moduleName + ".dll");
            return assemblyBuilder;
        }

#endif
    }
}

#endif