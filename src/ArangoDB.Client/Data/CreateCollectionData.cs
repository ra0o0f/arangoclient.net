using ArangoDB.Client.Common.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class CreateCollectionData
    {
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? WaitForSync { get;set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DoCompact { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsSystem { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsVolatile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? NumberOfShards { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ShardKeys { get; set; }
    }
}
