using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class ConsolidationPolicy
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? Threshold { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? SegmentsBytesFloor { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? SegmentsBytesMax { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? SegmentsMax { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? SegmentsMin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MinScore { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class PrimarySort
    {
        public string Field { get; set; }
        public string Direction { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class ViewLinkData
    {
        public ViewLinkData()
        {
            Collections = new Dictionary<string, object>();
        }
        
        [JsonExtensionData]
        public IDictionary<string, object> Collections { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class ViewLinkItemData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Analyzers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FieldData Fields { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IncludeAllFields { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? TrackListPositions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StoreValues { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class FieldData
    {
        public FieldData()
        {
            Fields = new Dictionary<string, object>();
        }
        
        [JsonExtensionData]
        public IDictionary<string, object> Fields { get; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class CreateSearchViewData
    {
        public string Name { get; set; }
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ViewLinkData Links { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<PrimarySort> PrimarySort { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CleanupIntervalStep { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CommitIntervalMsec { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ConsolidationIntervalMsec { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ConsolidationPolicy ConsolidationPolicy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? WritebufferIdle { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? WritebufferActive { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? WritebufferSizeMax { get; set; }
    }
}