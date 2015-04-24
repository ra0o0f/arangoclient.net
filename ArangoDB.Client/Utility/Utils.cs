using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Utility
{
    public class Utils
    {
        public static MemberInfo GetMemberInfo<T>(Expression<Func<T, object>> attribute)
        {
            var memberExpression = attribute.Body as MemberExpression;
            var unaryExpression = attribute.Body as UnaryExpression;
            if (unaryExpression != null)
                memberExpression = unaryExpression.Operand as MemberExpression;

            if (memberExpression == null || memberExpression.Member == null)
            {
                throw new InvalidOperationException("attribute should be value type");
            }

            return memberExpression.Member;
        }

        public static T CheckNotNull<T>(string argumentName, T actualValue)
        {
            if (actualValue == null)
                throw new ArgumentNullException(argumentName);

            return actualValue;
        }

        public static T ChangeIfNotSpecified<T>(T value,T newValue) where T : class
        {
            if (value == null)
                return newValue;
            else
                return value;
        }

        public static T ChangeIfNotSpecified<T>(Nullable<T> value, T newValue) where T : struct
        {
            if (!value.HasValue)
                return newValue;
            else
                return value.Value;
        }

        public static Nullable<T> ChangeIfNotSpecified<T>(Nullable<T> value, Nullable<T> newValue) where T : struct
        {
            if (value.HasValue)
                return value.Value;
            
            if(newValue.HasValue)
                return newValue.Value;

            return null;
        }

        public static string EdgeDirectionToString(EdgeDirection direction)
        {
            switch (direction)
            {
                case EdgeDirection.Any:
                    return "any";
                case EdgeDirection.Inbound:
                    return "inbound";
                case EdgeDirection.Outbound:
                    return "outbound";
                default:
                    return string.Empty;
            }
        }
    }
}
