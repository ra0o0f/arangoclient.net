using ArangoDB.Client.Common.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client
{
    public enum UniquenessType
    {
        None=0,
        Global=1,
        Path=2
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class TraversalUniqueness
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UniquenessType? Vertices { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UniquenessType? Edges { get; set; }
    }
}
