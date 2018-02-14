using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDB.Client.Config
{
    public class DatabaseSerializationConfigBuilder
    {
        DatabaseConfigBuilder configBuilder;
        IDatabaseConfig config;

        public DatabaseSerializationConfigBuilder(DatabaseConfigBuilder configBuilder, IDatabaseConfig config)
        {
            this.configBuilder = configBuilder;
            this.config = config;
        }

        public DatabaseConfigBuilder MetadataPropertyHandling(MetadataPropertyHandling metadataPropertyHandling)
        {
            config.Serialization.MetadataPropertyHandling = metadataPropertyHandling;
            return configBuilder;
        }

        public DatabaseConfigBuilder SerializeEnumAsInteger(bool serializeEnumAsInteger = true)
        {
            config.Serialization.SerializeEnumAsInteger = serializeEnumAsInteger;
            return configBuilder;
        }

        public DatabaseConfigBuilder AddConverter(JsonConverter converter)
        {
            config.Serialization.Converters.Add(converter);
            return configBuilder;
        }
    }
}
