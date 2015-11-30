using ArangoDB.Client.Common.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Test.Model
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase, CollectionName = "hosts")]
    public class Host
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [DocumentProperty(Identifier = IdentifierType.Key)]
        public string Key { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ip { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Tags { get; set; }
    }
}
