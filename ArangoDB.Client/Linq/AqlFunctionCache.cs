using ArangoDB.Client.Common.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Linq
{
    internal class AqlFunctionCache
    {
        ConcurrentDictionary<string, UserFunctionAttribute> attributes = new ConcurrentDictionary<string, UserFunctionAttribute>();

        public UserFunctionAttribute FindFunctionAttribute(MemberInfo memberInfo)
        {
            UserFunctionAttribute userFunction = null;
            string methodFullname = $"{memberInfo.DeclaringType.Name}.{memberInfo.Name}";
            if (!attributes.TryGetValue(methodFullname, out userFunction))
            {
                userFunction = CommonUtility.GetAttribute<UserFunctionAttribute>(memberInfo, false);
                attributes.TryAdd(methodFullname, userFunction);
            }
            return userFunction;
        }
    }
}
