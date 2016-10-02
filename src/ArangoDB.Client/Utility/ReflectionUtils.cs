using ArangoDB.Client.Query;
using ArangoDB.Client.Utility.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Utility
{
    public class ReflectionUtils
    {
        public static T GetAttribute<T>(object attributeProvider, bool inherit) where T : Attribute
        {
            return Newtonsoft.Json.ReflectionUtils.GetAttribute<T>(attributeProvider, inherit);
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
#if PORTABLE
            return TypeExtensions.GetProperty(type, name);
#else
            return type.GetProperty(name);
#endif
        }

        public static MemberInfo GetField(Type type, string name)
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
            return Newtonsoft.Json.ReflectionUtils.GetFieldsAndProperties(type, BindingFlags.Public | BindingFlags.Instance);
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
            LinqUtility.CheckNotNull("enumerableType", enumerableType);
            LinqUtility.CheckNotNullOrEmpty("argumentName", argumentName);

            Type itemType;
            if (!TryGetItemTypeOfClosedGenericIEnumerable(enumerableType, out itemType))
            {
                var message = string.Format("Expected a closed generic type implementing IEnumerable<T>, but found '{0}'.", enumerableType);
                throw new ArgumentException(message, argumentName);
            }

            return itemType;
        }

        private static bool TryGetItemTypeOfClosedGenericIEnumerable(Type possibleEnumerableType, out Type itemType)
        {
            LinqUtility.CheckNotNull("possibleEnumerableType", possibleEnumerableType);

            var possibleEnumerableTypeInfo = possibleEnumerableType.GetTypeInfo();

            if (possibleEnumerableTypeInfo.IsArray)
            {
                itemType = possibleEnumerableTypeInfo.GetElementType();
                return true;
            }

            if (!IsIEnumerable(possibleEnumerableTypeInfo))
            {
                itemType = null;
                return false;
            }

            if (possibleEnumerableTypeInfo.IsGenericTypeDefinition)
            {
                itemType = null;
                return false;
            }

            if (possibleEnumerableTypeInfo.IsGenericType && possibleEnumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                itemType = possibleEnumerableTypeInfo.GenericTypeArguments[0];
                return true;
            }

            var implementedEnumerableInterface = possibleEnumerableTypeInfo.ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIEnumerable);

            if (implementedEnumerableInterface == null)
            {
                itemType = null;
                return false;
            }
            
            itemType = implementedEnumerableInterface.GenericTypeArguments[0];
            return true;
        }

        private static bool IsIEnumerable(TypeInfo type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type);
        }

        private static bool IsGenericIEnumerable(TypeInfo enumerableType)
        {
            return IsIEnumerable(enumerableType)
                   && enumerableType.IsGenericType
                   && enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

    }
}
