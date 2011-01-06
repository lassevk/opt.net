using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Opt.Tests
{
    internal static class TypeCreator
    {
        private static readonly Dictionary<KeyValuePair<Type, Type>, Type> _ContainerTypes = new Dictionary<KeyValuePair<Type, Type>, Type>();

        public static Type CreateContainerType(Type propertyType, Type attributeType)
        {
            var key = new KeyValuePair<Type, Type>(propertyType, attributeType);
            if (_ContainerTypes.ContainsKey(key))
                return _ContainerTypes[key];

            var assemblyName = new AssemblyName("dummyAssembly");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            ModuleBuilder module = assembly.DefineDynamicModule("dummyModule", false);
            TypeBuilder type = module.DefineType("Container");
            FieldBuilder field = type.DefineField("_Value", propertyType, FieldAttributes.Private);
            PropertyBuilder property = type.DefineProperty("Value", PropertyAttributes.None, propertyType, null);

            var attr = new CustomAttributeBuilder(attributeType.GetConstructor(new[]
                {
                    typeof(string), typeof(string)
                }), new[]
                    {
                        "-p", "VALUE"
                    });
            property.SetCustomAttribute(attr);

            MethodBuilder getMethod = type.DefineMethod("get_Value", MethodAttributes.Public, propertyType, new Type[0]);
            MethodBuilder setMethod = type.DefineMethod("set_Value", MethodAttributes.Public, typeof(void), new[]
                {
                    propertyType
                });
            property.SetGetMethod(getMethod);
            property.SetSetMethod(setMethod);

            ILGenerator il = getMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);

            il = setMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);

            Type finishedType = type.CreateType();
            _ContainerTypes[key] = finishedType;
            return finishedType;
        }
    }
}