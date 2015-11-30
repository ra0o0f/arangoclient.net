using ArangoDB.Client.Common.Newtonsoft.Json.Utilities;
using ArangoDB.Client.Common.Remotion.Linq.Utilities;
using Remotion.Utilities;
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
#if PORTABLE
            return TypeExtensions.GetProperty(type, name);
#else
            return type.GetProperty(name);
#endif
        }

        public static MemberInfo GetField(Type type,string name)
        {
#if PORTABLE
            return TypeExtensions.GetField(type, name);
#else
            return type.GetField(name);
#endif
        }

        public static MethodInfo GetSetMethod(PropertyInfo propertyInfo)
        {
#if PORTABLE
            return TypeExtensions.GetSetMethod(propertyInfo);
#else
            return propertyInfo.GetSetMethod();
#endif
        }

        public static List<MemberInfo> GetFieldsAndProperties_PublicInstance(Type type)
        {
            return ReflectionUtils.GetFieldsAndProperties(type, BindingFlags.Public | BindingFlags.Instance);
        }

        public static MemberInfo[] GetMember(Type type, string member)
        {
#if PORTABLE
            return TypeExtensions.GetMember(type, member);
#else
            return type.GetMember(member);
#endif
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
#if PORTABLE
            return TypeExtensions.GetConstructors(type);
#else
            return type.GetConstructors();
#endif
        }

        public static Type GetItemTypeOfClosedGenericIEnumerable(Type enumerableType, string argumentName)
        {
            ArgumentUtility.CheckNotNull("enumerableType", enumerableType);
            ArgumentUtility.CheckNotNullOrEmpty("argumentName", argumentName);

            Type itemType = ReflectionUtility.TryGetItemTypeOfClosedGenericIEnumerable(enumerableType);
            if (itemType == null)
            {
                var message = string.Format("Expected a closed generic type implementing IEnumerable<T>, but found '{0}'.", enumerableType);
                throw new ArgumentException(message, argumentName);
            }

            return itemType;
        }
    }
}
