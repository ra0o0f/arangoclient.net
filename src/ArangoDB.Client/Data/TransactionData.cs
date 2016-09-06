using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class TransactionData
    {
        public TransactionCollection Collections { get; set; }

        public string Action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? WaitForSync { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? LockTimeout { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Params { get; set; }
    }

    [CollectionProperty(Naming = NamingConvention.ToCamelCase)]
    public class TransactionCollection
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public IList<string> Read { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Write { get; set; }
    }
}
