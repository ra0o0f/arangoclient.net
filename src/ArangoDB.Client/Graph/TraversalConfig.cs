using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class TraversalConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Sort { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EdgeDirection? Direction { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxIterations { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EdgeCollection { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Visitor { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TraversalItemOrder? ItemOrder { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TraversalOrder? Order { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Filter { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Init { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MinDepth { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxDepth { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TraversalUniqueness Uniqueness { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TraversalStrategy? Strategy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GraphName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Expander { get; set; }

        public string StartVertex { get; set; }
    }
}
