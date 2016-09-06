using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArangoDB.Client.Data
{
    public class DropGraphData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DropCollections { get; set; }
    }
}
