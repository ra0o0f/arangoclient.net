
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArangoDB.Client.Data
{
    public class CreateAnalyzerResult : BaseResult
    {
        
        public CreateAnalyzerResult()
        {
            Properties = new Dictionary<string, object>();
        }
        
        public string Name { get; set; }
        public string Type { get; set; }

        public IEnumerable<string> Features { get; set; }
        
        [JsonExtensionData]
        public IDictionary<string, object> Properties { get; }

    }
}