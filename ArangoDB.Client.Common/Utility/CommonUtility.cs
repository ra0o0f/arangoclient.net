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
            return TypeExtensions.GetProperty(type, name);
        }

        public static MemberInfo GetField(Type type,string name)
        {
            return TypeExtensions.GetField(type, name);
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
