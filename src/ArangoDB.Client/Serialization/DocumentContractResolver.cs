using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ArangoDB.Client.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Serialization
{
    public class DocumentContractResolver : DefaultContractResolver
    {
        private static readonly ConcurrentDictionary<DatabaseSharedSetting, DocumentContractResolver> _cachedContractResolvers = new ConcurrentDictionary<DatabaseSharedSetting, DocumentContractResolver>();

        DatabaseSharedSetting sharedSetting;

        private DocumentContractResolver() : base() 
        {
        }
        
        public static DocumentContractResolver GetContractResolver(IArangoDatabase db)
        {
            DocumentContractResolver contractResolver = null;
            if(_cachedContractResolvers.TryGetValue(db.SharedSetting, out contractResolver) == false)
            {
                contractResolver = new DocumentContractResolver() { sharedSetting = db.SharedSetting };
                _cachedContractResolvers.TryAdd(db.SharedSetting, contractResolver);
            }

            return contractResolver;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            IList<JsonProperty> convertedProperties = new List<JsonProperty>();
            foreach (var p in properties)
            {
                IDocumentPropertySetting documentProperty = null;
                sharedSetting.Collection.ChangeDocumentPropertyForType(type, p.UnderlyingName, x => documentProperty = x);
                if (documentProperty != null)
                {
                    if (documentProperty.IgnoreProperty)
                        continue;
                }

                p.PropertyName = sharedSetting.Collection.ResolvePropertyName(type, p.UnderlyingName);

                if (p.PropertyName == "_key" || p.PropertyName == "_id" || p.PropertyName == "_rev")
                    p.NullValueHandling = NullValueHandling.Ignore;

                convertedProperties.Add(p);
            }

            return convertedProperties;
        }
    }
}
