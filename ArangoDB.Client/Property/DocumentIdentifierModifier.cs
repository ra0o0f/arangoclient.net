using ArangoDB.Client.Common.Utility;
using ArangoDB.Client.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Property
{
    public class DocumentIdentifierModifier
    {
        DatabaseSharedSetting setting;

        public DocumentIdentifierModifier(DatabaseSharedSetting setting)
        {
            this.setting = setting;
        }

        ConcurrentDictionary<Type, IdentifierMethod> methods = new ConcurrentDictionary<Type, IdentifierMethod>();

        public class IdentifierMethod
        {
            public IdentifierMethod()
            {
                this.SetKey = this.SetRevision = this.SetHandle = (x, y) => { };
            }

            public Action<object, object> SetKey { get; set; }
            public Action<object, object> SetHandle { get; set; }
            public Action<object, object> SetRevision { get; set; }
            public Action<object, object> SetFrom { get; set; }
            public Action<object, object> SetTo { get; set; }
        }

        internal void ClearMethodCache()
        {
            methods.Clear();
        }

        internal void ClearMethodCache(Type type)
        {
            IdentifierMethod identifierMethod = null;
            methods.TryRemove(type, out identifierMethod);
        }

        public void Modify(object document,IDocumentIdentifierResult identifiers)
        {
            if (identifiers.Id != null && identifiers.Key != null && identifiers.Rev != null)
            {
                var methods = FindIdentifierMethodFor(document.GetType());
                methods.SetKey(document, identifiers.Key);
                methods.SetHandle(document, identifiers.Id);
                methods.SetRevision(document, identifiers.Rev);
            }
        }

        public void Modify(object document, IDocumentIdentifierResult identifiers,string from,string to)
        {
            if (identifiers.Id != null && identifiers.Key != null && identifiers.Rev != null)
            {
                var methods = FindIdentifierMethodFor(document.GetType());
                methods.SetKey(document, identifiers.Key);
                methods.SetHandle(document, identifiers.Id);
                methods.SetRevision(document, identifiers.Rev);
                methods.SetFrom(document, from);
                methods.SetTo(document, to);
            }
        }

        public IdentifierMethod FindIdentifierMethodFor(Type type)
        {
            IdentifierMethod identifierMethod = null;
            if(!methods.TryGetValue(type, out identifierMethod))
            {
                identifierMethod=new IdentifierMethod();
                var typeMemberInfos = CommonUtility.GetFieldsAndProperties_PublicInstance(type);
                foreach (var m in typeMemberInfos)
                {
                    string resolvedName = setting.Collection.ResolvePropertyName(type, m.Name);

                    switch(resolvedName)
                    {
                        case "_key":
                            identifierMethod.SetKey = BuildSetAccessor(type,m.Name);
                            break;
                        case "_id":
                            identifierMethod.SetHandle = BuildSetAccessor(type, m.Name);
                            break;
                        case "_rev":
                            identifierMethod.SetRevision = BuildSetAccessor(type, m.Name);
                            break;
                        case "_from":
                            identifierMethod.SetFrom = BuildSetAccessor(type, m.Name);
                            break;
                        case "_to":
                            identifierMethod.SetTo = BuildSetAccessor(type, m.Name);
                            break;
                    }
                }
            }

            return identifierMethod;
        }

        Action<object, object> BuildSetAccessor(Type type, string memberName)
        {
            var propertyInfo = CommonUtility.GetProperty(type, memberName);
            if (propertyInfo != null)
            {
                var setMethod = CommonUtility.GetSetMethod(propertyInfo);

                if (setMethod != null)
                {
                    return BuildAccessor(setMethod);
                }
            }

            var memberInfo = CommonUtility.GetField(type, memberName);
            if(memberInfo!=null)
            {
                var fieldInfo = memberInfo as FieldInfo;
                return (x, y) => { fieldInfo.SetValue(x, y); };
            }

            return (x, y) => { };
        }

        // from http://stackoverflow.com/a/10820869/1271333
        Action<object, object> BuildAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), "o");
            var value = Expression.Parameter(typeof(object));

            Expression<Action<object, object>> expr =
                Expression.Lambda<Action<object, object>>(
                    Expression.Call(
                        Expression.Convert(obj, method.DeclaringType),
                        method,
                        Expression.Convert(value, method.GetParameters()[0].ParameterType)),
                    obj,
                    value);

            return expr.Compile();
        }
    }
}
