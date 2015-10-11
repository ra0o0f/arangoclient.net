using ArangoDB.Client.Common.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class EdgeDefinitionData
    {
        public string Collection { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> From { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> To { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class EdgeDefinitionTypedData
    {
        public Type Collection { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<Type> From { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<Type> To { get; set; }
    }
}
