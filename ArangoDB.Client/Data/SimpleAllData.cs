using ArangoDB.Client.Common.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class SimpleData
    {
        public string Collection { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public int? BatchSize { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public int? Skip { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Limit { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Example { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Attribute { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Left { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Right { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Closed { get; set; }
    }
}
