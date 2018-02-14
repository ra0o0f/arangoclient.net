using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public interface IDatabaseSerializationConfig
    {
        MetadataPropertyHandling MetadataPropertyHandling { get; set; }

        IList<JsonConverter> Converters { get; set; }

        bool SerializeEnumAsInteger { get; set; }
    }
}
