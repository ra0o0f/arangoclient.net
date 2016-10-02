using Newtonsoft.Json;
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
    public class CollectionPropertySetting : ICollectionPropertySetting
    {
        static ConcurrentDictionary<Type, ICollectionPropertySetting> cachedCollectionProperties =
            new ConcurrentDictionary<Type, ICollectionPropertySetting>();

        ConcurrentDictionary<string, IDocumentPropertySetting> documentProperties = new ConcurrentDictionary<string, IDocumentPropertySetting>();

        public string CollectionName { get; set; }

        public NamingConvention Naming { get; set; }

        internal static ICollectionPropertySetting FindCollectionAttributeForType(Type type)
        {
            ICollectionPropertySetting collectionSetting = null;
            if (!cachedCollectionProperties.TryGetValue(type, out collectionSetting))
            {
                collectionSetting = ReflectionUtils.GetAttribute<CollectionPropertyAttribute>(type, false);

                cachedCollectionProperties.TryAdd(type, collectionSetting);
            }

            return collectionSetting;
        }

        internal static ICollectionPropertySetting FindCollectionAttributeForType<T>()
        {
            var type = typeof(T);

            return FindCollectionAttributeForType(type);
        }

        internal IDocumentPropertySetting FindDocumentPropertyForType(string memberName)
        {
            return documentProperties.GetOrAdd(memberName, new DocumentPropertySetting());
        }

        internal IDocumentPropertySetting FindDocumentPropertyForType<T>(Expression<Func<T, object>> attribute)
        {
            var memberInfo = Utils.GetMemberInfo<T>(attribute);

            return documentProperties.GetOrAdd(memberInfo.Name, new DocumentPropertySetting());
        }
    }
}
