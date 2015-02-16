using ArangoDB.Client.Common.Utility;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Property
{
    public class DocumentPropertySetting : IDocumentPropertySetting
    {
        static ConcurrentDictionary<Type, Dictionary<string, IDocumentPropertySetting>> cachedAttributeProperties =
            new ConcurrentDictionary<Type, Dictionary<string, IDocumentPropertySetting>>();

        public string PropertyName { get; set; }

        public bool IgnoreProperty { get; set; }

        public NamingConvention Naming { get; set; }

        public IdentifierType Identifier { get; set; }

        internal static IDocumentPropertySetting FindDocumentAttributeForType(Type type,string memberName)
        {
            Dictionary<string, IDocumentPropertySetting> typeSetting = null;
            if (!cachedAttributeProperties.TryGetValue(type, out typeSetting))
            {
                var typeMemberInfos = CommonUtility.GetFieldsAndProperties_PublicInstance(type);

                typeSetting = new Dictionary<string, IDocumentPropertySetting>();

                foreach (var m in typeMemberInfos)
                    typeSetting.Add(m.Name, CommonUtility.GetAttribute<DocumentPropertyAttribute>(m, false));

                cachedAttributeProperties.TryAdd(type, typeSetting);
            }

            return typeSetting[memberName];
        }

        internal static IDocumentPropertySetting FindDocumentAttributeForType<T>(Expression<Func<T, object>> attribute)
        {
            var type = typeof(T);

            var memberInfo = Utils.GetMemberInfo<T>(attribute);

            return FindDocumentAttributeForType(type, memberInfo.Name);
        }
    }


}
