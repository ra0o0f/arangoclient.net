using ArangoDB.Client.Common.Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Common.Utility
{
    public static class CommonUtility
    {
        public static T GetAttribute<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            return ReflectionUtils.GetAttribute<T>(attributeProvider, inherit);
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
            return TypeExtensions.GetProperty(type, name);
        }

        public static MethodInfo GetSetMethod(PropertyInfo propertyInfo)
        {
            return TypeExtensions.GetSetMethod(propertyInfo);
        }

        public static List<MemberInfo> GetFieldsAndProperties_PublicInstance(Type type)
        {
            return ReflectionUtils.GetFieldsAndProperties(type, BindingFlags.Public | BindingFlags.Instance);
        }

        public static MemberInfo[] GetMember(Type type, string member)
        {
            return TypeExtensions.GetMember(type, member);
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
            return TypeExtensions.GetConstructors(type);
        }
    }
}
