using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartFormat.Utilities
{
#if !NET40
    public static class TypeInfoAllMemberExtensions
    {
        public static IEnumerable<ConstructorInfo> GetAllConstructors(this TypeInfo typeInfo)
            => TypeInfoAllMemberExtensions.GetAll(typeInfo, ti => ti.DeclaredConstructors);

        public static IEnumerable<EventInfo> GetAllEvents(this TypeInfo typeInfo)
            => TypeInfoAllMemberExtensions.GetAll(typeInfo, ti => ti.DeclaredEvents);

        public static IEnumerable<FieldInfo> GetAllFields(this TypeInfo typeInfo)
            => TypeInfoAllMemberExtensions.GetAll(typeInfo, ti => ti.DeclaredFields);

        public static IEnumerable<MemberInfo> GetAllMembers(this TypeInfo typeInfo)
            => TypeInfoAllMemberExtensions.GetAll(typeInfo, ti => ti.DeclaredMembers);

        public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo typeInfo)
            => TypeInfoAllMemberExtensions.GetAll(typeInfo, ti => ti.DeclaredMethods);

        public static IEnumerable<TypeInfo> GetAllNestedTypes(this TypeInfo typeInfo)
            => TypeInfoAllMemberExtensions.GetAll(typeInfo, ti => ti.DeclaredNestedTypes);

        public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
            => TypeInfoAllMemberExtensions.GetAll(typeInfo, ti => ti.DeclaredProperties);

        private static IEnumerable<T> GetAll<T>(TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> accessor)
        {
            while (typeInfo != null)
            {
                foreach (var t in accessor(typeInfo))
                {
                    yield return t;
                }

                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }
        }
    }
#endif
}