using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseSerializationConfig : IDatabaseSerializationConfig
    {
        public DatabaseSerializationConfig()
        {
            SerializeEnumAsInteger = true;
            MetadataPropertyHandling = MetadataPropertyHandling.Default;
            Converters = new List<JsonConverter>();
        }

        public MetadataPropertyHandling MetadataPropertyHandling { get; set; }

        public IList<JsonConverter> Converters { get; set; }

        public bool SerializeEnumAsInteger { get; set; }
    }
}
