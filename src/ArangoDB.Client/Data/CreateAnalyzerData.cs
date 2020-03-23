using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class NgramData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Min { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Max { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? PreserveOriginal { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class TextAnalyzerData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Locale { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Case { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Stopwords { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Accent { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Stemming { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NgramData EdgeNgram { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class CreateAnalyzerData
    {
        public string Name { get; set; }
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Properties { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Features { get; set; }
    }
}