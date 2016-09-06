using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public enum KeyGeneratorType
    {
        Autoincrement = 0,
        Traditional = 1
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class CreateCollectionKeyOption
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? AllowUserKeys { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public KeyGeneratorType? Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Increment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Offset { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class CreateCollectionData
    {
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? JournalSize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CreateCollectionKeyOption KeyOptions { get; set; }

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
        public int? IndexBuckets { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ShardKeys { get; set; }
    }


}
